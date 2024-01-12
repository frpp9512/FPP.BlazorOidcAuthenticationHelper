namespace FPP.BlazorOidcAuthenticationHelper.Services;

public record MetadataResponse
{
    public required string Token_endpoint { get; init; }
}
