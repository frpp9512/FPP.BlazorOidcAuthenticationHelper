using FPP.BlazorOidcAuthenticationHelper.Contracts;
using FPP.BlazorOidcAuthenticationHelper.Helpers;
using FPP.BlazorOidcAuthenticationHelper.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace FPP.BlazorOidcAuthenticationHelper.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly NavigationManager _navigationManager;
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly OidcConfiguration _oidcConfiguration;
    private readonly ITokenService _tokenService;
    private readonly IJSRuntime _jSRuntime;
    private readonly IAuthenticationLock _authenticationLock;
    private readonly ISecureStorageService _storageService;
    private readonly ILogger<AuthenticationService> _logger;

    public event EventHandler<bool> UserLogStatusChanged = delegate { };
    public bool IsAuthenticated { get; private set; }
    public User? User { get; private set; }

    public AuthenticationService(NavigationManager navigationManager,
                                 AuthenticationStateProvider stateProvider,
                                 IOptions<OidcConfiguration> oidcConfigOptions,
                                 ITokenService tokenService,
                                 IJSRuntime jSRuntime,
                                 IAuthenticationLock authenticationLock,
                                 ISecureStorageService storageService,
                                 ILogger<AuthenticationService> logger)
    {
        _navigationManager = navigationManager;
        _authStateProvider = stateProvider;
        _oidcConfiguration = oidcConfigOptions.Value;
        _tokenService = tokenService;
        _jSRuntime = jSRuntime;
        _authenticationLock = authenticationLock;
        _storageService = storageService;
        _logger = logger;
        _tokenService.LogoutRequested += TokenService_LogoutRequestedAsync;
    }

    private async void TokenService_LogoutRequestedAsync(object? sender, EventArgs e)
    {
        await _storageService.SetValueAsync(StorageKeysConstants.SESSION_TIMEOUT_STORAGEKEY, true);
        await LogoutAsync();
    }

    public void Login() => _navigationManager.NavigateToLogin("authentication/login");

    public async Task LogoutAsync(string redirectRoute = "/")
    {
        await _jSRuntime.InvokeVoidAsync("removeAuthState");
        var idToken = await _tokenService.GetIdTokenAsync();
        var appBaseUrl = UrlEncoder.Default.Encode(_navigationManager.BaseUri);
        var postLogoutRedirectUri = $"{appBaseUrl}/{redirectRoute ?? _oidcConfiguration.PostLogoutRedirectRoute ?? ""}";
        var redirectUri = $"{_oidcConfiguration.Authority}/protocol/openid-connect/logout?post_logout_redirect_uri={postLogoutRedirectUri}&id_token_hint={idToken}";
        _navigationManager.NavigateTo(redirectUri, true);
    }

    public async Task LoadStoredAuthStateAsync()
    {
        var isAuthenticated = await _jSRuntime.InvokeAsync<bool>("loadAuthState");
        if (isAuthenticated)
        {
            _authenticationLock.Lock();
        }
    }

    public async Task UpdateStateAsync()
    {
        _logger?.LogInformation("Starting to update the authentication state.");

        _logger?.LogInformation("Updating the tokens state.");
        await _tokenService.UpdateTokensAsync();
        _logger?.LogInformation("Tokens state updated.");

        var state = await _authStateProvider.GetAuthenticationStateAsync();
        var authState = new Models.AuthenticationState
        {
            IsAuthenticated = state.User.Identity?.IsAuthenticated is true
        };

        if (authState.IsAuthenticated)
        {
            var tokenClaims = await _tokenService.GetIdTokenClaimsAsync();
            authState.LoggedUser = new User
            {
                Id = tokenClaims?.GetValueOrDefault("sid")?.ToString(),
                Name = state.User.Identity?.Name,
                LastName = tokenClaims?.GetValueOrDefault("given_name")?.ToString(),
                Email = tokenClaims?.GetValueOrDefault("email")?.ToString(),
                Username = tokenClaims?.GetValueOrDefault("preferred_username")?.ToString()
            };
        }

        SetState(authState);
        _logger?.LogInformation("Log in completed for user: {userName}.", state.User.Identity?.Name);
    }

    private void SetState(Models.AuthenticationState authenticationState)
    {
        IsAuthenticated = authenticationState.IsAuthenticated;
        User = authenticationState.LoggedUser;
        UserLogStatusChanged?.Invoke(this, IsAuthenticated);
        if (IsAuthenticated)
        {
            _authenticationLock.Unlock();
        }
    }
}
