using Auth.Service.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Auth.Service;

[ApiController, Route("api/auth/user")]
public class UserController : ControllerBase
{
    private UserManager<User> _UserManager;
    private SignInManager<User> _SignInManager;
    private ILogger<UserController> _Logger;

    public UserController(UserManager<User> manager, SignInManager<User> signInManager, ILogger<UserController> logger)
    {
        _UserManager = manager;
        _SignInManager = signInManager;
        _Logger = logger;
    }

    [HttpGet("[action]")]
    [Authorize()]
    public UserProfile Profile()
    {
        return new UserProfile(HttpContext.User);
    }
    
    [HttpPost("[action]")]
    public async Task<bool> Authenticate([FromBody] AuthenticationRequest request)
    {
        _Logger.Log(LogLevel.Debug, $"Authentication request: {request.Identifier}");

        User? user = request.Identifier.Contains('@')
            ? await _UserManager.FindByEmailAsync(request.Identifier)
            : await _UserManager.FindByNameAsync(request.Identifier);

        if (user == null)
        {
            _Logger.LogInformation($"No user found: {request.Identifier}");
            return false;
        }
        user.EmailConfirmed = true;
        
        _Logger.LogInformation(request.Password);
        SignInResult result = await _SignInManager.PasswordSignInAsync(user, request.Password, true, false);
        if (result.Succeeded)
        {
            _Logger.Log(LogLevel.Information, $"Successfully authenticated user \"{request.Identifier}\".");
            return true;
        }
        _Logger.LogInformation(result.IsNotAllowed.ToString());
        _Logger.Log(LogLevel.Information, $"Failed to authenticate user \"{request.Identifier}\".");
        return false;
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Unauthenticate()
    {
        await _SignInManager.SignOutAsync();
        return Ok();
    }
    
    [HttpPost("[action]")]
    public async Task<string[]> Register([FromBody] RegistrationRequest request)
    {
        IDisposable? scope = _Logger.BeginScope("register");
        var user = new User { Email = request.Email, UserName = request.Username, Id = Guid.NewGuid()};
        IdentityResult result = await _UserManager.CreateAsync(user, request.Password);
        if (result.Succeeded)
        {
            _Logger.Log(LogLevel.Information, $"Successfully registered user \"{request.Email}\".");
            return Array.Empty<string>();
        }
        
        _Logger.Log(LogLevel.Information, $"Failed to register user \"{request.Email}\"");
        _Logger.Log(LogLevel.Information, $"Errors:");
        foreach (IdentityError err in result.Errors)
        {
            _Logger.Log(LogLevel.Information, err.Description);
        }
        scope?.Dispose();
        return result.Errors.Select(e => e.Description).ToArray();
    }
}