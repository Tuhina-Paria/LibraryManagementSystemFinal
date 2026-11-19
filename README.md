# Library Management System

A Library Management System built using **ASP.NET Core MVC, C#, SQL Server, and ADO.NET**. This project manages students, book categories, books, and book issue/return transactions with automatic stock updates.

## Features

- **Student Master:** Add, view, update, and delete student records.
- **Book Type Master:** Create, view, update, and delete book categories.
- **Book Master:** Manage books, book categories, total quantity, and available quantity.
- **Transaction Management:** Issue and return books.
- **Automatic Reference Number:** Generates a unique reference number for each transaction.
- **Stock Management:** Automatically updates available book quantities when books are issued or returned.
- **Transaction History:** View, edit, and delete transaction records.
- **Validation:** Prevents invalid quantities, unavailable book issues, and invalid returns.
- **Delete Protection:** Prevents deleting a book category that has books, a book that has transactions, or a student who has transactions.
- **Data Integrity:** Editing or deleting a transaction can never push stock above the total quantity or leave a return without a matching borrow.

## Technology Stack

- ASP.NET Core MVC (.NET 8)
- C#
- SQL Server Express
- ADO.NET
- HTML5
- CSS3
- JavaScript
- Bootstrap
- Visual Studio 2022

## Database Structure

The project uses four main tables:

1. `Students`
2. `BookTypes`
3. `Books`
4. `BookTransaction`

Database operations are handled using SQL Server Stored Procedures for CRUD operations and transaction management.

**Database Script:** `Database/LibraryManagementDB.sql`

## Setup Instructions

1. Clone or download this repository.
2. Open `LibraryManagementSystem.sln` in Visual Studio 2022 (requires the .NET 8 SDK).
3. Open SQL Server Management Studio (SSMS) and connect to your SQL Server instance.
4. Open and execute `Database/LibraryManagementDB.sql`. The script creates the `LibraryManagementDB` database (if it does not exist), the tables, constraints, sample data, and all stored procedures.
5. Open `appsettings.json`. It is set up for SQL Server LocalDB (`(localdb)\MSSQLLocalDB`), which is installed with Visual Studio 2022.
6. If you use a different SQL Server instance, change the `Server=` part of the connection string (for example `.\SQLEXPRESS`).
7. Restore the NuGet packages.
8. Build the solution.
9. Run the application using Visual Studio.

**Note:** The script is meant for a fresh database. If the tables already exist it reports "Tables already exist. Use a fresh database." and the table and sample-data steps fail without changing existing data.

## Connection String

`appsettings.json` uses SQL Server LocalDB by default. To use another SQL Server instance, update the connection string with your server details.

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=LibraryManagementDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Replace `YOUR_SERVER_NAME` with your SQL Server instance name.

## Project Purpose

This project was developed as a practical learning assignment to understand ASP.NET Core MVC, C#, SQL Server integration, ADO.NET, Stored Procedures, CRUD operations, form validation, and database-driven application development.