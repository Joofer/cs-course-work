﻿using System;
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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Connect();
        }

        public void Connect()
        {
            string connect = @"Server=" + ConfigurationManager.AppSettings["server"] + ";Database=" + ConfigurationManager.AppSettings["database"] + ";Trusted_Connection=True;";

            Program.conn = new SqlConnection(connect);

            Form4 connection_dialog = new Form4();
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
                Pick("conference");
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

            label1.Show();
            label2.Show();
            label3.Show();
            label5.Show();
            button1.Show();
            button4.Show();
            button6.Show();
            button7.Show();
            button11.Show();

            tabControl1.Show();

            return;
        }

        private void OnConnectionFailed(string message = "")
        {
            if (message != String.Empty)
                MessageBox.Show(message);

            label4.Show();

            button7.Enabled = false;

            label1.Hide();
            label2.Hide();
            label3.Hide();
            label5.Hide();
            button1.Hide();
            button4.Hide();
            button6.Hide();
            button7.Hide();
            button11.Hide();

            tabControl1.Hide();

            return;
        }

        private void OpenSettings()
        {
            Form3 form3 = new Form3();

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

        private void button4_Click(object sender, EventArgs e)
        {
            Pick("conference");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Pick("participant");
        }

        private void button11_Click(object sender, EventArgs e)
        {
            Pick("speakers");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Pick("report");
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
                    Form7 form7 = new Form7();
                    if (ConfigurationManager.AppSettings["multi-window"] == "true")
                        form7.Show();
                    else
                        form7.ShowDialog();

                    break;

                case "participant":
                    Form5 form5 = new Form5();
                    if (ConfigurationManager.AppSettings["multi-window"] == "true")
                        form5.Show();
                    else
                        form5.ShowDialog();

                    break;

                case "speakers":
                    Form11 form11 = new Form11();
                    if (ConfigurationManager.AppSettings["multi-window"] == "true")
                        form11.Show();
                    else
                        form11.ShowDialog();
                    break;

                case "report":
                    Form15 form15 = new Form15();
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
                    Form8 form8 = new Form8();
                    if (ConfigurationManager.AppSettings["multi-window"] == "true")
                        form8.Show();
                    else
                        form8.ShowDialog();

                    break;

                case "participant":
                    Form6 form6 = new Form6();
                    if (ConfigurationManager.AppSettings["multi-window"] == "true")
                        form6.Show();
                    else
                        form6.ShowDialog();

                    break;

                case "speakers":
                    Form14 form14 = new Form14();
                    if (ConfigurationManager.AppSettings["multi-window"] == "true")
                        form14.Show();
                    else
                        form14.ShowDialog();
                    break;

                case "report":
                    Form16 form16 = new Form16();
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
                    Form9 form9 = new Form9();
                    if (ConfigurationManager.AppSettings["multi-window"] == "true")
                        form9.Show();
                    else
                        form9.ShowDialog();
                    break;

                case "participant":
                    Form10 form10 = new Form10();
                    if (ConfigurationManager.AppSettings["multi-window"] == "true")
                        form10.Show();
                    else
                        form10.ShowDialog();
                    break;

                case "speakers":
                    Form12 form12 = new Form12();
                    if (ConfigurationManager.AppSettings["multi-window"] == "true")
                        form12.Show();
                    else
                        form12.ShowDialog();
                    break;

                case "report":
                    Form17 form17 = new Form17();
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
    }
}