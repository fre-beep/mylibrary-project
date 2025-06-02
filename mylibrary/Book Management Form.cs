using System;
using System.Data;
using System.Windows.Forms;

public class BookForm : Form
{
    private DataGridView dgvBooks;
    private TextBox txtTitle;
    private TextBox txtAuthor;
    private TextBox txtYear;
    private TextBox txtQuantity;
    private Button btnAdd;
    private Button btnEdit;
    private Button btnDelete;
    private Button btnClear;

    private int selectedBookId = -1;

    public BookForm()
    {
        InitializeComponents();
        LoadBooks();
    }

    private void InitializeComponents()
    {
        this.Text = "Book Management";
        this.Size = new System.Drawing.Size(600, 500);
        this.StartPosition = FormStartPosition.CenterScreen;

        // Labels
        Label lblTitle = new Label() { Text = "Title:", Location = new System.Drawing.Point(20, 20), Size = new System.Drawing.Size(100, 20) };
        Label lblAuthor = new Label() { Text = "Author:", Location = new System.Drawing.Point(20, 60), Size = new System.Drawing.Size(100, 20) };
        Label lblYear = new Label() { Text = "Year:", Location = new System.Drawing.Point(20, 100), Size = new System.Drawing.Size(100, 20) };
        Label lblQuantity = new Label() { Text = "Available Copies:", Location = new System.Drawing.Point(20, 140), Size = new System.Drawing.Size(100, 20) };

        // TextBoxes
        txtTitle = new TextBox() { Location = new System.Drawing.Point(130, 20), Size = new System.Drawing.Size(200, 20) };
        txtAuthor = new TextBox() { Location = new System.Drawing.Point(130, 60), Size = new System.Drawing.Size(200, 20) };
        txtYear = new TextBox() { Location = new System.Drawing.Point(130, 100), Size = new System.Drawing.Size(200, 20) };
        txtQuantity = new TextBox() { Location = new System.Drawing.Point(130, 140), Size = new System.Drawing.Size(200, 20) };

        // Buttons
        btnAdd = new Button() { Text = "Add", Location = new System.Drawing.Point(350, 20), Size = new System.Drawing.Size(80, 30) };
        btnEdit = new Button() { Text = "Edit", Location = new System.Drawing.Point(350, 60), Size = new System.Drawing.Size(80, 30), Enabled = false };
        btnDelete = new Button() { Text = "Delete", Location = new System.Drawing.Point(350, 100), Size = new System.Drawing.Size(80, 30), Enabled = false };
        btnClear = new Button() { Text = "Clear", Location = new System.Drawing.Point(350, 140), Size = new System.Drawing.Size(80, 30) };

        btnAdd.Click += BtnAdd_Click;
        btnEdit.Click += BtnEdit_Click;
        btnDelete.Click += BtnDelete_Click;
        btnClear.Click += BtnClear_Click;

        // DataGridView
        dgvBooks = new DataGridView();
        dgvBooks.Location = new System.Drawing.Point(20, 180);
        dgvBooks.Size = new System.Drawing.Size(550, 250);
        dgvBooks.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvBooks.MultiSelect = false;
        dgvBooks.ReadOnly = true;
        dgvBooks.AllowUserToAddRows = false;
        dgvBooks.SelectionChanged += DgvBooks_SelectionChanged;

        // Add columns to DataGridView
        dgvBooks.Columns.Add("Id", "Id");
        dgvBooks.Columns.Add("Title", "Title");
        dgvBooks.Columns.Add("Author", "Author");
        dgvBooks.Columns.Add("Year", "Year");
        dgvBooks.Columns.Add("Quantity", "Available Copies");

        // Add controls to form
        this.Controls.AddRange(new Control[] {
            lblTitle, lblAuthor, lblYear, lblQuantity,
            txtTitle, txtAuthor, txtYear, txtQuantity,
            btnAdd, btnEdit, btnDelete, btnClear,
            dgvBooks
        });
    }

    private void LoadBooks()
    {
        dgvBooks.Rows.Clear();
        string query = "SELECT * FROM Books";
        DataTable dt = DatabaseHelper.ExecuteQuery(query);

        foreach (DataRow row in dt.Rows)
        {
            dgvBooks.Rows.Add(
                row["Id"],
                row["Title"],
                row["Author"],
                row["Year"],
                row["Quantity"]
            );
        }
    }

    private void DgvBooks_SelectionChanged(object sender, EventArgs e)
    {
        if (dgvBooks.SelectedRows.Count > 0)
        {
            DataGridViewRow row = dgvBooks.SelectedRows[0];
            selectedBookId = Convert.ToInt32(row.Cells["Id"].Value);
            txtTitle.Text = row.Cells["Title"].Value.ToString();
            txtAuthor.Text = row.Cells["Author"].Value.ToString();
            txtYear.Text = row.Cells["Year"].Value.ToString();
            txtQuantity.Text = row.Cells["Quantity"].Value.ToString();

            btnEdit.Enabled = true;
            btnDelete.Enabled = true;
            btnAdd.Enabled = false;
        }
    }

    private void BtnAdd_Click(object sender, EventArgs e)
    {
        if (ValidateBookInput())
        {
            string query = $@"
                INSERT INTO Books (Title, Author, Year, Quantity)
                VALUES ('{txtTitle.Text}', '{txtAuthor.Text}', {txtYear.Text}, {txtQuantity.Text})
            ";

            DatabaseHelper.ExecuteNonQuery(query);
            MessageBox.Show("Book added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearFields();
            LoadBooks();
        }
    }

    private void BtnEdit_Click(object sender, EventArgs e)
    {
        if (selectedBookId > 0 && ValidateBookInput())
        {
            string query = $@"
                UPDATE Books SET
                Title = '{txtTitle.Text}',
                Author = '{txtAuthor.Text}',
                Year = {txtYear.Text},
                Quantity = {txtQuantity.Text}
                WHERE Id = {selectedBookId}
            ";

            DatabaseHelper.ExecuteNonQuery(query);
            MessageBox.Show("Book updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearFields();
            LoadBooks();
        }
    }

    private void BtnDelete_Click(object sender, EventArgs e)
    {
        if (selectedBookId > 0)
        {
            if (MessageBox.Show("Are you sure you want to delete this book?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string query = $"DELETE FROM Books WHERE Id = {selectedBookId}";
                DatabaseHelper.ExecuteNonQuery(query);
                MessageBox.Show("Book deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadBooks();
            }
        }
    }

    private void BtnClear_Click(object sender, EventArgs e)
    {
        ClearFields();
    }

    private bool ValidateBookInput()
    {
        if (string.IsNullOrEmpty(txtTitle.Text))
        {
            MessageBox.Show("Please enter book title.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        if (string.IsNullOrEmpty(txtAuthor.Text))
        {
            MessageBox.Show("Please enter book author.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        if (!int.TryParse(txtYear.Text, out int year) || year <= 0)
        {
            MessageBox.Show("Please enter a valid publication year.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity < 0)
        {
            MessageBox.Show("Please enter a valid quantity.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        return true;
    }

    private void ClearFields()
    {
        txtTitle.Clear();
        txtAuthor.Clear();
        txtYear.Clear();
        txtQuantity.Clear();
        selectedBookId = -1;
        dgvBooks.ClearSelection();
        btnEdit.Enabled = false;
        btnDelete.Enabled = false;
        btnAdd.Enabled = true;
    }
}