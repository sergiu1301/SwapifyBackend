using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Swapify.Contracts.Models;
using Swapify.Contracts.Services;

namespace Swapify.API.Controllers;

/// <summary>
/// Admin User Roles Controller
/// </summary>
[Route("api/v{version:apiVersion}/admin/users")]
[Authorize(Policy = Policies.Policies.AdminPolicy)]
[ApiVersion("1.0")]
public class AdminUserRolesController : ApiBaseController
{
    private readonly IUserRoleService _userRoleService;

    /// <summary>
    /// Admin User Roles Constructor
    /// </summary>
    /// <param name="userRoleService">User Role Service</param>
    public AdminUserRolesController(IUserRoleService userRoleService)
    {
        _userRoleService = userRoleService;
    }

    /// <summary>
    /// Delete user roles
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="rolesName">User roles</param>
    /// <response code="204">No Content.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Forbidden.</response>
    /// <response code="404">Not Found.</response>
    [HttpDelete("{userId}/role")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUserRolesAsync([FromRoute][Required][StringLength(36)] string userId,
        [FromBody][Required] string roleName)
    {
        await _userRoleService.DeleteUserRoleAsync(userId, roleName);

        return NoContent();
    }

    /// <summary>
    /// Add user roles
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="roleName">User role</param>
    /// <response code="204">No Content.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Forbidden.</response>
    [HttpPost("{userId}/role")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AddUserRole([FromRoute][Required][StringLength(36)] string userId,
        [FromBody][Required] string roleName)
    {
        await _userRoleService.AddUserRoleAsync(userId, roleName);

        return NoContent();
    }

    /// <summary>
    /// Get user roles
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <response code="200">Returns a list of user roles.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Forbidden.</response>
    /// <response code="404">Not Found.</response>
    [HttpGet("{userId}/role")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserRole([FromRoute][Required][StringLength(36)] string userId)
    {
        IRole userRole = await _userRoleService.GetUserRoleAsync(userId);

        return Ok(userRole);
    }
}