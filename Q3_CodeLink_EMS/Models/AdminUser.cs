using System.ComponentModel.DataAnnotations;

namespace Q3_CodeLink_EMS.Models
{
    public class AdminUser
    {
        public Guid Id { get; set; } // Guid used for unique Id and value type can never be null

        [Required]
        public required string FullName { get; set; } // Required field
        // Would request to split FullName into FirstName and LastName, even possibly add a nullable MiddleName

        [Required]
        public required string EmailAddress { get; set; } // Required field

        public byte[] PasswordSalt { get; set; } // Salt used for hashing

        public byte[] PasswordHash { get; set; } // Hashed password

        public UserRole Role {  get; set; } // Will default to Admin if no role is specified

        public enum UserRole // Differnet possible AdminUser roles
        {
            Admin,
            SuperAdmin
        }
    }
}
