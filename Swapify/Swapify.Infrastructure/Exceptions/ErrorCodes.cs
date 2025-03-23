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
    public static readonly string ClientNotSupported = Format("CLIENT_NOT_SUPPORTED");
    public static readonly string UserCannotBeCreated = Format("USER_CANNOT_BE_CREATED");
    public static readonly string EmailCannotBeConfirmed = Format("EMAIL_CANNOT_BE_CONFIRMED");
    public static readonly string EmailAlreadyConfirmed = Format("EMAIL_ALREADY_CONFIRMED");
    public static readonly string EmailNotConfirmed = Format("EMAIL_NOT_CONFIRMED");
    public static readonly string UserCannotBeDeleted = Format("USER_CANNOT_BE_DELETED");
    public static readonly string PasswordCannotBeChanged = Format("PASSWORD_CANNOT_BE_CHANGED");

    private const string Name = "SWAPIFY";
    private static string Format(string code) => $"{Name}__{code}";
}