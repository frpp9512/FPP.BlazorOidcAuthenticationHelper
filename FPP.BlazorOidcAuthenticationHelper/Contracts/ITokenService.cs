
namespace FPP.BlazorOidcAuthenticationHelper.Contracts;
public interface ITokenService
{
    event EventHandler LogoutRequested;

    /// <summary>
    /// Get the actual id token for the authenticated user.
    /// </summary>
    /// <returns>The user id token.</returns>
    public Task<string?> GetIdTokenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the actual access token for the authenticated user.
    /// </summary>
    /// <returns>The user access token.</returns>
    public Task<string?> GetAccessTokenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Resets the tokens cache async.
    /// </summary>
    public Task ResetCacheAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates all the token values.
    /// </summary>
    Task UpdateTokensAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the actual refresh token value.
    /// </summary>
    Task<string?> GetRefreshTokenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Refresh the token values from the identity server.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> RefreshTokenAsync(CancellationToken cancellationToken = default);
    Task<Dictionary<string, object>?> GetIdTokenClaimsAsync();

    /// <summary>
    /// The current cached value of the access token.
    /// </summary>
    public string? AccessToken { get; }

    /// <summary>
    /// The current cached value of the identity token.
    /// </summary>
    public string? IdToken { get; }

    /// <summary>
    /// The current cached value of the refresh token.
    /// </summary>
    string? RefreshToken { get; }

    /// <summary>
    /// The current issued date of the access token.
    /// </summary>
    DateTime? Issued { get; }

    /// <summary>
    /// The current expiration date of the access token.
    /// </summary>
    DateTime? Expires { get; }

    /// <summary>
    /// <see langword="true"/> if the access token is currently expired.
    /// </summary>
    bool IsExpired { get; }
}
