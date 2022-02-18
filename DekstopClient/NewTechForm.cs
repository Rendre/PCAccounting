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
            comboBox1.Items.AddRange(new object[]{1, 2, 3});
            comboBox2.Items.AddRange(new object[] { 1, 2, 3 });
        }

        private void AddClick(object sender, EventArgs e)
        {
            var name = textBox1.Text;
            var status = (int)comboBox1.SelectedItem;
            var employer = (int)comboBox2.SelectedItem;
            var date = dateTimePicker1.Value;
            var cpu = textBox5.Text;
            var value = textBox6.Text;

            if (name.Length > 0 &&
                status > 0 &&
                employer > 0 &&
                cpu.Length > 0 &&
                value.Length > 0 &&
                date.Year > 2010)
            {
                var str = string.Format("server={0}; database={1}; charset=utf8; user id={2}; password={3}; pooling=false;", "127.0.0.1", "retraincorp", "root", "root");
                using (var connection = new MySqlConnection(str))
                {

                    string sqlExpression = "INSERT INTO `technick` (`Name`, `StatusID`, `EmployerID`, `DateCreated`, `Cpu`, `Price`) " +
                                           $"VALUES ('{name}', '{status}', '{employer}', '{date.ToString("yyyy-MM-dd HH:mm:ss")}', '{cpu}', '{value}');";

                    connection.Open();
                    MySqlCommand command = new MySqlCommand(sqlExpression, connection);
                    command.ExecuteNonQuery();
                }

                MessageBox.Show("Данные успешно добавлены.");
                this.Close();
            }
            else
            {
                MessageBox.Show("Заполните все поля формы!");
            }
        }

        private void ExitClick(object sender, EventArgs e)
        {
            this.Close();
        }

        private void NewTechForm_Load(object sender, EventArgs e)
        {

        }
    }
}
