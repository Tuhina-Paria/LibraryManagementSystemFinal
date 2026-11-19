
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Controllers
{
    public class BookTypeController : Controller
    {
        private readonly DbConnection _db;

        public BookTypeController(DbConnection db)
        {
            _db = db;
        }

        // GET: BookType/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BookType/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BookType model)
        {
            if (!IsValidName(model))
            {
                return View(model);
            }

            try
            {
                using var connection = _db.GetConnection();
                using var command = new SqlCommand("InsertBookType", connection);

                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@BookTypeName", SqlDbType.NVarChar, 100)
                    .Value = model.BookTypeName.Trim();

                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                ModelState.AddModelError("BookTypeName", DbErrors.ToMessage(ex));
                return View(model);
            }

            TempData["Success"] = "Book Type added successfully.";

            return RedirectToAction(nameof(Index));
        }

        // GET: BookType/Index
        public IActionResult Index()
        {
            List<BookType> bookTypes = new List<BookType>();

            using var connection = _db.GetConnection();
            using var command = new SqlCommand("GetAllBookTypes", connection);

            command.CommandType = CommandType.StoredProcedure;

            connection.Open();

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                bookTypes.Add(new BookType
                {
                    BookTypeId = Convert.ToInt32(reader["BookTypeId"]),
                    BookTypeName = reader["BookTypeName"].ToString()!
                });
            }

            return View(bookTypes);
        }

        // GET: BookType/Edit/5
        public IActionResult Edit(int id)
        {
            BookType? bookType = null;

            using var connection = _db.GetConnection();
            using var command = new SqlCommand("GetBookTypeById", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@BookTypeId", SqlDbType.Int).Value = id;

            connection.Open();

            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                bookType = new BookType
                {
                    BookTypeId = Convert.ToInt32(reader["BookTypeId"]),
                    BookTypeName = reader["BookTypeName"].ToString()!
                };
            }

            if (bookType == null)
                return NotFound();

            return View(bookType);
        }

        // POST: BookType/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(BookType model)
        {
            if (!IsValidName(model))
            {
                return View(model);
            }

            try
            {
                using var connection = _db.GetConnection();
                using var command = new SqlCommand("UpdateBookType", connection);

                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@BookTypeId", SqlDbType.Int)
                    .Value = model.BookTypeId;

                command.Parameters.Add("@BookTypeName", SqlDbType.NVarChar, 100)
                    .Value = model.BookTypeName.Trim();

                connection.Open();

                int rows = command.ExecuteNonQuery();

                if (rows == 0)
                    return NotFound();
            }
            catch (SqlException ex)
            {
                ModelState.AddModelError("BookTypeName", DbErrors.ToMessage(ex));
                return View(model);
            }

            TempData["Success"] = "Book Type updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        // Validates the book type name (required, max 100 characters)
        private bool IsValidName(BookType model)
        {
            if (!ModelState.IsValid)
                return false;

            if (string.IsNullOrWhiteSpace(model.BookTypeName))
            {
                ModelState.AddModelError("BookTypeName", "Book type name is required.");
                return false;
            }

            if (model.BookTypeName.Trim().Length > 100)
            {
                ModelState.AddModelError("BookTypeName", "Book type name cannot be longer than 100 characters.");
                return false;
            }

            return true;
        }

        // POST: BookType/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            using var connection = _db.GetConnection();
            using var command = new SqlCommand("DeleteBookType", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@BookTypeId", SqlDbType.Int).Value = id;

            connection.Open();

            int result = Convert.ToInt32(command.ExecuteScalar());

            if (result == 0)
            {
                TempData["Error"] =
                    "This Book Type cannot be deleted because books are linked to it.";

                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = "Book Type deleted successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}
