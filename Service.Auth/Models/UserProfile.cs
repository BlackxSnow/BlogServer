using System.Security.Claims;

namespace Auth.Service.Models;

public class UserProfile
{
    public string Name { get; }
    public string Email { get; }

    public UserProfile(ClaimsPrincipal claims)
    {
        Name = claims.FindFirstValue(ClaimTypes.Name)!;
        Email = claims.FindFirstValue(ClaimTypes.Email)!;
    }
}