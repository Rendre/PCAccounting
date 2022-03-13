using DB.Entities;
using DB.Repositories.Computer;
using DB.Repositories.Employer;
using DB.Repositories.User;
using DB.Utils;
using DekstopClient.Services.LoginService;
using DekstopClient.Services.RegistrationService;

namespace DekstopClient;

public partial class LoginForm : Form
{
    public LoginForm()
    {
        InitializeComponent();
        textBox2.PasswordChar = '*';
        textBox1.TextAlign = HorizontalAlignment.Center;
        textBox2.TextAlign = HorizontalAlignment.Center;
        textBox1.Text = "test";
        textBox2.Text = "test";
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
        var password = Util.Encode(textBox2.Text);
        var user = service.Login(login);
        if (user is {Pass: { }} && (user.Pass.Equals(password)))
        {
            var mainForm = new MainForm(new EmployerDapperRepository(), new ComputerDapperRepository(), user, this);
            mainForm.Show();
            Hide();
        }
        else if (user == null)
        {
            MessageBox.Show(
                "Введен неверный логин!",
                "Внимание",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.DefaultDesktopOnly);
        }
        else
        {
            MessageBox.Show(
                "Введен неверный пароль!",
                "Внимание",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.DefaultDesktopOnly);
        }
    }

    private void RegistrationClick(object sender, EventArgs e)
    {
        IRegistrationService registrationService = new RegistrationService(new UserDefaultRepository());
        Registration(registrationService);
    }

    private void Registration(IRegistrationService registrationService)
    {
        var login = textBox1.Text;
        var password = Util.Encode(textBox2.Text);
        var user = new User() {Login = login, Pass = password};
        var isSuccess = registrationService.Registration(user);

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