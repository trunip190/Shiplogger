using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Shiplogger
{
    public partial class Form1 : Form
    {
        private string FilterCode = "";
        private List<string> LoadedFiles = new List<string>();
        private string WorkingDir;

        public Form1()
        {
            InitializeComponent();
            WorkingDir = Properties.Settings.Default.BaseDir;
        }

        public bool PopulateFiles(string location)
        {
            string[] _files = Directory.GetFiles(location);

            try
            {
                LoadedFiles = _files.Where(o => o.ToLower().EndsWith("csv")).ToList();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<ShippingEntry> ParseEntries(string[] rawEntries, DateTime date)
        {
            List<ShippingEntry> result = new List<ShippingEntry>();
            

            for (int i = 1; i < rawEntries.Length; i++)
            {
                ShippingEntry entry = new ShippingEntry(date, rawEntries[i]);
                result.Add(entry);
            }

            return result;
        }

        public void UpdateListBox()
        {
            PopulateFiles(WorkingDir);

            lvDates.BeginUpdate();
            lvDates.Items.Clear();

            foreach (string s in LoadedFiles)
            {
                ListViewItem Entry = new ListViewItem
                {
                    Name = s,
                    Text = Path.GetFileNameWithoutExtension(s),
                    ForeColor = Color.Black,
                };
                Entry.Text = ParseDate(s).ToLongDateString();

                if ( ContainsFilterCode(s))
                {
                    Entry.ForeColor = Color.Blue;
                    lvDates.Items.Add(Entry);
                }
                else if (cbFilter.Checked == false)
                {
                    lvDates.Items.Add(Entry);
                }
            }
            lvDates.EndUpdate();
        }

        public void UpdateLVEntries()
        {
            Debug.WriteLine($"Running UpdateLVEntries");
            string FileLocation = lvDates.SelectedItems[0].Name.ToString();

            lvEntries.BeginUpdate();
            lvEntries.Clear();

            string[] Lines = File.ReadAllLines(FileLocation);
            
            
            List<ShippingEntry> Entries = ParseEntries(Lines, ParseDate(FileLocation)).OrderBy(o => o.CustomerCode).ToList();
            string[] ColumnsToAdd = Lines[0].Split(',');

            // Add in Date field.
            lvEntries.Columns.Add("Date");
            foreach (string s in ColumnsToAdd)
            {
                    lvEntries.Columns.Add(s);
                
            }

            //for (int i = 1; i < Lines.Length; i++) //start at 1 to skip column headers
            //{
            //    EntryToAdd = Lines[i].Split(',');
            //    ListViewItem lvi = new ListViewItem(EntryToAdd[0]);
            //    lvi.SubItems.AddRange(EntryToAdd);

            //    lvEntries.Items.Add(lvi);
            //}

            foreach (ShippingEntry entry in Entries)
            {

                if (FilterCode == "" || entry.CustomerCode.ToLower().Contains(FilterCode))
                {
                    lvEntries.Items.Add(entry.ToListViewItem());
                }
            }

            lvEntries.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            lvEntries.EndUpdate();
        }

        public DateTime ParseDate(string s)
        {
            string[] Date = Path.GetFileNameWithoutExtension(s).Split('_');

            int[] converted = new int[3];
            converted[0] = Int32.Parse(Date[0]);
            converted[1] = Int32.Parse(Date[1]);
            converted[2] = Int32.Parse(Date[2]);

            DateTime result = new DateTime(converted[0], converted[1], converted[2]);

            return result;
        }

        public bool ContainsFilterCode(string FileLocation)
        {
            // Load lines and convert to List<ShippingEntry>.
            string[] Lines = File.ReadAllLines(FileLocation);
            List<ShippingEntry> Entries = ParseEntries(Lines, DateTime.Now).OrderBy(o => o.CustomerCode).ToList();

            // Cycle through each entry to check against FilterCode.
            foreach (ShippingEntry entry in Entries)
            {
                // If CustomerCode matches FilterCode, end method and return.
                if (entry.CustomerCode.ToLower().Contains(FilterCode))
                {
                    Debug.WriteLine($"ContainsFilterCode match: {entry.CustomerCode}");
                    return true;
                }
            }

            // Nothing matched.
            Debug.WriteLine($"No match founder for {FilterCode}");
            return false;
        }

        private void btnPick_Click(object sender, EventArgs e)
        {
            if (!CheckDirectories())
            {
                if (fbdFolder.ShowDialog() == DialogResult.OK)
                {
                    WorkingDir = fbdFolder.SelectedPath;
                    txtAddress.Text = fbdFolder.SelectedPath;
                }
                else
                {
                    return;
                }
            }

            UpdateListBox();
        }

        public bool CheckDirectories()
        {
            if (Directory.Exists(txtAddress.Text))
            {
                return true;
            }

            if (Directory.Exists(Properties.Settings.Default.BaseDir))
            {
                WorkingDir = Properties.Settings.Default.BaseDir;
                txtAddress.Text = Properties.Settings.Default.BaseDir;
                return true;
            }

            if (Directory.Exists(Properties.Settings.Default.BaseDir2))
            {
                WorkingDir = Properties.Settings.Default.BaseDir2;
                txtAddress.Text = Properties.Settings.Default.BaseDir2;
                return true;
            }

            return false;
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            FilterCode = txtCustomer.Text.ToLower();

            UpdateListBox();

            if (lvEntries.SelectedItems.Count < 1)
                return;

            UpdateLVEntries();
        }

        private void lvDates_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvDates.SelectedItems.Count <1 || lvDates.SelectedItems[0].ToString() == "")
                return;

            UpdateLVEntries();
        }

        private void txtCustomer_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnFilter_Click(sender, e);
                
            }
        }

        private void txtAddress_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnPick_Click(sender, e);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            btnPick_Click(sender, e);
        }
    }

    public class ShippingEntry
    {
        public DateTime Date = DateTime.Now;
        public string ShipmentCode;
        public string CustomerCode;
        public string CustomerName;
        public string Reference1;
        public string Reference2;
        public string Reference3;
        public string Reference4;
        public string Reference5;
        public string PackagePIN;
        public string SPI;
        public string TotalCostwithTax;
        public string TotalCostBeforeTax;
        public string GSTAmount;
        public string HSTAmount;
        public string QSTAmount;
        public string BaseCost;
        public string ResidentialAreaCharge;
        public string FuelSurcharge;
        public string ExpCheqSurcharge;
        public string ReferencePerPiece;

        public ShippingEntry() { }

        public ShippingEntry(DateTime date, string item)
        {
            string[] split = item.Split(',');

            Date = date;
            ShipmentCode = split[0];
            CustomerCode = split[1];
            CustomerName = split[2];
            Reference1 = split[3];
            Reference2 = split[4];
            Reference3 = split[5];
            Reference4 = split[6];
            Reference5 = split[7];
            if (split.Length < 21)
            {
                PackagePIN = split[8];
                SPI = split[9];
                TotalCostwithTax = split[10];
                TotalCostBeforeTax = split[11];
                GSTAmount = split[12];
                HSTAmount = split[13];
                QSTAmount = split[14];
                BaseCost = split[15];
                ResidentialAreaCharge = split[16];
                FuelSurcharge = split[17];
                ExpCheqSurcharge = split[18];
                ReferencePerPiece = split[19];
            }
            else
            {
                PackagePIN = split[9];
                SPI = split[10];
                TotalCostwithTax = split[11];
                TotalCostBeforeTax = split[12];
                GSTAmount = split[13];
                HSTAmount = split[14];
                QSTAmount = split[15];
                BaseCost = split[16];
                ResidentialAreaCharge = split[17];
                FuelSurcharge = split[18];
                ExpCheqSurcharge = split[19];
                ReferencePerPiece = split[20];
            }
        }

        public ListViewItem ToListViewItem()
        {
            ListViewItem result = new ListViewItem(Date.ToShortDateString());

            result.SubItems.Add(ShipmentCode);
            result.SubItems.Add(CustomerCode.ToUpper());
            result.SubItems.Add(CustomerName);
            result.SubItems.Add(Reference1);
            result.SubItems.Add(Reference2);
            result.SubItems.Add(Reference3);
            result.SubItems.Add(Reference4);
            result.SubItems.Add(Reference5);
            result.SubItems.Add(PackagePIN);
            result.SubItems.Add(SPI);
            result.SubItems.Add(TotalCostwithTax);
            result.SubItems.Add(TotalCostBeforeTax);
            result.SubItems.Add(GSTAmount);
            result.SubItems.Add(HSTAmount);
            result.SubItems.Add(QSTAmount);
            result.SubItems.Add(BaseCost);
            result.SubItems.Add(ResidentialAreaCharge);
            result.SubItems.Add(FuelSurcharge);
            result.SubItems.Add(ExpCheqSurcharge);
            result.SubItems.Add(ReferencePerPiece);

            return result;
        }
    
    }
}
