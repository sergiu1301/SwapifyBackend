namespace Swapify.API.Utils;

internal class RegexConstants
{
    public const string EmailRegex = @"^(?!.{255})(?![^@]{65})((([^<>()\[\]\\.,;:\s@""]+(\.[^<>()\[\]\\.,;:\s@""]+)*)|("".+"")))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$";

    public const int EmailRegexTimeout = 2000;

    public const int PasswordMaxLength = 32;

    public const int PasswordMinLength = 10;

    public const string PasswordRegex = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@#$!%*?&])[A-Za-z\d@#$%^&£*\-_+=[\]{}|\\:',?\/`~""()<>;!]{10,32}$";
}