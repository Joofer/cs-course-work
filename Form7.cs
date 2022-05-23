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
    public partial class Form7 : Form
    {
        public Form7()
        {
            InitializeComponent();
        }

        private void Save()
        {
            string name = textBox1.Text;
            DateTime date = dateTimePicker1.Value;
            DateTime time = dateTimePicker2.Value;

            string query = @"INSERT INTO [" + ConfigurationManager.AppSettings["conference"] + @"] VALUES ('" + name + @"', '" + date + @"', '" + time + @"');";

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

            MessageBox.Show("Мероприятие добвалено.");

            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (textBox1.Text != String.Empty)
                    Save();
                else
                    MessageBox.Show("Некоторые поля не заполнены.");
            }
        }
    }
}
