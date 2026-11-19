/* ==========================================
   LIBRARY MANAGEMENT SYSTEM
   PART 1: DATABASE, TABLES AND SEED DATA
========================================== */

IF DB_ID(N'LibraryManagementDB') IS NULL
BEGIN
    CREATE DATABASE [LibraryManagementDB];
END;
GO

USE [LibraryManagementDB];
GO

IF OBJECT_ID(N'dbo.Books', N'U') IS NOT NULL
   OR OBJECT_ID(N'dbo.BookTypes', N'U') IS NOT NULL
   OR OBJECT_ID(N'dbo.Students', N'U') IS NOT NULL
   OR OBJECT_ID(N'dbo.BookTransaction', N'U') IS NOT NULL
BEGIN
    THROW 50000,
    'Tables already exist. Use a fresh database.',
    1;
END;
GO

/* ==========================================
   1. BOOKS TABLE
========================================== */

CREATE TABLE [dbo].[Books]
(
    [BookId] INT IDENTITY(1,1) NOT NULL,
    [BookName] NVARCHAR(150) NOT NULL,
    [BookTypeId] INT NOT NULL,
    [TotalQuantity] INT NOT NULL,
    [AvailableQuantity] INT NOT NULL,

    CONSTRAINT [PK_Books]
        PRIMARY KEY CLUSTERED ([BookId])
);
GO

/* ==========================================
   2. BOOK TRANSACTION TABLE
========================================== */

CREATE TABLE [dbo].[BookTransaction]
(
    [TransactionId] INT IDENTITY(1,1) NOT NULL,
    [ReferenceNo] NVARCHAR(60) NOT NULL,
    [ReferenceDate] DATE NOT NULL,
    [StudentId] INT NOT NULL,
    [BookId] INT NOT NULL,
    [Quantity] INT NOT NULL,
    [TransactionType] NVARCHAR(20) NOT NULL,

    CONSTRAINT [PK_BookTransaction]
        PRIMARY KEY CLUSTERED ([TransactionId])
);
GO

/* ==========================================
   3. BOOK TYPES TABLE
========================================== */

CREATE TABLE [dbo].[BookTypes]
(
    [BookTypeId] INT IDENTITY(1,1) NOT NULL,
    [BookTypeName] NVARCHAR(100) NOT NULL,

    CONSTRAINT [PK_BookTypes]
        PRIMARY KEY CLUSTERED ([BookTypeId])
);
GO

/* ==========================================
   4. STUDENTS TABLE
========================================== */

CREATE TABLE [dbo].[Students]
(
    [StudentId] INT IDENTITY(1,1) NOT NULL,
    [StudentName] NVARCHAR(100) NOT NULL,
    [PhoneNumber] NVARCHAR(15) NULL,

    CONSTRAINT [PK_Students]
        PRIMARY KEY CLUSTERED ([StudentId])
);
GO

/* ==========================================
   5. INSERT BOOK TYPES
========================================== */

SET IDENTITY_INSERT [dbo].[BookTypes] ON;

INSERT INTO [dbo].[BookTypes]
    ([BookTypeId], [BookTypeName])
VALUES
    (2, N'Fiction'),
    (4, N'Programming'),
    (3, N'Science');

SET IDENTITY_INSERT [dbo].[BookTypes] OFF;
GO

/* ==========================================
   6. INSERT STUDENTS
========================================== */

SET IDENTITY_INSERT [dbo].[Students] ON;

INSERT INTO [dbo].[Students]
    ([StudentId], [StudentName], [PhoneNumber])
VALUES
    (16, N'Kamal Das', N'3456789123'),
    (21, N'Kumari Das', N'7812345690');

SET IDENTITY_INSERT [dbo].[Students] OFF;
GO

/* ==========================================
   7. INSERT BOOKS
========================================== */

SET IDENTITY_INSERT [dbo].[Books] ON;

INSERT INTO [dbo].[Books]
    ([BookId], [BookName], [BookTypeId],
     [TotalQuantity], [AvailableQuantity])
VALUES
    (3, N'The Alchemist', 2, 10, 10),
    (4, N'Harry Potter', 2, 8, 8),
    (5, N'A Brief History of Time', 3, 5, 5),
    (6, N'Physics Fundamentals', 3, 7, 7),
    (7, N'JavaScript Basics', 4, 10, 10),
    (8, N'C Programming', 4, 6, 6),
    (9, N'Python', 4, 5, 5);

SET IDENTITY_INSERT [dbo].[Books] OFF;
GO

/* ==========================================
   8. INSERT TRANSACTIONS
========================================== */

SET IDENTITY_INSERT [dbo].[BookTransaction] ON;

INSERT INTO [dbo].[BookTransaction]
    ([TransactionId], [ReferenceNo], [ReferenceDate],
     [StudentId], [BookId], [Quantity], [TransactionType])
VALUES
    (19, N'REF20261116224724685',
     '2026-11-16', 16, 9, 2, N'Borrow'),

    (22, N'REF20261117220214332',
     '2026-11-17', 16, 9, 2, N'Return');

SET IDENTITY_INSERT [dbo].[BookTransaction] OFF;
GO
/* ==========================================
   PART 2: CONSTRAINTS
========================================== */

USE [LibraryManagementDB];
GO

/* Foreign Key: Books -> BookTypes */
ALTER TABLE [dbo].[Books]
ADD CONSTRAINT [FK_Books_BookTypes]
FOREIGN KEY ([BookTypeId])
REFERENCES [dbo].[BookTypes] ([BookTypeId]);
GO

/* Foreign Key: Transactions -> Books */
ALTER TABLE [dbo].[BookTransaction]
ADD CONSTRAINT [FK_BookTransaction_Books]
FOREIGN KEY ([BookId])
REFERENCES [dbo].[Books] ([BookId]);
GO

/* Foreign Key: Transactions -> Students */
ALTER TABLE [dbo].[BookTransaction]
ADD CONSTRAINT [FK_BookTransaction_Students]
FOREIGN KEY ([StudentId])
REFERENCES [dbo].[Students] ([StudentId]);
GO

/* Check: Quantity must be positive */
ALTER TABLE [dbo].[BookTransaction]
ADD CONSTRAINT [CK_BookTransaction_Quantity]
CHECK ([Quantity] > 0);
GO

/* Check: Transaction type */
ALTER TABLE [dbo].[BookTransaction]
ADD CONSTRAINT [CK_BookTransaction_Type]
CHECK ([TransactionType] IN (N'Borrow', N'Return'));
GO

/* Unique: Reference number */
ALTER TABLE [dbo].[BookTransaction]
ADD CONSTRAINT [UQ_BookTransaction_ReferenceNo]
UNIQUE ([ReferenceNo]);
GO

/* Unique: Book type name */
ALTER TABLE [dbo].[BookTypes]
ADD CONSTRAINT [UQ_BookTypes_BookTypeName]
UNIQUE ([BookTypeName]);
GO
/* ==========================================
   PART 3: BOOK TYPE AND STUDENT PROCEDURES
========================================== */

USE [LibraryManagementDB];
GO

/* ---------- BOOK TYPE PROCEDURES ---------- */

CREATE OR ALTER PROCEDURE [dbo].[GetAllBookTypes]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT BookTypeId, BookTypeName
    FROM BookTypes
    ORDER BY BookTypeName;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[GetBookTypeById]
    @BookTypeId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT BookTypeId, BookTypeName
    FROM BookTypes
    WHERE BookTypeId = @BookTypeId;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[InsertBookType]
    @BookTypeName NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM BookTypes WHERE BookTypeName = @BookTypeName)
        THROW 50031, 'This book type already exists.', 1;

    INSERT INTO BookTypes (BookTypeName)
    VALUES (@BookTypeName);
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[UpdateBookType]
    @BookTypeId INT,
    @BookTypeName NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1
        FROM BookTypes
        WHERE BookTypeName = @BookTypeName
          AND BookTypeId <> @BookTypeId
    )
        THROW 50032, 'This book type already exists.', 1;

    UPDATE BookTypes
    SET BookTypeName = @BookTypeName
    WHERE BookTypeId = @BookTypeId;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[DeleteBookType]
    @BookTypeId INT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1
        FROM Books
        WHERE BookTypeId = @BookTypeId
    )
    BEGIN
        SELECT 0 AS Result;
        RETURN;
    END;

    DELETE FROM BookTypes
    WHERE BookTypeId = @BookTypeId;

    SELECT @@ROWCOUNT AS Result;
END;
GO

/* ---------- STUDENT PROCEDURES ---------- */

CREATE OR ALTER PROCEDURE [dbo].[GetAllStudents]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT StudentId, StudentName, PhoneNumber
    FROM Students
    ORDER BY StudentName;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[GetStudentById]
    @StudentId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT StudentId, StudentName, PhoneNumber
    FROM Students
    WHERE StudentId = @StudentId;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[InsertStudent]
    @StudentName NVARCHAR(100),
    @PhoneNumber NVARCHAR(15) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Students (StudentName, PhoneNumber)
    VALUES (@StudentName, @PhoneNumber);
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[UpdateStudent]
    @StudentId INT,
    @StudentName NVARCHAR(100),
    @PhoneNumber NVARCHAR(15) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Students
    SET StudentName = @StudentName,
        PhoneNumber = @PhoneNumber
    WHERE StudentId = @StudentId;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[DeleteStudent]
    @StudentId INT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1
        FROM BookTransaction
        WHERE StudentId = @StudentId
    )
    BEGIN
        SELECT 0 AS Result;
        RETURN;
    END;

    DELETE FROM Students
    WHERE StudentId = @StudentId;

    SELECT @@ROWCOUNT AS Result;
END;
GO
/* ==========================================
   PART 4: BOOK PROCEDURES
========================================== */

USE [LibraryManagementDB];
GO

CREATE OR ALTER PROCEDURE [dbo].[GetAllBooks]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        B.BookId,
        B.BookName,
        B.BookTypeId,
        BT.BookTypeName,
        B.TotalQuantity,
        B.AvailableQuantity
    FROM Books B
    INNER JOIN BookTypes BT
        ON B.BookTypeId = BT.BookTypeId
    ORDER BY B.BookName;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[GetBookById]
    @BookId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        B.BookId,
        B.BookName,
        B.BookTypeId,
        BT.BookTypeName,
        B.TotalQuantity,
        B.AvailableQuantity
    FROM Books B
    INNER JOIN BookTypes BT
        ON B.BookTypeId = BT.BookTypeId
    WHERE B.BookId = @BookId;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[InsertBook]
    @BookName NVARCHAR(150),
    @BookTypeId INT,
    @TotalQuantity INT
AS
BEGIN
    SET NOCOUNT ON;

    IF @TotalQuantity <= 0
        THROW 50001, 'Total quantity must be greater than zero.', 1;

    INSERT INTO Books
        (BookName, BookTypeId, TotalQuantity, AvailableQuantity)
    VALUES
        (@BookName, @BookTypeId, @TotalQuantity, @TotalQuantity);
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[UpdateBook]
    @BookId INT,
    @BookName NVARCHAR(150),
    @BookTypeId INT,
    @TotalQuantity INT
AS
BEGIN
    SET NOCOUNT OFF;

    UPDATE Books
    SET
        BookName = @BookName,
        BookTypeId = @BookTypeId,
        AvailableQuantity =
            @TotalQuantity - (TotalQuantity - AvailableQuantity),
        TotalQuantity = @TotalQuantity
    WHERE BookId = @BookId
      AND @TotalQuantity >= (TotalQuantity - AvailableQuantity);
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[DeleteBook]
    @BookId INT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1
        FROM BookTransaction
        WHERE BookId = @BookId
    )
    BEGIN
        SELECT 0 AS Result;
        RETURN;
    END;

    DELETE FROM Books
    WHERE BookId = @BookId;

    SELECT @@ROWCOUNT AS Result;
END;
GO
/* ==========================================
   PART 5: TRANSACTION PROCEDURES
========================================== */

USE [LibraryManagementDB];
GO

/* ---------- GET BOOKS BY TYPE ---------- */

CREATE OR ALTER PROCEDURE [dbo].[GetBooksByType]
    @BookTypeId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        BookId,
        BookName,
        TotalQuantity,
        AvailableQuantity
    FROM Books
    WHERE BookTypeId = @BookTypeId
    ORDER BY BookName;
END;
GO

/* ---------- GET ALL TRANSACTIONS ---------- */

CREATE OR ALTER PROCEDURE [dbo].[GetAllTransactions]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        T.TransactionId,
        T.ReferenceNo,
        T.ReferenceDate,
        T.StudentId,
        S.StudentName,
        S.PhoneNumber,
        T.BookId,
        B.BookName,
        B.BookTypeId,
        BT.BookTypeName,
        T.Quantity,
        T.TransactionType
    FROM BookTransaction T
    INNER JOIN Students S
        ON T.StudentId = S.StudentId
    INNER JOIN Books B
        ON T.BookId = B.BookId
    INNER JOIN BookTypes BT
        ON B.BookTypeId = BT.BookTypeId
    ORDER BY T.TransactionId DESC;
END;
GO

/* ---------- GET TRANSACTION DETAILS ---------- */

CREATE OR ALTER PROCEDURE [dbo].[GetTransactionDetails]
    @TransactionId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        T.TransactionId,
        T.ReferenceNo,
        T.ReferenceDate,
        T.StudentId,
        S.StudentName,
        S.PhoneNumber,
        T.BookId,
        B.BookName,
        B.BookTypeId,
        BT.BookTypeName,
        T.Quantity,
        T.TransactionType
    FROM BookTransaction T
    INNER JOIN Students S
        ON T.StudentId = S.StudentId
    INNER JOIN Books B
        ON T.BookId = B.BookId
    INNER JOIN BookTypes BT
        ON B.BookTypeId = BT.BookTypeId
    WHERE T.TransactionId = @TransactionId;
END;
GO

/* ---------- GET TRANSACTION FOR EDIT ---------- */

USE [LibraryManagementDB];
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[GetTransactionForEdit]
    @TransactionId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        t.TransactionId,
        t.ReferenceNo,
        t.ReferenceDate,
        t.StudentId,
        t.BookId,
        t.Quantity,
        t.TransactionType,
        b.BookTypeId
    FROM BookTransaction t
    INNER JOIN Books b ON t.BookId = b.BookId
    WHERE t.TransactionId = @TransactionId;
END;
GO

/* ---------- INSERT TRANSACTION ---------- */

CREATE OR ALTER PROCEDURE [dbo].[InsertTransaction]
    @ReferenceNo NVARCHAR(60),
    @ReferenceDate DATE,
    @StudentId INT,
    @BookId INT,
    @Quantity INT,
    @TransactionType NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF @Quantity <= 0
            THROW 50001, 'Quantity must be greater than zero.', 1;

        IF @TransactionType NOT IN (N'Borrow', N'Return')
            THROW 50002, 'Invalid transaction type.', 1;

        IF NOT EXISTS (
            SELECT 1
            FROM Students
            WHERE StudentId = @StudentId
        )
            THROW 50003, 'Student not found.', 1;

        DECLARE @Available INT;

        SELECT @Available = AvailableQuantity
        FROM Books WITH (UPDLOCK, HOLDLOCK)
        WHERE BookId = @BookId;

        IF @Available IS NULL
            THROW 50004, 'Book not found.', 1;

        IF @TransactionType = N'Borrow'
        BEGIN
            IF @Available < @Quantity
                THROW 50005, 'Not enough books available.', 1;

            UPDATE Books
            SET AvailableQuantity = AvailableQuantity - @Quantity
            WHERE BookId = @BookId;
        END
        ELSE
        BEGIN
            DECLARE @Borrowed INT;
            DECLARE @Returned INT;

            SELECT
                @Borrowed = ISNULL(SUM(
                    CASE WHEN TransactionType = N'Borrow'
                    THEN Quantity ELSE 0 END
                ), 0),
                @Returned = ISNULL(SUM(
                    CASE WHEN TransactionType = N'Return'
                    THEN Quantity ELSE 0 END
                ), 0)
            FROM BookTransaction
            WHERE StudentId = @StudentId
              AND BookId = @BookId;

            IF @Quantity > (@Borrowed - @Returned)
                THROW 50006, 'Return quantity exceeds borrowed quantity.', 1;

            UPDATE Books
            SET AvailableQuantity = AvailableQuantity + @Quantity
            WHERE BookId = @BookId;
        END;

        INSERT INTO BookTransaction
        (
            ReferenceNo,
            ReferenceDate,
            StudentId,
            BookId,
            Quantity,
            TransactionType
        )
        VALUES
        (
            @ReferenceNo,
            @ReferenceDate,
            @StudentId,
            @BookId,
            @Quantity,
            @TransactionType
        );

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH;
END;
GO

/* ---------- UPDATE TRANSACTION ---------- */

CREATE OR ALTER PROCEDURE [dbo].[UpdateTransaction]
    @TransactionId INT,
    @StudentId INT,
    @BookId INT,
    @Quantity INT,
    @TransactionType NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF @Quantity <= 0
            THROW 50011, 'Quantity must be greater than zero.', 1;

        IF @TransactionType NOT IN (N'Borrow', N'Return')
            THROW 50012, 'Invalid transaction type.', 1;

        DECLARE
            @OldStudentId INT,
            @OldBookId INT,
            @OldQuantity INT,
            @OldType NVARCHAR(20),
            @Available INT,
            @Borrowed INT,
            @Returned INT;

        SELECT
            @OldStudentId = StudentId,
            @OldBookId = BookId,
            @OldQuantity = Quantity,
            @OldType = TransactionType
        FROM BookTransaction WITH (UPDLOCK, HOLDLOCK)
        WHERE TransactionId = @TransactionId;

        IF @OldBookId IS NULL
            THROW 50013, 'Transaction not found.', 1;

        /* Restore old transaction stock effect */
        IF @OldType = N'Borrow'
        BEGIN
            UPDATE Books
            SET AvailableQuantity = AvailableQuantity + @OldQuantity
            WHERE BookId = @OldBookId;
        END
        ELSE
        BEGIN
            UPDATE Books
            SET AvailableQuantity = AvailableQuantity - @OldQuantity
            WHERE BookId = @OldBookId;
        END;

        /* Lock and read selected book */
        SELECT @Available = AvailableQuantity
        FROM Books WITH (UPDLOCK, HOLDLOCK)
        WHERE BookId = @BookId;

        IF @Available IS NULL
            THROW 50014, 'Book not found.', 1;

        IF NOT EXISTS (
            SELECT 1
            FROM Students
            WHERE StudentId = @StudentId
        )
            THROW 50015, 'Student not found.', 1;

        IF @TransactionType = N'Borrow'
        BEGIN
            IF @Available < @Quantity
                THROW 50016, 'Not enough books available.', 1;

            UPDATE Books
            SET AvailableQuantity = AvailableQuantity - @Quantity
            WHERE BookId = @BookId;
        END
        ELSE
        BEGIN
            SELECT
                @Borrowed = ISNULL(SUM(
                    CASE WHEN TransactionType = N'Borrow'
                    THEN Quantity ELSE 0 END
                ), 0),
                @Returned = ISNULL(SUM(
                    CASE WHEN TransactionType = N'Return'
                    THEN Quantity ELSE 0 END
                ), 0)
            FROM BookTransaction
            WHERE StudentId = @StudentId
              AND BookId = @BookId
              AND TransactionId <> @TransactionId;

            IF @Quantity > (@Borrowed - @Returned)
                THROW 50017, 'Return quantity exceeds borrowed quantity.', 1;

            UPDATE Books
            SET AvailableQuantity = AvailableQuantity + @Quantity
            WHERE BookId = @BookId;
        END;

        UPDATE BookTransaction
        SET
            StudentId = @StudentId,
            BookId = @BookId,
            Quantity = @Quantity,
            TransactionType = @TransactionType
        WHERE TransactionId = @TransactionId;

        /* Integrity: stock of affected books must stay between 0 and total */
        IF EXISTS (
            SELECT 1
            FROM Books
            WHERE BookId IN (@OldBookId, @BookId)
              AND (AvailableQuantity < 0 OR AvailableQuantity > TotalQuantity)
        )
            THROW 50018, 'This change is not allowed because it would make the book stock invalid.', 1;

        /* Integrity: for the old and new student/book pair, returns must not exceed borrows */
        IF EXISTS (
            SELECT 1
            FROM BookTransaction
            WHERE (StudentId = @OldStudentId AND BookId = @OldBookId)
               OR (StudentId = @StudentId AND BookId = @BookId)
            GROUP BY StudentId, BookId
            HAVING SUM(CASE WHEN TransactionType = N'Borrow' THEN Quantity ELSE -Quantity END) < 0
        )
            THROW 50019, 'This change is not allowed because the student has already returned books against this borrow.', 1;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH;
END;
GO

/* ---------- DELETE TRANSACTION ---------- */

USE [LibraryManagementDB];
GO

CREATE OR ALTER PROCEDURE [dbo].[DeleteTransaction]
    @TransactionId INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE
            @StudentId INT,
            @BookId INT,
            @Quantity INT,
            @TransactionType NVARCHAR(20),
            @Available INT,
            @Total INT;

        SELECT
            @StudentId = StudentId,
            @BookId = BookId,
            @Quantity = Quantity,
            @TransactionType = TransactionType
        FROM dbo.BookTransaction WITH (UPDLOCK, HOLDLOCK)
        WHERE TransactionId = @TransactionId;

        IF @BookId IS NULL
            THROW 50021, 'Transaction was not found.', 1;

        SELECT
            @Available = AvailableQuantity,
            @Total = TotalQuantity
        FROM dbo.Books WITH (UPDLOCK, HOLDLOCK)
        WHERE BookId = @BookId;

        IF @Available IS NULL
            THROW 50022, 'Related book was not found.', 1;

        IF @TransactionType = N'Borrow'
        BEGIN
            IF @Available + @Quantity > @Total
                THROW 50023, 'Cannot delete this Borrow transaction because stock validation failed.', 1;

            UPDATE dbo.Books
            SET AvailableQuantity = AvailableQuantity + @Quantity
            WHERE BookId = @BookId;
        END
        ELSE IF @TransactionType = N'Return'
        BEGIN
            IF @Available < @Quantity
                THROW 50024, 'Cannot delete this Return transaction because stock validation failed.', 1;

            UPDATE dbo.Books
            SET AvailableQuantity = AvailableQuantity - @Quantity
            WHERE BookId = @BookId;
        END
        ELSE
        BEGIN
            THROW 50025, 'Invalid transaction type.', 1;
        END;

        DELETE FROM dbo.BookTransaction
        WHERE TransactionId = @TransactionId;

        /* Integrity: the student's returns for this book must not exceed borrows */
        IF EXISTS (
            SELECT 1
            FROM dbo.BookTransaction
            WHERE StudentId = @StudentId
              AND BookId = @BookId
            GROUP BY StudentId, BookId
            HAVING SUM(CASE WHEN TransactionType = N'Borrow' THEN Quantity ELSE -Quantity END) < 0
        )
            THROW 50026, 'This Borrow cannot be deleted because the student has already returned books against it.', 1;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH;
END;
GO
