using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swapify.API.Requests;
using Swapify.Contracts.Models;
using Swapify.Contracts.Services;

namespace Swapify.API.Controllers;

/// <summary>
/// User Controller
/// </summary>
[Route("api/v{version:apiVersion}/user")]
[Authorize(Policy = Policies.Policies.UserOrAdminPolicy)]
[ApiVersion("1.0")]
public class UserController : ApiController
{
    private readonly IUserService _userService;
    private readonly IContextService _contextService;

    /// <summary>
    /// User Constructor
    /// </summary>
    /// <param name="userService">User Service</param>
    /// <param name="contextService">Context Service</param>
    public UserController(IUserService userService, IContextService contextService)
    {
        _userService = userService;
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
        string userEmail = await _contextService.GetCurrentContextAsync();
        IUser user = await _userService.GetUserAsync(userEmail);

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
        await _userService.DeleteUserAsync(userId);

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
    [HttpPut("{userId}/confirm-email")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConfirmUserEmailAsync([FromRoute][Required] string userId)
    {
        string id = Uri.UnescapeDataString(userId);
        await _userService.ConfirmUserEmailAsync(id);

        return NoContent();
    }

    /// <summary>
    /// Change user name
    /// </summary>
    /// <param name="userName">User name</param> 
    /// <response code="204">No Content.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Forbidden.</response>
    /// <response code="404">Not Found.</response>
    [HttpPut]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeUserAsync([FromBody][Required] UserRequest request)
    {
        string userId = await _contextService.GetCurrentUserIdAsync();
        IUser user = await _userService.ChangeUserNameAsync(userId, request.Username, request.FirstName, request.LastName);

        return Ok(user);
    }

    /// <summary>
    /// Forgot user password
    /// </summary>
    /// <response code="204">No Content.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Forbidden.</response>
    /// <response code="404">Not Found.</response>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ForgotUserPasswordAsync([FromBody] EmailRequest request)
    {
        await _userService.ForgotUserPasswordAsync(request.Email);

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
    [HttpPut("{userId}/change-password")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeUserPasswordAsync([FromRoute][Required] string userId, [FromBody] ChangePasswordRequest request)
    {
        string id = Uri.UnescapeDataString(userId);
        await _userService.ChangeUserPasswordAsync(id, request.Password);

        return NoContent();
    }
}