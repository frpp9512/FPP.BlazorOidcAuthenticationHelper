using FPP.BlazorOidcAuthenticationHelper.Contracts;
using Microsoft.Extensions.Logging;

namespace FPP.BlazorOidcAuthenticationHelper.Services;

public class AuthenticationLock(ILogger<AuthenticationLock> logger) : IAuthenticationLock
{
    private readonly ILogger<AuthenticationLock> _logger = logger;

    public bool IsLocked { get; private set; }

    public event EventHandler Locked = delegate { };
    public event EventHandler Unlocked = delegate { };

    public void Lock()
    {
        IsLocked = true;
        Locked.Invoke(this, EventArgs.Empty);
        _logger.LogInformation("Authentication lock activated.");
    }

    public void Unlock()
    {
        IsLocked = false;
        Unlocked.Invoke(this, EventArgs.Empty);
        _logger.LogInformation("Authentication lock deactivated.");
    }
}
