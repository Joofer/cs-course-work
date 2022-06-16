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
    public partial class Form5 : Form
    {
        DataSet participantsDataSet, conferencesDataSet, reportsDataSet;
        int participant_passport;

        public Form5()
        {
            InitializeComponent();
            GetData();
        }

        private void GetConferences()
        {
            string query = @"SELECT * FROM [" + ConfigurationManager.AppSettings["conference"] + @"];";

            conferencesDataSet = new DataSet();

            try
            {
                Program.adapter = new SqlDataAdapter(query, Program.conn);

                Program.adapter.Fill(conferencesDataSet);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return;
            }

            if (conferencesDataSet.Tables[0].Rows.Count == 0)
            {
                comboBox1.Text = "- 0 соответствий -";

                return;
            }

            comboBox1.Items.Clear();
            comboBox1.Items.Add("- Без мероприятия -");
            foreach (DataRow row in conferencesDataSet.Tables[0].Rows)
            {
                comboBox1.Items.Add(row.ItemArray[1]);
            }

            comboBox1.Text = "- " + (comboBox1.Items.Count - 1) + " соответствий -";
        }

        private void GetReports()
        {
            string query = @"SELECT * FROM [" + ConfigurationManager.AppSettings["report"] + @"];";

            reportsDataSet = new DataSet();

            try
            {
                Program.adapter = new SqlDataAdapter(query, Program.conn);

                Program.adapter.Fill(reportsDataSet);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return;
            }

            if (reportsDataSet.Tables[0].Rows.Count == 0)
            {
                comboBox2.Text = "- 0 соответствий -";

                return;
            }

            comboBox2.Items.Clear();
            comboBox2.Items.Add("- Без доклада -");
            foreach (DataRow row in reportsDataSet.Tables[0].Rows)
            {
                comboBox2.Items.Add(row.ItemArray[1]);
            }

            comboBox2.Text = "- " + (comboBox2.Items.Count - 1) + " соответствий -";
        }

        private int GetParticipant(int passport)
        {
            string query = @"SELECT * FROM [" + ConfigurationManager.AppSettings["participant"] + @"] WHERE passport = " + passport + @";";

            participantsDataSet = new DataSet();

            try
            {
                Program.adapter = new SqlDataAdapter(query, Program.conn);

                Program.adapter.Fill(participantsDataSet);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return -1;
            }

            if (participantsDataSet.Tables[0].Rows.Count == 0)
            {
                comboBox2.Text = "- 0 соответствий -";

                return -1;
            }

            return (int)participantsDataSet.Tables[0].Rows[0].ItemArray[0];
        }

        private void GetData()
        {
            GetConferences();
            GetReports();
        }

        private void AddReport()
        {
            //
            
            Form15 form = new Form15();

            if (ConfigurationManager.AppSettings["multi-window"] == "true")
                form.Show();
            else
                form.ShowDialog();

            //

            GetReports(); // Обновление списка докладов
        }

        private bool SaveParticipant()
        {
            string name = textBox1.Text;
            DateTime date_of_birth = dateTimePicker1.Value;
            int passport;

            try
            {
                passport = Convert.ToInt32(textBox3.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }

            string query = @"INSERT INTO [" + ConfigurationManager.AppSettings["participant"] + @"] VALUES ('" + name + @"', '" + date_of_birth + @"', " + passport + @");";

            Program.dataSet = new DataSet();

            try
            {
                Program.adapter = new SqlDataAdapter(query, Program.conn);

                Program.adapter.Fill(Program.dataSet);
            }
            catch (SqlException ex)
            {
                if (ex.ErrorCode != -2146232060)
                    MessageBox.Show(String.Format("Ошибка при добавлении участника: {0}.", ex.Message));
                else
                    MessageBox.Show("Ошибка при добавлении участника: номер паспорта должен быть уникальным.");

                return false;
            }

            participant_passport = passport; // Сохранение паспорта для последующего поиска участника

            return true;
        }

        private bool SaveSpeakers(int participant)
        {
            int conference = (int)conferencesDataSet.Tables[0].Rows[comboBox1.SelectedIndex - 1].ItemArray[0];

            string query;

            if (comboBox2.SelectedIndex != -1 && comboBox2.SelectedIndex != 0)
            {
                int report = (int)reportsDataSet.Tables[0].Rows[comboBox2.SelectedIndex - 1].ItemArray[0];
                query = @"INSERT INTO [" + ConfigurationManager.AppSettings["speakers"] + @"] VALUES ('" + participant + @"', '" + report + @"', '" + conference + @"');";

            }
            else
            {
                query = @"INSERT INTO [" + ConfigurationManager.AppSettings["speakers"] + @"](participant, conference) VALUES ('" + participant + @"', '" + conference + @"');";
            }

            Program.dataSet = new DataSet();

            try
            {
                Program.adapter = new SqlDataAdapter(query, Program.conn);

                Program.adapter.Fill(Program.dataSet);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Ошибка при добавлении в список докладчиков: {0}.", ex.Message));

                return false;
            }

            return true;
        }

        private void Save()
        {
            if (textBox1.Text != String.Empty && textBox3.Text != String.Empty)
            {
                if (SaveParticipant())
                {
                    int participant = GetParticipant(participant_passport);

                    if (comboBox1.SelectedIndex != -1 && comboBox1.SelectedIndex != 0)
                    {
                        if (!SaveSpeakers(participant))
                        {
                            return;
                        }
                    }

                    MessageBox.Show("Участник добавлен.");
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("Некоторые поля не заполнены.");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AddReport();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                Save();
        }
    }
}
