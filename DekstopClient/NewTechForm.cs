using System.Text.Json;
using DB;
using DB.Entities;
using DB.Repositories.Computers;
using DB.Repositories.Employers;
using DB.Repositories.Files;
using SharedKernel.Services;
using SharedKernel.Utils;

namespace DekstopClient;

public partial class NewTechForm : Form
{
    // посмотреть нужен ли вообще ComputerID
    public uint ComputerID { get; set; }//по этому айди делается запрос и заполняется форма
    private Computer _computer = new();
    private byte[] _fileBytes;
    private byte[] _reserveFileBytes;
    private FileEntity? _file;
    private bool _isChanged;
    private string? _filePath;
    private readonly IComputerRepository _computerRepository;
    private readonly IEmployerRepository _employerRepository;
    private readonly IFileRepository _fileRepository;
    public NewTechForm(IComputerRepository computerRepository, IEmployerRepository employerRepository, IFileRepository fileRepository)
    {
        InitializeComponent();
        _computerRepository = computerRepository;
        _employerRepository = employerRepository;
        _fileRepository = fileRepository;
    }

    private void NewTechForm_Load(object sender, EventArgs e)
    {
        comboBox1.Items.AddRange(new object[] { StatusEnum.Defective, StatusEnum.Properly, StatusEnum.UnderRepair, StatusEnum.Null });
        comboBox2.Items.Add(new Employer { ID = 0, Name = "null" });
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

        _computer = _computerRepository.GetItem(ComputerID);
        textBox1.Text = _computer.Name;
        var index = comboBox1.Items.IndexOf((StatusEnum)_computer.StatusID);
        comboBox1.SelectedIndex = index;
        var employers = comboBox2.Items.Cast<Employer>().First(p => p.ID == _computer.EmployerID);
        index = comboBox2.Items.IndexOf(employers);
        comboBox2.SelectedIndex = index;
        dateTimePicker1.Value = _computer.DateCreated;
        textBox5.Text = _computer.Cpu;
        textBox6.Text = _computer.Price.ToString();

        //вывод картинки при просмотре компа
        var files = _fileRepository.GetItems(computerID: ComputerID, orderBy: "ID", desc: true, skip: 0, take: 1);
        if (files.Count <= 0) return;

        _file = files[0];
        _reserveFileBytes = File.ReadAllBytes(_file.Path);
        pictureBox1.Image = Image.FromStream(new MemoryStream(_reserveFileBytes));
        pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

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
                _computer.EmployerID = employer.ID;
                _computer.DateCreated = date;
                _computer.Cpu = cpu;
                _computer.Price = Convert.ToDecimal(price);

                _computerRepository.SaveItem(_computer);

                // тут у нового компа появился айди
                // сохр картинку
                if (!string.IsNullOrEmpty(_filePath))
                {
                    var dekstopSave = new DekstopSave();
                    var directory = new DirectoryInfo(Environment.CurrentDirectory).Parent?.Parent?.Parent?.Parent;
                    var pathForSaveFile = directory + "\\Images\\";
                    var fileBytes = File.ReadAllBytes(_filePath);
                    dekstopSave.SaveItem(_computer.ID, fileBytes, _filePath, pathForSaveFile, "",out _file);
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
        var currComputer = _computerRepository.GetItem(ComputerID);
        if (currComputer != null)
        {
            currComputer.Name = textBox1.Text;
            currComputer.StatusID = (uint) (StatusEnum) comboBox1.SelectedItem;
            currComputer.EmployerID = ((Employer) comboBox2.SelectedItem).ID;
            currComputer.DateCreated = dateTimePicker1.Value;
            currComputer.Cpu = textBox5.Text;
            currComputer.Price = Convert.ToDecimal(textBox6.Text);
            // сохр картинки в изменении компа
            if (!string.IsNullOrEmpty(_filePath))
            {
                _reserveFileBytes = null!;
                var dekstopSave = new DekstopSave();
                var directory = new DirectoryInfo(Environment.CurrentDirectory).Parent?.Parent?.Parent?.Parent;
                var pathForSaveFile = directory + "\\Images\\";
                var fileBytes = File.ReadAllBytes(_filePath);
                //var kek = Convert.ToBase64String(fileBytes);
                dekstopSave.SaveItem(_computer.ID, fileBytes, _filePath, pathForSaveFile, "",out _file);
                button5.Show();
            }

            _computerRepository.SaveItem(currComputer);
        }

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
            if (_file is { ID: > 0 })
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
            var employers = comboBox2.Items.Cast<Employer>().First(e => e.ID == _computer.EmployerID);
            index = comboBox2.Items.IndexOf(employers);
            comboBox2.SelectedIndex = index;
            dateTimePicker1.Value = _computer.DateCreated;
            textBox5.Text = _computer.Cpu;
            textBox6.Text = _computer.Price.ToString();
            // тут надо вернуть старую картинку если изменения не подтвердили
            button4.Hide();
            button5.Hide();
            if (_reserveFileBytes != null)
            {
                pictureBox1.Image = Image.FromStream(new MemoryStream(_reserveFileBytes));
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
        if (openFileDialog1.ShowDialog() != DialogResult.OK) return;

        _filePath = openFileDialog1.FileName;
        if (string.IsNullOrEmpty(_filePath)) return;

        if (!Util.CheckFileExtension(_filePath))
        {
            MessageBox.Show(
                "Неверный формат изображения!\n" +
                "Поддерживаемые форматы: jpg, jpeg, bmp, png",
                "Внимание",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.DefaultDesktopOnly);

            _filePath = "";
            return;
        }

        _fileBytes = File.ReadAllBytes(_filePath);
        pictureBox1.Image = Image.FromStream(new MemoryStream(_fileBytes));
        pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
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
            _file.IsDeleted = true;
            _fileRepository.SaveItem(_file);
            // этот код дублируется - потом прибери его
            var files = _fileRepository.GetItems(computerID: ComputerID, orderBy: "ID", desc: true, skip: 0, take: 1);
            if (files.Count > 0)
            {
                _file = files[0];
                if (_file.Path != null) _reserveFileBytes = File.ReadAllBytes(_file.Path);
                pictureBox1.Image = Image.FromStream(new MemoryStream(_reserveFileBytes));
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                _reserveFileBytes = null;
                return;
            }

            _file = null;
            button5.Hide();
            _reserveFileBytes = null;
            pictureBox1.Image = null;
        }
    }

    private async void UploadFileByWebClick(object sender, EventArgs e)
    {
        const string connectionAddress = "https://localhost:7204/File";

        if (openFileDialog1.ShowDialog() != DialogResult.OK) return;

        _filePath = openFileDialog1.FileName;
        if (string.IsNullOrEmpty(_filePath)) return;

        if (!Util.CheckFileExtension(_filePath))
        {
            MessageBox.Show(
                "Неверный формат изображения!\n" +
                "Поддерживаемые форматы: jpg, jpeg, bmp, png",
                "Внимание",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.DefaultDesktopOnly);

            _filePath = "";
            return;
        }

        var fName = openFileDialog1.SafeFileName;

        const int _count = 1024 * 1024;
        var fileStream = new FileStream(_filePath, FileMode.Open, FileAccess.Read);
        var buffer = new byte[_count];
        var result = new ResultClass();
        var jsonDeserializeOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        while (fileStream.Read(buffer, 0, _count) != 0)
        {
            var fileID = result.Data is {ID: > 0} ? result.Data.ID.ToString() : "";
            var resultJson = Utils.Util.RequestHelper(buffer, connectionAddress, fName, ComputerID.ToString(), fileID);
            var strResult = await resultJson.ConfigureAwait(false);
            result = JsonSerializer.Deserialize<ResultClass>(strResult, jsonDeserializeOptions);
            if (result.Success == 0)
            {
                var answer = MessageBox.Show(
                    "Ошибка при загрузке!",
                    "Внимание",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.DefaultDesktopOnly);

                if (answer == DialogResult.OK) return;
            }
        }
        var files = _fileRepository.GetItems(computerID: ComputerID, orderBy: "ID", desc: true, skip: 0, take: 1);
        if (files.Count > 0)
        {
            _file = files[0];
            if (_file != null && _file.Path != null) _reserveFileBytes = File.ReadAllBytes(_file.Path);
            pictureBox1.Image = Image.FromStream(new MemoryStream(_reserveFileBytes));
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            _reserveFileBytes = null;
        }
    }

    class ResultClass
    {
        public int Success { get; set; }
        public FileEntity? Data { get; set; }
    }
}