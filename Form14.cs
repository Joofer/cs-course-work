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
    public partial class Form14 : Form
    {
        DataSet reportsDataSet, conferencesDataSet, participantsDataSet, nonParticipantsDataSet;

        public Form14()
        {
            InitializeComponent();

            GetData();
        }

        public Form14(int id)
        {
            InitializeComponent();

            GetData();
        }

        private void GetConferences()
        {
            conferencesDataSet = new DataSet();

            string query;

            query = String.Format("SELECT [{0}].id, [{0}].name FROM [{0}];", ConfigurationManager.AppSettings["conference"]);

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

            comboBox3.Items.Clear();
            foreach (DataRow row in conferencesDataSet.Tables[0].Rows)
            {
                comboBox3.Items.Add(row.ItemArray[1].ToString());
            }

            comboBox3.Text = "- " + conferencesDataSet.Tables[0].Rows.Count + " соответствий -";
        }

        private void GetReports(int conference)
        {
            //

            comboBox4.Items.Clear();
            comboBox1.Items.Clear();
            comboBox2.Items.Clear();

            //

            reportsDataSet = new DataSet();

            string query;

            query = String.Format("SELECT [{2}].id, [{2}].title FROM " +
            "(([{0}] INNER JOIN [{1}] ON [{0}].conference = [{1}].id) " +
            "INNER JOIN [{2}] ON [{0}].report = [{2}].id) " +
            "GROUP BY [{1}].id, [{2}].id, [{2}].title " +
            "HAVING [{1}].id = {3};",
            ConfigurationManager.AppSettings["speakers"], ConfigurationManager.AppSettings["conference"], ConfigurationManager.AppSettings["report"], conference);

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

            comboBox4.Items.Clear();
            foreach (DataRow row in reportsDataSet.Tables[0].Rows)
            {
                comboBox4.Items.Add(row.ItemArray[1].ToString());
            }

            comboBox4.Text = "- " + reportsDataSet.Tables[0].Rows.Count + " соответствий -";
        }

        private void GetParticipants(int conference, int report)
        {
            participantsDataSet = new DataSet();

            string query;

            query = String.Format("SELECT [{1}].id, [{1}].name FROM " +
            "((([{0}] INNER JOIN [{1}] ON [{0}].participant = [{1}].id) " +
            "INNER JOIN [{2}] ON [{0}].conference = [{2}].id) " +
            "INNER JOIN [{3}] ON [{0}].report = [{3}].id) " +
            "GROUP BY [{2}].id, [{3}].id, [{1}].id, [{1}].name, [{2}].name " +
            "HAVING [{2}].id = {4} AND [{3}].id = {5};",
            ConfigurationManager.AppSettings["speakers"], ConfigurationManager.AppSettings["participant"], ConfigurationManager.AppSettings["conference"], ConfigurationManager.AppSettings["report"], conference, report);

            try
            {
                Program.adapter = new SqlDataAdapter(query, Program.conn);

                Program.adapter.Fill(participantsDataSet);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return;
            }

            comboBox1.Items.Clear();
            foreach (DataRow row in participantsDataSet.Tables[0].Rows)
            {
                comboBox1.Items.Add(row.ItemArray[1].ToString());
            }

            comboBox1.Text = "- " + participantsDataSet.Tables[0].Rows.Count + " соответствий -";
        }

        private void GetNonParticipants(int conference, int report)
        {
            nonParticipantsDataSet = new DataSet();

            string query;

            query = String.Format("SELECT [{1}].id, [{1}].name FROM " +
            "[{1}] WHERE id NOT IN (SELECT [{1}].id FROM " +
            "((([{0}] INNER JOIN [{1}] ON [{0}].participant = [{1}].id) " +
            "INNER JOIN [{2}] ON [{0}].conference = [{2}].id) " +
            "INNER JOIN [{3}] ON [{0}].report = [{3}].id) " +
            "GROUP BY [{2}].id, [{3}].id, [{1}].id, [{1}].name " +
            "HAVING [{3}].id = {5} AND [{2}].id = {4});",
            ConfigurationManager.AppSettings["speakers"], ConfigurationManager.AppSettings["participant"], ConfigurationManager.AppSettings["conference"], ConfigurationManager.AppSettings["report"], conference, report);

            try
            {
                Program.adapter = new SqlDataAdapter(query, Program.conn);

                Program.adapter.Fill(nonParticipantsDataSet);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return;
            }

            comboBox2.Items.Clear();
            foreach (DataRow row in nonParticipantsDataSet.Tables[0].Rows)
            {
                comboBox2.Items.Add(row.ItemArray[1].ToString());
            }

            comboBox2.Text = "- " + nonParticipantsDataSet.Tables[0].Rows.Count + " соответствий -";
        }

        private void GetData()
        {
            GetConferences();
        }

        private void GetInfo(int id)
        {
            string query;

            //

            conferencesDataSet = new DataSet();

            query = String.Format("SELECT [{0}].id, [{0}].name FROM [{0}];", ConfigurationManager.AppSettings["conference"]);

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

            comboBox3.Items.Clear();
            foreach (DataRow row in conferencesDataSet.Tables[0].Rows)
            {
                comboBox3.Items.Add(row.ItemArray[1].ToString());
            }

            comboBox3.Text = "- " + conferencesDataSet.Tables[0].Rows.Count + " соответствий -";

            //

            participantsDataSet = new DataSet();

            query = String.Format("SELECT [{1}].id, [{1}].name, [{2}].name FROM " +
            "((([{0}] INNER JOIN [{1}] ON [{0}].participant = [{1}].id) " +
            "INNER JOIN [{2}] ON [{0}].conference = [{2}].id) " +
            "INNER JOIN [{3}] ON [{0}].report = [{3}].id) " +
            "GROUP BY [{3}].id, [{1}].id, [{1}].name, [{2}].name " +
            "HAVING [{3}].id = {4};",
            ConfigurationManager.AppSettings["speakers"], ConfigurationManager.AppSettings["participant"], ConfigurationManager.AppSettings["conference"], ConfigurationManager.AppSettings["report"], id);

            try
            {
                Program.adapter = new SqlDataAdapter(query, Program.conn);

                Program.adapter.Fill(participantsDataSet);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return;
            }

            comboBox1.Items.Clear();
            foreach (DataRow row in participantsDataSet.Tables[0].Rows)
            {
                comboBox1.Items.Add(String.Format("{0} - {1}", row.ItemArray[2].ToString(), row.ItemArray[1].ToString()));
            }

            comboBox1.Text = "- " + participantsDataSet.Tables[0].Rows.Count + " соответствий -";

            //

            nonParticipantsDataSet = new DataSet();

            query = String.Format("SELECT [{1}].id, [{1}].name FROM " +
            "[{1}] WHERE id NOT IN (SELECT [{1}].id FROM " +
            "((([{0}] INNER JOIN [{1}] ON [{0}].participant = [{1}].id) " +
            "INNER JOIN [{2}] ON [{0}].conference = [{2}].id) " +
            "INNER JOIN [{3}] ON [{0}].report = [{3}].id) " +
            "GROUP BY [{3}].id, [{1}].id, [{1}].name " +
            "HAVING [{3}].id = {4});",
            ConfigurationManager.AppSettings["speakers"], ConfigurationManager.AppSettings["participant"], ConfigurationManager.AppSettings["conference"], ConfigurationManager.AppSettings["report"], id);

            try
            {
                Program.adapter = new SqlDataAdapter(query, Program.conn);

                Program.adapter.Fill(nonParticipantsDataSet);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return;
            }

            comboBox2.Items.Clear();
            foreach (DataRow row in nonParticipantsDataSet.Tables[0].Rows)
            {
                comboBox2.Items.Add(row.ItemArray[1].ToString());
            }

            comboBox2.Text = "- " + nonParticipantsDataSet.Tables[0].Rows.Count + " соответствий -";

            //
        }

        private void _Add(int participant, int report, int conference)
        {
            string query;

            //

            query = @"SELECT * FROM [" + ConfigurationManager.AppSettings["speakers"] + @"] WHERE [participant] = '" + participant + @"' AND [report] = '" + report + @"' AND [conference] = '" + conference + @"';";

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

            query = @"INSERT INTO [" + ConfigurationManager.AppSettings["speakers"] + @"] VALUES ('" + participant + @"', '" + report + @"', '" + conference + @"');";

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

        private void _Remove(int participant, int report, int conference)
        {
            string query;

            //

            query = @"SELECT * FROM [" + ConfigurationManager.AppSettings["speakers"] + @"] WHERE [participant] = '" + participant + @"' AND [report] = '" + report + @"' AND [conference] = '" + conference + @"';";

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

            if (Program.dataSet.Tables[0].Rows.Count < 1)
            {
                MessageBox.Show("Ошибка удаления. Докладчика не существует.");

                return;
            }

            //

            query = @"DELETE [" + ConfigurationManager.AppSettings["speakers"] + @"] WHERE [participant] = '" + participant + @"' AND [report] = '" + report + @"' AND [conference] = '" + conference + @"';";

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

            MessageBox.Show("Докладчик удален.");

            this.Close();
        }

        private void Save()
        {
            if (comboBox2.SelectedIndex != -1)
            {
                if (comboBox3.SelectedIndex != -1)
                    _Add((int)nonParticipantsDataSet.Tables[0].Rows[comboBox2.SelectedIndex].ItemArray[0], (int)reportsDataSet.Tables[0].Rows[comboBox4.SelectedIndex].ItemArray[0], (int)conferencesDataSet.Tables[0].Rows[comboBox3.SelectedIndex].ItemArray[0]);
                else
                    MessageBox.Show("Мероприятие не выбрано. Докладчик не добавлен.");
            }
            if (comboBox1.SelectedIndex != -1)
            {
                if (comboBox3.SelectedIndex != -1)
                    _Remove((int)participantsDataSet.Tables[0].Rows[comboBox1.SelectedIndex].ItemArray[0], (int)reportsDataSet.Tables[0].Rows[comboBox4.SelectedIndex].ItemArray[0], (int)conferencesDataSet.Tables[0].Rows[comboBox3.SelectedIndex].ItemArray[0]);
                else
                    MessageBox.Show("Мероприятие не выбрано. Докладчик не удален.");
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetReports((int)(conferencesDataSet.Tables[0].Rows[comboBox3.SelectedIndex].ItemArray[0]));
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetParticipants((int)(conferencesDataSet.Tables[0].Rows[comboBox3.SelectedIndex].ItemArray[0]), (int)(reportsDataSet.Tables[0].Rows[comboBox4.SelectedIndex].ItemArray[0]));
            GetNonParticipants((int)(conferencesDataSet.Tables[0].Rows[comboBox3.SelectedIndex].ItemArray[0]), (int)(reportsDataSet.Tables[0].Rows[comboBox4.SelectedIndex].ItemArray[0]));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
