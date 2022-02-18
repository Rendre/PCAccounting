using System.Data;
using System.Windows.Forms.VisualStyles;
using MySql.Data.MySqlClient;

namespace DekstopClient
{
    public partial class NewTechForm : Form
    {
        public int ComputerID { get; set; }//по этому айди делается запрос и заполняется форма
        public NewTechForm()
        {
            InitializeComponent();
            //подключиться к бд и заполнить комбобоксы
            //comboBox1.Items.Add(1);
        }

        private void AddClick(object sender, EventArgs e)
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
                var str = string.Format("server={0}; database={1}; charset=utf8; user id={2}; password={3}; pooling=false;", "127.0.0.1", "retraincorp", "root", "root");
                using (var connection = new MySqlConnection(str))
                {

                    string sqlExpression = "INSERT INTO `technick` (`Name`, `StatusID`, `EmployerID`, `DateCreated`, `Cpu`, `Price`) " +
                                           $"VALUES ('{name}', '{status}', '{employer.Id}', '{date.ToString("yyyy-MM-dd HH:mm:ss")}', '{cpu}', '{value}');";

                    connection.Open();
                    MySqlCommand command = new MySqlCommand(sqlExpression, connection);
                    command.ExecuteNonQuery();
                }

                MessageBox.Show("Данные успешно добавлены.");
                Close();
            }
            else
            {
                MessageBox.Show("Заполните все поля формы!");
            }
        }

        private void ExitClick(object sender, EventArgs e)
        {
            Close();
        }

        private void NewTechForm_Load(object sender, EventArgs e)
        {
            var str = string.Format(
                "server={0}; database={1}; charset=utf8; user id={2}; password={3}; pooling=false;", "127.0.0.1",
                "retraincorp", "root", "root");
            var connection = new MySqlConnection(str);
            if (ComputerID == 0)
            {
                //cod
                comboBox1.Items.AddRange(new object[]
                    {StatusEnum.Defective, StatusEnum.Properly, StatusEnum.UnderRepair});
                using (connection)
                {
                    var sqlExpression = "SELECT ID, Name FROM employers";
                    connection.Open();
                    var command = new MySqlCommand(sqlExpression, connection);
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var employer = new Employer(){Id = (int)reader.GetValue(0), Name = (string)reader.GetValue(1)};
                        comboBox2.Items.Add(employer);
                    }
                }
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

                var name = reader.GetValue(1);
                var status = reader.GetValue(2);
                var employer = reader.GetValue(3);
                var date = reader.GetValue(4);
                var cpu = reader.GetValue(5);
                var value = reader.GetValue(6);

                textBox1.Text = (string) name;
                comboBox1.Text = status.ToString();
                comboBox2.Text = employer.ToString();
                dateTimePicker1.Value = Convert.ToDateTime(date);
                textBox5.Text = (string) cpu;
                textBox6.Text = value.ToString();
            }
        }
    }
}
