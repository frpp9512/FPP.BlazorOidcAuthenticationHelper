using System.Text.Json;

namespace FPP.BlazorOidcAuthenticationHelper.Helpers;
internal static class SerializationConstants
{
    public static JsonSerializerOptions BasicJsonSerializerOptions => new() { PropertyNameCaseInsensitive = true };
}
