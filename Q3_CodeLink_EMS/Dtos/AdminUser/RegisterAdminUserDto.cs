using System.ComponentModel.DataAnnotations;
using static Q3_CodeLink_EMS.Models.AdminUser;

namespace Q3_CodeLink_EMS.Dtos.AdminUser
{
    public class RegisterAdminUserDto
    {
        [Required]
        public string FullName { get; set; }

        [Required, EmailAddress]
        public string EmailAddress { get; set; }

        [Required]
        public string PasswordInPlainText { get; set; } // Plain text password from input

        [Required, Compare("PasswordInPlainText", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } // Checking if both passwords entered match

        [Required]
        public UserRole Role { get; set; } = UserRole.Admin; // Default to Admin if no value is provided
    }
}
