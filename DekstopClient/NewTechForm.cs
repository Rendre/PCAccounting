using System.Globalization;
using DB.Entities;
using MySql.Data.MySqlClient;

namespace DekstopClient
{
    public partial class NewTechForm : Form
    {
        public int ComputerID { get; set; }//по этому айди делается запрос и заполняется форма
        private Computer computer = new();
        public NewTechForm()
        {
            InitializeComponent();
            //подключиться к бд и заполнить комбобоксы
            //comboBox1.Items.Add(1);
        }

        private void AddClick(object sender, EventArgs e)
        {
            var str = string.Format(
                "server={0}; database={1}; charset=utf8; user id={2}; password={3}; pooling=false;",
                "127.0.0.1", "retraincorp", "root", "root");
            var connection = new MySqlConnection(str);
            string sqlExpression;
            MySqlCommand command;
            if (!checkBox1.Checked)
            {
                var name = textBox1.Text;
                var status = (int)(StatusEnum)comboBox1.SelectedItem;
                var employer = (Employer)comboBox2.SelectedItem;
                var date = dateTimePicker1.Value;
                var cpu = textBox5.Text;
                var value = textBox6.Text;

                if (name.Length > 0 &&
                    status > 0 &&
                    employer != null &&
                    cpu.Length > 0 &&
                    value.Length > 0 &&
                    date.Year > 2010)
                {
                    using (connection)
                    {

                        sqlExpression =
                            "INSERT INTO `technick` (`Name`, `StatusID`, `EmployerID`, `DateCreated`, `Cpu`, `Price`) " +
                            $"VALUES ('{name}', '{status}', '{employer.Id}', '{date.ToString("yyyy-MM-dd HH:mm:ss")}', '{cpu}', '{value}');";

                        connection.Open();
                        command = new MySqlCommand(sqlExpression, connection);
                        command.ExecuteNonQuery();
                        //------------------------------------
                        //как обратиться к дата грид вью в другой форме?

                        //sqlExpression = "SELECT * FROM technick";
                        //command = new MySqlCommand(sqlExpression, connection);
                        //var reader = command.ExecuteReader();
                        //var table = new DataTable();
                        //table.Load(reader);
                        //MainForm.
                        //dataGridView3.DataSource = table;
                        //reader.Close();
                    }

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
            var currComputer = new Computer();
            currComputer.Name = textBox1.Text;
            currComputer.Status = (int)(StatusEnum)comboBox1.SelectedItem;
            currComputer.EmployerId = ((Employer)comboBox2.SelectedItem).Id;
            currComputer.Date = dateTimePicker1.Value;
            currComputer.Cpu = textBox5.Text;
            currComputer.Price = Convert.ToDecimal(textBox6.Text);
            using (connection)
            {
                sqlExpression = $"UPDATE technick SET Name = '{currComputer.Name}', " +
                                $"StatusID = {currComputer.Status}, " +
                                $"EmployerID = {currComputer.EmployerId}, " +
                                $"DateCreated = '{currComputer.Date:yyyy-MM-dd HH:mm:ss}', " +
                                $"Cpu = '{currComputer.Cpu}', " +
                                $"Price = '{currComputer.Price.ToString(CultureInfo.InvariantCulture)}' " +
                                $"WHERE ID = {ComputerID}";
                connection.Open();
                command = new MySqlCommand(sqlExpression, connection);
                command.ExecuteNonQuery();
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ExitClick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void NewTechForm_Load(object sender, EventArgs e)
        {
            comboBox1.Items.AddRange(new object[] { StatusEnum.Defective, StatusEnum.Properly, StatusEnum.UnderRepair });
            var str = string.Format(
                "server={0}; database={1}; charset=utf8; user id={2}; password={3}; pooling=false;", "127.0.0.1",
                "retraincorp", "root", "root");
            var connection = new MySqlConnection(str);
            using (connection)
            {
                var sqlExpression = "SELECT ID, Name FROM employers";
                connection.Open();
                var command = new MySqlCommand(sqlExpression, connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var employer = new Employer() { Id = (int)reader.GetValue(0), Name = (string)reader.GetValue(1) };
                    comboBox2.Items.Add(employer);
                }
            }

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
            using (connection)
            {
                var sqlExpression = $"SELECT * FROM technick WHERE ID = {ComputerID}";
                connection.Open();
                var command = new MySqlCommand(sqlExpression, connection);
                var reader = command.ExecuteReader();
                reader.Read();

                var name = (string)reader.GetValue(1);
                var statusId = Convert.ToInt32(reader.GetValue(2));
                var employerId = Convert.ToInt32(reader.GetValue(3));
                var date = Convert.ToDateTime(reader.GetValue(4));
                var cpu = (string)reader.GetValue(5);
                var price = (decimal)reader.GetValue(6);

                textBox1.Text = name;
                var index = comboBox1.Items.IndexOf((StatusEnum)statusId);
                comboBox1.SelectedIndex = index;
                var employers = comboBox2.Items.Cast<Employer>().First(e => e.Id == employerId);
                index = comboBox2.Items.IndexOf(employers);
                comboBox2.SelectedIndex = index;
                dateTimePicker1.Value = date;
                textBox5.Text = cpu;
                textBox6.Text = price.ToString();

                computer.Name = name;
                computer.Status = statusId;
                computer.EmployerId = employerId;
                computer.Date = date;
                computer.Cpu = cpu;
                computer.Price = price;
            }
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
                var index = comboBox1.Items.IndexOf((StatusEnum)computer.Status);
                comboBox1.SelectedIndex = index;
                // из сотрудников, хранящихся в комбобоксе - берем того, у которого индекс совпадает с искомым
                var employers = comboBox2.Items.Cast<Employer>().First(e => e.Id == computer.EmployerId);
                index = comboBox2.Items.IndexOf(employers);
                comboBox2.SelectedIndex = index;
                dateTimePicker1.Value = computer.Date;
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
