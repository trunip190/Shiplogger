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
                
        private void button1_Click(object sender, EventArgs e)
        {
            FTPMethods.DownloadNewFiles();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
