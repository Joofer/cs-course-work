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
    public partial class Form15 : Form
    {
        public Form15()
        {
            InitializeComponent();
        }

        private void Save()
        {
            string title = textBox1.Text;
            string topic = textBox2.Text;
            string description = richTextBox1.Text;

            if (title == String.Empty)
            {
                MessageBox.Show("Некоторые поля не заполнены.");
                return;
            }

            string query = @"INSERT INTO [" + ConfigurationManager.AppSettings["report"] + @"] VALUES ('" + title + @"', '" + topic + @"', '" + description + @"');";

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
                Save();
        }
    }
}
