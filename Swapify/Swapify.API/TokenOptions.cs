namespace Swapify.Api;

public class TokenOptions
{
    public TimeSpan TokenLifeTime { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string GoogleClientId { get; set; }
}