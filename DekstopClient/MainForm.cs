using System.Data;
using DekstopClient.Entities;
using DekstopClient.Repositories;
using DekstopClient.Utils;
using MySql.Data.MySqlClient;

namespace DekstopClient
{
    public partial class MainForm : Form
    {
        public User User { get; set; }
        public LoginForm LoginForm { get; set; }

        public MainForm()
        {
            InitializeComponent();
            radioButton1.Checked = true;
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            this.Text = $"Добро пожаловать, {User.Login}";
            textBox3.MaxLength = 12;
            textBox3.TextAlign = HorizontalAlignment.Center;
        }

        private void RefreshDbClick(object sender, EventArgs e)
        {
            RefreshDB();
        }

        private void AddNewTechClick(object sender, EventArgs e)
        {
            //на кнопку просмотр
            var newTechForm = new NewTechForm();
            var result = newTechForm.ShowDialog();
            if (result == DialogResult.OK)
            {
                RefreshDB();
            }
        }

        private void ShowTechClick(object sender, EventArgs e)
        {
            var newTechForm = new NewTechForm();
            try
            {
                var currentRow = dataGridView3.CurrentCell.RowIndex;
                newTechForm.ComputerID = (int)dataGridView3[0, currentRow].Value;
                var result = newTechForm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    RefreshDB();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Обновите состояние базы данных и выбереите устройство для просмотра!");
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex != 1) return;

            var employerRepository = new EmployerRepository();
            var employers =  employerRepository.GetItems();
            dataGridView2.DataSource = employers;
            dataGridView2.Columns["Id"]!.DisplayIndex = 0;

        }

        private void AddOrChangeClick(object sender, EventArgs e)
        {
            var name = textBox1.Text;
            var job = textBox2.Text;
            var tel = Util.CheckTelNumber(textBox3.Text);
            if (name.Length == 0 ||
                job.Length == 0 ||
                tel.Length == 0)
            {
                MessageBox.Show("dfgf");
                return;
            }

            if (radioButton1.Checked)
            {
                var str = string.Format(
                        "server={0}; database={1}; charset=utf8; user id={2}; password={3}; pooling=false;", "127.0.0.1",
                        "retraincorp", "root", "root");
                    using var connection = new MySqlConnection(str);
                    string sqlExpression = "INSERT INTO employers (Name, Position, Tel) " +
                                                 $"VALUES ('{name}', '{job}', '{tel}')";
                    connection.Open();
                    var command = new MySqlCommand(sqlExpression, connection);
                    command.ExecuteNonQuery();
                    sqlExpression = "SELECT * FROM employers";
                    command = new MySqlCommand(sqlExpression, connection);
                    var reader = command.ExecuteReader();
                    var table = new DataTable();
                    table.Load(reader);

                    dataGridView2.DataSource = table;
                    reader.Close();
            }
            else if (radioButton2.Checked)
            {

                var currentRow = dataGridView2.CurrentCell.RowIndex;
                var currentColumn = dataGridView2.CurrentCell.ColumnIndex;
                var id = (int)dataGridView2["Id", currentRow].Value;
                var str = string.Format(
                    "server={0}; database={1}; charset=utf8; user id={2}; password={3}; pooling=false;", "127.0.0.1",
                    "retraincorp", "root", "root");
                using var connection = new MySqlConnection(str);
                string sqlExpression = $"UPDATE employers SET Name = '{name}', " +
                                       $"Position = '{job}', " +
                                       $"Tel = '{tel}' " +
                                       $"WHERE ID = {id}";
                connection.Open();
                var command = new MySqlCommand(sqlExpression, connection);
                command.ExecuteNonQuery();
                sqlExpression = "SELECT * FROM employers";
                command = new MySqlCommand(sqlExpression, connection);
                var reader = command.ExecuteReader();
                var table = new DataTable();
                table.Load(reader);

                dataGridView2.DataSource = table;
                reader.Close();
                //курсор возвр на прежнее место
                dataGridView2.CurrentCell = dataGridView2.Rows[currentRow].Cells[currentColumn];

            }
        }

        private void ChangeRadioButton(object sender, EventArgs e)
        {
            button7.Text = "Изменить";
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
            button7.Text = "Добавить";
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
        }

        private void DeleteTechClick(object sender, EventArgs e)
        {
            try
            {
                var currentRow = dataGridView3.CurrentCell.RowIndex;
                var Id = (int)dataGridView3[0, currentRow].Value;
                var str = string.Format(
                    "server={0}; database={1}; charset=utf8; user id={2}; password={3}; pooling=false;", "127.0.0.1",
                    "retraincorp", "root", "root");
                using var connection = new MySqlConnection(str);
                string sqlExpression = $"DELETE FROM technick WHERE ID = {Id}";
                connection.Open();
                var command = new MySqlCommand(sqlExpression, connection);

                DialogResult result = MessageBox.Show(
                    "Вы действительно хотите удалить устройсво?",
                    "Внимание",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button2,
                    MessageBoxOptions.DefaultDesktopOnly);

                if (result == DialogResult.Yes)
                {
                    command.ExecuteNonQuery();

                    sqlExpression = "SELECT * FROM technick";
                    command = new MySqlCommand(sqlExpression, connection);
                    var reader = command.ExecuteReader();
                    var table = new DataTable();
                    table.Load(reader);

                    dataGridView3.DataSource = table;
                    reader.Close();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Обновите состояние базы данных и выбереите устройство для удаления!");
            }
        }

        private void RefreshDB()
        {
            var str = string.Format("server={0}; database={1}; charset=utf8; user id={2}; password={3}; pooling=false;", "127.0.0.1", "retraincorp", "root", "root");
            using var connection = new MySqlConnection(str);
            const string sqlExpression = "SELECT * FROM technick";
            connection.Open();
            var command = new MySqlCommand(sqlExpression, connection);
            var reader = command.ExecuteReader();
            var table = new DataTable();
            table.Load(reader);

            dataGridView3.DataSource = table;
            reader.Close();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            LoginForm.Close();
        }
    }
}