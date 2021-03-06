using DB.Entities;
using DB.Repositories.Computers;
using DB.Repositories.Employers;
using DB.Repositories.Files;
using SharedKernel.Utils;

namespace DekstopClient;

public partial class MainForm : Form
{
    public User User { get; set; }
    public LoginForm LoginForm { get; set; }

    private readonly IEmployerRepository _employerRepository;
    private readonly IComputerRepository _computerRepository;

    public MainForm(IEmployerRepository employerRepository, IComputerRepository computerRepository,
        User user, LoginForm loginForm)
    {
        InitializeComponent();
        radioButton1.Checked = true;
        _employerRepository = employerRepository;
        _computerRepository = computerRepository;
        User = user;
        LoginForm = loginForm;
    }

    private void MainForm_Shown(object sender, EventArgs e)
    {
        Text = $"????? ??????????, {User.Login}";
        textBox3.MaxLength = 12;
        textBox3.TextAlign = HorizontalAlignment.Center;
    }

    private void RefreshDbClick(object sender, EventArgs e)
    {
        RefreshDb();
    }

    private void AddNewTechClick(object sender, EventArgs e)
    {
        //?? ?????? ????????
        var newTechForm = new NewTechForm(new ComputerDapperRepository(), new EmployerDapperRepository(), new FileDapperRepository());
        var result = newTechForm.ShowDialog();
        if (result == DialogResult.OK)
        {
            RefreshDb();
        }
    }

    private void ShowTechClick(object sender, EventArgs e)
    {
        var newTechForm = new NewTechForm(new ComputerDapperRepository(), new EmployerDapperRepository(), new FileDapperRepository());
        try
        {
            var currentRow = dataGridView3.CurrentCell.RowIndex;
            newTechForm.ComputerID = (uint)dataGridView3["Id", currentRow].Value;
            var result = newTechForm.ShowDialog();
            if (result == DialogResult.OK)
            {
                RefreshDb();
            }
        }
        catch (NullReferenceException)
        {
            MessageBox.Show("???????? ????????? ???? ?????? ? ????????? ?????????? ??? ?????????!");
        }
    }

    private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (tabControl1.SelectedIndex != 1) return;

        var employers = _employerRepository.GetItems();
        dataGridView2.DataSource = employers;
        dataGridView2.Columns["Id"]!.DisplayIndex = 0;
        dataGridView2.Columns["IsDeleted"]!.Visible = false;
    }

    private void AddOrChangeClick(object sender, EventArgs e)
    {
        var name = textBox1.Text;
        var position = textBox2.Text;
        var tel = Util.CheckTelNumber(textBox3.Text);
        if (name.Length == 0 ||
            position.Length == 0 ||
            tel.Length == 0)
        {
            MessageBox.Show("dfgf");
            return;
        }

        if (radioButton1.Checked)
        {
            var employer = new Employer {Name = name, Position = position, Tel = tel};
            _employerRepository.SaveItem(employer);

            var table = _employerRepository.GetItems();
            dataGridView2.DataSource = table;

        }
        else if (radioButton2.Checked)
        {

            var currentRow = dataGridView2.CurrentCell.RowIndex;
            var currentColumn = dataGridView2.CurrentCell.ColumnIndex;
            var id = (uint)dataGridView2["Id", currentRow].Value;
            var employer = new Employer {ID = id, Name = name, Position = position, Tel = tel};

            _employerRepository.SaveItem(employer);

            var table = _employerRepository.GetItems();
            dataGridView2.DataSource = table;


            //?????? ????? ?? ??????? ?????
            dataGridView2.CurrentCell = dataGridView2.Rows[currentRow].Cells[currentColumn];

        }
    }

    private void ChangeRadioButton(object sender, EventArgs e)
    {
        button7.Text = "????????";
        var currentRow = dataGridView2.CurrentCell.RowIndex;
        textBox1.Text = (string)dataGridView2["Name", currentRow].Value;
        textBox2.Text = (string)dataGridView2["Position", currentRow].Value;
        textBox3.Text = (string)dataGridView2["Tel", currentRow].Value;
    }
    private void DataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
    {
        if (radioButton2.Checked)
        {
            var currentRow = dataGridView2.CurrentCell.RowIndex;
            textBox1.Text = (string)dataGridView2["Name", currentRow].Value;
            textBox2.Text = (string)dataGridView2["Position", currentRow].Value;
            textBox3.Text = (string)dataGridView2["Tel", currentRow].Value;
        }

    }

    private void AddRadioButton(object sender, EventArgs e)
    {
        button7.Text = "????????";
        textBox1.Clear();
        textBox2.Clear();
        textBox3.Clear();
    }

    private void DeleteTechClick(object sender, EventArgs e)
    {
        try
        {
            var currentRow = dataGridView3.CurrentCell.RowIndex;
            var id = (uint)dataGridView3["Id", currentRow].Value;

            var result = MessageBox.Show(
                "?? ????????????? ?????? ??????? ??????????",
                "????????",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button2,
                MessageBoxOptions.DefaultDesktopOnly);

            if (result == DialogResult.Yes)
            {
                var computer = _computerRepository.GetItem(id);
                computer.IsDeleted = true;
                _computerRepository.SaveItem(computer);
                RefreshDb();
            }
        }
        catch (Exception)
        {
            MessageBox.Show("???????? ????????? ???? ?????? ? ????????? ?????????? ??? ????????!");
        }
    }

    private void RefreshDb()
    {
        dataGridView3.DataSource = null;
        var table = _computerRepository.GetItems();
        dataGridView3.DataSource = table;
        dataGridView3.Columns["ID"]!.DisplayIndex = 0;
        dataGridView3.Columns["IsDeleted"]!.Visible = false;
    }

    private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
    {
        LoginForm.Close();
    }
}