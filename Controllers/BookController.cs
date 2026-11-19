
using System.Data;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace LibraryManagementSystem.Controllers
{
    public class BookController : Controller
    {
        private readonly DbConnection _db;

        public BookController(DbConnection db)
        {
            _db = db;
        }

        // Display all books
        public IActionResult Index()
        {
            List<Book> books = new List<Book>();

            using (SqlConnection con = _db.GetConnection())
            using (SqlCommand cmd = new SqlCommand("GetAllBooks", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        books.Add(new Book
                        {
                            BookId = Convert.ToInt32(reader["BookId"]),
                            BookName = reader["BookName"].ToString()!,
                            BookTypeId = Convert.ToInt32(reader["BookTypeId"]),
                            TotalQuantity = Convert.ToInt32(reader["TotalQuantity"]),
                            AvailableQuantity = Convert.ToInt32(reader["AvailableQuantity"]),
                            BookTypeName = reader["BookTypeName"].ToString()!
                        });
                    }
                }
            }

            return View(books);
        }

        // Load book types for dropdown
        private void LoadBookTypes()
        {
            List<SelectListItem> bookTypes = new List<SelectListItem>();

            using (SqlConnection con = _db.GetConnection())
            using (SqlCommand cmd = new SqlCommand("GetAllBookTypes", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        bookTypes.Add(new SelectListItem
                        {
                            Value = reader["BookTypeId"].ToString(),
                            Text = reader["BookTypeName"].ToString()
                        });
                    }
                }
            }

            ViewBag.BookTypes = bookTypes;
        }

        // GET: Book/Create
        public IActionResult Create()
        {
            LoadBookTypes();
            return View();
        }

        // POST: Book/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Book book)
        {
            if (string.IsNullOrWhiteSpace(book.BookName))
                ModelState.AddModelError("BookName", "Book name is required.");
            else if (book.BookName.Trim().Length > 150)
                ModelState.AddModelError("BookName", "Book name cannot be longer than 150 characters.");

            if (book.BookTypeId <= 0)
                ModelState.AddModelError("BookTypeId", "Please select a book type.");

            if (book.TotalQuantity <= 0)
                ModelState.AddModelError("TotalQuantity", "Quantity must be greater than zero.");

            if (!ModelState.IsValid)
            {
                LoadBookTypes();
                return View(book);
            }

            using (SqlConnection con = _db.GetConnection())
            using (SqlCommand cmd = new SqlCommand("InsertBook", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@BookName", SqlDbType.NVarChar, 150)
                    .Value = book.BookName.Trim();

                cmd.Parameters.Add("@BookTypeId", SqlDbType.Int)
                    .Value = book.BookTypeId;

                cmd.Parameters.Add("@TotalQuantity", SqlDbType.Int)
                    .Value = book.TotalQuantity;

                con.Open();
                cmd.ExecuteNonQuery();
            }

            TempData["Success"] = "Book added successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Book/Edit/5
        public IActionResult Edit(int id)
        {
            Book? book = null;

            using (SqlConnection con = _db.GetConnection())
            using (SqlCommand cmd = new SqlCommand("GetBookById", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@BookId", SqlDbType.Int).Value = id;

                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        book = new Book
                        {
                            BookId = Convert.ToInt32(reader["BookId"]),
                            BookName = reader["BookName"].ToString()!,
                            BookTypeId = Convert.ToInt32(reader["BookTypeId"]),
                            TotalQuantity = Convert.ToInt32(reader["TotalQuantity"]),
                            AvailableQuantity = Convert.ToInt32(reader["AvailableQuantity"])
                        };
                    }
                }
            }

            if (book == null)
                return NotFound();

            LoadBookTypes();
            return View(book);
        }

        // POST: Book/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Book book)
        {
            if (string.IsNullOrWhiteSpace(book.BookName))
                ModelState.AddModelError("BookName", "Book name is required.");
            else if (book.BookName.Trim().Length > 150)
                ModelState.AddModelError("BookName", "Book name cannot be longer than 150 characters.");

            if (book.BookTypeId <= 0)
                ModelState.AddModelError("BookTypeId", "Please select a book type.");

            if (book.TotalQuantity <= 0)
                ModelState.AddModelError("TotalQuantity", "Quantity must be greater than zero.");

            if (!ModelState.IsValid)
            {
                LoadBookTypes();
                return View(book);
            }

            using (SqlConnection con = _db.GetConnection())
            {
                con.Open();

                using (SqlTransaction transaction = con.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("UpdateBook", con, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.Add("@BookId", SqlDbType.Int)
                                .Value = book.BookId;

                            cmd.Parameters.Add("@BookName", SqlDbType.NVarChar, 150)
                                .Value = book.BookName.Trim();

                            cmd.Parameters.Add("@BookTypeId", SqlDbType.Int)
                                .Value = book.BookTypeId;

                            cmd.Parameters.Add("@TotalQuantity", SqlDbType.Int)
                                .Value = book.TotalQuantity;

                            int rows = cmd.ExecuteNonQuery();

                            if (rows == 0)
                            {
                                transaction.Rollback();

                                ModelState.AddModelError(
                                    "TotalQuantity",
                                    "Total quantity cannot be less than the number of borrowed books.");

                                LoadBookTypes();
                                return View(book);
                            }
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }

            TempData["Success"] = "Book updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Book/Details/5
        public IActionResult Details(int id)
        {
            Book? book = null;

            using (SqlConnection con = _db.GetConnection())
            using (SqlCommand cmd = new SqlCommand("GetBookById", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@BookId", SqlDbType.Int).Value = id;

                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        book = new Book
                        {
                            BookId = Convert.ToInt32(reader["BookId"]),
                            BookName = reader["BookName"].ToString()!,
                            BookTypeId = Convert.ToInt32(reader["BookTypeId"]),
                            BookTypeName = reader["BookTypeName"].ToString()!,
                            TotalQuantity = Convert.ToInt32(reader["TotalQuantity"]),
                            AvailableQuantity = Convert.ToInt32(reader["AvailableQuantity"])
                        };
                    }
                }
            }

            if (book == null)
                return NotFound();

            return View(book);
        }

        // GET: Book/Delete/5
        public IActionResult Delete(int id)
        {
            Book? book = null;

            using (SqlConnection con = _db.GetConnection())
            using (SqlCommand cmd = new SqlCommand("GetBookById", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@BookId", SqlDbType.Int).Value = id;

                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        book = new Book
                        {
                            BookId = Convert.ToInt32(reader["BookId"]),
                            BookName = reader["BookName"].ToString()!,
                            BookTypeName = reader["BookTypeName"].ToString()!,
                            TotalQuantity = Convert.ToInt32(reader["TotalQuantity"]),
                            AvailableQuantity = Convert.ToInt32(reader["AvailableQuantity"])
                        };
                    }
                }
            }

            if (book == null)
                return NotFound();

            return View(book);
        }

        // POST: Book/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            using (SqlConnection con = _db.GetConnection())
            using (SqlCommand cmd = new SqlCommand("DeleteBook", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@BookId", SqlDbType.Int).Value = id;

                con.Open();

                int result = Convert.ToInt32(cmd.ExecuteScalar());

                if (result == 0)
                {
                    TempData["Error"] =
                        "This book cannot be deleted because transaction records exist, or the book was not found.";

                    return RedirectToAction(nameof(Index));
                }
            }

            TempData["Success"] = "Book deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
