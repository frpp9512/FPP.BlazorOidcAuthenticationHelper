using FPP.BlazorOidcAuthenticationHelper.Contracts;
using Microsoft.AspNetCore.Components;

namespace FPP.BlazorOidcAuthenticationHelper.Pages;

public partial class AuthenticationPage
{
    private const string _callbackAction = "login-callback";
    [Inject] private IAuthenticationLock? AuthenticationLock { get; set; }

    [Parameter] public required string Action { get; set; }

    protected override void OnParametersSet()
    {
        if (Action == _callbackAction && AuthenticationLock is not null)
        {
            AuthenticationLock.Lock();
        }
    }
}
