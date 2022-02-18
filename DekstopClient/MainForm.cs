using System.Data;
using MySql.Data.MySqlClient;

namespace DekstopClient
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            radioButton1.Checked = true;
        }

        private void RefreshDbClick(object sender, EventArgs e)
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

        private void AddNewTechClick(object sender, EventArgs e)
        {
            //на кнопку просмотр
            var newTechForm = new NewTechForm();
            newTechForm.ShowDialog();
        }

        private void ShowTechClick(object sender, EventArgs e)
        {
            var newTechForm = new NewTechForm();
            var currentRow = dataGridView3.CurrentCell.RowIndex;
            newTechForm.ComputerID = (int)dataGridView3[0, currentRow].Value;
            newTechForm.ShowDialog();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 1)
            {
                var str = string.Format(
                    "server={0}; database={1}; charset=utf8; user id={2}; password={3}; pooling=false;", "127.0.0.1",
                    "retraincorp", "root", "root");
                using var connection = new MySqlConnection(str);
                const string sqlExpression = "SELECT * FROM employers";
                connection.Open();
                var command = new MySqlCommand(sqlExpression, connection);
                var reader = command.ExecuteReader();
                var table = new DataTable();
                table.Load(reader);

                dataGridView2.DataSource = table;
                reader.Close();
            }

        }

        private void AddOrChangeClick(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                var name = textBox1.Text;
                var job = textBox2.Text;
                var tel = textBox3.Text;
                if (name.Length > 0 &&
                    job.Length > 0 &&
                    tel.Length > 0)
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
            }
            else if (radioButton2.Checked)
            {
                var currentRow = dataGridView2.CurrentCell.RowIndex;
                var currentColumn = dataGridView2.CurrentCell.ColumnIndex;
                var currentCell = dataGridView2.CurrentCell;
                var id = (int)dataGridView2[0, currentRow].Value;
                var name = textBox1.Text;
                var job = textBox2.Text;
                var tel = textBox3.Text;
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
                //dataGridView2.ClearSelection();
                //dataGridView2.Rows[currentRow].Cells[currentColumn].Selected = true;
                //dataGridView2.CurrentCell = currentCell;

            }
        }

        private void ChangeRadioButton(object sender, EventArgs e)
        {
            button7.Text = "Изменить";
            var currentRow = dataGridView2.CurrentCell.RowIndex;
            textBox1.Text = (string)dataGridView2[1, currentRow].Value;
            textBox2.Text = (string)dataGridView2[2, currentRow].Value;
            textBox3.Text = (string)dataGridView2[3, currentRow].Value;
        }
        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var currentRow = dataGridView2.CurrentCell.RowIndex;
            textBox1.Text = (string)dataGridView2[1, currentRow].Value;
            textBox2.Text = (string)dataGridView2[2, currentRow].Value;
            textBox3.Text = (string)dataGridView2[3, currentRow].Value;

        }

        private void AddRadioButton(object sender, EventArgs e)
        {
            button7.Text = "Добавить";
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
        }
    }
}