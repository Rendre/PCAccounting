using System.Data;
using MySql.Data.MySqlClient;

namespace DekstopClient
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
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
    }
}