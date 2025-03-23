using System.ComponentModel.DataAnnotations;
using Swapify.API.Utils;

namespace Swapify.API.Requests;

public class ForgotPasswordRequest
{
    [Required]
    [RegularExpression(RegexConstants.EmailRegex, ErrorMessage = ErrorMessageConstants.EmailIsInvalidMessage, MatchTimeoutInMilliseconds = RegexConstants.EmailRegexTimeout)]
    public string Email { get; set; }

    [Required]
    [MaxLength(100)]
    public string ClientId { get; set; }

    [Required]
    [MaxLength(256)]
    public string ClientSecret { get; set; }
}