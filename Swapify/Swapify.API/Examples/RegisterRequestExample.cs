using Swapify.API.Requests;
using Swashbuckle.AspNetCore.Filters;

namespace Swapify.API.Examples;

public class RegisterRequestExample : IExamplesProvider<RegisterRequest>
{
    public RegisterRequest GetExamples()
    {
        return new RegisterRequest
        {
            Email = "sergiusuciu2002@gmail.com",
            Password = "PasswordExample0!",
            ClientId = "swagger.pkce",
            ClientSecret = "dcf044f6-8251-4890-9da7-34468e37faa4"
        };
    }
}