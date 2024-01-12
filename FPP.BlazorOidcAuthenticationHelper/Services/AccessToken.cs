namespace FPP.BlazorOidcAuthenticationHelper.Services;
public record AccessToken
{
    public long Exp { get; init; }
    public int Iat { get; init; }
    public int Auth_time { get; init; }
    public string? Jti { get; init; }
    public string? Iss { get; init; }
    public string? Aud { get; init; }
    public string? Sub { get; init; }
    public string? Typ { get; init; }
    public string? Azp { get; init; }
    public string? Session_state { get; init; }
    public string? Acr { get; init; }
    public string? Scope { get; init; }
    public string? Sid { get; init; }
    public string? Player_id { get; init; }
    public bool Email_verified { get; init; }
    public string? Name { get; init; }
    public string? Preferred_username { get; init; }
    public string? Given_name { get; init; }
}