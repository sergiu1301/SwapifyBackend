using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using Swapify.Api;
using Swapify.API.Examples;
using Swapify.API.Requests;
using Swapify.Contracts.Models;
using Swapify.Contracts.Services;
using Swashbuckle.AspNetCore.Filters;

namespace Swapify.API.Controllers;

/// <summary>
/// Authentication Controller
/// </summary>
[Route("connect")]
[ApiController]
public class AuthController : Controller
{
    private readonly ApiOptions _apiOptions;
    private readonly TokenOptions _tokenOptions;
    private readonly IUserManagerService _userManagerService;

    /// <summary>
    /// Authentication Constructor
    /// </summary>
    /// <param name="apiOptions">Api options</param>
    /// <param name="tokenOptions">Token options</param>
    /// <param name="userManagerService">User Manager Service</param>
    public AuthController(IUserManagerService userManagerService, ApiOptions apiOptions, TokenOptions tokenOptions)
    {
        _userManagerService = userManagerService;
        _apiOptions = apiOptions;
        _tokenOptions = tokenOptions;
    }

    /// <summary>
    /// The register endpoint can be used to register in application
    /// </summary>
    /// <param name="request">Register request</param>
    /// <response code="200">Returns the token.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="404">Not Found.</response>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest request)
    {
        await _userManagerService.RegisterAsync(request.Email, request.Password, request.ClientId, request.ClientSecret);

        return Ok();
    }

    /// <summary>
    /// The token endpoint can be used to obtain a token
    /// </summary>
    /// <param name="request">Token request</param>
    /// <response code="200">Returns the token.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="403">Forbidden.</response>
    /// <response code="404">Not Found.</response>
    [HttpPost("token")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerRequestExample(typeof(TokenGenerationRequest), typeof(TokenGenerationRequestExample))]
    public async Task<IActionResult> GenerateToken([FromBody] TokenGenerationRequest request)
    {
        byte[] key = Encoding.UTF8.GetBytes(_apiOptions.ApiSecret);

        IUser user = await _userManagerService.VerifyAsync(request.Email, request.Password, request.ClientId, request.ClientSecret);

        List<Claim> claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, user.UserId),
            new(ClaimTypes.Role, user.RoleName)
        };

        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(_tokenOptions.TokenLifeTime),
            Issuer = _tokenOptions.Issuer,
            Audience = _tokenOptions.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

        SecurityToken? token = tokenHandler.CreateToken(tokenDescriptor);

        return Ok(new { accessToken = tokenHandler.WriteToken(token)});
    }

    [HttpGet("swagger-login")]
    public IActionResult SwaggerLogin([FromQuery] string client_id)
    {
        var clientSecret = "dcf044f6-8251-4890-9da7-34468e37faa4";
        string html = string.Format(@"
        <!DOCTYPE html>
        <html lang='en'>
        <head>
            <meta charset='UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <title>Swagger Login</title>
        </head>
        <body>
            <h2>Login for Swagger</h2>
            <form id='loginForm'>
                <input type='email' id='email' placeholder='Email' required /><br>
                <input type='password' id='password' placeholder='Password' required /><br>
                <input type='hidden' id='clientId' value='{0}' />
                <input type='hidden' id='clientSecret' value='{1}' />
                <button type='submit'>Login</button>
            </form>
            <script>
                document.getElementById('loginForm').addEventListener('submit', async function(e) {{
                    e.preventDefault();
                    const email = document.getElementById('email').value;
                    const password = document.getElementById('password').value;
                    const clientId = document.getElementById('clientId').value;
                    const clientSecret = document.getElementById('clientSecret').value;

                    const response = await fetch('token', {{
                        method: 'POST',
                        headers: {{ 'Content-Type': 'application/json' }},
                        body: JSON.stringify({{ email, password, clientId, clientSecret }})
                    }});

                    const data = await response.json();
                    if (data.token) {{
                        window.location.href = '/swagger/oauth2-redirect.html?access_token=' + data.token;
                    }} else {{
                        alert('Login failed!');
                    }}
                }});
            </script>
        </body>
        </html>", client_id, clientSecret);

        return Content(html, "text/html");
    }
}