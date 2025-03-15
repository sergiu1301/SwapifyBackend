using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swapify.API.Requests;
using Swapify.Contracts.Models;
using Swapify.Contracts.Services;

namespace Swapify.API.Controllers;

/// <summary>
/// Admin Role Controller
/// </summary>
[Route("api/v{version:apiVersion}/admin/roles")]
[Authorize(Policy = Policies.Policies.UserOrAdminPolicy)]
[ApiVersion("1.0")]
public class AdminRoleController : ApiController
{
    private readonly IRoleService _roleService;

    /// <summary>
    /// Admin Role Constructor
    /// </summary>
    /// <param name="roleService">Role Service</param>
    public AdminRoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    /// <summary>
    /// Delete a role
    /// </summary>
    /// <param name="roleName">Role name</param>
    /// <response code="204">No Content.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Forbidden.</response>
    /// <response code="404">Not Found.</response>
    [HttpDelete("{roleName}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRoleAsync([FromRoute][Required][StringLength(256)] string roleName)
    {
        await _roleService.DeleteRoleAsync(roleName);

        return NoContent();
    }

    /// <summary>
    /// Get roles
    /// </summary>
    /// <response code="200">Returns the role.</response>
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
    public async Task<IActionResult> GetRolesAsync()
    {
        IReadOnlyList<IRole> roles = await _roleService.GetRolesAsync();

        return Ok(new
        {
            Roles = roles
        });
    }

    /// <summary>
    /// Update a role
    /// </summary>
    /// <param name="roleId">Role identifier</param>
    /// <param name="roleRequest">Request</param>
    /// <response code="200">Returns the role.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Forbidden.</response>
    /// <response code="404">Not Found.</response>
    [HttpPut("{roleId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateRoleAsync([FromRoute][Required][StringLength(36)] string roleId, [FromBody][Required] RoleRequest roleRequest)
    {
        await _roleService.UpdateRoleAsync(roleId, roleRequest.Name, roleRequest.Description);

        return NoContent();
    }

    /// <summary>
    /// Create a role
    /// </summary>
    /// <param name="roleRequest">Request</param>
    /// <response code="200">Returns the role.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Forbidden.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateRoleAsync([FromBody][Required] RoleRequest roleRequest)
    {
        IRole role = await _roleService.CreateRoleAsync(roleRequest.Name, roleRequest.Description);

        return Ok(role);
    }
}