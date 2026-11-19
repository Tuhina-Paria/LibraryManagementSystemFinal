
namespace LibraryManagementSystem.Models
{
    public class BookTransaction
    {
        public int TransactionId { get; set; }

        public string ReferenceNo { get; set; } = string.Empty;

        public DateTime ReferenceDate { get; set; }

        public int StudentId { get; set; }

        public int BookId { get; set; }

        public int Quantity { get; set; }

        public string TransactionType { get; set; } = string.Empty;

        // Display properties for Transaction Grid
        public string StudentName { get; set; } = string.Empty;

        public string BookName { get; set; } = string.Empty;

        public string BookTypeName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public int BookTypeId { get; set; }

    }

}
