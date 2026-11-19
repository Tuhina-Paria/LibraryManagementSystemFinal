using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class BookType
    {
        public int BookTypeId { get; set; }

        [Required(ErrorMessage = "Book type name is required.")]
        [StringLength(100, ErrorMessage = "Book type name cannot be longer than 100 characters.")]
        public string BookTypeName { get; set; } = string.Empty;
    }
}
