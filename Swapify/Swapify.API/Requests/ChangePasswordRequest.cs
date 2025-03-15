using System.ComponentModel.DataAnnotations;
using Swapify.API.Utils;

namespace Swapify.API.Requests;

public class ChangePasswordRequest
{
    [Required]
    [StringLength(RegexConstants.PasswordMaxLength, ErrorMessage = ErrorMessageConstants.PasswordIncorrectLengthMessage, MinimumLength = RegexConstants.PasswordMinLength)]
    [RegularExpression(RegexConstants.PasswordRegex, ErrorMessage = ErrorMessageConstants.PasswordDoesNotContainRequiredCharactersMessage)]
    public string Password { get; set; }
}