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
    public partial class speakersReportForm : Form
    {
        DataSet speakersDataSet, speakersHiddenDataSet, participantsDataSet, conferencesDataSet, reportsDataSet;
        int id;

        public speakersReportForm()
        {
            InitializeComponent();

            connectingForm form4 = new connectingForm();
            form4.Show();

            GetData();

            form4.Close();

            dateTimePicker1.Value = dateTimePicker1.MinDate;
            dateTimePicker3.Value = dateTimePicker3.MinDate;
            dateTimePicker4.Value = dateTimePicker4.MinDate;
            dateTimePicker5.Value = dateTimePicker5.MinDate;
            dateTimePicker6.Value = dateTimePicker6.MinDate;
        }

        private void GetData()
        {
            string query;

            //

            conferencesDataSet = new DataSet();

            query = String.Format("SELECT * FROM [{0}];",
            ConfigurationManager.AppSettings["conference"]);

            _SearchSilent(ref conferencesDataSet, query);

            comboBox2.Items.Clear();
            foreach (DataRow row in conferencesDataSet.Tables[0].Rows)
            {
                comboBox2.Items.Add(row.ItemArray[1].ToString());
            }

            comboBox2.Text = "- " + conferencesDataSet.Tables[0].Rows.Count + " соответствий -";

            //

            reportsDataSet = new DataSet();

            query = String.Format("SELECT * FROM [{0}];",
            ConfigurationManager.AppSettings["report"]);

            _SearchSilent(ref reportsDataSet, query);

            comboBox3.Items.Clear();
            foreach (DataRow row in reportsDataSet.Tables[0].Rows)
            {
                comboBox3.Items.Add(row.ItemArray[1].ToString());
            }

            comboBox3.Text = "- " + reportsDataSet.Tables[0].Rows.Count + " соответствий -";

            //
        }

        private bool CheckString(string str, bool onlyDigits = false)
        {
            foreach (char c in str)
            {
                if (!onlyDigits)
                {
                    if (!Char.IsLetterOrDigit(c))
                        return false;
                }
                else
                {
                    if (!Char.IsDigit(c))
                        return false;
                }
            }
            return true;
        }

        private void Search_All()
        {
            //

            GetSpeakers();

            //

            GetSpeakersHidden();

            //

            GetParticipants();

            //

            if (speakersDataSet != null)
            {
                if (speakersDataSet.Tables.Count > 0)
                {
                    if (speakersDataSet.Tables[0].Rows.Count > 0)
                    {
                        dataGridView1.DataSource = speakersDataSet.Tables[0];
                        label12.Text = speakersDataSet.Tables[0].Rows.Count.ToString();
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

        private void GetSpeakers(List<int> speakersList = null, bool isSilent = false)
        {
            string query;

            //

            string condition = "";

            if (speakersList != null)
            {
                condition = "HAVING [{0}].id = " + speakersList[0].ToString();

                foreach (int speaker in speakersList)
                {
                    condition += " OR [{0}].id = " + speaker.ToString();
                }

                condition = String.Format(condition, ConfigurationManager.AppSettings["speakers"]);
            }

            //

            query = String.Format("SELECT [{1}].name AS [participant], [{2}].name AS [conference], [{3}].title AS [report] FROM " +
            "((([{0}] INNER JOIN [{1}] ON [{0}].participant = [{1}].id) " +
            "INNER JOIN [{2}] ON [{0}].conference = [{2}].id) " +
            "LEFT OUTER JOIN [{3}] ON [{0}].report = [{3}].id) " +
            "GROUP BY [{0}].id, [{1}].name, [{2}].name, [{3}].title " +
            "{4};",
            ConfigurationManager.AppSettings["speakers"], ConfigurationManager.AppSettings["participant"], ConfigurationManager.AppSettings["conference"], ConfigurationManager.AppSettings["report"], condition);

            _SearchSilent(ref speakersDataSet, query);

            if (!isSilent)
            {
                // if (speakersDataSet.Tables[0].Rows.Count > 0)
                    // MessageBox.Show("Найдено " + speakersDataSet.Tables[0].Rows.Count + " соответствий.");
                // else
                    // MessageBox.Show("Ничего не найдено.");
            }
        }

        private void GetSpeakersHidden(List<int> speakersList = null)
        {
            string query;

            //

            string condition = "";

            if (speakersList != null)
            {
                condition = "HAVING [{0}].id = " + speakersList[0].ToString();

                foreach (int speaker in speakersList)
                {
                    condition += " OR [{0}].id = " + speaker.ToString();
                }

                condition = String.Format(condition, ConfigurationManager.AppSettings["speakers"]);
            }

            //

            query = String.Format("SELECT [participant], [conference], [report] FROM [{0}] " +
            "GROUP BY [{0}].id, [participant], [conference], [report] " +
            "{1};",
            ConfigurationManager.AppSettings["speakers"], condition);

            _SearchSilent(ref speakersHiddenDataSet, query);
        }

        private void GetParticipants(List<int> participantsList = null)
        {
            string query;

            //

            string condition = "";

            if (participantsList != null)
            {
                condition = "HAVING [{0}].id = " + participantsList[0].ToString();

                foreach (int participant in participantsList)
                {
                    condition += " OR [{0}].id = " + participant.ToString();
                }

                condition = String.Format(condition, ConfigurationManager.AppSettings["participant"]);
            }

            //

            query = String.Format("SELECT [{1}].id, [{1}].name, [{1}].date_of_birth, [{1}].passport FROM " +
            "((([{0}] INNER JOIN [{1}] ON [{0}].participant = [{1}].id) " +
            "INNER JOIN [{2}] ON [{0}].conference = [{2}].id) " +
            "LEFT OUTER JOIN [{3}] ON [{0}].report = [{3}].id) " +
            "GROUP BY [{1}].id, [{1}].name, [{1}].date_of_birth, [{1}].passport " +
            "{4};",
            ConfigurationManager.AppSettings["speakers"], ConfigurationManager.AppSettings["participant"], ConfigurationManager.AppSettings["conference"], ConfigurationManager.AppSettings["report"], condition);

            _SearchSilent(ref participantsDataSet, query);

            comboBox1.Items.Clear();
            foreach (DataRow row in participantsDataSet.Tables[0].Rows)
            {
                comboBox1.Items.Add(row.ItemArray[1].ToString());
            }

            comboBox1.Text = "- " + participantsDataSet.Tables[0].Rows.Count + " соответствий -";
        }

        private void _Search(string query, bool updateCombobox = true)
        {
            speakersDataSet = new DataSet();

            try
            {
                Program.adapter = new SqlDataAdapter(query, Program.conn);

                Program.adapter.Fill(speakersDataSet);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                if (updateCombobox)
                {
                    comboBox1.Items.Clear();
                    comboBox1.Text = "- 0 соответствий -";
                }

                return;
            }

            if (updateCombobox)
            {
                comboBox1.Items.Clear();
                foreach (DataRow row in speakersDataSet.Tables[0].Rows)
                {
                    comboBox1.Items.Add(row.ItemArray[1]);
                }

                comboBox1.Text = "- " + comboBox1.Items.Count + " соответствий -";
            }

            // MessageBox.Show("Найдено " + speakersDataSet.Tables[0].Rows.Count + " соответствий.");
        }

        private void _SearchSilent(ref DataSet dataSet, string query)
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

                return;
            }

            if (dataSet.Tables[0].Rows.Count == 0)
            {
                return;
            }
        }

        private void Search()
        {
            string name, passport;
            int conference = -1, report = -1;
            DateTime date_of_birth, conference_date, conference_time;

            //

            textBox2.Text = "";
            textBox5.Text = "";
            dateTimePicker2.Value = DateTime.Now;
            listBox1.Items.Clear();

            comboBox1.Text = "- 0 соответствий -";
            comboBox1.Items.Clear();

            //

            string condition = "[{1}].id != -1";

            //

            name = textBox1.Text;
            passport = textBox4.Text;

            //

            if (!CheckString(name))
            {
                MessageBox.Show("Некоторые поля неверно заполнены. Допустимые символы: a-Z, 0-9.");
                return;
            }
            if (!CheckString(passport, true))
            {
                MessageBox.Show("Некоторые поля неверно заполнены. Допустимые символы: 0-9.");
                return;
            }

            //

            if (comboBox2.SelectedIndex != -1)
                conference = (int)conferencesDataSet.Tables[0].Rows[comboBox2.SelectedIndex].ItemArray[0];
            if (comboBox3.SelectedIndex != -1)
                report = (int)reportsDataSet.Tables[0].Rows[comboBox3.SelectedIndex].ItemArray[0];

            date_of_birth = dateTimePicker1.Value;
            conference_date = dateTimePicker4.Value;
            conference_time = dateTimePicker3.Value;

            if (name != String.Empty)
                condition += " AND [{3}].name LIKE '%" + name + "%'";
            if (passport != String.Empty)
                condition += " AND [{3}].passport LIKE '%" + passport + "%'";
            if (conference != -1)
                condition += " AND [{1}].id = " + conference;
            if (report != -1)
                condition += " AND [{2}].id = " + report;
            if (date_of_birth != dateTimePicker1.MinDate)
                condition += " AND [{3}].date_of_birth = '" + date_of_birth + "'";
            if (conference_date != dateTimePicker4.MinDate)
                condition += " AND [{1}].date = '" + conference_date + "'";
            if (conference_time != dateTimePicker3.MinDate)
                condition += " AND [{1}].time = '" + conference_time + "'";

            condition = String.Format(condition, ConfigurationManager.AppSettings["speakers"], ConfigurationManager.AppSettings["conference"], ConfigurationManager.AppSettings["report"], ConfigurationManager.AppSettings["participant"]);

            //

            string query;

            query = String.Format("SELECT [{0}].id, [{1}].id, [{1}].name AS [participant], [{2}].id, [{2}].name AS [conference], [{3}].id, [{3}].title AS [report] FROM " +
            "((([{0}] INNER JOIN [{1}] ON [{0}].participant = [{1}].id) " +
            "INNER JOIN [{2}] ON [{0}].conference = [{2}].id) " +
            "LEFT OUTER JOIN [{3}] ON [{0}].report = [{3}].id) " +
            "GROUP BY [{0}].id, [{1}].id, [{1}].name, [{2}].id, [{2}].name, [{3}].id, [{3}].title, [{2}].date, [{2}].time, [{1}].passport, [{1}].date_of_birth " +
            "HAVING {4};",
            ConfigurationManager.AppSettings["speakers"], ConfigurationManager.AppSettings["participant"], ConfigurationManager.AppSettings["conference"], ConfigurationManager.AppSettings["report"], condition);

            _Search(query, false);

            //

            dateTimePicker1.Value = dateTimePicker1.MinDate;
            dateTimePicker3.Value = dateTimePicker3.MinDate;
            dateTimePicker4.Value = dateTimePicker4.MinDate;

            //

            if (speakersDataSet != null)
            {
                if (speakersDataSet.Tables.Count > 0)
                {
                    if (speakersDataSet.Tables[0].Rows.Count > 0)
                    {
                        dataGridView1.DataSource = speakersDataSet.Tables[0];

                        dataGridView1.Columns[0].Visible = false;
                        dataGridView1.Columns[1].Visible = false;
                        dataGridView1.Columns[3].Visible = false;
                        dataGridView1.Columns[5].Visible = false;
                    }
                    else
                    {
                        dataGridView1.DataSource = null;
                        label12.Text = "0";
                        return;
                    }
                }
                else
                {
                    dataGridView1.DataSource = null;
                    label12.Text = "0";
                    return;
                }
            }
            else
            {
                dataGridView1.DataSource = null;
                label12.Text = "0";
                return;
            }

            //

            List<int> speakersList = new List<int>();

            foreach (DataRow row in speakersDataSet.Tables[0].Rows)
            {
                if (!speakersList.Contains((int)row.ItemArray[0]))
                {
                    speakersList.Add((int)row.ItemArray[0]);
                }
            }

            //

            List<int> participantsList = new List<int>();

            foreach (DataRow row in speakersDataSet.Tables[0].Rows)
            {
                if (!participantsList.Contains((int)row.ItemArray[1]))
                {
                    participantsList.Add((int)row.ItemArray[1]);
                }
            }

            //

            GetSpeakers(speakersList, true);
            GetSpeakersHidden(speakersList);
            GetParticipants(participantsList);

            //

            label12.Text = speakersDataSet.Tables[0].Rows.Count.ToString();
        }

        private void Search_Condition()
        {
            string column = "";
            string symbol = "";
            DateTime value_datetime = DateTime.Now;

            //

            if (comboBox5.SelectedIndex == -1 || comboBox4.SelectedIndex == -1)
            {
                MessageBox.Show("Некоторые поля неверно заполнены.");
                return;
            }

            //

            textBox2.Text = "";
            textBox5.Text = "";
            dateTimePicker2.Value = DateTime.Now;
            listBox1.Items.Clear();

            comboBox1.Text = "- 0 соответствий -";
            comboBox1.Items.Clear();

            //

            string query;

            switch (comboBox5.SelectedIndex)
            {
                case 0:
                    column = "[{0}].date";
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

                case 1:
                    column = "[{0}].time";
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

            switch (comboBox4.SelectedIndex)
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

            string condition = column + @" " + symbol + @" '" + value_datetime + @"'";
            condition = String.Format(condition, ConfigurationManager.AppSettings["conference"]);

            query = String.Format("SELECT [{0}].id, [{1}].id, [{1}].name AS [participant], [{2}].id, [{2}].name AS [conference], [{2}].date, [{2}].time, [{3}].id, [{3}].title AS [report] FROM " +
            "((([{0}] INNER JOIN [{1}] ON [{0}].participant = [{1}].id) " +
            "INNER JOIN [{2}] ON [{0}].conference = [{2}].id) " +
            "INNER JOIN [{3}] ON [{0}].report = [{3}].id) " +
            "GROUP BY [{0}].id, [{1}].id, [{1}].name, [{2}].id, [{2}].name, [{3}].id, [{3}].title, [{2}].date, [{2}].time " +
            "HAVING {4};",
            ConfigurationManager.AppSettings["speakers"], ConfigurationManager.AppSettings["participant"], ConfigurationManager.AppSettings["conference"], ConfigurationManager.AppSettings["report"], condition);

            _Search(query, false);

            //

            dateTimePicker5.Value = dateTimePicker5.MinDate;
            dateTimePicker6.Value = dateTimePicker6.MinDate;

            //

            if (speakersDataSet != null)
            {
                if (speakersDataSet.Tables.Count > 0)
                {
                    if (speakersDataSet.Tables[0].Rows.Count > 0)
                    {
                        dataGridView1.DataSource = speakersDataSet.Tables[0];

                        dataGridView1.Columns[0].Visible = false;
                        dataGridView1.Columns[1].Visible = false;
                        dataGridView1.Columns[3].Visible = false;
                        dataGridView1.Columns[5].Visible = false;
                        dataGridView1.Columns[6].Visible = false;
                        dataGridView1.Columns[7].Visible = false;
                    }
                    else
                    {
                        dataGridView1.DataSource = null;
                        label12.Text = "0";
                        return;
                    }

                }
                else
                {
                    dataGridView1.DataSource = null;
                    label12.Text = "0";
                    return;
                }
            }
            else
            {
                dataGridView1.DataSource = null;
                label12.Text = "0";
                return;
            }

            //

            List<int> speakersList = new List<int>();

            foreach (DataRow row in speakersDataSet.Tables[0].Rows)
            {
                if (!speakersList.Contains((int)row.ItemArray[0]))
                {
                    speakersList.Add((int)row.ItemArray[0]);
                }
            }

            //

            List<int> participantsList = new List<int>();

            foreach (DataRow row in speakersDataSet.Tables[0].Rows)
            {
                if (!participantsList.Contains((int)row.ItemArray[1]))
                {
                    participantsList.Add((int)row.ItemArray[1]);
                }
            }

            //

            GetSpeakers(speakersList, true);
            GetSpeakersHidden(speakersList);
            GetParticipants(participantsList);

            //

            label12.Text = speakersDataSet.Tables[0].Rows.Count.ToString();
        }

        private void Select(string name)
        {
            int k = 0;

            id = (int)participantsDataSet.Tables[0].Rows[comboBox1.SelectedIndex].ItemArray[0];

            textBox2.Text = (string)participantsDataSet.Tables[0].Rows[comboBox1.SelectedIndex].ItemArray[1];
            dateTimePicker2.Value = (DateTime)participantsDataSet.Tables[0].Rows[comboBox1.SelectedIndex].ItemArray[2];
            textBox5.Text = participantsDataSet.Tables[0].Rows[comboBox1.SelectedIndex].ItemArray[3].ToString();

            //

            listBox1.Items.Clear();
            foreach (DataRow row in speakersHiddenDataSet.Tables[0].Rows)
            {
                if ((int)row.ItemArray[0] == id)
                {
                    if (speakersDataSet.Tables[0].Rows[k].ItemArray[2].ToString() != "")
                        listBox1.Items.Add(String.Format("{0}: {1} - {2}", speakersHiddenDataSet.Tables[0].Rows[k].ItemArray[2], speakersDataSet.Tables[0].Rows[k].ItemArray[1], speakersDataSet.Tables[0].Rows[k].ItemArray[2]));
                }

                k += 1;
            }

            //

            label10.Text = listBox1.Items.Count.ToString();
        }

        private void ShowInfo()
        {
            int id = -1;
            int k = 0;

            foreach (DataRow row in speakersHiddenDataSet.Tables[0].Rows)
            {
                if (String.Format("{0}: {1} - {2}", row.ItemArray[2], speakersDataSet.Tables[0].Rows[k].ItemArray[1], speakersDataSet.Tables[0].Rows[k].ItemArray[2]) == listBox1.SelectedItem.ToString())
                {
                    id = (int)row.ItemArray[2];
                    break;
                }

                k += 1;
            }

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
            if (!checkBox1.Checked)
                Search();
            else
                Search_Condition();
        }

        private void comboBox5_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Search_Condition();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count > 0)
                ShowInfo();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Select(comboBox1.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Search_All();
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
    }
}
