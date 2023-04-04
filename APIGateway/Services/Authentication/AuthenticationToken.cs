using System.Text.Json.Serialization;

namespace APIGateway.Services;

public readonly struct AuthenticationToken
{
    public string Id { get; init; }
    public Guid Token { get; init; }

    public AuthenticationToken(string id)
    {
        Id = id;
        Token = Guid.NewGuid();
    }
    public AuthenticationToken(string id, Guid token)
    {
        Id = id;
        Token = token;
    }

    public override int GetHashCode() => Token.GetHashCode();
    public override bool Equals(object? obj)
    {
        return obj is AuthenticationToken other && other.Token == Token;
        
    }
}