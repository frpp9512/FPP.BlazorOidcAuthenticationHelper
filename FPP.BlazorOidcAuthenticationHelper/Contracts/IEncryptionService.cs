namespace FPP.BlazorOidcAuthenticationHelper.Contracts;

public interface IEncryptionService
{
    Task<string> Encrypt(string value);
    Task<string> Decrypt(string value);
}