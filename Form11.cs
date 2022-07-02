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
    public partial class Form11 : Form
    {
        DataSet dataSet1, dataSet2, dataSet3;

        public Form11()
        {
            InitializeComponent();

            Form4 form4 = new Form4();
            form4.Show();

            GetData();

            form4.Close();
        }

        private void GetData()
        {
            string query;
            
            //

            query = @"SELECT * FROM [" + ConfigurationManager.AppSettings["participant"] + @"];";


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

                return;
            }

            comboBox1.Items.Clear();
            foreach (DataRow row in dataSet1.Tables[0].Rows)
            {
                comboBox1.Items.Add(row.ItemArray[1]);
            }

            comboBox1.Text = "- " + comboBox1.Items.Count + " соответствий -";

            //

            query = @"SELECT * FROM [" + ConfigurationManager.AppSettings["conference"] + @"];";


            dataSet2 = new DataSet();

            try
            {
                Program.adapter = new SqlDataAdapter(query, Program.conn);

                Program.adapter.Fill(dataSet2);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return;
            }

            if (dataSet2.Tables[0].Rows.Count == 0)
            {
                comboBox2.Text = "- 0 соответствий -";

                return;
            }

            comboBox2.Items.Clear();
            foreach (DataRow row in dataSet2.Tables[0].Rows)
            {
                comboBox2.Items.Add(row.ItemArray[1]);
            }

            comboBox2.Text = "- " + comboBox2.Items.Count + " соответствий -";

            //

            query = @"SELECT * FROM [" + ConfigurationManager.AppSettings["report"] + @"];";


            dataSet3 = new DataSet();

            try
            {
                Program.adapter = new SqlDataAdapter(query, Program.conn);

                Program.adapter.Fill(dataSet3);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return;
            }

            if (dataSet3.Tables[0].Rows.Count == 0)
            {
                comboBox3.Text = "- 0 соответствий -";

                return;
            }

            comboBox3.Items.Clear();
            comboBox3.Items.Add("- Без доклада -");
            foreach (DataRow row in dataSet3.Tables[0].Rows)
            {
                comboBox3.Items.Add(row.ItemArray[1]);
            }

            comboBox3.Text = "- " + (comboBox3.Items.Count - 1) + " соответствий -";

            //
        }

        private void Save()
        {
            int participant = Convert.ToInt32(dataSet1.Tables[0].Rows[comboBox1.SelectedIndex].ItemArray[0]);
            int conference = Convert.ToInt32(dataSet2.Tables[0].Rows[comboBox2.SelectedIndex].ItemArray[0]);
            int report = (comboBox3.SelectedIndex != -1 && comboBox3.SelectedIndex != 0 ? Convert.ToInt32(dataSet3.Tables[0].Rows[comboBox3.SelectedIndex - 1].ItemArray[0]) : -1);

            string query;

            //

            if (comboBox3.SelectedIndex != -1 && comboBox3.SelectedIndex != 0)
                query = @"SELECT * FROM [" + ConfigurationManager.AppSettings["speakers"] + @"] WHERE [participant] = '" + participant + @"' AND [report] = '" + report + @"' AND [conference] = '" + conference + @"';";
            else
                query = @"SELECT * FROM [" + ConfigurationManager.AppSettings["speakers"] + @"] WHERE [participant] = '" + participant + @"' AND [conference] = '" + conference + @"';";


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

            if (Program.dataSet.Tables[0].Rows.Count > 0)
            {
                MessageBox.Show("Ошибка добавления. Докладчик уже существует.");

                return;
            }

            //

            if (comboBox3.SelectedIndex != -1 && comboBox3.SelectedIndex != 0)
                query = @"INSERT INTO [" + ConfigurationManager.AppSettings["speakers"] + @"] VALUES ('" + participant + @"', '" + report + @"', '" + conference + @"');";
            else
                query = @"INSERT INTO [" + ConfigurationManager.AppSettings["speakers"] + @"](participant, conference) VALUES ('" + participant + @"', '" + conference + @"');";
            
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
            
            MessageBox.Show("Докладчик добавлен.");
            
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex >= 0 && comboBox2.SelectedIndex >= 0)
                Save();
            else
                MessageBox.Show("Некоторые поля не заполнены.");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void comboBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (comboBox1.SelectedIndex >= 0 && comboBox2.SelectedIndex >= 0)
                    Save();
                else
                    MessageBox.Show("Некоторые поля не заполнены.");
            }
        }
    }
}
