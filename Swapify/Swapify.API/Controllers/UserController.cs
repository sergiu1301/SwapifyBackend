using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swapify.API.Requests;
using Swapify.Contracts.Models;
using Swapify.Contracts.Services;
using ForgotPasswordRequest = Swapify.API.Requests.ForgotPasswordRequest;

namespace Swapify.API.Controllers;

/// <summary>
/// User Controller
/// </summary>
[Route("api/v{version:apiVersion}/user")]
[Authorize(Policy = Policies.Policies.UserOrAdminPolicy)]
[ApiVersion("1.0")]
public class UserController : ApiBaseController
{
    private readonly IUserManagerService _userManagerService;
    private readonly IContextService _contextService;

    /// <summary>
    /// User Constructor
    /// </summary>
    /// <param name="userManagerService">User Manager Service</param>
    /// <param name="contextService">Context Service</param>
    public UserController(IUserManagerService userManagerService, IContextService contextService)
    {
        _userManagerService = userManagerService;
        _contextService = contextService;
    }

    /// <summary>
    /// Get current user
    /// </summary>
    /// <response code="200">Returns the user.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Forbidden.</response>
    /// <response code="404">Not Found.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserAsync()
    {
        string userId = await _contextService.GetCurrentUserIdAsync();

        IUser user = await _userManagerService.GetByIdAsync(userId);

        if (user == null)
        {
            return NotFound(new { message = "User not found." });
        }

        return Ok(user);
    }

    /// <summary>
    /// Delete current user
    /// </summary>
    /// <response code="204">No Content.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Forbidden.</response>
    /// <response code="404">Not Found.</response>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUserAsync()
    {
        string userId = await _contextService.GetCurrentUserIdAsync();
        await _userManagerService.DeleteAsync(userId);

        return NoContent();
    }

    /// <summary>
    /// Forgot user password
    /// </summary>
    /// <response code="204">No Content.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Forbidden.</response>
    /// <response code="404">Not Found.</response>
    [HttpPut("forgot-password")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ForgotUserPasswordAsync([FromBody] ForgotPasswordRequest request)
    {
        await _userManagerService.ForgotPasswordAsync(request.Email, request.ClientId, request.ClientSecret);

        return NoContent();
    }

    /// <summary>
    /// Forgot user password
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="request">Request</param>
    /// <response code="204">No Content.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Forbidden.</response>
    /// <response code="404">Not Found.</response>
    [HttpPut("reset-password")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeUserPasswordAsync([FromQuery] string userId, [FromQuery] string token, [FromBody] ChangePasswordRequest request)
    {
        await _userManagerService.ResetPasswordAsync(userId, token, request.NewPassword, request.ClientId, request.ClientSecret);

        return NoContent();
    }

    /// <summary>
    /// Confirm user email
    /// </summary>
    /// <param name="userId">User identifier</param> 
    /// <response code="204">No Content.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Forbidden.</response>
    /// <response code="404">Not Found.</response>
    [HttpGet("confirm-email")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConfirmEmailAsync([FromQuery] string userId, [FromQuery] string token)
    {
        await _userManagerService.ConfirmEmailAsync(userId, token);

        return NoContent();
    }
}