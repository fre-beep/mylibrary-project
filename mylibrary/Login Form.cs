using System;
using System.Windows.Forms;

public class LoginForm : Form
{
    private TextBox txtUsername;
    private TextBox txtPassword;
    private CheckBox chkShowPassword;
    private Button btnLogin;

    public LoginForm()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        this.Text = "Login";
        this.Size = new System.Drawing.Size(300, 250);
        this.StartPosition = FormStartPosition.CenterScreen;

        Label lblUsername = new Label();
        lblUsername.Text = "Username";
        lblUsername.Location = new System.Drawing.Point(30, 30);
        lblUsername.Size = new System.Drawing.Size(100, 20);

        txtUsername = new TextBox();
        txtUsername.Location = new System.Drawing.Point(30, 50);
        txtUsername.Size = new System.Drawing.Size(200, 20);

        Label lblPassword = new Label();
        lblPassword.Text = "Password";
        lblPassword.Location = new System.Drawing.Point(30, 80);
        lblPassword.Size = new System.Drawing.Size(100, 20);

        txtPassword = new TextBox();
        txtPassword.Location = new System.Drawing.Point(30, 100);
        txtPassword.Size = new System.Drawing.Size(200, 20);
        txtPassword.PasswordChar = '*';

        chkShowPassword = new CheckBox();
        chkShowPassword.Text = "Show Password";
        chkShowPassword.Location = new System.Drawing.Point(30, 130);
        chkShowPassword.Size = new System.Drawing.Size(150, 20);
        chkShowPassword.CheckedChanged += (sender, e) => {
            txtPassword.PasswordChar = chkShowPassword.Checked ? '\0' : '*';
        };

        btnLogin = new Button();
        btnLogin.Text = "Login";
        btnLogin.Location = new System.Drawing.Point(30, 160);
        btnLogin.Size = new System.Drawing.Size(200, 30);
        btnLogin.Click += BtnLogin_Click;

        this.Controls.Add(lblUsername);
        this.Controls.Add(txtUsername);
        this.Controls.Add(lblPassword);
        this.Controls.Add(txtPassword);
        this.Controls.Add(chkShowPassword);
        this.Controls.Add(btnLogin);
    }

    private void BtnLogin_Click(object sender, EventArgs e)
    {
        string username = txtUsername.Text.Trim();
        string password = txtPassword.Text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            MessageBox.Show("Please enter both username and password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        string query = $"SELECT COUNT(*) FROM Users WHERE Username = '{username}' AND Password = '{password}'";
        int count = Convert.ToInt32(DatabaseHelper.ExecuteScalar(query));

        if (count > 0)
        {
            MessageBox.Show("Login successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Hide();
            MainForm mainForm = new MainForm();
            mainForm.Show();
        }
        else
        {
            MessageBox.Show("Invalid username or password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}