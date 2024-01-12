using Blazored.LocalStorage;
using FPP.BlazorOidcAuthenticationHelper.Contracts;
using FPP.BlazorOidcAuthenticationHelper.Handlers;
using FPP.BlazorOidcAuthenticationHelper.Models;
using FPP.BlazorOidcAuthenticationHelper.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FPP.BlazorOidcAuthenticationHelper.Helpers.DI;
public static class DIHelper
{
    public static IServiceCollection AddBlazorOidcAuthentication(this IServiceCollection services, Action<BlazorAuthenticationConfiguration> config)
    {
        var authConfig = new BlazorAuthenticationConfiguration();
        config(authConfig);
        var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
        var oidcConfigSection = configuration.GetSection(authConfig.OidcConfigurationSectionName);
        _ = services.Configure<OidcConfiguration>(oidcConfigSection);
        var oidcConfig = oidcConfigSection.Get<OidcConfiguration>();
        if (oidcConfig is null)
        {
            return services;
        }

        _ = services.AddOidcAuthentication(config =>
        {
            config.ProviderOptions.MetadataUrl = oidcConfig.MetadataUrl;
            config.ProviderOptions.Authority = oidcConfig.Authority;
            config.ProviderOptions.ClientId = oidcConfig.ClientId;
            config.ProviderOptions.ResponseType = oidcConfig.ResponseType;
            config.UserOptions.NameClaim = "name";
            if (oidcConfig.DefaultScopes is not null)
            {
                foreach (var scope in oidcConfig.DefaultScopes)
                {
                    config.ProviderOptions.DefaultScopes.Add(scope);
                }
            }
        });

        _ = services.AddScoped<ITokenService, TokenService>();
        _ = services.AddScoped<IAuthenticationService, AuthenticationService>();
        _ = services.AddScoped<IAuthenticationLock, AuthenticationLock>();
        services.AddTransient<CheckTokenHandler>();
        services.AddBlazoredLocalStorageAsSingleton();
        services.AddSecureLocalStorage(config => config.StaticEncryptionKey = authConfig.StorageEncryptionKey);

        return services;
    }

    /// <summary>
    /// Add and configures the <see cref="ISecureStorageService"/> implementation.
    /// </summary>
    public static IServiceCollection AddSecureLocalStorage(this IServiceCollection services, Action<SecureLocalStorageConfig> configuration)
    {
        var configValue = new SecureLocalStorageConfig();
        configuration.Invoke(configValue);
        _ = services.AddBlazoredLocalStorage();
        _ = services.AddSingleton<ISecureKeyProvider>(new StaticSecureKeyProvider(configValue.StaticEncryptionKey));
        _ = services.AddTransient<IEncryptionService, AESEncryptionService>();
        _ = services.AddTransient<ISecureStorageService, AESEncryptedSecureStorageService>();
        return services;
    } 
}

public record BlazorAuthenticationConfiguration
{
    public string OidcConfigurationSectionName { get; set; } = "OidcConfiguration";
    public string StorageEncryptionKey { get; set; } = "d1.2.1md21o2d099120d9__0921di019i20di0912";
}