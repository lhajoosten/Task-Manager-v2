using System.ComponentModel.DataAnnotations;

namespace TaskManager.Api.Models.Auth;

public class RegisterRequest
{
	[Required]
	[EmailAddress]
	[StringLength(255, ErrorMessage = "Email must not exceed 255 characters")]
	public string Email { get; set; } = string.Empty;

	[Required]
	[StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 100 characters")]
	[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).*$", ErrorMessage = "Password must contain at least one lowercase letter, one uppercase letter, and one digit")]
	public string Password { get; set; } = string.Empty;

	[Required]
	[Compare("Password", ErrorMessage = "Password and confirmation password do not match")]
	public string ConfirmPassword { get; set; } = string.Empty;

	[Required]
	[StringLength(100, ErrorMessage = "First name must not exceed 100 characters")]
	public string FirstName { get; set; } = string.Empty;

	[Required]
	[StringLength(100, ErrorMessage = "Last name must not exceed 100 characters")]
	public string LastName { get; set; } = string.Empty;
}
