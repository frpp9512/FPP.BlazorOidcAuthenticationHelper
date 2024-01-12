namespace FPP.BlazorOidcAuthenticationHelper.Contracts;

public interface ISecureKeyProvider
{
    string GetKeyValue();
    Task<string> GetKeyValueAsync();
    byte[] GetKey();
    Task<byte[]> GetKeyAsync();
}