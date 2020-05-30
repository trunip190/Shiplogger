using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shiplogger
{
    public partial class FTPForm1 : Form
    {
        public FTPForm1()
        {
            InitializeComponent();
        }

        private static string[] GetDirectoryListing()
        {
            FtpWebRequest directoryListRequest = (FtpWebRequest)WebRequest.Create(@"ftp://10.13.10.102:21");
            directoryListRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            directoryListRequest.Credentials = new NetworkCredential("ftpuser", "ftpuser");
            directoryListRequest.UsePassive = false;

            using (FtpWebResponse directoryListResponse = (FtpWebResponse)directoryListRequest.GetResponse())
            {
                using (StreamReader directoryListResponseReader = new StreamReader(directoryListResponse.GetResponseStream()))
                {
                    string responseString = directoryListResponseReader.ReadToEnd();
                    string[] results = responseString.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    return results;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] files = GetDirectoryListing();
            string[] details;

            lvFiles.BeginUpdate();
            lvFiles.Items.Clear();
            foreach (string s in files)
            {
                details = ParseTextString(s);
                ListViewItem lvi = new ListViewItem
                {
                    Name = Path.GetFileName(details[1]),
                    Text = Path.GetFileName(details[1])
                };

                lvi.SubItems.Add(details[0]);

                lvFiles.Items.Add(lvi);
            }
            lvFiles.EndUpdate();
        }

        private string[] ParseTextString(string RAW)
        {
            string temp = $"-rw-r--r--" +
                $"1" +
                $"ftp" +
                $"ftp" +
                $"" +
                $"" +
                $"" +
                $"" +
                $"" +
                $"" +
                $"" +
                $"" +
                $"" +
                $"" +
                $"3100" +
                $"May" +
                $"13" +
                $"22:44" +
                $"MAN0016.CSV";

            string[] result = RAW.Split(' ');

            return new string[] {$"{result[16]} {result[15]} {DateTime.Now.Year}", result[18] };
        }
    }
}
