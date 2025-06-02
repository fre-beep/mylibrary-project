using System;
using System.Windows.Forms;

static class Program
{
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // Initialize database
        DatabaseHelper.InitializeDatabase();

        // Start with login form
        Application.Run(new LoginForm());
    }
}