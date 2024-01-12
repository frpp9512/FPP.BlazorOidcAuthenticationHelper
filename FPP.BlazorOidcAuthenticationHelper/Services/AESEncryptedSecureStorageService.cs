using Blazored.LocalStorage;
using FPP.BlazorOidcAuthenticationHelper.Contracts;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace FPP.BlazorOidcAuthenticationHelper.Services;

public class AESEncryptedSecureStorageService(ILocalStorageService storageService, IEncryptionService encryptionService, ILogger<AESEncryptedSecureStorageService> logger) : ISecureStorageService
{
    private readonly ILocalStorageService _storageService = storageService;
    private readonly IEncryptionService _encryptionService = encryptionService;
    private readonly ILogger<AESEncryptedSecureStorageService> _logger = logger;

    public async Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T>> valueFactory)
    {
        var storedValue = await _storageService.GetItemAsync<string>(key);
        if (!string.IsNullOrEmpty(storedValue))
        {
            return await GetDeserializedValueAsync<T>(storedValue);
        }

        var newValue = await valueFactory();
        await SetValueAsync(key, newValue);
        return newValue;
    }

    public async Task<T?> GetValueAsync<T>(string key)
    {
        var storedValue = await _storageService.GetItemAsync<string>(key);
        if (string.IsNullOrEmpty(storedValue))
        {
            return default;
        }

        try
        {
            return await GetDeserializedValueAsync<T>(storedValue);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error parsing read data from the storage. Error message: {errorMessage}", ex.Message);
            return default;
        }
    }

    private async Task<T?> GetDeserializedValueAsync<T>(string storedValue)
    {
        var decryptedValue = await _encryptionService.Decrypt(storedValue);
        var value = JsonSerializer.Deserialize<T?>(decryptedValue);
        return value;
    }

    public async Task SetValueAsync<T>(string key, T value)
    {
        try
        {
            var serializedValue = JsonSerializer.Serialize(value);
            var encryptedValue = await _encryptionService.Encrypt(serializedValue);
            await _storageService.SetItemAsync(key, encryptedValue);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error setting value in the storage. Error message: {errorMessage}", ex.Message);
            throw;
        }
    }

    public async Task RemoveValueAsync(string key)
    {
        try
        {
            await _storageService.RemoveItemAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error removing value from the storage. Error message: {errorMessage}", ex.Message);
            throw;
        }
    }

    public async Task<T?> PopValueAsync<T>(string key)
    {
        try
        {
            var value = await GetValueAsync<T>(key);
            await RemoveValueAsync(key);
            return value;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error popping value in the storage. Error message: {errorMessage}", ex.Message);
            throw;
        }
    }
}