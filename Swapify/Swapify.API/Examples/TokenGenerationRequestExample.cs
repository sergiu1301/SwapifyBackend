using Swapify.API.Requests;
using Swashbuckle.AspNetCore.Filters;

namespace Swapify.API.Examples;

public class TokenGenerationRequestExample : IExamplesProvider<TokenGenerationRequest>
{
    public TokenGenerationRequest GetExamples()
    {
        return new TokenGenerationRequest
        {
            Email = "sergiusuciu2002@gmail.com",
            Password = "PasswordExample0!",
            Scope = "application_scope"
        };
    }
}