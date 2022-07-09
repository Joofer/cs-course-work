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
    public partial class reportsReportForm : Form
    {
        DataSet dataSet;
        int id;

        public reportsReportForm()
        {
            InitializeComponent();
        }

        private void Search_All()
        {
            string query = @"SELECT * FROM [" + ConfigurationManager.AppSettings["report"] + @"];";

            _Search(query);

            if (dataSet != null)
            {
                if (dataSet.Tables.Count > 0)
                {
                    if (dataSet.Tables[0].Rows.Count > 0)
                    {
                        dataGridView1.DataSource = dataSet.Tables[0];
                        label12.Text = dataSet.Tables[0].Rows.Count.ToString();
                    }
                    else
                    {
                        dataGridView1.DataSource = null;
                        label12.Text = "0";
                    }
                }
                else
                {
                    dataGridView1.DataSource = null;
                    label12.Text = "0";
                }
            }
            else
            {
                dataGridView1.DataSource = null;
                label12.Text = "0";
            }
        }

        private void _Search(string query)
        {
            button2.Enabled = false;

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
            string topic = textBox4.Text;
            string description = richTextBox1.Text;

            textBox2.Text = "";
            textBox5.Text = "";
            richTextBox2.Text = "";
            button3.Enabled = false;

            string query = @"SELECT * FROM [" + ConfigurationManager.AppSettings["report"] + @"] WHERE title LIKE '%" + title + @"%' AND topic LIKE '%" + topic + @"%' AND description LIKE '%" + description + @"%';";

            _Search(query);

            if (dataSet != null)
            {
                if (dataSet.Tables.Count > 0)
                {
                    if (dataSet.Tables[0].Rows.Count > 0)
                    {
                        dataGridView1.DataSource = dataSet.Tables[0];
                        label12.Text = dataSet.Tables[0].Rows.Count.ToString();
                    }
                    else
                    {
                        dataGridView1.DataSource = null;
                        label12.Text = "0";
                    }
                }
                else
                {
                    dataGridView1.DataSource = null;
                    label12.Text = "0";
                }
            }
            else
            {
                dataGridView1.DataSource = null;
                label12.Text = "0";
            }
        }

        private void Select(string name)
        {
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                if ((string)row.ItemArray[1] == name)
                {
                    id = (int)row.ItemArray[0];

                    textBox2.Text = (string)row.ItemArray[1];
                    textBox5.Text = (string)row.ItemArray[2];
                    richTextBox2.Text = (string)row.ItemArray[3];

                    button3.Enabled = true;

                    return;
                }
            }
        }

        private void ShowInfo()
        {
            reportInfoForm form = new reportInfoForm(id);

            if (ConfigurationManager.AppSettings["multi-window"] == "true")
                form.Show();
            else
                form.ShowDialog();
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

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Select(comboBox1.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Search_All();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ShowInfo();
        }
    }
}
