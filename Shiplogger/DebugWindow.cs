using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shiplogger
{
    public partial class DebugWindow : Form
    {
        private readonly List<string> lines = new List<string>();

        public DebugWindow()
        {
            InitializeComponent();
        }

        public void LoadString(string[] s)
        {
            lines.AddRange(s);
            UpdateText();
        }

        public void UpdateText()
        {
            for (int i = lines.Count - 1; i > 0; i--)
            {
                richTextBox1.AppendText($"{lines[i]}\r\n");
                lines.RemoveAt(i);
            }
        }
    }
}
