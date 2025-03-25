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
    public async Task<IActionResult> GenerateToken([FromForm] TokenGenerationRequest request)
    {
        if (!request.GrantType.Equals("password", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new
            {
                error = "unsupported_grant_type",
                error_description = "Grant type must be 'password'."
            });
        }

        IUser user = await _userManagerService.VerifyAsync(request.Email, request.Password, request.ClientId, request.ClientSecret);

        byte[] key = Encoding.UTF8.GetBytes(_apiOptions.ApiSecret);

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
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256Signature
                )
        };

        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken? securityToken = tokenHandler.CreateToken(tokenDescriptor);
        string? accessToken = tokenHandler.WriteToken(securityToken);

        return Ok(new
        {
            access_token = accessToken,
            token_type = "Bearer",
            expires_in = (int)_tokenOptions.TokenLifeTime.TotalSeconds
        });
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet("authorize")]
    public IActionResult ShowLoginPage(
    [FromQuery] string response_type,
    [FromQuery] string client_id,
    [FromQuery] string redirect_uri,
    [FromQuery] string state)
    {
        if (!string.Equals(response_type, "token", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Invalid response_type - expected 'token'");
        }

        string openEyeSvg = ReadFromWwwRoot("svg", "open-eye.svg");
        string slashEyeSvg = ReadFromWwwRoot("svg", "slash-eye.svg");
        string htmlContent = ReadFromWwwRoot("html", "authorize.html");
        string cssContent = ReadFromWwwRoot("html", "authorize.css");
        string jsContent = ReadFromWwwRoot("html", "authorize.js");

        var jsPlaceholders = new Dictionary<string, string>
        {
            { "{{OPEN_EYE_SVG}}",  openEyeSvg },
            { "{{SLASH_EYE_SVG}}", slashEyeSvg },
            { "{{CLIENT_ID}}",     client_id },
            { "{{CLIENT_SECRET}}", _apiOptions.ClientSecret },
            { "{{REDIRECT_URI}}",  redirect_uri },
            { "{{STATE}}",         state ?? "" }
        };

        jsContent = ReplaceMultiple(jsContent, jsPlaceholders);

        htmlContent = htmlContent.Replace("{{OPEN_EYE_SVG}}", openEyeSvg);

        string finalHtml = htmlContent.Replace(
            "</body>",
            $@"
                <style>
                    {cssContent}
                </style>
                <script>
                    {jsContent}
                </script>
            </body>"
        );

        return Content(finalHtml, "text/html");
    }

    private string ReadFromWwwRoot(params string[] pathSegments)
    {
        string fullPath = Path.Combine(
            new[] { Directory.GetCurrentDirectory(), "wwwroot" }
                .Concat(pathSegments)
                .ToArray()
        );

        return System.IO.File.ReadAllText(fullPath);
    }

    private string ReplaceMultiple(string text, Dictionary<string, string> placeholders)
    {
        foreach (KeyValuePair<string, string> kvp in placeholders)
        {
            text = text.Replace(kvp.Key, kvp.Value);
        }
        return text;
    }
}