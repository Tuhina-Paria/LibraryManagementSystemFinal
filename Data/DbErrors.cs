using Microsoft.Data.SqlClient;

namespace LibraryManagementSystem.Data
{
    // Converts SQL Server errors into messages that are safe to show to the user.
    public static class DbErrors
    {
        public static string ToMessage(SqlException ex)
        {
            // Errors raised by our own stored procedures (THROW 50000+) are already user friendly.
            if (ex.Number >= 50000)
                return ex.Message;

            // 2627 / 2601 = unique key or unique index violation
            if (ex.Number == 2627 || ex.Number == 2601)
            {
                if (ex.Message.Contains("ReferenceNo"))
                    return "This reference number already exists. Please open the form again to get a new number.";

                if (ex.Message.Contains("BookTypeName"))
                    return "This book type already exists.";

                return "This record already exists.";
            }

            // 547 = foreign key or check constraint violation
            if (ex.Number == 547)
                return "This action is not allowed because related records exist or the data is invalid.";

            return "A database error occurred. Please try again.";
        }
    }
}
