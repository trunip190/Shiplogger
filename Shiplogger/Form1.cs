using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Shiplogger
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public List<string> PopulateFiles(string location)
        {
            string[] _files = Directory.GetFiles(location);

            return _files.Where(o => o.ToLower().EndsWith("csv")).ToList();
        }

        public void LoadFile(string FileLocation)
        {
            String[] Lines = File.ReadAllLines(FileLocation);

            foreach ( string s in Lines)
            {
                System.Diagnostics.Debug.WriteLine(s);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void btnPick_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(textBox1.Text))
            {
                if ( fbdFolder.ShowDialog() == DialogResult.OK)
                {
                    textBox1.Text = fbdFolder.SelectedPath;
                }
                else
                {
                    return;
                }
            }

            listBox1.BeginUpdate();
            listBox1.Items.Clear();
            List<string> Files = PopulateFiles(textBox1.Text);
            System.Diagnostics.Debug.WriteLine($"{Files.Count} Files.");

            foreach (string s in Files)
            {
                listBox1.Items.Add(s);
            }
            listBox1.EndUpdate();
        }

        private void listBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem.ToString() == "")
                return;

            string FileLocation = listBox1.SelectedItem.ToString();

            listView1.BeginUpdate();
            listView1.Clear();

            string[] Lines = File.ReadAllLines(FileLocation);
            string[] ColumnsToAdd = Lines[0].Split(',');
            string[] EntryToAdd;

            foreach (string s in ColumnsToAdd)
            {
                listView1.Columns.Add(s);
            }

            for (int i = 1; i < Lines.Length; i++) //start at 1 to skip column headers
            {
                EntryToAdd = Lines[i].Split(',');
                ListViewItem lvi = new ListViewItem(EntryToAdd[0]);
                lvi.SubItems.AddRange(EntryToAdd);

                listView1.Items.Add(lvi);
            }

            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listView1.EndUpdate();
        }
    }
}
