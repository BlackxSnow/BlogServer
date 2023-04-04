namespace APIGateway.Services;

public interface IAuthenticator
{
    bool Validate(AuthenticationToken token);

    bool ActivateToken(AuthenticationToken token);

    bool DeactivateToken(AuthenticationToken token);

    AuthenticationToken GenerateToken(string id);
}