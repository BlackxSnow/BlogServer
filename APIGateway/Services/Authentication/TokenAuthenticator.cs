using System.Text.Json;

namespace APIGateway.Services.Authentication;

public class TokenAuthenticator : IAuthenticator
{
    private readonly string TokenPath;
    
    private readonly HashSet<AuthenticationToken> _ValidTokens = new();
    private readonly HashSet<AuthenticationToken> _ActiveTokens = new();

    public TokenAuthenticator(string tokenPath = "data/validtokens.json")
    {
        TokenPath = tokenPath;
        if (!File.Exists(tokenPath)) return;
        var tokens = JsonSerializer.Deserialize<List<AuthenticationToken>>(File.ReadAllText(tokenPath));
        if (tokens == null) return;
        foreach (AuthenticationToken token in tokens)
        {
            _ValidTokens.Add(token);
        }
    }

    private void SerialiseTokens()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(TokenPath) ?? throw new InvalidOperationException());
        File.WriteAllText(TokenPath, JsonSerializer.Serialize(_ValidTokens));
    }
    
    public bool Validate(AuthenticationToken token)
    {
        return _ValidTokens.Contains(token) && !_ActiveTokens.Contains(token);
    }

    public bool ActivateToken(AuthenticationToken token)
    {
        bool isValid = Validate(token);
        if (isValid) _ActiveTokens.Add(token);
        return isValid;
    }

    public bool DeactivateToken(AuthenticationToken token)
    {
        return _ActiveTokens.Remove(token);
    }

    public AuthenticationToken GenerateToken(string id)
    {
        var token = new AuthenticationToken(id);
        _ValidTokens.Add(token);
        SerialiseTokens();
        return token;
    }
}