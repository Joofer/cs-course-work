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
    public partial class mainForm : Form
    {
        public mainForm()
        {
            InitializeComponent();
            SetComboItems();
            Connect();
        }

        private void SetComboItems()
        {
            comboBox1.Items.AddRange(new string[]
            {
                "Мероприятия",
                "Участники",
                "Список участников и докладчиков",
                "Доклады"
            });
        }

        public void Connect()
        {
            string connect = @"Server=" + ConfigurationManager.AppSettings["server"] + ";Database=" + ConfigurationManager.AppSettings["database"] + ";User Id=" + ConfigurationManager.AppSettings["username"] + ";Password=" + ConfigurationManager.AppSettings["password"] + ";";

            Program.conn = new SqlConnection(connect);

            connectingForm connection_dialog = new connectingForm();
            connection_dialog.Show();

            try
            {
                Program.conn.Open();
            }
            catch (Exception ex)
            {
                OnConnectionFailed(ex.Message);

                connection_dialog.Close();

                return;
            }

            connection_dialog.Close();

            if (!CheckConnection())
            {
                OnConnectionFailed("Ошибка подключения.");
            }
            else
            {
                OnConnectionSuccess();
                Pick(0);
            }
        }

        private bool CheckConnection()
        {
            string query;

            //

            query = @"SELECT * FROM [" + ConfigurationManager.AppSettings["speakers"] + @"];";

            Program.dataSet = new DataSet();

            try
            {
                Program.adapter = new SqlDataAdapter(query, Program.conn);

                Program.adapter.Fill(Program.dataSet);
            }
            catch (Exception ex)
            {
                return false;
            }

            //

            query = @"SELECT * FROM [" + ConfigurationManager.AppSettings["conference"] + @"];";

            Program.dataSet = new DataSet();

            try
            {
                Program.adapter = new SqlDataAdapter(query, Program.conn);

                Program.adapter.Fill(Program.dataSet);
            }
            catch (Exception ex)
            {
                return false;
            }

            //

            query = @"SELECT * FROM [" + ConfigurationManager.AppSettings["report"] + @"];";

            Program.dataSet = new DataSet();

            try
            {
                Program.adapter = new SqlDataAdapter(query, Program.conn);

                Program.adapter.Fill(Program.dataSet);
            }
            catch (Exception ex)
            {
                return false;
            }

            //

            query = @"SELECT * FROM [" + ConfigurationManager.AppSettings["participant"] + @"];";

            Program.dataSet = new DataSet();

            try
            {
                Program.adapter = new SqlDataAdapter(query, Program.conn);

                Program.adapter.Fill(Program.dataSet);
            }
            catch (Exception ex)
            {
                return false;
            }
            
            //

            return true;
        }

        private void OnConnectionSuccess(string message = "")
        {
            if (message != String.Empty)
                MessageBox.Show(message);

            label4.Hide();

            comboBox1.Show();

            button7.Show();

            tabControl1.Show();

            return;
        }

        private void OnConnectionFailed(string message = "")
        {
            if (message != String.Empty)
                MessageBox.Show(message);

            label4.Show();

            comboBox1.Hide();

            button7.Enabled = false;

            button7.Hide();

            tabControl1.Hide();

            return;
        }

        private void OpenSettings()
        {
            settingsForm form3 = new settingsForm();

            form3.ShowDialog();
        }

        private void GetData(string data_name)
        {
            string query = @"SELECT * FROM [" + data_name + @"];";

            Program.dataSet = new DataSet();

            try
            {
                Program.adapter = new SqlDataAdapter(query, Program.conn);

                Program.adapter.Fill(Program.dataSet);
            }
            catch (Exception ex)
            {
                OnConnectionFailed(ex.Message);

                return;
            }

            if (Program.dataSet.Tables.Count > 0)
            {
                if (Program.dataSet.Tables[0].Rows.Count > 0)
                {
                    dataGridView1.DataSource = Program.dataSet.Tables[0];
                }
                else
                {
                    dataGridView1.DataSource = null;
                }
            }
            else
            {
                dataGridView1.DataSource = null;
            }

            button7.Enabled = true;
        }

        private void SaveData()
        {
            SqlCommandBuilder commandBuilder = new SqlCommandBuilder(Program.adapter);

            try
            {
                Program.adapter.Update(Program.dataSet);
                Program.dataSet.Clear();
                Program.adapter.Fill(Program.dataSet);
            }
            catch (Exception ex)
            {
                OnConnectionFailed(ex.Message);

                return;
            }

            MessageBox.Show("Данные сохранены.");
        }

        private void Pick(int index)
        {
            comboBox1.SelectedIndex = index;
            switch (index)
            {
                case 0:
                    Pick("conference");
                    break;
                case 1:
                    Pick("participant");
                    break;
                case 2:
                    Pick("speakers");
                    break;
                case 3:
                    Pick("report");
                    break;
            }
        }

        private void Pick(string name)
        {
            Program.current_tab = name;

            switch (Program.current_tab)
            {
                case "conference":
                    label7.Text = "Мероприятия";
                    break;

                case "participant":
                    label7.Text = "Участники";
                    break;

                case "speakers":
                    label7.Text = "Список участников и докладчиков";
                    break;

                case "report":
                    label7.Text = "Доклады";
                    break;
            }

            GetData(ConfigurationManager.AppSettings[name]);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            SaveData();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            OpenSettings();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (Program.conn.State == ConnectionState.Open)
                Program.conn.Close();

            Connect();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            switch (Program.current_tab)
            {
                case "conference":
                    eventAddForm form7 = new eventAddForm();
                    if (ConfigurationManager.AppSettings["multi-window"] == "true")
                        form7.Show();
                    else
                        form7.ShowDialog();

                    break;

                case "participant":
                    participantAddForm form5 = new participantAddForm();
                    if (ConfigurationManager.AppSettings["multi-window"] == "true")
                        form5.Show();
                    else
                        form5.ShowDialog();

                    break;

                case "speakers":
                    speakerAddForm form11 = new speakerAddForm();
                    if (ConfigurationManager.AppSettings["multi-window"] == "true")
                        form11.Show();
                    else
                        form11.ShowDialog();
                    break;

                case "report":
                    reportAddForm form15 = new reportAddForm();
                    if (ConfigurationManager.AppSettings["multi-window"] == "true")
                        form15.Show();
                    else
                        form15.ShowDialog();
                    break;
            }

            GetData(ConfigurationManager.AppSettings[Program.current_tab]);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            switch (Program.current_tab)
            {
                case "conference":
                    eventSearchForm form8 = new eventSearchForm();
                    if (ConfigurationManager.AppSettings["multi-window"] == "true")
                        form8.Show();
                    else
                        form8.ShowDialog();

                    break;

                case "participant":
                    participantSearchForm form6 = new participantSearchForm();
                    if (ConfigurationManager.AppSettings["multi-window"] == "true")
                        form6.Show();
                    else
                        form6.ShowDialog();

                    break;

                case "speakers":
                    speakerSearchForm form14 = new speakerSearchForm();
                    if (ConfigurationManager.AppSettings["multi-window"] == "true")
                        form14.Show();
                    else
                        form14.ShowDialog();
                    break;

                case "report":
                    reportSearchForm form16 = new reportSearchForm();
                    if (ConfigurationManager.AppSettings["multi-window"] == "true")
                        form16.Show();
                    else
                        form16.ShowDialog();
                    break;
            }

            GetData(ConfigurationManager.AppSettings[Program.current_tab]);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            switch (Program.current_tab)
            {
                case "conference":
                    eventsReportForm form9 = new eventsReportForm();
                    if (ConfigurationManager.AppSettings["multi-window"] == "true")
                        form9.Show();
                    else
                        form9.ShowDialog();
                    break;

                case "participant":
                    participantsReportForm form10 = new participantsReportForm();
                    if (ConfigurationManager.AppSettings["multi-window"] == "true")
                        form10.Show();
                    else
                        form10.ShowDialog();
                    break;

                case "speakers":
                    speakersReportForm form12 = new speakersReportForm();
                    if (ConfigurationManager.AppSettings["multi-window"] == "true")
                        form12.Show();
                    else
                        form12.ShowDialog();
                    break;

                case "report":
                    reportsReportForm form17 = new reportsReportForm();
                    if (ConfigurationManager.AppSettings["multi-window"] == "true")
                        form17.Show();
                    else
                        form17.ShowDialog();
                    break;
            }

            GetData(ConfigurationManager.AppSettings[Program.current_tab]);
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("Некоторые поля неверно заполены.");
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Pick(comboBox1.SelectedIndex);
        }
    }
}
