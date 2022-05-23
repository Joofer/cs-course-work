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
    public partial class Form9 : Form
    {
        DataSet dataSet, dataSet1;
        int id;

        public Form9()
        {
            InitializeComponent();

            dateTimePicker1.Value = dateTimePicker1.MinDate;
            dateTimePicker3.Value = dateTimePicker3.MinDate;
            dateTimePicker5.Value = dateTimePicker5.MinDate;
            dateTimePicker6.Value = dateTimePicker6.MinDate;
        }

        private void Search_All()
        {
            string query = @"SELECT * FROM [" + ConfigurationManager.AppSettings["conference"] + @"];";

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

        private void SearchCondition()
        {
            string column = "";
            string speakers = "";
            string symbol = "";
            int value_int = 0;
            DateTime value_datetime = DateTime.Now;
            bool useInnerJoin = false;

            string query;

            speakers = ConfigurationManager.AppSettings["speakers"];

            switch (comboBox2.SelectedIndex)
            {
                case 0:
                    column = ConfigurationManager.AppSettings["participant"];
                    try
                    {
                        value_int = Convert.ToInt32(textBox3.Text);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Некоторые поля неверно заполнены.");
                        return;
                    }
                    useInnerJoin = true;
                    break;

                case 1:
                    column = ConfigurationManager.AppSettings["report"];
                    try
                    {
                        value_int = Convert.ToInt32(textBox3.Text);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Некоторые поля неверно заполнены.");
                        return;
                    }
                    useInnerJoin = true;
                    break;

                case 2:
                    column = "date";
                    if (dateTimePicker6.Value != dateTimePicker6.MinDate)
                    {
                        value_datetime = dateTimePicker6.Value;
                    }
                    else
                    {
                        MessageBox.Show("Некоторые поля неверно заполнены.");
                        return;
                    }
                    break;

                case 3:
                    column = "time";
                    if (dateTimePicker5.Value != dateTimePicker5.MinDate)
                    {
                        value_datetime = dateTimePicker5.Value;
                    }
                    else
                    {
                        MessageBox.Show("Некоторые поля неверно заполнены.");
                        return;
                    }
                    break;
            }

            switch (comboBox3.SelectedIndex)
            {
                case 0:
                    symbol = ">";
                    break;

                case 1:
                    symbol = "<";
                    break;

                case 2:
                    symbol = ">=";
                    break;

                case 3:
                    symbol = "<=";
                    break;

                case 4:
                    symbol = "=";
                    break;

                case 5:
                    symbol = "!=";
                    break;
            }

            if (useInnerJoin)
            {
                string condition = @"cnt " + symbol + @" '" + value_int.ToString() + @"'";

                query = String.Format("SELECT * FROM " +
                                    "(SELECT [conference], COUNT({0}) AS cnt " +
                                    "FROM (SELECT DISTINCT [conference], [{0}] FROM [{1}]) AS conferences " +
                                    "GROUP BY [conference]) AS total " +
                                    "WHERE {2};",
                column, speakers, condition);

                dataSet1 = new DataSet();

                try
                {
                    Program.adapter = new SqlDataAdapter(query, Program.conn);

                    Program.adapter.Fill(dataSet1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);

                    comboBox1.Items.Clear();
                    comboBox1.Text = "- 0 соответствий -";

                    return;
                }

                if (dataSet1.Tables[0].Rows.Count == 0)
                {
                    comboBox1.Items.Clear();
                    comboBox1.Text = "- 0 соответствий -";

                    // MessageBox.Show("Ничего не найдено.");

                    return;
                }

                comboBox1.Items.Clear();
                foreach (DataRow row in dataSet1.Tables[0].Rows)
                {
                    comboBox1.Items.Add(row.ItemArray[1]);
                }

                comboBox1.Text = "- " + comboBox1.Items.Count + " соответствий -";
                // MessageBox.Show("Найдено " + comboBox1.Items.Count + " соответствий.");

                //

                query = @"SELECT * FROM [" + ConfigurationManager.AppSettings["conference"] + @"] WHERE id = 0 ";

                foreach (DataRow row in dataSet1.Tables[0].Rows)
                {
                    query += @" or id = '" + row.ItemArray[0] + @"' ";
                }

                query += @";";

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

                if (dataSet.Tables[0].Rows.Count == 0)
                {
                    // MessageBox.Show("Ничего не найдено.");

                    comboBox1.Items.Clear();
                    comboBox1.Text = "- 0 соответствий -";

                    return;
                }

                comboBox1.Items.Clear();
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    comboBox1.Items.Add(row.ItemArray[1]);
                }
            }
            else
            {
                string condition = @"[" + column + @"] " + symbol + @" '" + value_datetime.Year + @"-" + value_datetime.Month + @"-" + value_datetime.Day + @"'";

                query = String.Format("SELECT * FROM [{0}] " +
                                    "WHERE {1}",
                ConfigurationManager.AppSettings["conference"], condition);

                dataSet1 = new DataSet();

                try
                {
                    Program.adapter = new SqlDataAdapter(query, Program.conn);

                    Program.adapter.Fill(dataSet1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);

                    return;
                }

                if (dataSet1.Tables[0].Rows.Count == 0)
                {
                    comboBox1.Text = "- 0 соответствий -";
                    // MessageBox.Show("Ничего не найдено.");

                    return;
                }

                comboBox1.Items.Clear();
                foreach (DataRow row in dataSet1.Tables[0].Rows)
                {
                    comboBox1.Items.Add(row.ItemArray[1]);
                }

                comboBox1.Text = "- " + comboBox1.Items.Count + " соответствий -";
                // MessageBox.Show("Найдено " + comboBox1.Items.Count + " соответствий.");

                //

                query = @"SELECT * FROM [" + ConfigurationManager.AppSettings["conference"] + @"] WHERE id = 0 ";

                foreach (DataRow row in dataSet1.Tables[0].Rows)
                {
                    query += @" or id = '" + row.ItemArray[0] + @"' ";
                }

                query += @";";

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

                if (dataSet.Tables[0].Rows.Count == 0)
                {
                    // MessageBox.Show("Ничего не найдено.");

                    return;
                }

                comboBox1.Items.Clear();
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    comboBox1.Items.Add(row.ItemArray[1]);
                }
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

            comboBox1.Text = "- " + comboBox1.Items.Count + " соответствий -";
            // MessageBox.Show("Найдено " + dataSet.Tables[0].Rows.Count + " соответствий.");
        }

        private string GetCount(int id, string column)
        {
            string query;
            query = String.Format("SELECT * FROM " +
            "(SELECT [conference], COUNT({0}) AS cnt " +
            "FROM (SELECT DISTINCT [conference], [{0}] FROM [{1}]) AS conferences " +
            "GROUP BY conference) AS total " +
            "WHERE conference = {2};",
            column, ConfigurationManager.AppSettings["speakers"], id);

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
            textBox2.Text = "";
            dateTimePicker2.Value = DateTime.Now;
            dateTimePicker4.Value = DateTime.Now;
            comboBox1.Items.Clear();

            if (!checkBox1.Checked)
            {
                string name = textBox1.Text;
                DateTime date = dateTimePicker1.Value;
                DateTime time = dateTimePicker3.Value;

                string query = @"SELECT * FROM [" + ConfigurationManager.AppSettings["conference"] + @"] WHERE name LIKE '%" + name + @"%'";

                if (date != dateTimePicker1.MinDate)
                    query += @" AND date = '" + date + @"'";
                if (time != dateTimePicker2.MinDate)
                    query += @" AND time = '" + time + @"'";

                query += @";";

                _Search(query);
            }
            else
            {
                SearchCondition();
            }

            dateTimePicker1.Value = dateTimePicker1.MinDate;
            dateTimePicker3.Value = dateTimePicker3.MinDate;
            dateTimePicker5.Value = dateTimePicker5.MinDate;
            dateTimePicker6.Value = dateTimePicker6.MinDate;

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

                    string[] time_str = row.ItemArray[3].ToString().Split(':');
                    DateTime temp_date = new DateTime(((DateTime)row.ItemArray[2]).Year, ((DateTime)row.ItemArray[2]).Month, ((DateTime)row.ItemArray[2]).Day, Convert.ToInt32(time_str[0]), Convert.ToInt32(time_str[1]), Convert.ToInt32(time_str[2]));

                    textBox2.Text = (string)row.ItemArray[1];
                    dateTimePicker2.Value = temp_date;
                    dateTimePicker4.Value = temp_date;

                    //

                    label8.Text = GetCount(Convert.ToInt32(row.ItemArray[0]), ConfigurationManager.AppSettings["participant"]);
                    label10.Text = GetCount(Convert.ToInt32(row.ItemArray[0]), ConfigurationManager.AppSettings["report"]);

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
            if (checkBox1.Checked)
            {
                if (comboBox2.SelectedIndex != -1 && comboBox3.SelectedIndex != -1)
                    Search();
                else
                    MessageBox.Show("Некоторые поля не заполнены.");
            }
            else
            {
                Search();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Select(comboBox1.Text);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                groupBox1.Enabled = false;
                groupBox4.Enabled = true;
            }
            else
            {
                groupBox1.Enabled = true;
                groupBox4.Enabled = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Search_All();
        }

        private void comboBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (comboBox2.SelectedIndex != -1 && comboBox3.SelectedIndex != -1)
                    Search();
                else
                    MessageBox.Show("Некоторые поля не заполнены.");
            }
        }
    }
}
