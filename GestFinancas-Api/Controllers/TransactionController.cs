
using Microsoft.AspNetCore.Mvc;
using GestFinancas_Api.Models;
using System;
using System.Threading.Tasks;

namespace GestFinancas_Api.Controllers
{
    [ApiController]
    [Route("ai/Transaction")]
    public class TransactionController : ControllerBase
    {
        private readonly ILogger<TransactionController> _logger;
        private readonly AppDbContext _db;

        public TransactionController(ILogger<TransactionController> logger, AppDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpPost("draft")]
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
        public IActionResult GetDraftsByUser(Guid userId)
        {
            var drafts = _db.DraftTransactions
                .Where(d => d.UserId == userId && !d.Confirmed)
                .ToList();
            return Ok(drafts);
        }

        [HttpPost("confirm/{id}")]
        public async Task<IActionResult> Confirm(Guid id)
        {
            var draft = await _db.DraftTransactions.FindAsync(id);
            if (draft == null)
                return NotFound(new { message = "Rascunho não encontrado." });

            if (draft.Confirmed)
                return BadRequest(new { message = "Transação já foi confirmada." });

            // Converter draft em transação definitiva
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
            
            // Marcar draft como confirmado
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