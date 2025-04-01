using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class TokenController : ControllerBase
{
    [HttpPost("token")]
    public IActionResult GetToken()
    {
        // Token oluşturma işlemi burada yapılacak
        var tokenResponse = new TokenResponse
        {
            token_type = "Bearer",
            expires_in = 3600,
            access_token = "your-access-token"
        };

        return Ok(tokenResponse);
    }
}
