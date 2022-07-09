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
    public partial class reportInfoForm : Form
    {
        DataSet reportDataSet, conferencesDataSet, participantsDataSet;
        int id;

        public reportInfoForm(int id)
        {
            InitializeComponent();

            GetData(id);

            this.id = id;
        }

        private void GetData(int report)
        {
            GetReport(report);
            GetConferences(report);

            if (conferencesDataSet.Tables.Count > 0)
                if (conferencesDataSet.Tables[0].Rows.Count > 0)
                    GetParticipants(report, (int)conferencesDataSet.Tables[0].Rows[0].ItemArray[0]);
        }

        private void GetReport(int report)
        {
            string query;

            reportDataSet = new DataSet();

            query = String.Format("SELECT [{0}].title FROM [{0}] WHERE [{0}].id = {1};",
            ConfigurationManager.AppSettings["report"], report);

            try
            {
                Program.adapter = new SqlDataAdapter(query, Program.conn);

                Program.adapter.Fill(reportDataSet);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return;
            }

            if (reportDataSet.Tables[0].Rows.Count == 0)
            {
                textBox1.Text = "- Ошибка -";
                MessageBox.Show("Ошибка получения информации.");

                return;
            }

            textBox1.Text = reportDataSet.Tables[0].Rows[0].ItemArray[0].ToString();
        }

        private void GetConferences(int report)
        {
            string query;

            conferencesDataSet = new DataSet();

            query = String.Format("SELECT [{2}].id, [{2}].name FROM " +
            "((([{0}] INNER JOIN [{1}] ON [{0}].participant = [{1}].id) " +
            "INNER JOIN [{2}] ON [{0}].conference = [{2}].id) " +
            "INNER JOIN [{3}] ON [{0}].report = [{3}].id) " +
            "GROUP BY [{3}].id, [{2}].id, [{2}].name " +
            "HAVING [{3}].id = {4};",
            ConfigurationManager.AppSettings["speakers"], ConfigurationManager.AppSettings["participant"], ConfigurationManager.AppSettings["conference"], ConfigurationManager.AppSettings["report"], report);

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
                listBox1.Items.Add("- Нет записей -");

                return;
            }

            listBox1.Items.Clear();
            foreach (DataRow row in conferencesDataSet.Tables[0].Rows)
            {
                if (!listBox1.Items.Contains(row.ItemArray[1].ToString()))
                    listBox1.Items.Add(row.ItemArray[1].ToString());
            }
        }

        private void GetParticipants(int report, int conference)
        {
            string query;

            participantsDataSet = new DataSet();

            query = String.Format("SELECT [{1}].id, [{1}].name FROM " +
            "((([{0}] INNER JOIN [{1}] ON [{0}].participant = [{1}].id) " +
            "INNER JOIN [{2}] ON [{0}].conference = [{2}].id) " +
            "INNER JOIN [{3}] ON [{0}].report = [{3}].id) " +
            "GROUP BY [{2}].id, [{3}].id, [{1}].id, [{1}].name " +
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

            if (participantsDataSet.Tables[0].Rows.Count == 0)
            {
                listBox2.Items.Add("- Нет записей -");

                return;
            }

            listBox2.Items.Clear();
            foreach (DataRow row in participantsDataSet.Tables[0].Rows)
            {
                if (!listBox2.Items.Contains(row.ItemArray[1].ToString()))
                    listBox2.Items.Add(row.ItemArray[1].ToString());
            }
        }

        private void listBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count > 0)
                GetParticipants(id, (int)conferencesDataSet.Tables[0].Rows[listBox1.SelectedIndex].ItemArray[0]);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            speakerSearchForm form14 = new speakerSearchForm(id);

            if (ConfigurationManager.AppSettings["multi-window"] != "true")
                form14.ShowDialog();
            else
                form14.Show();

            GetConferences(id);
        }
    }
}
