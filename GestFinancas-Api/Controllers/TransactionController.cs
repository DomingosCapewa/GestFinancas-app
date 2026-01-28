
using Microsoft.AspNetCore.Mvc;


namespace GestFinancas_Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ILogger<TransactionController> _logger;

        public TransactionController(ILogger<TransactionController> logger)
        {
            _logger = logger;
        }

        [HttpPost("draft")]
        public async Task<IActionResult> CreateDraft(DraftTransaction draft)
        {
            // _db.TransactionDrafts.Add(draft);
            // await _db.SaveChangesAsync();
            // return Ok(draft);
            return Ok(new { message = "Rascunho de transação criado com sucesso, aprove ou rejeite." });
        }


        [HttpPost("ai/propose")]
        public IActionResult ProposeTransaction( )
        {
            //precisa receber AiTransactionDto dto
            return Ok(new { message = "O Julius AI sugeriu uma nova transação." });
        }

        [HttpPost("ai/confirm/{id}")]
        public IActionResult Confirm(Guid id)
        {
            // if(amount < 0 || amount > 100000) reject();

            return Ok(new { message = $"Transação {id} confirmada com sucesso." });
        // public IActionResult Confirm(Guid id) {
        // // _service.CommitTransaction(id);

        }

        [HttpPost("reject/{id}")]
        public IActionResult Reject(Guid id)
        {
            // Lógica para rejeitar uma transação via AI

            return Ok(new { message = $"Transação {id} rejeitada com sucesso." });
        }

        [HttpPost("clarify/{id}")]
        public IActionResult Clarify(Guid id)
        {
            // Lógica para esclarecer uma transação via AI

            return Ok(new { message = $"Solicitação de esclarecimento para a transação {id} enviada com sucesso." });
        }

    }
}