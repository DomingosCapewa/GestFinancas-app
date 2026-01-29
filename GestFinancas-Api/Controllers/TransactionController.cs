
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
            draft.Confirmed = false;
            _db.DraftTransactions.Add(draft);
            await _db.SaveChangesAsync();
            return Ok(draft);
        }

        [HttpPost("propose")]
        public IActionResult ProposeTransaction()
        {
            // precisa receber AiTransactionDto dto
            return Ok(new { message = "O Julius AI sugeriu uma nova transação." });
        }

        [HttpPost("confirm/{id}")]
        public IActionResult Confirm(Guid id)
        {
            return Ok(new { message = $"Transação {id} confirmada com sucesso." });
        }

        [HttpPost("reject/{id}")]
        public IActionResult Reject(Guid id)
        {
            return Ok(new { message = $"Transação {id} rejeitada com sucesso." });
        }

        [HttpPost("clarify/{id}")]
        public IActionResult Clarify(Guid id)
        {
            return Ok(new { message = $"Solicitação de esclarecimento para a transação {id} enviada com sucesso." });
        }
    }
}