using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Swapify.API.Utils;

namespace Swapify.API.Requests;

public class TokenGenerationRequest
{
    [Required]
    [FromForm(Name = "grant_type")]
    public string GrantType { get; set; }

    [Required]
    [FromForm(Name = "email")]
    [RegularExpression(RegexConstants.EmailRegex, ErrorMessage = ErrorMessageConstants.EmailIsInvalidMessage, MatchTimeoutInMilliseconds = RegexConstants.EmailRegexTimeout)]
    public string Email { get; set; }

    [Required]
    [FromForm(Name = "password")]
    [StringLength(RegexConstants.PasswordMaxLength, ErrorMessage = ErrorMessageConstants.PasswordIncorrectLengthMessage, MinimumLength = RegexConstants.PasswordMinLength)]
    [RegularExpression(RegexConstants.PasswordRegex, ErrorMessage = ErrorMessageConstants.PasswordDoesNotContainRequiredCharactersMessage)]
    public string Password { get; set; }

    [Required]
    [FromForm(Name = "client_id")]
    [MaxLength(100)]
    public string ClientId { get; set; }

    [Required]
    [FromForm(Name = "client_secret")]
    [MaxLength(256)]
    public string ClientSecret { get; set; }
}