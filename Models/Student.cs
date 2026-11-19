
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class Student
    {
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Student name is required.")]
        [StringLength(100, ErrorMessage = "Student name cannot be longer than 100 characters.")]
        public string StudentName { get; set; } = string.Empty;

        [StringLength(15, ErrorMessage = "Phone number cannot be longer than 15 characters.")]
        [RegularExpression(@"^[0-9+\- ]*$", ErrorMessage = "Phone number can contain only digits, spaces, + and -.")]
        public string? PhoneNumber { get; set; }
    }
}
