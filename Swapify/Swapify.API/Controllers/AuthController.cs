using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using Swapify.Api;
using Swapify.API.Examples;
using Swapify.API.Requests;
using Swapify.Contracts.Models;
using Swapify.Contracts.Services;
using Swashbuckle.AspNetCore.Filters;
using AuthorizeAttribute = Microsoft.AspNetCore.Authorization.AuthorizeAttribute;

namespace Swapify.API.Controllers;

/// <summary>
/// Authentication Controller
/// </summary>
[Route("connect")]
[ApiController]
public class AuthController : Controller
{
    private readonly IUserService _userService;
    private readonly IContextService _contextService;
    private readonly ApiOptions _apiOptions;
    private readonly TokenOptions _tokenOptions;

    /// <summary>
    /// Authentication Constructor
    /// </summary>
    /// <param name="userService">User Service</param>
    /// <param name="contextService">Context Service</param>
    public AuthController(IUserService userService, IContextService contextService, ApiOptions apiOptions, TokenOptions tokenOptions)
    {
        _userService = userService;
        _contextService = contextService;
        _apiOptions = apiOptions;
        _tokenOptions = tokenOptions;
    }

    /// <summary>
    /// The token endpoint can be used to obtain a token
    /// </summary>
    /// <param name="request">Token request</param>
    /// <response code="200">Returns the token.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="404">Not Found.</response>
    [HttpPost("token")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerRequestExample(typeof(TokenGenerationRequest), typeof(TokenGenerationRequestExample))]
    public async Task<IActionResult> GenerateToken([FromBody] TokenGenerationRequest request)
    {
        if (request.Scope != _apiOptions.ApiScope)
        {
            return Unauthorized();
        }

        var tokenHandler = new JwtSecurityTokenHandler();

        var key = Encoding.UTF8.GetBytes(_apiOptions.ApiSecret);

        IUser user = await _userService.GetUserAsync(request.Email, request.Password);
        
        if (user.IsBlocked || !user.EmailConfirmed)
        {
            return Forbid();
        }

        var claims = new List<Claim>
        {
            new("user_id", user.UserId),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, user.Email),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.Role, user.RoleName)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(_tokenOptions.TokenLifeTime),
            Issuer = _tokenOptions.Issuer,
            Audience = _tokenOptions.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwt = tokenHandler.WriteToken(token);

        return Ok(jwt);
    }

    /// <summary>
    /// The token endpoint can be used to obtain a token
    /// </summary>
    /// <response code="200">Returns the token.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="404">Not Found.</response>
    [HttpPost("token/google")]
    [Authorize(AuthenticationSchemes = GoogleDefaults.AuthenticationScheme)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerRequestExample(typeof(TokenGenerationRequest), typeof(TokenGenerationRequestExample))]
    public async Task<IActionResult> GenerateTokenWithGoogleToken()
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var key = Encoding.UTF8.GetBytes(_apiOptions.ApiSecret);

        var userEmail = await _contextService.GetCurrentContextAsync();

        IUser? user = await _userService.GetUserAsync(userEmail);

        if (user == null)
        {
            var firstName = await _contextService.GetCurrentGoogleFirstNameAsync();
            var lastName = await _contextService.GetCurrentGoogleLastNameAsync();
            user = await _userService.CreateGoogleUserAsync(userEmail, firstName, lastName);
        }
        else
        {
            if (user.IsBlocked || !user.EmailConfirmed)
            {
                return Forbid();
            }
        }

        var claims = new List<Claim>
        {
            new("user_id", user.UserId),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, user.Email),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.Role, user.RoleName)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(_tokenOptions.TokenLifeTime),
            Issuer = _tokenOptions.Issuer,
            Audience = _tokenOptions.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwt = tokenHandler.WriteToken(token);

        return Ok(jwt);
    }

    /// <summary>
    /// The register endpoint can be used for a user to register
    /// </summary>
    /// <param name="request">Register request</param>
    /// <response code="200">Returns the token.</response>
    /// <response code="400">Bad Request.</response>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerRequestExample(typeof(RegisterRequest), typeof(RegisterRequestExample))]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (request.Scope != _apiOptions.ApiScope)
        {
            return Unauthorized();
        }

        IUser user = await _userService.CreateUserAsync(request.Email, request.Password, request.FirstName, request.LastName);

        return Ok(user);
    }

    /// <summary>
    /// The verify token endpoint can be used to verify a token
    /// </summary>
    /// <response code="200">Returns the token.</response>
    /// <response code="400">Bad Request.</response>
    [HttpPost("token/validate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyToken()
    {
        try
        {
            var userEmail = await _contextService.GetCurrentContextAsync();
            bool exists = await _userService.ExistsUserAsync(userEmail);
            
            if (!exists)
            {
                return NotFound();
            }
            
            return Ok();
        }
        catch (SecurityTokenValidationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}