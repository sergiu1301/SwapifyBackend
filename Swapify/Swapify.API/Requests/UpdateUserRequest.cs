namespace Swapify.API.Requests;

/// <summary>
/// Request for updating user profile.
/// </summary>
public class UpdateUserRequest
{
    /// <summary>
    /// New first name (optional)
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// New last name (optional)
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// New phone number (optional)
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// New username (optional)
    /// </summary>
    public string? UserName { get; set; }
}