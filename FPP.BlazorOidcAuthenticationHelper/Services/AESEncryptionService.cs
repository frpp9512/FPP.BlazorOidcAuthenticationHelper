using FPP.BlazorOidcAuthenticationHelper.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace FPP.BlazorOidcAuthenticationHelper.Services;
public class AESEncryptionService(IJSRuntime jsRuntime, ISecureKeyProvider secureKeyProvider, ILogger<AESEncryptionService> logger) : IEncryptionService
{
    private readonly IJSRuntime _jsRuntime = jsRuntime;
    private readonly ISecureKeyProvider _secureKeyProvider = secureKeyProvider;
    private readonly ILogger<AESEncryptionService> _logger = logger;

    public async Task<string> Encrypt(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        try
        {
            var secureKey = await _secureKeyProvider.GetKeyAsync();
            var key = secureKey.Select(x => (int)x).ToArray();
            return await _jsRuntime.InvokeAsync<string>("encryptText", value, key);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while encrypting value. Error message: {errorMessage}", ex.Message);
            return string.Empty;
        }
    }

    public async Task<string> Decrypt(string encryptedValue)
    {
        if (string.IsNullOrEmpty(encryptedValue))
        {
            return string.Empty;
        }

        try
        {
            var secureKey = await _secureKeyProvider.GetKeyAsync();
            var key = secureKey.Select(x => (int)x).ToArray();
            return await _jsRuntime.InvokeAsync<string>("decryptText", encryptedValue, key);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while decrypting value. Error message: {errorMessage}", ex.Message);
            return string.Empty;
        }
    }
}