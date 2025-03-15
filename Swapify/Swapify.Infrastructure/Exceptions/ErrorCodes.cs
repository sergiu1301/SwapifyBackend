namespace Swapify.Infrastructure.Exceptions;

public static class ErrorCodes
{
    public static readonly string RoleNotFound = Format("ROLE_NOT_FOUND");
    public static readonly string UserNameOrPasswordNotFound = Format("USER_NAME_OR_PASSWORD_NOT_FOUND");
    public static readonly string UserNotFound = Format("USER_NOT_FOUND");
    public static readonly string RoleAlreadyExists = Format("ROLE_ALREADY_EXISTS");
    public static readonly string EmailWasNotSent = Format("EMAIL_WAS_NOT_SENT");
    public static readonly string UserAlreadyExists = Format("USER_ALREADY_EXISTS");
    public static readonly string RolesNotExist = Format("ROLES_NOT_EXIST");

    private const string Name = "VISI_PROJECT";
    private static string Format(string code) => $"{Name}__{code}";
}