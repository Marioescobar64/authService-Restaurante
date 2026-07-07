using System.ComponentModel.DataAnnotations;

namespace AuthService.Application.DTOs;

public class VerifyLoginDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public int Code { get; set; }
}
