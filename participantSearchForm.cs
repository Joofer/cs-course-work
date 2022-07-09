using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace u17
{
    public partial class participantSearchForm : Form
    {
        DataSet dataSet;
        int id;

        public participantSearchForm()
        {
            InitializeComponent();

            dateTimePicker1.Value = dateTimePicker1.MinDate;
        }

        private void _Search(string query)
        {
            button2.Enabled = false;
            button3.Enabled = false;

            dataSet = new DataSet();

            try
            {
                Program.adapter = new SqlDataAdapter(query, Program.conn);

                Program.adapter.Fill(dataSet);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                comboBox1.Items.Clear();
                comboBox1.Text = "- 0 соответствий -";

                return;
            }

            if (dataSet.Tables[0].Rows.Count != 0)
            {
                button2.Enabled = true;
                button3.Enabled = true;
            }

            comboBox1.Items.Clear();
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                comboBox1.Items.Add(row.ItemArray[1]);
            }

            comboBox1.Text = "- " + dataSet.Tables[0].Rows.Count + " соответствий -";
            // MessageBox.Show("Найдено " + dataSet.Tables[0].Rows.Count + " соответствий.");
        }

        private void Search()
        {
            string name = textBox1.Text;
            DateTime date_of_birth = dateTimePicker1.Value;
            string passport = textBox3.Text;

            textBox2.Text = "";
            dateTimePicker2.Value = DateTime.Now;
            textBox4.Text = "";

            string query;

            if (dateTimePicker1.Value != dateTimePicker1.MinDate)
                query = @"SELECT * FROM [" + ConfigurationManager.AppSettings["participant"] + @"] WHERE name LIKE '%" + name + @"%' AND date_of_birth = '" + date_of_birth + @"' AND passport LIKE '%" + passport + @"%';";
            else
                query = @"SELECT * FROM [" + ConfigurationManager.AppSettings["participant"] + @"] WHERE name LIKE '%" + name + @"%' AND passport LIKE '%" + passport + @"%';";

            _Search(query);

            dateTimePicker1.Value = dateTimePicker1.MinDate;
        }

        private void Select(string name)
        {
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                if ((string)row.ItemArray[1] == name)
                {
                    id = (int)row.ItemArray[0];

                    List<int> date = new List<int>();

                    textBox2.Text = (string)row.ItemArray[1];
                    dateTimePicker2.Value = (DateTime)row.ItemArray[2];
                    textBox4.Text = (string)row.ItemArray[3];

                    return;
                }
            }
        }

        private void SaveData()
        {
            SqlCommandBuilder commandBuilder = new SqlCommandBuilder(Program.adapter);

            string name = textBox2.Text;
            DateTime date_of_birth = dateTimePicker2.Value;
            string passport = textBox4.Text;

            string query = @"UPDATE [" + ConfigurationManager.AppSettings["participant"] + @"] SET name = '" + name + @"', date_of_birth = '" + date_of_birth + @"', passport = '" + passport + @"' WHERE id = '" + id.ToString() + @"';";

            Program.dataSet = new DataSet();

            try
            {
                Program.adapter = new SqlDataAdapter(query, Program.conn);

                Program.adapter.Fill(Program.dataSet);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return;
            }

            MessageBox.Show("Данные сохранены.");
        }

        private void DeleteData()
        {
            SqlCommandBuilder commandBuilder = new SqlCommandBuilder(Program.adapter);

            string name = textBox2.Text;
            DateTime date_of_birth = dateTimePicker2.Value;
            string passport = textBox4.Text;

            string query = @"DELETE [" + ConfigurationManager.AppSettings["participant"] + @"] WHERE id = '" + id + @"';";

            Program.dataSet = new DataSet();

            try
            {
                Program.adapter = new SqlDataAdapter(query, Program.conn);

                Program.adapter.Fill(Program.dataSet);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return;
            }

            MessageBox.Show("Участник удален.");
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Search();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Search();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != String.Empty && textBox4.Text != String.Empty)
                SaveData();
            else
                MessageBox.Show("Некоторые поля не заполнены.");
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Select(comboBox1.Text);
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (textBox2.Text != String.Empty && textBox4.Text != String.Empty)
                SaveData();
            else
                MessageBox.Show("Некоторые поля не заполнены.");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex != -1)
            {
                DeleteData();
                Search();
            }
            else
            {
                MessageBox.Show("Участник не выбран.");
            }
        }
    }
}
