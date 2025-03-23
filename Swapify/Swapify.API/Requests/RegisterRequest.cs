using System.ComponentModel.DataAnnotations;
using Swapify.API.Utils;

namespace Swapify.API.Requests;

public class RegisterRequest
{
    [Required]
    [RegularExpression(RegexConstants.EmailRegex, ErrorMessage = ErrorMessageConstants.EmailIsInvalidMessage, MatchTimeoutInMilliseconds = RegexConstants.EmailRegexTimeout)]
    public string Email { get; set; }

    [Required]
    [StringLength(RegexConstants.PasswordMaxLength, ErrorMessage = ErrorMessageConstants.PasswordIncorrectLengthMessage, MinimumLength = RegexConstants.PasswordMinLength)]
    [RegularExpression(RegexConstants.PasswordRegex, ErrorMessage = ErrorMessageConstants.PasswordDoesNotContainRequiredCharactersMessage)]
    public string Password { get; set; }

    [Required]
    [MaxLength(100)]
    public string ClientId { get; set; }

    [Required]
    [MaxLength(256)]
    public string ClientSecret { get; set; }
}