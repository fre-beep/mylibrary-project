using System;
using System.Windows.Forms;

public class MainForm : Form
{
    private Button btnBookManagement;
    private Button btnBorrowersManagement;

    public MainForm()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        this.Text = "Library Management System";
        this.Size = new System.Drawing.Size(400, 300);
        this.StartPosition = FormStartPosition.CenterScreen;

        btnBookManagement = new Button();
        btnBookManagement.Text = "Book Management";
        btnBookManagement.Location = new System.Drawing.Point(100, 50);
        btnBookManagement.Size = new System.Drawing.Size(200, 50);
        btnBookManagement.Click += (sender, e) => {
            BookForm bookForm = new BookForm();
            bookForm.ShowDialog();
        };

        btnBorrowersManagement = new Button();
        btnBorrowersManagement.Text = "Borrowers Management";
        btnBorrowersManagement.Location = new System.Drawing.Point(100, 120);
        btnBorrowersManagement.Size = new System.Drawing.Size(200, 50);
        btnBorrowersManagement.Click += (sender, e) => {
            BorrowersForm borrowersForm = new BorrowersForm();
            borrowersForm.ShowDialog();
        };

        this.Controls.Add(btnBookManagement);
        this.Controls.Add(btnBorrowersManagement);
    }
}