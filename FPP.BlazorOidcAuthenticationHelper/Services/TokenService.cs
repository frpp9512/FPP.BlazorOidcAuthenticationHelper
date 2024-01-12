using FPP.BlazorOidcAuthenticationHelper.Contracts;
using FPP.BlazorOidcAuthenticationHelper.Helpers;
using FPP.BlazorOidcAuthenticationHelper.Models;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using System.Text.Json;
using System.Threading;

namespace FPP.BlazorOidcAuthenticationHelper.Services;
public class TokenService(IJSRuntime jsRuntime, IOptions<OidcConfiguration> oidcConfiguration, ILogger<TokenService> logger) : ITokenService
{
    private readonly IJSRuntime _jsRuntime = jsRuntime;
    private readonly ILogger<TokenService> _logger = logger;
    private readonly OidcConfiguration _oidcConfiguration = oidcConfiguration.Value;
    private string? _tokenEndpoint;

    public string? AccessToken { get; private set; } = null;
    public string? IdToken { get; private set; } = null;
    public string? RefreshToken { get; private set; } = null;

    public DateTime? Issued { get; private set; }
    public DateTime? Expires { get; private set; }

    public bool IsExpired => Expires <= DateTime.Now;

    public event EventHandler LogoutRequested = delegate { };

    public async Task<string?> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrEmpty(AccessToken))
        {
            return AccessToken;
        }

        AccessToken = await GetSessionStringValue("getAccessToken", GetSessionKey(), cancellationToken);
        if (AccessToken is null)
        {
            return AccessToken;
        }

        await UpdateExpirationDateAsync(cancellationToken);

        return AccessToken;
    }

    public async Task<string?> GetIdTokenAsync(CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrEmpty(IdToken))
        {
            return IdToken;
        }

        IdToken = await GetSessionStringValue("getIdToken", GetSessionKey(), cancellationToken);
        return IdToken;
    }

    public async Task<string?> GetRefreshTokenAsync(CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrEmpty(RefreshToken))
        {
            return RefreshToken;
        }

        RefreshToken = await GetSessionStringValue("getRefreshToken", GetSessionKey(), cancellationToken);
        return RefreshToken;
    }

    public async Task UpdateTokensAsync(CancellationToken cancellationToken = default)
    {
        await ResetCacheAsync(cancellationToken);
        var value = await _jsRuntime.InvokeAsync<string[]?>(
            "getTokens",
            cancellationToken,
            GetSessionKey());
        if (value is string[] tokens && tokens.Length == 3)
        {
            AccessToken = tokens[0];
            IdToken = tokens[1];
            RefreshToken = tokens[2];
            await UpdateExpirationDateAsync(cancellationToken);
        }
    }

    public async Task<bool> RefreshTokenAsync(CancellationToken cancellationToken = default)
    {
        if (!IsExpired)
        {
            return true;
        }

        if (_tokenEndpoint is null)
        {
            await UpdateTokenEndpointFromMetadataAsync(cancellationToken);
        }

        var refreshToken = await GetRefreshTokenAsync(cancellationToken);
        if (refreshToken is null)
        {
            LogoutRequested.Invoke(this, EventArgs.Empty);
            return false;
        }

        var requestContent = new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                { "grant_type", "refresh_token" },
                { "refresh_token", refreshToken },
                { "client_id", _oidcConfiguration.ClientId }
            });

        using var client = new HttpClient();
        var refreshTokenResult = await client.PostAsync(
            _tokenEndpoint,
            requestContent,
            cancellationToken);

        if (!refreshTokenResult.IsSuccessStatusCode)
        {
            LogoutRequested.Invoke(this, EventArgs.Empty);
            return false;
        }

        var requestResponse = await refreshTokenResult.Content.ReadAsStreamAsync(cancellationToken);
        var tokenResponse = await JsonSerializer.DeserializeAsync<TokenResponse>(
            requestResponse,
            SerializationConstants.BasicJsonSerializerOptions,
            cancellationToken)
            ?? throw new InvalidDataException("The received token request response is invalid and couldn't be deserialized.");

        await _jsRuntime.InvokeVoidAsync(
            "setAuthState",
            cancellationToken,
            $"oidc.user:{_oidcConfiguration.Authority}:{_oidcConfiguration.ClientId}",
            tokenResponse);

        AccessToken = tokenResponse.Access_token;
        IdToken = tokenResponse.Id_token;
        RefreshToken = tokenResponse.Refresh_token;

        await UpdateExpirationDateAsync(cancellationToken);

        return true;
    }

    public async Task<Dictionary<string, object>?> GetIdTokenClaimsAsync()
    {
        if (IdToken is null)
        {
            return null;
        }

        var tokenParts = IdToken.Split('.');
        var convertedPayload = Base64UrlTextEncoder.Decode(tokenParts[1]);
        var serializationOptions = SerializationConstants.BasicJsonSerializerOptions;
        var tokenClaims = await JsonSerializer.DeserializeAsync<Dictionary<string, object>>(
            new MemoryStream(convertedPayload),
            serializationOptions);

        return tokenClaims;
    }

    public Task ResetCacheAsync(CancellationToken cancellationToken = default)
    {
        IdToken = null;
        AccessToken = null;
        RefreshToken = null;
        Issued = null;
        Expires = null;
        return Task.CompletedTask;
    }

    private async Task UpdateTokenEndpointFromMetadataAsync(CancellationToken cancellationToken)
    {
        using var client = new HttpClient();
        var metadataResult = await client.GetAsync(_oidcConfiguration.MetadataUrl, cancellationToken);
        if (!metadataResult.IsSuccessStatusCode)
        {
            return;
        }

        var requestResponse = await metadataResult.Content.ReadAsStreamAsync(cancellationToken);
        var metadataResponse = await JsonSerializer.DeserializeAsync<MetadataResponse>(requestResponse, SerializationConstants.BasicJsonSerializerOptions, cancellationToken);
        if (metadataResponse is null)
        {
            return;
        }

        _tokenEndpoint = metadataResponse.Token_endpoint;
    }

    private string GetSessionKey() => $"oidc.user:{_oidcConfiguration.Authority}:{_oidcConfiguration.ClientId}";

    private async Task<string?> GetSessionStringValue(string function, string sessionKey, CancellationToken cancellationToken = default)
    {
        var value = await _jsRuntime.InvokeAsync<string>(function, cancellationToken, sessionKey);
        return value;
    }

    private async Task UpdateExpirationDateAsync(CancellationToken cancellationToken = default)
    {
        if (AccessToken is null)
        {
            return;
        }

        var tokenParts = AccessToken.Split('.');
        var convertedPayload = Base64UrlTextEncoder.Decode(tokenParts[1]);
        var serializationOptions = SerializationConstants.BasicJsonSerializerOptions;
        var tokenClaims = await JsonSerializer.DeserializeAsync<Dictionary<string, object>>(
            new MemoryStream(convertedPayload),
            serializationOptions,
            cancellationToken);
        if (tokenClaims is null)
        {
            return;
        }

        var iatValue = tokenClaims.GetValueOrDefault("iat")?.ToString() ?? "0";
        var expValue = tokenClaims.GetValueOrDefault("exp")?.ToString() ?? "0";
        
        try
        {
            Issued = DateTimeOffset.FromUnixTimeSeconds(long.Parse(iatValue)).LocalDateTime;
            Expires = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expValue)).LocalDateTime;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while reading the expiration value from token. Read values: iat: {iat}, exp: {exp}. Error message: {errorMessage}", iatValue, expValue, ex.Message);
        }
    }
}
