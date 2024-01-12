using FPP.BlazorOidcAuthenticationHelper.Models;

namespace FPP.BlazorOidcAuthenticationHelper.Contracts;

public interface IAuthenticationService
{
    /// <summary>
    /// Defines if any user is currently authenticated.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Get current authenticated user instance.
    /// </summary>
    User? User { get; }

    /// <summary>
    /// Initializes the login flow.
    /// </summary>
    void Login();

    /// <summary>
    /// Initializes the logout flow.
    /// </summary>
    Task LogoutAsync(string postLogoutRedirectUri = "/");

    /// <summary>
    /// Loads the stored authentication state.
    /// </summary>
    /// <returns></returns>
    Task LoadStoredAuthStateAsync();
    Task UpdateStateAsync();

    /// <summary>
    /// Triggers when the login status changes.
    /// </summary>
    event EventHandler<bool> UserLogStatusChanged;
}
