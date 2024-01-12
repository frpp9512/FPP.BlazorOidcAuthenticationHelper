namespace FPP.BlazorOidcAuthenticationHelper.Services;
public record TokenResponse
{
    public required string Access_token { get; init; }
    public int Expires_in { get; init; }
    public int Refresh_expires_in { get; init; }
    public required string Refresh_token { get; init; }
    public required string Token_type { get; init; }
    public required string Id_token { get; init; }
    public int Notbeforepolicy { get; init; }
    public required string Session_state { get; init; }
    public required string Scope { get; init; }
}
