using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;

using System;
using System.Threading.Tasks;
using System.Security.Claims;

using GestFinancas_Api.Models;
using GestFinancas_Api.Dtos;

namespace GestFinancas_Api.Controllers
{
    [ApiController]
    [Route("api/Transaction")]
    public class TransactionController : ControllerBase
    {
        private readonly ILogger<TransactionController> _logger;
        private readonly AppDbContext _db;

        public TransactionController(ILogger<TransactionController> logger, AppDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet]
        [Authorize]
        [SwaggerOperation(Summary = "Obtém todas as transações do usuário logado")]
        public IActionResult GetUserTransactions()
        {
            var userIdString = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized(new { message = "ID do usuário não encontrado no token." });
            }

            var transactions = _db.Transactions
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.Date)
                .ToList();

            return Ok(transactions);
        }

        [HttpPost]
        [Authorize]
        [SwaggerOperation(Summary = "Cria uma nova transação para o usuário logado")]
        public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionDto dto)
        {
            var userIdString = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized(new { message = "ID do usuário não encontrado no token." });
            }

            if (dto.Amount <= 0)
                return BadRequest(new { message = "Valor deve ser maior que zero." });

            if (string.IsNullOrWhiteSpace(dto.Description))
                return BadRequest(new { message = "Descrição é obrigatória." });

            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = dto.Amount,
                Description = dto.Description,
                Category = dto.Category,
                Type = Enum.Parse<TransactionType>(dto.Type, ignoreCase: true),
                Date = dto.Date ?? DateTime.UtcNow,
                Source = TransactionSource.Manual,
                CreatedAt = DateTime.UtcNow
            };

            _db.Transactions.Add(transaction);
            await _db.SaveChangesAsync();

            return Ok(new { 
                message = "Transação criada com sucesso",
                data = transaction 
            });
        }

        [HttpPost("draft")]
        [SwaggerOperation(Summary = "Cria um novo rascunho de transação para o usuário logado")]
        public async Task<IActionResult> CreateDraft([FromBody] DraftTransaction draft)
        {
            if (draft.Amount <= 0)
                return BadRequest(new { message = "Valor deve ser maior que zero." });

            if (string.IsNullOrWhiteSpace(draft.Description))
                return BadRequest(new { message = "Descrição é obrigatória." });

            draft.Id = Guid.NewGuid();
            draft.Confirmed = false;
            draft.Date = DateTime.UtcNow;

            _db.DraftTransactions.Add(draft);
            await _db.SaveChangesAsync();

            return Ok(new { 
                message = "Rascunho criado com sucesso",
                draftId = draft.Id,
                draft = draft 
            });
        }

        [HttpGet("drafts/{userId}")]
        [SwaggerOperation(Summary = "Obtém todos os rascunhos de transações para um usuário específico")]
        public IActionResult GetDraftsByUser(int userId)
        {
            var drafts = _db.DraftTransactions
                .Where(d => d.UserId == userId && !d.Confirmed)
                .ToList();
            return Ok(drafts);
        }

        [HttpPost("confirm/{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Confirma um rascunho de transação e cria a transação definitiva")]
        public async Task<IActionResult> Confirm(Guid id)
        {
            _logger.LogInformation($"Tentando confirmar draft com ID: {id}");
            
            var userIdString = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized(new { message = "ID do usuário não encontrado no token." });
            }
            
            var draft = await _db.DraftTransactions.FindAsync(id);
            if (draft == null)
            {
                _logger.LogWarning($"Rascunho não encontrado: {id}");
                return NotFound(new { message = "Rascunho não encontrado." });
            }
            
            if (draft.UserId != userId)
            {
                _logger.LogWarning($"Usuário {userId} tentou confirmar draft de outro usuário: {draft.UserId}");
                return Forbid();
            }

            if (draft.Confirmed)
            {
                _logger.LogWarning($"Transação já confirmada: {id}");
                return BadRequest(new { message = "Transação já foi confirmada." });
            }

            try
            {
                var transaction = new Transaction
                {
                    Id = Guid.NewGuid(),
                    UserId = draft.UserId,
                    Amount = draft.Amount,
                    Description = draft.Description,
                    Category = draft.Category,
                    Type = draft.Type,
                    Date = draft.Date,
                    Source = TransactionSource.AI,
                    CreatedAt = DateTime.UtcNow
                };

                _db.Transactions.Add(transaction);
                
                draft.Confirmed = true;
                _db.DraftTransactions.Update(draft);

                await _db.SaveChangesAsync();

                _logger.LogInformation($"Transação confirmada com sucesso: {transaction.Id}");

                return Ok(new { 
                    message = "Transação confirmada e salva com sucesso",
                    transactionId = transaction.Id,
                    transaction = transaction 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao confirmar transação: {id}");
                return StatusCode(500, new { message = "Erro ao confirmar transação.", error = ex.Message });
            }
        }

        [HttpPost("reject/{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Rejeita um rascunho de transação e o remove")]
        public async Task<IActionResult> Reject(Guid id)
        {
            var userIdString = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized(new { message = "ID do usuário não encontrado no token." });
            }
            
            var draft = await _db.DraftTransactions.FindAsync(id);
            if (draft == null)
                return NotFound(new { message = "Rascunho não encontrado." });
            
            if (draft.UserId != userId)
            {
                return Forbid();
            }

            _db.DraftTransactions.Remove(draft);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Rascunho rejeitado e removido com sucesso." });
            }
    }
}