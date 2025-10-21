using System.ComponentModel.DataAnnotations;

namespace Q3_CodeLink_EMS.Dtos.AdminUser
{
    public class LoginAdminUserDto
    {
        [Required, EmailAddress]
        public string EmailAddress { get; set; }

        [Required]
        public string PasswordInPlainText { get; set; }
    }
}
