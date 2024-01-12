namespace FPP.BlazorOidcAuthenticationHelper.Models;
public record User
{
    /// <summary>
    /// Id of the user.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Title of the user.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Name of the user.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Last name of the user.
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Username of the user.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Email of the user.
    /// </summary>
    public string? Email { get; set; }
}
