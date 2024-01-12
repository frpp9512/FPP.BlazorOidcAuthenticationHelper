namespace FPP.BlazorOidcAuthenticationHelper.Contracts;
public interface ISecureStorageService
{
    /// <summary>
    /// Set a value securely in the local storage.
    /// </summary>
    /// <typeparam name="T">The data type of the value to be stored.</typeparam>
    /// <param name="key">The key of the stored value.</param>
    /// <param name="value">The value of type <typeparamref name="T"/> to store.</param>
    Task SetValueAsync<T>(string key, T value);

    /// <summary>
    /// Get a secure value from the local storage.
    /// </summary>
    /// <typeparam name="T">The data type of the value stored.</typeparam>
    /// <param name="key">The key of the stored value.</param>
    /// <returns>The stored value of type <typeparamref name="T"/>.</returns>
    Task<T?> GetValueAsync<T>(string key);

    /// <summary>
    /// Get or create a value from the local storage using the value factory.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The key used to store the value.</param>
    /// <param name="valueFactory">The factory function used for generate a new value.</param>
    /// <returns>The stored and/or generated value.</returns>
    Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T>> valueFactory);

    /// <summary>
    /// Removes a value from the local storage.
    /// </summary>
    /// <param name="key">The key to remove the value.</param>
    Task RemoveValueAsync(string key);

    /// <summary>
    /// Retrieves and removes from storage a value with the given key.
    /// </summary>
    /// <param name="key">The key used for store the value.</param>
    /// <returns>The stored value of type <typeparamref name="T"/>.</returns>
    Task<T?> PopValueAsync<T>(string key);
}
