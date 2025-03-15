namespace Swapify.API.Utils;

internal class ErrorMessageConstants
{
    public const string EmailIsInvalidMessage = "The email provided is invalid!";

    public const string PasswordIncorrectLengthMessage = "The {0} must be at least {2} and at max {1} characters long.";

    public const string PasswordDoesNotContainRequiredCharactersMessage =
        "Passwords must be a minimum of 10 characters (no more than 32) in length and must contain at least one uppercase letter, one lowercase letter, one number and one of the following symbols: !@#$%&?";
}