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
    public partial class Form16 : Form
    {
        DataSet dataSet;
        int id;

        public Form16()
        {
            InitializeComponent();
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
            string title = textBox1.Text;
            string topic = textBox3.Text;
            string description = richTextBox1.Text;

            textBox2.Text = "";
            textBox4.Text = "";
            richTextBox2.Text = "";
            button4.Enabled = false;
            button5.Enabled = false;

            string query = @"SELECT * FROM [" + ConfigurationManager.AppSettings["report"] + @"] WHERE title LIKE '%" + title + @"%' AND topic LIKE '%" + topic + @"%' AND description LIKE '%" + description + @"%';";

            _Search(query);
        }

        private void Select(string name)
        {
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                if ((string)row.ItemArray[1] == name)
                {
                    id = (int)row.ItemArray[0];

                    textBox2.Text = (string)row.ItemArray[1];
                    textBox4.Text = (string)row.ItemArray[2];
                    richTextBox2.Text = (string)row.ItemArray[3];

                    button4.Enabled = true;
                    button5.Enabled = true;

                    return;
                }
            }
        }

        private void ShowInfo()
        {
            Form13 form = new Form13(id);

            if (ConfigurationManager.AppSettings["multi-window"] == "true")
                form.Show();
            else
                form.ShowDialog();
        }

        private void Edit()
        {
            Form14 form14 = new Form14(id);

            if (ConfigurationManager.AppSettings["multi-window"] != "true")
                form14.ShowDialog();
            else
                form14.Show();
        }

        private void SaveData()
        {
            SqlCommandBuilder commandBuilder = new SqlCommandBuilder(Program.adapter);

            string title = textBox2.Text;
            string topic = textBox4.Text;
            string description = richTextBox2.Text;

            string query = @"UPDATE [" + ConfigurationManager.AppSettings["report"] + @"] SET title = '" + title + @"', topic = '" + topic + @"', description = '" + description + @"' WHERE id = '" + id.ToString() + @"';";

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

            string title = textBox2.Text;
            string topic = textBox4.Text;
            string description = richTextBox2.Text;

            string query = @"DELETE [" + ConfigurationManager.AppSettings["report"] + @"] WHERE id = '" + id + @"';";

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

            MessageBox.Show("Доклад удален.");
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
                MessageBox.Show("Доклад не выбран.");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ShowInfo();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Edit();
        }
    }
}
