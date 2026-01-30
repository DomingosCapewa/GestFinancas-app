using Microsoft.AspNetCore.Mvc;
using GestFinancas_Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GestFinancas_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DatabaseFixController : ControllerBase
    {
        private readonly AppDbContext _db;

        public DatabaseFixController(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost("fix-userid-type")]
        public async Task<IActionResult> FixUserIdType()
        {
            try
            {
                // Limpar dados existentes
                await _db.Database.ExecuteSqlRawAsync("DELETE FROM DraftTransactions");
                await _db.Database.ExecuteSqlRawAsync("DELETE FROM Transactions");
                
                // Alterar tipo da coluna
                await _db.Database.ExecuteSqlRawAsync("ALTER TABLE Transactions MODIFY COLUMN UserId INT NOT NULL");
                await _db.Database.ExecuteSqlRawAsync("ALTER TABLE DraftTransactions MODIFY COLUMN UserId INT NOT NULL");

                return Ok(new { message = "Tipo da coluna UserId alterado com sucesso de Guid para Int!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao alterar tipo da coluna", error = ex.Message, stack = ex.StackTrace });
            }
        }

        [HttpGet("check-column-type")]
        public async Task<IActionResult> CheckColumnType()
        {
            try
            {
                var result = await _db.Database.SqlQueryRaw<string>(
                    @"SELECT COLUMN_TYPE 
                      FROM INFORMATION_SCHEMA.COLUMNS 
                      WHERE TABLE_SCHEMA = DATABASE() 
                      AND TABLE_NAME = 'Transactions' 
                      AND COLUMN_NAME = 'UserId'").ToListAsync();

                return Ok(new { 
                    message = "Tipo atual da coluna UserId em Transactions", 
                    columnType = result.FirstOrDefault() ?? "Not found",
                    database = _db.Database.GetConnectionString()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao verificar tipo da coluna", error = ex.Message });
            }
        }

        [HttpGet("debug-transactions")]
        public async Task<IActionResult> DebugTransactions()
        {
            try
            {
                var allTransactions = await _db.Transactions.ToListAsync();
                return Ok(new { 
                    message = "Todas as transações no banco",
                    count = allTransactions.Count,
                    transactions = allTransactions.Select(t => new {
                        t.Id,
                        t.UserId,
                        t.Description,
                        t.Amount,
                        t.Date
                    })
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao buscar transações", error = ex.Message });
            }
        }

        [HttpPost("recreate-database")]
        public async Task<IActionResult> RecreateDatabase()
        {
            try
            {
                // Dropar e recriar o banco
                await _db.Database.EnsureDeletedAsync();
                await _db.Database.EnsureCreatedAsync();

                return Ok(new { message = "Banco de dados recriado com sucesso!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao recriar banco", error = ex.Message, stack = ex.StackTrace });
            }
        }
    }
}
