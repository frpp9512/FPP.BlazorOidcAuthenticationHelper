namespace FPP.BlazorOidcAuthenticationHelper.Models;

public record AuthenticationState
{
    public bool IsAuthenticated { get; set; }
    public User? LoggedUser { get; set; }
}
