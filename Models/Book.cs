namespace LibraryManagementSystem.Models
{
    public class Book
    {
        public int BookId { get; set; }

        public string BookName { get; set; } = string.Empty;
        public string BookTypeName { get; set; } = string.Empty;

        public int BookTypeId { get; set; }

        public int TotalQuantity { get; set; }

        public int AvailableQuantity { get; set; }
    }
}