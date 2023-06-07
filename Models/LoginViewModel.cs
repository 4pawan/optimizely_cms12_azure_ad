using System.ComponentModel.DataAnnotations;

namespace optimizely_cms12_azure_ad.Models;

public class LoginViewModel
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }
}
