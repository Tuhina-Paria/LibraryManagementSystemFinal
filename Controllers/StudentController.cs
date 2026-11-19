
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using System.Data;

namespace LibraryManagementSystem.Controllers
{
    public class StudentController : Controller
    {
        private readonly DbConnection _db;

        public StudentController(DbConnection db)
        {
            _db = db;
        }

        // GET: Create Student
        public IActionResult Create()
        {
            return View();
        }

        // POST: Save Student
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Student model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using var connection = _db.GetConnection();

            using var command = new SqlCommand("InsertStudent", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@StudentName", model.StudentName);
            command.Parameters.AddWithValue(
                "@PhoneNumber",
                (object?)model.PhoneNumber ?? DBNull.Value
            );

            connection.Open();
            command.ExecuteNonQuery();

            TempData["SuccessMessage"] = "Student added successfully.";

            return RedirectToAction("Index");
        }

        // Student List
        public IActionResult Index()
        {
            List<Student> students = new List<Student>();

            using var connection = _db.GetConnection();

            using var command = new SqlCommand("GetAllStudents", connection);
            command.CommandType = CommandType.StoredProcedure;

            connection.Open();

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                Student student = new Student
                {
                    StudentId = Convert.ToInt32(reader["StudentId"]),
                    StudentName = reader["StudentName"].ToString()!,
                    PhoneNumber = reader["PhoneNumber"] == DBNull.Value
                        ? null
                        : reader["PhoneNumber"].ToString()
                };

                students.Add(student);
            }

            return View(students);
        }

        // GET: Edit Student
        public IActionResult Edit(int id)
        {
            Student? student = null;

            using var connection = _db.GetConnection();

            using var command = new SqlCommand("GetStudentById", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@StudentId", id);

            connection.Open();

            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                student = new Student
                {
                    StudentId = Convert.ToInt32(reader["StudentId"]),
                    StudentName = reader["StudentName"].ToString()!,
                    PhoneNumber = reader["PhoneNumber"] == DBNull.Value
                        ? null
                        : reader["PhoneNumber"].ToString()
                };
            }

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Update Student
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Student model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using var connection = _db.GetConnection();

            using var command = new SqlCommand("UpdateStudent", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@StudentId", model.StudentId);
            command.Parameters.AddWithValue("@StudentName", model.StudentName);
            command.Parameters.AddWithValue(
                "@PhoneNumber",
                (object?)model.PhoneNumber ?? DBNull.Value
            );

            connection.Open();
            command.ExecuteNonQuery();

            TempData["SuccessMessage"] = "Student updated successfully.";

            return RedirectToAction("Index");
        }

        // POST: Delete Student
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            using var connection = _db.GetConnection();

            using var command = new SqlCommand("DeleteStudent", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@StudentId", id);

            connection.Open();

            int result = Convert.ToInt32(command.ExecuteScalar());

            if (result == 0)
            {
                TempData["ErrorMessage"] =
                    "This student cannot be deleted because transaction records exist.";
            }
            else
            {
                TempData["SuccessMessage"] = "Student deleted successfully.";
            }

            return RedirectToAction("Index");
        }
    }
}
