
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System.Data;

namespace LibraryManagementSystem.Controllers
{
    public class TransactionController : Controller
    {
        private readonly DbConnection _db;

        public TransactionController(DbConnection db)
        {
            _db = db;
        }

        // Load Book Types
        private void LoadBookTypes(int selectedBookTypeId = 0)
        {
            List<SelectListItem> bookTypes = new();

            using var connection = _db.GetConnection();
            using var command = new SqlCommand("GetAllBookTypes", connection);
            command.CommandType = CommandType.StoredProcedure;

            connection.Open();

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                int id = Convert.ToInt32(reader["BookTypeId"]);

                bookTypes.Add(new SelectListItem
                {
                    Value = id.ToString(),
                    Text = reader["BookTypeName"].ToString(),
                    Selected = id == selectedBookTypeId
                });
            }

            ViewBag.BookTypes = bookTypes;
        }

        // Load Books
        private void LoadBooks(int bookTypeId, int selectedBookId = 0)
        {
            List<SelectListItem> books = new();

            if (bookTypeId <= 0)
            {
                ViewBag.Books = books;
                return;
            }

            using var connection = _db.GetConnection();
            using var command = new SqlCommand("GetBooksByType", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@BookTypeId", bookTypeId);

            connection.Open();

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                int id = Convert.ToInt32(reader["BookId"]);

                books.Add(new SelectListItem
                {
                    Value = id.ToString(),
                    Text = reader["BookName"].ToString(),
                    Selected = id == selectedBookId
                });
            }

            ViewBag.Books = books;
        }

        // Load Students
        private void LoadStudents(int selectedStudentId = 0)
        {
            List<SelectListItem> students = new();

            using var connection = _db.GetConnection();
            using var command = new SqlCommand("GetAllStudents", connection);
            command.CommandType = CommandType.StoredProcedure;

            connection.Open();

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                int id = Convert.ToInt32(reader["StudentId"]);

                string name = reader["StudentName"].ToString() ?? "";

                string phone = reader["PhoneNumber"] == DBNull.Value
                    ? ""
                    : reader["PhoneNumber"].ToString() ?? "";

                students.Add(new SelectListItem
                {
                    Value = id.ToString(),
                    Text = name + " - " + phone,
                    Selected = id == selectedStudentId
                });
            }

            ViewBag.Students = students;
        }

        // GET: Create Transaction
        public IActionResult Create()
        {
            BookTransaction model = new()
            {
                ReferenceNo = "REF" + DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                ReferenceDate = DateTime.Today
            };

            LoadBookTypes();
            LoadStudents();

            return View(model);
        }

        // Get Books by Book Type
        public IActionResult GetBooksByType(int bookTypeId)
        {
            List<Book> books = new();

            using var connection = _db.GetConnection();
            using var command = new SqlCommand("GetBooksByType", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@BookTypeId", bookTypeId);

            connection.Open();

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                books.Add(new Book
                {
                    BookId = Convert.ToInt32(reader["BookId"]),
                    BookName = reader["BookName"].ToString()!
                });
            }

            return Json(books);
        }

        // Transaction List
        public IActionResult Index()
        {
            List<BookTransaction> transactions = new();

            using var connection = _db.GetConnection();

            using var command = new SqlCommand("GetAllTransactions", connection);
            command.CommandType = CommandType.StoredProcedure;

            connection.Open();

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                transactions.Add(new BookTransaction
                {
                    TransactionId = Convert.ToInt32(reader["TransactionId"]),
                    ReferenceNo = reader["ReferenceNo"].ToString()!,
                    ReferenceDate = Convert.ToDateTime(reader["ReferenceDate"]),
                    Quantity = Convert.ToInt32(reader["Quantity"]),
                    TransactionType = reader["TransactionType"].ToString()!,
                    StudentId = Convert.ToInt32(reader["StudentId"]),
                    StudentName = reader["StudentName"].ToString()!,
                    BookId = Convert.ToInt32(reader["BookId"]),
                    BookName = reader["BookName"].ToString()!,
                    BookTypeId = Convert.ToInt32(reader["BookTypeId"]),
                    BookTypeName = reader["BookTypeName"].ToString()!
                });
            }

            return View(transactions);
        }

        // View Transaction Details
        public IActionResult Details(int id)
        {
            BookTransaction? transaction = null;

            using var connection = _db.GetConnection();

            using var command = new SqlCommand("GetTransactionDetails", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@TransactionId", id);

            connection.Open();

            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                transaction = new BookTransaction
                {
                    TransactionId = Convert.ToInt32(reader["TransactionId"]),
                    ReferenceNo = reader["ReferenceNo"].ToString()!,
                    ReferenceDate = Convert.ToDateTime(reader["ReferenceDate"]),
                    Quantity = Convert.ToInt32(reader["Quantity"]),
                    TransactionType = reader["TransactionType"].ToString()!,
                    StudentName = reader["StudentName"].ToString()!,
                    PhoneNumber = reader["PhoneNumber"] == DBNull.Value
                        ? ""
                        : reader["PhoneNumber"].ToString() ?? "",
                    BookName = reader["BookName"].ToString()!,
                    BookTypeName = reader["BookTypeName"].ToString()!
                };
            }

            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // GET: Edit Transaction
        public IActionResult Edit(int id)
        {
            BookTransaction? transaction = null;

            using var connection = _db.GetConnection();

            using var command = new SqlCommand("GetTransactionForEdit", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@TransactionId", id);

            connection.Open();

            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                transaction = new BookTransaction
                {
                    TransactionId = Convert.ToInt32(reader["TransactionId"]),
                    ReferenceNo = reader["ReferenceNo"].ToString()!,
                    ReferenceDate = Convert.ToDateTime(reader["ReferenceDate"]),
                    StudentId = Convert.ToInt32(reader["StudentId"]),
                    BookId = Convert.ToInt32(reader["BookId"]),
                    Quantity = Convert.ToInt32(reader["Quantity"]),
                    TransactionType = reader["TransactionType"].ToString()!,
                    BookTypeId = Convert.ToInt32(reader["BookTypeId"])
                };
            }

            if (transaction == null)
            {
                return NotFound();
            }

            LoadBookTypes(transaction.BookTypeId);
            LoadBooks(transaction.BookTypeId, transaction.BookId);
            LoadStudents(transaction.StudentId);

            return View(transaction);
        }

        // POST: Edit Transaction
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(BookTransaction model)
        {
            if (model.Quantity <= 0)
                ModelState.AddModelError("", "Quantity must be greater than zero.");

            if (model.TransactionType != "Borrow" &&
                model.TransactionType != "Return")
                ModelState.AddModelError("", "Invalid transaction type.");

            if (model.StudentId <= 0)
                ModelState.AddModelError("", "Please select a student.");

            if (model.BookId <= 0)
                ModelState.AddModelError("", "Please select a book.");

            if (!ModelState.IsValid)
            {
                LoadBookTypes(model.BookTypeId);
                LoadBooks(model.BookTypeId, model.BookId);
                LoadStudents(model.StudentId);

                return View(model);
            }

            try
            {
                using var connection = _db.GetConnection();

                using var command = new SqlCommand("UpdateTransaction", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@TransactionId", model.TransactionId);
                command.Parameters.AddWithValue("@StudentId", model.StudentId);
                command.Parameters.AddWithValue("@BookId", model.BookId);
                command.Parameters.AddWithValue("@Quantity", model.Quantity);
                command.Parameters.AddWithValue("@TransactionType", model.TransactionType);

                connection.Open();
                command.ExecuteNonQuery();

                TempData["Success"] = "Transaction updated successfully.";

                return RedirectToAction("Index");
            }
            catch (SqlException ex)
            {
                ModelState.AddModelError("", DbErrors.ToMessage(ex));

                LoadBookTypes(model.BookTypeId);
                LoadBooks(model.BookTypeId, model.BookId);
                LoadStudents(model.StudentId);

                return View(model);
            }
        }

        // POST: Delete Transaction
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                using var connection = _db.GetConnection();

                using var command = new SqlCommand("DeleteTransaction", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@TransactionId", id);

                connection.Open();
                command.ExecuteNonQuery();

                TempData["Success"] = "Transaction deleted successfully.";
            }
            catch (SqlException ex)
            {
                TempData["Error"] = DbErrors.ToMessage(ex);
            }

            return RedirectToAction("Index");
        }

        // POST: Create Transaction
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BookTransaction model, int BookTypeId)
        {
            if (model.Quantity <= 0)
                ModelState.AddModelError("", "Quantity must be greater than zero.");

            if (model.TransactionType != "Borrow" &&
                model.TransactionType != "Return")
                ModelState.AddModelError("", "Invalid transaction type.");

            if (model.StudentId <= 0)
                ModelState.AddModelError("", "Please select a student.");

            if (model.BookId <= 0)
                ModelState.AddModelError("", "Please select a book.");

            if (!ModelState.IsValid)
            {
                LoadBookTypes(BookTypeId);
                LoadBooks(BookTypeId, model.BookId);
                LoadStudents(model.StudentId);

                return View(model);
            }

            try
            {
                using var connection = _db.GetConnection();

                using var command = new SqlCommand("InsertTransaction", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@ReferenceNo", model.ReferenceNo);
                command.Parameters.AddWithValue("@ReferenceDate", model.ReferenceDate);
                command.Parameters.AddWithValue("@StudentId", model.StudentId);
                command.Parameters.AddWithValue("@BookId", model.BookId);
                command.Parameters.AddWithValue("@Quantity", model.Quantity);
                command.Parameters.AddWithValue("@TransactionType", model.TransactionType);

                connection.Open();
                command.ExecuteNonQuery();

                TempData["Success"] = "Transaction saved successfully.";

                return RedirectToAction("Index");
            }
            catch (SqlException ex)
            {
                ModelState.AddModelError("", DbErrors.ToMessage(ex));

                LoadBookTypes(BookTypeId);
                LoadBooks(BookTypeId, model.BookId);
                LoadStudents(model.StudentId);

                return View(model);
            }
        }
    }
}
