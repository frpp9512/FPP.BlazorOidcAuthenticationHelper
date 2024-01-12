using FPP.BlazorOidcAuthenticationHelper.Contracts;
using System.Net.Http.Headers;

namespace FPP.BlazorOidcAuthenticationHelper.Handlers;
public class CheckTokenHandler(ITokenService tokenService) : DelegatingHandler
{
    private readonly ITokenService _tokenService = tokenService;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await SendRequestAsync(request, cancellationToken);
        if (response.StatusCode != System.Net.HttpStatusCode.Unauthorized)
        {
            return response;
        }

        if (await _tokenService.RefreshTokenAsync(cancellationToken))
        {
            return await SendRequestAsync(request, cancellationToken);
        }

        return new HttpResponseMessage
        {
            StatusCode = System.Net.HttpStatusCode.Unauthorized,
            ReasonPhrase = "The token is expired or invalid. Requested logout.",
            RequestMessage = request
        };
    }

    private async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _tokenService.GetAccessTokenAsync(cancellationToken);
        if (token is null)
        {
            return await base.SendAsync(request, cancellationToken);
        }

        var authorizationHeader = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Authorization = authorizationHeader;
        var response = await base.SendAsync(request, cancellationToken);
        return response;
    }
}
