using System.ComponentModel.DataAnnotations;

namespace BungalowApi.Web.ViewModels;

public class LoginVM
{
    [Required] 
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    public bool RememberMe { get; set; }
    public string? RedirectUrl { get; set; }
}