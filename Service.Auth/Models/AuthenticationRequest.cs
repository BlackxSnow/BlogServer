namespace Auth.Service.Models;

public class AuthenticationRequest
{
    public string Identifier { get; set; }
    public string Password { get; set; }
}