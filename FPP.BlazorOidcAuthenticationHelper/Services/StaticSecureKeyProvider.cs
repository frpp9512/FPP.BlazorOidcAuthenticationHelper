using FPP.BlazorOidcAuthenticationHelper.Contracts;
using System.Text;

namespace FPP.BlazorOidcAuthenticationHelper.Services;
public class StaticSecureKeyProvider : ISecureKeyProvider
{
    private readonly string _key;

    public StaticSecureKeyProvider() => _key = "qopd9210p1k92ke019kd0 12k9dk012kd";

    public StaticSecureKeyProvider(string key) => _key = key;

    public byte[] GetKey() => Encoding.UTF8.GetBytes(_key).Take(32).ToArray();
    public Task<byte[]> GetKeyAsync() => Task.FromResult(Encoding.UTF8.GetBytes(_key).Take(32).ToArray());
    public string GetKeyValue() => _key;
    public Task<string> GetKeyValueAsync() => Task.FromResult(_key);
}