using System;
using System.Data.SQLite;
using System.Data;

public static class DatabaseHelper
{
    private static string connectionString = "Data Source=library.db;Version=3;";

    public static void InitializeDatabase()
    {
        if (!System.IO.File.Exists("library.db"))
        {
            SQLiteConnection.CreateFile("library.db");
            
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                
                // Create Users table
                string createUsersTable = @"
                CREATE TABLE Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL,
                    Password TEXT NOT NULL
                )";
                
                // Create Books table
                string createBooksTable = @"
                CREATE TABLE Books (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT NOT NULL,
                    Author TEXT NOT NULL,
                    Year INTEGER,
                    Quantity INTEGER NOT NULL
                )";
                
                // Create Borrowers table
                string createBorrowersTable = @"
                CREATE TABLE Borrowers (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Email TEXT,
                    Phone TEXT
                )";
                
                // Create IssuedBooks table
                string createIssuedBooksTable = @"
                CREATE TABLE IssuedBooks (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    BookId INTEGER NOT NULL,
                    BorrowerId INTEGER NOT NULL,
                    IssueDate TEXT NOT NULL,
                    ReturnDate TEXT,
                    FOREIGN KEY(BookId) REFERENCES Books(Id),
                    FOREIGN KEY(BorrowerId) REFERENCES Borrowers(Id)
                )";
                
                ExecuteNonQuery(createUsersTable);
                ExecuteNonQuery(createBooksTable);
                ExecuteNonQuery(createBorrowersTable);
                ExecuteNonQuery(createIssuedBooksTable);
                
                // Add default admin user
                string addAdmin = "INSERT INTO Users (Username, Password) VALUES ('admin', 'admin')";
                ExecuteNonQuery(addAdmin);
            }
        }
    }
    
    public static DataTable ExecuteQuery(string query)
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            using (var command = new SQLiteCommand(query, connection))
            {
                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(command))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }
    }
    
    public static int ExecuteNonQuery(string query)
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            using (var command = new SQLiteCommand(query, connection))
            {
                return command.ExecuteNonQuery();
            }
        }
    }
    
    public static object ExecuteScalar(string query)
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            using (var command = new SQLiteCommand(query, connection))
            {
                return command.ExecuteScalar();
            }
        }
    }
}