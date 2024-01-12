using FPP.BlazorOidcAuthenticationHelper.Contracts;
using Microsoft.AspNetCore.Components;

namespace FPP.BlazorOidcAuthenticationHelper.Components;
public partial class AuthenticationLockView
{
    [Inject] private IAuthenticationLock? AuthenticationLock { get; set; }

    [Parameter] public RenderFragment? Locked { get; set; }
    [Parameter] public required RenderFragment Unlocked { get; set; }

    protected override void OnInitialized()
    {
        if (AuthenticationLock == null)
        {
            return;
        }

        AuthenticationLock.Locked += AuthenticationLock_Locked;
        AuthenticationLock.Unlocked += AuthenticationLock_Unlocked;
    }

    private void AuthenticationLock_Unlocked(object? sender, EventArgs e)
    {
        StateHasChanged();
    }

    private void AuthenticationLock_Locked(object? sender, EventArgs e)
    {
        StateHasChanged();
    }
}
