using FPP.BlazorOidcAuthenticationHelper.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace FPP.BlazorOidcAuthenticationHelper.Components;

public partial class AuthenticationStateManager
{
    [Inject] private AuthenticationStateProvider? AuthenticationStateProvider { get; set; }
    [Inject] private IAuthenticationService? AuthenticationService { get; set; }

    protected override async void OnInitialized()
    {
        if (AuthenticationService is not null)
        {
            await AuthenticationService.LoadStoredAuthStateAsync();
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && AuthenticationStateProvider is not null)
        {
            AuthenticationStateProvider.AuthenticationStateChanged += AuthenticationStateProvider_AuthenticationStateChanged;
        }

        await UpdateStateAsync();
    }

    private void AuthenticationStateProvider_AuthenticationStateChanged(Task<AuthenticationState> task)
    {
        StateHasChanged();
    }

    private async Task UpdateStateAsync()
    {
        if (AuthenticationService is null)
        {
            return;
        }

        await AuthenticationService.UpdateStateAsync();
    }
}
