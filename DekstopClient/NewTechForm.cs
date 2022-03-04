using DB.Entities;
using DB.Repositories.Computer;
using DB.Repositories.Employer;

namespace DekstopClient
{
    public partial class NewTechForm : Form
    {
        public uint ComputerID { get; set; }//по этому айди делается запрос и заполняется форма
        private Computer computer = new();
        private readonly IComputerRepository _computerRepository;
        private readonly IEmployerRepository _employerRepository;
        public NewTechForm(IComputerRepository computerRepository, IEmployerRepository employerRepository)
        {
            InitializeComponent();
            _computerRepository = computerRepository;
            _employerRepository = employerRepository;
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
                    computer.Name = name;
                    computer.StatusID = status;
                    computer.EmployerId = employer.Id;
                    computer.DateCreated = date;
                    computer.Cpu = cpu;
                    computer.Price = Convert.ToDecimal(price);

                    _computerRepository.CreateComputer(computer);

                    DialogResult = DialogResult.OK;
                    MessageBox.Show("Данные успешно добавлены.");
                    Close();
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
            currComputer.EmployerId = ((Employer)comboBox2.SelectedItem).Id;
            currComputer.DateCreated = dateTimePicker1.Value;
            currComputer.Cpu = textBox5.Text;
            currComputer.Price = Convert.ToDecimal(textBox6.Text);
            
            _computerRepository.ChangeComputer(currComputer);
            DialogResult = DialogResult.OK;
        }

        private void ExitClick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void NewTechForm_Load(object sender, EventArgs e)
        {
            comboBox1.Items.AddRange(new object[] { StatusEnum.Defective, StatusEnum.Properly, StatusEnum.UnderRepair, StatusEnum.Null });
            comboBox2.Items.Add(new Employer() {Id = 0, Name = "null"});
            var employerList = _employerRepository.GetItems(null, null, null);
            comboBox2.Items.AddRange(employerList.ToArray());

            if (ComputerID == 0)
            {
                checkBox1.Hide();
                return;
            }

            button1.Hide();
            button2.Hide();
            textBox1.ReadOnly = true;
            textBox5.ReadOnly = true;
            textBox6.ReadOnly = true;
            comboBox1.Enabled = false;
            comboBox2.Enabled = false;
            dateTimePicker1.Enabled = false;

            computer = _computerRepository.GetComputer(ComputerID);
            textBox1.Text = computer.Name;
            var index = comboBox1.Items.IndexOf((StatusEnum)computer.StatusID);
            comboBox1.SelectedIndex = index;
            var employers = comboBox2.Items.Cast<Employer>().First(e => e.Id == computer.EmployerId);
            index = comboBox2.Items.IndexOf(employers);
            comboBox2.SelectedIndex = index;
            dateTimePicker1.Value = computer.DateCreated;
            textBox5.Text = computer.Cpu;
            textBox6.Text = computer.Price.ToString();
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
            }
            else
            {
                button2.Hide();
                textBox1.Text = computer.Name;
                var index = comboBox1.Items.IndexOf((StatusEnum)computer.StatusID);
                comboBox1.SelectedIndex = index;
                // из сотрудников, хранящихся в комбобоксе - берем того, у которого индекс совпадает с искомым
                var employers = comboBox2.Items.Cast<Employer>().First(e => e.Id == computer.EmployerId);
                index = comboBox2.Items.IndexOf(employers);
                comboBox2.SelectedIndex = index;
                dateTimePicker1.Value = computer.DateCreated;
                textBox5.Text = computer.Cpu;
                textBox6.Text = computer.Price.ToString();
            }

            textBox1.ReadOnly = !isChecked;
            comboBox1.Enabled = isChecked;
            comboBox2.Enabled = isChecked;
            dateTimePicker1.Enabled = isChecked;
            textBox5.ReadOnly = !isChecked;
            textBox6.ReadOnly = !isChecked;
        }
    }
}
