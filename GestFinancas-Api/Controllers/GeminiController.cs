using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

[ApiController]
[Route("api/gemini")]
public class GeminiController : ControllerBase
{
    private readonly HttpClient _http;
    private readonly string _apiKey;

    public GeminiController(HttpClient http, IConfiguration config)
    {
        _http = http;
        _apiKey = config["Gemini:ApiKey"];
    }

    [HttpPost("chat")]
    public async Task<IActionResult> Chat([FromBody] GeminiRequest request)
    {
        var url = $"https://generativelanguage.googleapis.com/v1/models/gemini-2.5-flash:generateContent?key={_apiKey}";

        var body = new
        {
            contents = new[]
            {
                new { parts = new[] { new { text = request.Prompt } } }
            }
        };

        var response = await _http.PostAsJsonAsync(url, body);
        var result = await response.Content.ReadAsStringAsync();

        return Ok(result);
    }
}

public record GeminiRequest(string Prompt);
