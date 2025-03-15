namespace Swapify.Api;

public class ApiOptions
{
    public string ApiSecret { get; set; }
    public string ApiScope  { get; set; }
    public TokenOptions TokenOptions { get; set; }
}