using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GestFinancas_Api.Models;
using System;
using System.Threading.Tasks;
using System.Security.Claims;
using GestFinancas_Api.Dtos;
using Swashbuckle.AspNetCore.Annotations;

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
        [SwaggerOperation(Summary = "Obtém todas as transações do usuário logado")]
        public IActionResult GetUserTransactions()
        {
            // TODO: Implementar filtro por usuário quando o tipo de UserId for consistente
            // Por enquanto, retorna todas as transações
            var transactions = _db.Transactions
                .OrderByDescending(t => t.Date)
                .ToList();

            return Ok(transactions);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Cria uma nova transação para o usuário logado")]
        public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionDto dto)
        {
            if (dto.Amount <= 0)
                return BadRequest(new { message = "Valor deve ser maior que zero." });

            if (string.IsNullOrWhiteSpace(dto.Description))
                return BadRequest(new { message = "Descrição é obrigatória." });

            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(), // TODO: Usar o ID do usuário logado
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
        public IActionResult GetDraftsByUser(Guid userId)
        {
            var drafts = _db.DraftTransactions
                .Where(d => d.UserId == userId && !d.Confirmed)
                .ToList();
            return Ok(drafts);
        }

        [HttpPost("confirm/{id}")]
        [SwaggerOperation(Summary = "Confirma um rascunho de transação e cria a transação definitiva")]
        public async Task<IActionResult> Confirm(Guid id)
        {
            var draft = await _db.DraftTransactions.FindAsync(id);
            if (draft == null)
                return NotFound(new { message = "Rascunho não encontrado." });

            if (draft.Confirmed)
                return BadRequest(new { message = "Transação já foi confirmada." });

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

            return Ok(new { 
                message = "Transação confirmada e salva com sucesso",
                transactionId = transaction.Id,
                transaction = transaction 
            });
        }

        [HttpPost("reject/{id}")]
        [SwaggerOperation(Summary = "Rejeita um rascunho de transação e o remove")]
        public async Task<IActionResult> Reject(Guid id)
        {
            var draft = await _db.DraftTransactions.FindAsync(id);
            if (draft == null)
                return NotFound(new { message = "Rascunho não encontrado." });

            _db.DraftTransactions.Remove(draft);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Rascunho rejeitado e removido com sucesso." });
            }
    }
}