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
            FirstName = "Example",
            LastName = "Example",
            Scope = "application_scope"
        };
    }
}