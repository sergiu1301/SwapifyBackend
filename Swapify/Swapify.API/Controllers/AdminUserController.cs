using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swapify.Contracts.Models;
using Swapify.Contracts.Services;

namespace Swapify.API.Controllers;

/// <summary>
/// Admin User Controller
/// </summary>
[Route("api/v{version:apiVersion}/admin/users")]
[ApiVersion("1.0")]
public class AdminUserController : ApiController
{
    private readonly IUserService _userService;

    /// <summary>
    /// Admin User Constructor
    /// </summary>
    /// <param name="userService">User Service</param>
    public AdminUserController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Delete a user
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <response code="204">No Content.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Forbidden.</response>
    /// <response code="404">Not Found.</response>
    [HttpDelete("{userId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = Policies.Policies.AdminPolicy)]
    public async Task<IActionResult> DeleteUserAsync([FromRoute][Required][StringLength(36)] string userId)
    {
        await _userService.DeleteUserAsync(userId);

        return NoContent();
    }

    /// <summary>
    /// Get a user
    /// </summary>
    /// <param name="userEmail">User email.</param>
    /// <response code="200">Returns the user.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Forbidden.</response>
    /// <response code="404">Not Found.</response>
    [HttpGet("{userEmail}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = Policies.Policies.AdminPolicy)]
    public async Task<IActionResult> GetUserAsync([FromRoute][Required][StringLength(36)] string userEmail)
    {
        IUser user = await _userService.GetUserAsync(userEmail);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    /// <summary>
    /// Block a user
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <response code="204">No Content.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Forbidden.</response>
    /// <response code="404">Not Found.</response>
    [HttpPut("{userId}/block")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = Policies.Policies.AdminPolicy)]
    public async Task<IActionResult> BlockUserAsync([FromRoute][Required][StringLength(36)] string userId, [FromBody][Required] bool blocked)
    {
        if (blocked)
        {
            await _userService.BlockUserAsync(userId);
        }
        else
        {
            await _userService.UnblockUserAsync(userId);
        }

        return NoContent();
    }

    /// <summary>
    /// Get users
    /// </summary>
    /// <response code="200">Return the users.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Forbidden.</response>
    /// <response code="404">Not Found.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = Policies.Policies.UserOrAdminPolicy)]
    public async Task<IActionResult> GetUsersAsync([FromQuery] int pageNumber, [FromQuery] int pageSize, [FromBody] string? query = null)
    {
        (int, IReadOnlyList<IUser>) users = await _userService.GetUsersAsync(pageNumber, pageSize, query);

        return Ok(new {
            NoUsers = users.Item1, Users =  users.Item2
        });
    }
}