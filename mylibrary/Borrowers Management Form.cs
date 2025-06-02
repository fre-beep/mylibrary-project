using System;
using System.Data;
using System.Windows.Forms;

public class BorrowersForm : Form
{
    private DataGridView dgvBorrowers;
    private TextBox txtName;
    private TextBox txtEmail;
    private TextBox txtPhone;
    private Button btnAdd;
    private Button btnEdit;
    private Button btnDelete;
    private Button btnIssueBook;
    private Button btnReturnBook;

    private int selectedBorrowerId = -1;

    public BorrowersForm()
    {
        InitializeComponents();
        LoadBorrowers();
    }

    private void InitializeComponents()
    {
        this.Text = "Borrowers Management";
        this.Size = new System.Drawing.Size(650, 500);
        this.StartPosition = FormStartPosition.CenterScreen;

        // Labels
        Label lblName = new Label() { Text = "Name:", Location = new System.Drawing.Point(20, 20), Size = new System.Drawing.Size(100, 20) };
        Label lblEmail = new Label() { Text = "Email:", Location = new System.Drawing.Point(20, 60), Size = new System.Drawing.Size(100, 20) };
        Label lblPhone = new Label() { Text = "Phone:", Location = new System.Drawing.Point(20, 100), Size = new System.Drawing.Size(100, 20) };

        // TextBoxes
        txtName = new TextBox() { Location = new System.Drawing.Point(130, 20), Size = new System.Drawing.Size(200, 20) };
        txtEmail = new TextBox() { Location = new System.Drawing.Point(130, 60), Size = new System.Drawing.Size(200, 20) };
        txtPhone = new TextBox() { Location = new System.Drawing.Point(130, 100), Size = new System.Drawing.Size(200, 20) };

        // Buttons
        btnAdd = new Button() { Text = "Add", Location = new System.Drawing.Point(350, 20), Size = new System.Drawing.Size(80, 30) };
        btnEdit = new Button() { Text = "Edit", Location = new System.Drawing.Point(350, 60), Size = new System.Drawing.Size(80, 30), Enabled = false };
        btnDelete = new Button() { Text = "Delete", Location = new System.Drawing.Point(350, 100), Size = new System.Drawing.Size(80, 30), Enabled = false };
        btnIssueBook = new Button() { Text = "Issue Book", Location = new System.Drawing.Point(450, 20), Size = new System.Drawing.Size(100, 30) };
        btnReturnBook = new Button() { Text = "Return Book", Location = new System.Drawing.Point(450, 60), Size = new System.Drawing.Size(100, 30) };

        btnAdd.Click += BtnAdd_Click;
        btnEdit.Click += BtnEdit_Click;
        btnDelete.Click += BtnDelete_Click;
        btnIssueBook.Click += BtnIssueBook_Click;
        btnReturnBook.Click += BtnReturnBook_Click;

        // DataGridView
        dgvBorrowers = new DataGridView();
        dgvBorrowers.Location = new System.Drawing.Point(20, 140);
        dgvBorrowers.Size = new System.Drawing.Size(600, 300);
        dgvBorrowers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvBorrowers.MultiSelect = false;
        dgvBorrowers.ReadOnly = true;
        dgvBorrowers.AllowUserToAddRows = false;
        dgvBorrowers.SelectionChanged += DgvBorrowers_SelectionChanged;

        // Add columns to DataGridView
        dgvBorrowers.Columns.Add("Id", "Id");
        dgvBorrowers.Columns.Add("Name", "Name");
        dgvBorrowers.Columns.Add("Email", "Email");
        dgvBorrowers.Columns.Add("Phone", "Phone");

        // Add controls to form
        this.Controls.AddRange(new Control[] {
            lblName, lblEmail, lblPhone,
            txtName, txtEmail, txtPhone,
            btnAdd, btnEdit, btnDelete, btnIssueBook, btnReturnBook,
            dgvBorrowers
        });
    }

    private void LoadBorrowers()
    {
        dgvBorrowers.Rows.Clear();
        string query = "SELECT * FROM Borrowers";
        DataTable dt = DatabaseHelper.ExecuteQuery(query);

        foreach (DataRow row in dt.Rows)
        {
            dgvBorrowers.Rows.Add(
                row["Id"],
                row["Name"],
                row["Email"],
                row["Phone"]
            );
        }
    }

    private void DgvBorrowers_SelectionChanged(object sender, EventArgs e)
    {
        if (dgvBorrowers.SelectedRows.Count > 0)
        {
            DataGridViewRow row = dgvBorrowers.SelectedRows[0];
            selectedBorrowerId = Convert.ToInt32(row.Cells["Id"].Value);
            txtName.Text = row.Cells["Name"].Value.ToString();
            txtEmail.Text = row.Cells["Email"]?.Value?.ToString() ?? "";
            txtPhone.Text = row.Cells["Phone"]?.Value?.ToString() ?? "";

            btnEdit.Enabled = true;
            btnDelete.Enabled = true;
            btnAdd.Enabled = false;
        }
    }

    private void BtnAdd_Click(object sender, EventArgs e)
    {
        if (ValidateBorrowerInput())
        {
            string query = $@"
                INSERT INTO Borrowers (Name, Email, Phone)
                VALUES ('{txtName.Text}', '{txtEmail.Text}', '{txtPhone.Text}')
            ";

            DatabaseHelper.ExecuteNonQuery(query);
            MessageBox.Show("Borrower added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearFields();
            LoadBorrowers();
        }
    }

    private void BtnEdit_Click(object sender, EventArgs e)
    {
        if (selectedBorrowerId > 0 && ValidateBorrowerInput())
        {
            string query = $@"
                UPDATE Borrowers SET
                Name = '{txtName.Text}',
                Email = '{txtEmail.Text}',
                Phone = '{txtPhone.Text}'
                WHERE Id = {selectedBorrowerId}
            ";

            DatabaseHelper.ExecuteNonQuery(query);
            MessageBox.Show("Borrower updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearFields();
            LoadBorrowers();
        }
    }

    private void BtnDelete_Click(object sender, EventArgs e)
    {
        if (selectedBorrowerId > 0)
        {
            if (MessageBox.Show("Are you sure you want to delete this borrower?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string query = $"DELETE FROM Borrowers WHERE Id = {selectedBorrowerId}";
                DatabaseHelper.ExecuteNonQuery(query);
                MessageBox.Show("Borrower deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadBorrowers();
            }
        }
    }

    private void BtnIssueBook_Click(object sender, EventArgs e)
    {
        IssueReturnBookForm issueForm = new IssueReturnBookForm(false);
        issueForm.ShowDialog();
    }

    private void BtnReturnBook_Click(object sender, EventArgs e)
    {
        IssueReturnBookForm returnForm = new IssueReturnBookForm(true);
        returnForm.ShowDialog();
    }

    private bool ValidateBorrowerInput()
    {
        if (string.IsNullOrEmpty(txtName.Text))
        {
            MessageBox.Show("Please enter borrower name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        return true;
    }

    private void ClearFields()
    {
        txtName.Clear();
        txtEmail.Clear();
        txtPhone.Clear();
        selectedBorrowerId = -1;
        dgvBorrowers.ClearSelection();
        btnEdit.Enabled = false;
        btnDelete.Enabled = false;
        btnAdd.Enabled = true;
    }
}

public class IssueReturnBookForm : Form
{
    private ComboBox cmbBorrowers;
    private ComboBox cmbBooks;
    private Button btnAction;
    private bool isReturn;

    public IssueReturnBookForm(bool isReturn)
    {
        this.isReturn = isReturn;
        InitializeComponents();
        LoadData();
    }

    private void InitializeComponents()
    {
        this.Text = isReturn ? "Return Book" : "Issue Book";
        this.Size = new System.Drawing.Size(400, 200);
        this.StartPosition = FormStartPosition.CenterScreen;

        Label lblBorrower = new Label() { Text = "Borrower:", Location = new System.Drawing.Point(20, 20), Size = new System.Drawing.Size(100, 20) };
        Label lblBook = new Label() { Text = "Book:", Location = new System.Drawing.Point(20, 60), Size = new System.Drawing.Size(100, 20) };

        cmbBorrowers = new ComboBox() { Location = new System.Drawing.Point(130, 20), Size = new System.Drawing.Size(200, 20), DropDownStyle = ComboBoxStyle.DropDownList };
        cmbBooks = new ComboBox() { Location = new System.Drawing.Point(130, 60), Size = new System.Drawing.Size(200, 20), DropDownStyle = ComboBoxStyle.DropDownList };

        btnAction = new Button()
        {
            Text = isReturn ? "Return Book" : "Issue Book",
            Location = new System.Drawing.Point(130, 100),
            Size = new System.Drawing.Size(200, 30)
        };
        btnAction.Click += BtnAction_Click;

        this.Controls.AddRange(new Control[] {
            lblBorrower, lblBook,
            cmbBorrowers, cmbBooks,
            btnAction
        });
    }

    private void LoadData()
    {
        if (isReturn)
        {
            // Load only borrowers with issued books
            string query = @"
                SELECT DISTINCT b.Id, b.Name 
                FROM Borrowers b
                JOIN IssuedBooks ib ON b.Id = ib.BorrowerId
                WHERE ib.ReturnDate IS NULL
            ";
            DataTable dt = DatabaseHelper.ExecuteQuery(query);

            cmbBorrowers.DisplayMember = "Name";
            cmbBorrowers.ValueMember = "Id";
            cmbBorrowers.DataSource = dt;

            cmbBorrowers.SelectedIndexChanged += (sender, e) => {
                if (cmbBorrowers.SelectedValue != null)
                {
                    int borrowerId = Convert.ToInt32(cmbBorrowers.SelectedValue);
                    LoadBooksForReturn(borrowerId);
                }
            };
        }
        else
        {
            // Load all borrowers
            string borrowerQuery = "SELECT Id, Name FROM Borrowers";
            DataTable borrowerDt = DatabaseHelper.ExecuteQuery(borrowerQuery);

            cmbBorrowers.DisplayMember = "Name";
            cmbBorrowers.ValueMember = "Id";
            cmbBorrowers.DataSource = borrowerDt;

            // Load available books
            string bookQuery = "SELECT Id, Title FROM Books WHERE Quantity > 0";
            DataTable bookDt = DatabaseHelper.ExecuteQuery(bookQuery);

            cmbBooks.DisplayMember = "Title";
            cmbBooks.ValueMember = "Id";
            cmbBooks.DataSource = bookDt;
        }
    }

    private void LoadBooksForReturn(int borrowerId)
    {
        string query = $@"
            SELECT b.Id, b.Title 
            FROM Books b
            JOIN IssuedBooks ib ON b.Id = ib.BookId
            WHERE ib.BorrowerId = {borrowerId} AND ib.ReturnDate IS NULL
        ";
        DataTable dt = DatabaseHelper.ExecuteQuery(query);

        cmbBooks.DisplayMember = "Title";
        cmbBooks.ValueMember = "Id";
        cmbBooks.DataSource = dt;
    }

    private void BtnAction_Click(object sender, EventArgs e)
    {
        if (cmbBorrowers.SelectedValue == null || cmbBooks.SelectedValue == null)
        {
            MessageBox.Show("Please select both borrower and book.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        int borrowerId = Convert.ToInt32(cmbBorrowers.SelectedValue);
        int bookId = Convert.ToInt32(cmbBooks.SelectedValue);

        if (isReturn)
        {
            ReturnBook(bookId, borrowerId);
        }
        else
        {
            IssueBook(bookId, borrowerId);
        }
    }

    private void IssueBook(int bookId, int borrowerId)
    {
        // Check if book is available
        string checkQuery = $"SELECT Quantity FROM Books WHERE Id = {bookId}";
        int quantity = Convert.ToInt32(DatabaseHelper.ExecuteScalar(checkQuery));

        if (quantity <= 0)
        {
            MessageBox.Show("This book is not available for issue.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        // Decrease book quantity
        string updateQuery = $"UPDATE Books SET Quantity = Quantity - 1 WHERE Id = {bookId}";
        DatabaseHelper.ExecuteNonQuery(updateQuery);

        // Record issue
        string issueQuery = $@"
            INSERT INTO IssuedBooks (BookId, BorrowerId, IssueDate)
            VALUES ({bookId}, {borrowerId}, datetime('now'))
        ";
        DatabaseHelper.ExecuteNonQuery(issueQuery);

        MessageBox.Show("Book issued successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        this.Close();
    }

    private void ReturnBook(int bookId, int borrowerId)
    {
        // Increase book quantity
        string updateQuery = $"UPDATE Books SET Quantity = Quantity + 1 WHERE Id = {bookId}";
        DatabaseHelper.ExecuteNonQuery(updateQuery);

        // Record return
        string returnQuery = $@"
            UPDATE IssuedBooks 
            SET ReturnDate = datetime('now')
            WHERE BookId = {bookId} AND BorrowerId = {borrowerId} AND ReturnDate IS NULL
        ";
        DatabaseHelper.ExecuteNonQuery(returnQuery);

        MessageBox.Show("Book returned successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        this.Close();
    }
}