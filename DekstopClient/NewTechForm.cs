using DB.Entities;
using DB.Repositories.Computer;
using DB.Repositories.Employer;
using DB.Repositories.Picture;
using SharedKernel.Services;
namespace DekstopClient;

public partial class NewTechForm : Form
{
    // посмотреть нужен ли вообще ComputerID
    public uint ComputerID { get; set; }//по этому айди делается запрос и заполняется форма
    private Computer _computer = new();
    private byte[] _pictureBytes;
    private byte[] _reservePictureBytes;
    private Picture _picture;
    private bool _isChanged;
    private string? _filePath;
    private readonly IComputerRepository _computerRepository;
    private readonly IEmployerRepository _employerRepository;
    private readonly IPictureRepository _pictureRepository;
    public NewTechForm(IComputerRepository computerRepository, IEmployerRepository employerRepository, IPictureRepository pictureRepository)
    {
        InitializeComponent();
        _computerRepository = computerRepository;
        _employerRepository = employerRepository;
        _pictureRepository = pictureRepository;
    }

    private void NewTechForm_Load(object sender, EventArgs e)
    {
        comboBox1.Items.AddRange(new object[] { StatusEnum.Defective, StatusEnum.Properly, StatusEnum.UnderRepair, StatusEnum.Null });
        comboBox2.Items.Add(new Employer() { ID = 0, Name = "null" });
        var employerList = _employerRepository.GetItems(null, null, null);
        comboBox2.Items.AddRange(employerList.ToArray());


        if (ComputerID == 0)
        {
            checkBox1.Hide();
            button5.Hide();
            return;
        }

        button1.Hide();
        button2.Hide();
        button4.Hide();
        button5.Hide();
        textBox1.ReadOnly = true;
        textBox5.ReadOnly = true;
        textBox6.ReadOnly = true;
        comboBox1.Enabled = false;
        comboBox2.Enabled = false;
        dateTimePicker1.Enabled = false;

        _computer = _computerRepository.GetComputer(ComputerID);
        textBox1.Text = _computer.Name;
        var index = comboBox1.Items.IndexOf((StatusEnum)_computer.StatusID);
        comboBox1.SelectedIndex = index;
        var employers = comboBox2.Items.Cast<Employer>().First(e => e.ID == _computer.EmployerId);
        index = comboBox2.Items.IndexOf(employers);
        comboBox2.SelectedIndex = index;
        dateTimePicker1.Value = _computer.DateCreated;
        textBox5.Text = _computer.Cpu;
        textBox6.Text = _computer.Price.ToString();

        //вывод картинки при просмотре компа
        var pictures = _pictureRepository.GetItems(ComputerID, "ID", true, 0, 1);
        if (pictures.Count > 0)
        {
            _picture = pictures[0];
            _reservePictureBytes = File.ReadAllBytes(_picture.Path);
            pictureBox1.Image = Image.FromStream(new MemoryStream(_reservePictureBytes));
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        }

    }


    private void AddClick(object sender, EventArgs e)
    {
        if (!checkBox1.Checked)
        {
            var name = textBox1.Text;
            var status = (uint)(StatusEnum)comboBox1.SelectedItem;
            var employer = (Employer)comboBox2.SelectedItem;
            var date = dateTimePicker1.Value;
            var cpu = textBox5.Text;
            var price = textBox6.Text;

            if (name.Length > 0 &&
                status > 0 &&
                employer != null &&
                cpu.Length > 0 &&
                price.Length > 0 &&
                date.Year > 2010)
            {
                _computer.Name = name;
                _computer.StatusID = status;
                _computer.EmployerId = employer.ID;
                _computer.DateCreated = date;
                _computer.Cpu = cpu;
                _computer.Price = Convert.ToDecimal(price);

                _computerRepository.CreateComputer(_computer);

                // тут у нового компа появился айди
                // сохр картинку
                if (!string.IsNullOrEmpty(_filePath))
                {
                    var dekstopSave = new DekstopSave();
                    var directory = Environment.CurrentDirectory;
                    var pathForSavePicture = directory + "../../../../../Images/";
                    dekstopSave.SaveItem(_computer.ID, _filePath, pathForSavePicture, out _picture);
                }
                DialogResult = DialogResult.OK;
                MessageBox.Show("Данные успешно добавлены.");
            }
            else
            {
                MessageBox.Show("Заполните все поля формы!");
            }
            return;
        }

        //изменения компуктера
        var currComputer = _computerRepository.GetComputer(ComputerID);
        currComputer.Name = textBox1.Text;
        currComputer.StatusID = (uint)(StatusEnum)comboBox1.SelectedItem;
        currComputer.EmployerId = ((Employer)comboBox2.SelectedItem).ID;
        currComputer.DateCreated = dateTimePicker1.Value;
        currComputer.Cpu = textBox5.Text;
        currComputer.Price = Convert.ToDecimal(textBox6.Text);
        // сохр картинки в изменении компа
        if (!string.IsNullOrEmpty(_filePath))
        {
            var dekstopSave = new DekstopSave();
            var directory = Environment.CurrentDirectory;
            var pathForSavePicture = directory + "../../../../../Images/";
            dekstopSave.SaveItem(_computer.ID, _filePath, pathForSavePicture, out _picture);
            button5.Show();
        }
        _computerRepository.ChangeComputer(currComputer);
        _isChanged = true;
    }

    private void ExitClick(object sender, EventArgs e)
    {
        if (_isChanged)
        {
            DialogResult = DialogResult.OK;
            Close();
            return;
        }

        DialogResult = DialogResult.Cancel;
        Close();
    }

    private void checkBox1_CheckedChanged(object sender, EventArgs e)
    {
        //сохраняем состояние выбранного объекта
        SwitchMode(checkBox1.Checked);
    }

    private void SwitchMode(bool isChecked)
    {
        button2.Text = isChecked ? "Изменить" : "Добавить";
        if (isChecked)
        {
            button2.Show();
            button4.Show();
            if (_picture is { ID: > 0 })
            {
                button5.Show();
            }
        }
        else
        {
            button2.Hide();
            textBox1.Text = _computer.Name;
            var index = comboBox1.Items.IndexOf((StatusEnum)_computer.StatusID);
            comboBox1.SelectedIndex = index;
            // из сотрудников, хранящихся в комбобоксе - берем того, у которого индекс совпадает с искомым
            var employers = comboBox2.Items.Cast<Employer>().First(e => e.ID == _computer.EmployerId);
            index = comboBox2.Items.IndexOf(employers);
            comboBox2.SelectedIndex = index;
            dateTimePicker1.Value = _computer.DateCreated;
            textBox5.Text = _computer.Cpu;
            textBox6.Text = _computer.Price.ToString();
            // тут надо вернуть старую картинку если изменения не подтвердили
            button4.Hide();
            button5.Hide();
            if (_reservePictureBytes != null)
            {
                pictureBox1.Image = Image.FromStream(new MemoryStream(_reservePictureBytes));
            }
            
        }

        textBox1.ReadOnly = !isChecked;
        comboBox1.Enabled = isChecked;
        comboBox2.Enabled = isChecked;
        dateTimePicker1.Enabled = isChecked;
        textBox5.ReadOnly = !isChecked;
        textBox6.ReadOnly = !isChecked;
    }

    private void button4_Click(object sender, EventArgs e)
    {
        if (openFileDialog1.ShowDialog() == DialogResult.OK)
        {
            _filePath = openFileDialog1.FileName;
            if (!string.IsNullOrEmpty(_filePath))
            {
                _pictureBytes = File.ReadAllBytes(_filePath);
                pictureBox1.Image = Image.FromStream(new MemoryStream(_pictureBytes));
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }
    }

    private void DeleteClick(object sender, EventArgs e)
    {
        var result = MessageBox.Show(
            "Вы действительно хотите удалить изображение?",
            "Внимание",
            MessageBoxButtons.OKCancel,
            MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button2,
            MessageBoxOptions.DefaultDesktopOnly);

        if (result == DialogResult.OK)
        {
            _pictureRepository.DeleteItem(_picture.ID);
            // этот код дублируется - потом прибери его
            var pictures = _pictureRepository.GetItems(ComputerID, "ID", true, 0, 1);
            if (pictures.Count > 0)
            {
                _picture = pictures[0];
                _reservePictureBytes = File.ReadAllBytes(_picture.Path);
                pictureBox1.Image = Image.FromStream(new MemoryStream(_reservePictureBytes));
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                _reservePictureBytes = null;
                return;
            }

            _picture = null;
            button5.Hide();
            _reservePictureBytes = null;
            pictureBox1.Image = null;
        }
    }
}