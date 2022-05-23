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
    public partial class Form10 : Form
    {
        DataSet dataSet;
        int id;

        public Form10()
        {
            InitializeComponent();

            dateTimePicker1.Value = dateTimePicker1.MinDate;
        }

        private void Search_All()
        {
            string query = @"SELECT * FROM [" + ConfigurationManager.AppSettings["participant"] + @"];";

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

            comboBox1.Items.Clear();
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                comboBox1.Items.Add(row.ItemArray[1]);
            }

            comboBox1.Text = "- " + dataSet.Tables[0].Rows.Count + " соответствий -";
            // MessageBox.Show("Найдено " + comboBox1.Items.Count + " соответствий.");
        }

        private string GetCount(int id)
        {
            string query;
            query = String.Format("SELECT [{0}], COUNT({0}) FROM {1} " +
            "GROUP BY [participant] " +
            "HAVING [participant] = {2};",
            ConfigurationManager.AppSettings["participant"], ConfigurationManager.AppSettings["speakers"], id);

            //

            DataSet temp_ds = new DataSet();

            try
            {
                Program.adapter = new SqlDataAdapter(query, Program.conn);

                Program.adapter.Fill(temp_ds);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return "#";
            }

            if (temp_ds.Tables[0].Rows.Count > 0)
                foreach (DataRow r in temp_ds.Tables[0].Rows)
                    return r.ItemArray[1].ToString();
            else
                return "0";

            return "#";
        }

        private void Search()
        {
            string name = textBox1.Text;
            DateTime date_of_birth = dateTimePicker1.Value;
            string passport = textBox4.Text;

            textBox2.Text = "";
            dateTimePicker2.Value = DateTime.Now;
            textBox5.Text = "";

            string query;

            if (dateTimePicker1.Value != dateTimePicker1.MinDate)
                query = @"SELECT * FROM [" + ConfigurationManager.AppSettings["participant"] + @"] WHERE name LIKE '%" + name + @"%' AND date_of_birth = '" + date_of_birth + @"' AND passport LIKE '%" + passport + @"%';";
            else
                query = @"SELECT * FROM [" + ConfigurationManager.AppSettings["participant"] + @"] WHERE name LIKE '%" + name + @"%' AND passport LIKE '%" + passport + @"%';";

            _Search(query);

            dateTimePicker1.Value = dateTimePicker1.MinDate;

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
                    dateTimePicker2.Value = (DateTime)row.ItemArray[2];
                    textBox5.Text = (string)row.ItemArray[3];

                    //

                    label10.Text = GetCount(Convert.ToInt32(row.ItemArray[0]));

                    //

                    return;
                }
            }
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
    }
}
