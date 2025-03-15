using System.ComponentModel.DataAnnotations;
using Swapify.API.Utils;

namespace Swapify.API.Requests;

public class EmailRequest
{
    [Required]
    [RegularExpression(RegexConstants.EmailRegex, ErrorMessage = ErrorMessageConstants.EmailIsInvalidMessage, MatchTimeoutInMilliseconds = RegexConstants.EmailRegexTimeout)]
    public string Email { get; set; }
}