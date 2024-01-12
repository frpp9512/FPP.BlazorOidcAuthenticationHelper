using Microsoft.AspNetCore.Components;
using System.Text.Json;

namespace FPP.BlazorOidcAuthenticationHelper.Client.Pages;

public partial class Dogs
{
    private string? Url { get; set; }
    private bool Error { get; set; }
    [Inject] private IHttpClientFactory? HttpClientFactory { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        await LoadDogImageAsync();
    }

    private async Task LoadDogImageAsync()
    {
        if (HttpClientFactory is null)
        {
            return;
        }

        var client = HttpClientFactory.CreateClient("dogs");
        var response = await client.GetAsync("breeds/image/random");
        if (!response.IsSuccessStatusCode)
        {
            Error = true;
            return;
        }

        var content = await response.Content.ReadAsStreamAsync();
        var apiImageResponse = await JsonSerializer.DeserializeAsync<DogImageApiResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Url = apiImageResponse?.Message;
        StateHasChanged();
    }

    public record DogImageApiResponse
    {
        public required string Message { get; init; }
        public string? Status { get; init; }
    }
}
