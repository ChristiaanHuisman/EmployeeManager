using System.ComponentModel.DataAnnotations;

namespace Q3_CodeLink_EMS.Models
{
    public class Employee
    {
        public Guid Id { get; set; } // Guid used for unique Id and value type can never be null

        [Required]
        public required string FullName { get; set; } // Required field
        // Would request to split FullName into FirstName and LastName, even possibly add a nullable MiddleName

        [Required]
        public required string EmailAddress { get; set; } // Required field

        public string? JobTitle { get; set; } // Can be null

        public string? Department { get; set; } // Can be null
    }
}
