using FPP.BlazorOidcAuthenticationHelper.Client;
using FPP.BlazorOidcAuthenticationHelper.Handlers;
using FPP.BlazorOidcAuthenticationHelper.Helpers.DI;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

builder.Services.AddScoped(sp =>
{
    var client = new HttpClient
    {
        BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
    };
    return client;
});

builder.Services.AddHttpClient("dogs", client => client.BaseAddress = new Uri("https://dog.ceo/api/"))
    .AddHttpMessageHandler<CheckTokenHandler>();

var config = builder.Services.BuildServiceProvider().GetRequiredService<IConfiguration>();

builder.Services.AddBlazorOidcAuthentication(
    config =>
    {
        config.OidcConfigurationSectionName = "OidcConfiguration";
        config.StorageEncryptionKey = "jd293j982j3dj32j8d32nd.pc238u3dp9823cu45";
    });

await builder.Build().RunAsync();
