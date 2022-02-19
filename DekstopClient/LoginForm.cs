using System.Data;
using System.Drawing.Text;
using DekstopClient.Entities;
using DekstopClient.Services;
using DekstopClient.Services.LoginService;
using DekstopClient.Services.RegistrationService;
using MySql.Data.MySqlClient;

namespace DekstopClient
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            textBox1.Text = "rendre";
            textBox2.Text = "1234";
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }

        private void EnterClick(object sender, EventArgs e)
        {
            var simpleLoginService = new SimpleLoginService();
            Login(simpleLoginService);
        }

        private void Login(ILoginService service)
        {
            var login = textBox1.Text;
            var password = textBox2.Text;
            var user = service.Login(login, password);
            if (user != null)
            {
                var mainForm = new MainForm();
                mainForm.User = user;
                mainForm.LoginForm = this;
                mainForm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show(
                    "Введен неверный логин или пароль!",
                    "Внимание",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.DefaultDesktopOnly);
            }
        }

        private void RegistrationClick(object sender, EventArgs e)
        {
            IRegistrationService registrationService = new RegistrationService();
            Registration(registrationService);

        }

        private void Registration(IRegistrationService registrationService)
        {
            var login = textBox1.Text;
            var password = textBox2.Text;
            var isSuccess = registrationService.Registration(login, password);

            if (isSuccess)
            {
                MessageBox.Show(
                    "Регистрация завершена",
                    "Поздравляем!)))",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.DefaultDesktopOnly);
            }
            else
            {
                MessageBox.Show(
                    "Такой логин уже существует!",
                    "Внимание",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.DefaultDesktopOnly);
            }

        }
    }
}




