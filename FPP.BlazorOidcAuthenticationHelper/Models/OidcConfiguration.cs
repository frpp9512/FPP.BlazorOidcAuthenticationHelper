namespace FPP.BlazorOidcAuthenticationHelper.Models;
public record OidcConfiguration
{
    public required string MetadataUrl { get; init; }
    public required string Authority { get; init; }
    public required string ClientId { get; init; }
    public required string ResponseType { get; init; } // default value: "code"
    public string[]? DefaultScopes { get; set; } // default value: ["openid", "profile", "email"]
    public string? PostLogoutRedirectRoute { get; set; }
}
