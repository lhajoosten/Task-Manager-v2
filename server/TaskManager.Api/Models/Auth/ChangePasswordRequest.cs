using System.ComponentModel.DataAnnotations;

namespace TaskManager.Api.Models.Auth;

public class ChangePasswordRequest
{
    [Required]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 100 characters")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).*$", ErrorMessage = "Password must contain at least one lowercase letter, one uppercase letter, and one digit")]
    public string NewPassword { get; set; } = string.Empty;

    [Required]
    [Compare("NewPassword", ErrorMessage = "New password and confirmation password do not match")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}
