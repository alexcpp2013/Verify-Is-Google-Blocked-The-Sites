using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VerifySiteAttributes
{
    public partial class Form1 : Form
    {
        List<string> Sites = new List<string>();
        string FileSites = "";

        WebBrowser Web = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void bClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //WebBrowser async default

            if (tbSites.Text == "")
            {
                MessageBox.Show("Введите имя файла для парсинга. ", 
                                "Информационное сообщение", 
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Initialize();

            try
            {
                progressBar1.Visible = true;
                var rs = new XmlReaderConfig();
                var ra = new XmlReaderConfig();
                rs.GetParameters(FileSites, Sites, "site");
                string title = "Предупреждение о вредоносном ПО";
                string content = "";

                ClearWebBrowser();
                using (Web = new WebBrowser())
                {
                    SetWebBrowserOptions();
                    rtbReport.Text += "Заблокирвоаны сайты: \n\n";
                    foreach (var s in Sites)
                    {
                        string newaddress = s;
                        if (!s.StartsWith("http://") &&
                            !s.StartsWith("https://"))
                        {
                            newaddress = "http://" + s;
                        }
                        LoadSite("https://www.google.com.ua/interstitial?url=" +
                                 newaddress);

                        //or Like str OR Containe str
                        string tmp = GetDocumentTitle();
                        var t = Web.Document;
                        if (tmp == title)
                            content += "\nСайт: " + s;

                        rtbReport.Text += content;
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("Возникла ошибка:  \n" + err.Message,
                                "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                progressBar1.Visible = false;
            }
            
            MessageBox.Show("Проверка закончена\n",
                            "Информационное сообщение",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        protected string GetDocumentTitle()
        {
            return Web.DocumentTitle;
        }

        private void Initialize()
        {
            rtbReport.Clear();
            Sites.Clear();
            FileSites = tbSites.Text;
        }

        private void btNUnit_Click(object sender, EventArgs e)
        {
            if (ofdSites.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    tbSites.Text = ofdSites.FileName;
                }
                catch (Exception ex)
                {
                    tbSites.Text = "";
                    MessageBox.Show("Hе возможно считать файл: " + ex.Message,
                                    "Ошибка",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            Refresh();
        }

        protected bool Navigate(String address)
        {
            if (String.IsNullOrEmpty(address)) return false;
            if (address.Equals("about:blank")) return false;
            if (!address.StartsWith("http://") &&
                !address.StartsWith("https://"))
            {
                address = "http://" + address;
            }

            try
            {
                Web.Navigate(new Uri(address));
                return true;
            }
            catch (System.UriFormatException err)
            {
                return false;
            }
            catch (Exception err)
            {
                return false;
            }
        }

        private void SetWebBrowserOptions()
        {
            Web.ScriptErrorsSuppressed = true;
            Web.Visible = false;
        }

        protected void ClearWebBrowser()
        {
            if (Web != null)
                Web.Dispose();
            Web = null;
        }

        protected void LoadSite(string url)
        {
            if (Navigate(url) != true)
            {
                throw(new Exception("Не корректный url."));
            }

            while (Web.ReadyState != WebBrowserReadyState.Complete)
                Application.DoEvents();
        }
    }
}
