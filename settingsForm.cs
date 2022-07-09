using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace u17
{
    public partial class settingsForm : Form
    {
        public settingsForm()
        {
            InitializeComponent();

            textBox1.Text = ReadSetting("server");
            textBox2.Text = ReadSetting("database");
            textBox7.Text = ReadSetting("username");
            textBox8.Text = ReadSetting("password");

            textBox4.Text = ReadSetting("conference");
            textBox3.Text = ReadSetting("participant");
            textBox6.Text = ReadSetting("speakers");
            textBox5.Text = ReadSetting("report");

            if (ReadSetting("multi-window") == "true")
                checkBox1.Checked = true;
            else
                checkBox1.Checked = false;
        }

        static string ReadSetting(string key)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                string result = appSettings[key] ?? "Not Found";

                return result;
            }
            catch (ConfigurationErrorsException)
            {
                return "";
            }
        }

        static void AddUpdateAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }

        private void Save()
        {
            string server = textBox1.Text;
            string database = textBox2.Text;
            string username = textBox7.Text;
            string password = textBox8.Text;

            AddUpdateAppSettings("server", server);
            AddUpdateAppSettings("database", database);
            AddUpdateAppSettings("username", username);
            AddUpdateAppSettings("password", password);

            string conference = textBox4.Text;
            string participant = textBox3.Text;
            string speakers = textBox6.Text;
            string report = textBox5.Text;

            AddUpdateAppSettings("conference", conference);
            AddUpdateAppSettings("participant", participant);
            AddUpdateAppSettings("speakers", speakers);
            AddUpdateAppSettings("report", report);

            if (checkBox1.Checked)
                AddUpdateAppSettings("multi-window", "true");
            else
                AddUpdateAppSettings("multi-window", "false");

            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                Save();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Режим мультиоконности – при открытии нового окна, предыдущее остается незаблокированным.",
                "Справка"
                );
        }
    }
}
