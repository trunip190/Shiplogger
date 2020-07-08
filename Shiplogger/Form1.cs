﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using Shiplogger.Properties;
using System.Net;
using System.Windows.Forms;
using System.CodeDom;

namespace Shiplogger
{
    public partial class Form1 : Form
    {
        #region variables
        private string FilterCode = "";
        private string OrderNo = "";
        private string FilterBOL = "";
        private List<string> LoadedFiles = new List<string>();
        private List<ShippingEntry> Entries = new List<ShippingEntry>();
        private string FileLocation = "";
        private string WorkingDir;
        private readonly bool shpVersion = false;
        private List<string> companies = new List<string>();
        #endregion

        #region Methods
        public Form1()
        {
            Debug.WriteLine($"Form1");
            InitializeComponent();
            shpVersion = Settings.Default.SHPMode;
            BtnConvert.Enabled = Settings.Default.DebugMode;
            string s = $"{Environment.CurrentDirectory}\\purofiles\\";
            if (Directory.Exists(s))
            {
                Settings.Default.BaseDir = s;
                Settings.Default.Save();
            }
        }

        private void ConvertAll()
        {
            string[] files = Directory.GetFiles(WorkingDir, $"*.csv");
            foreach (string s in files)
            {
                //UpdateLVEntries();

                // Load lines and convert to List<ShippingEntry>.
                string[] Lines = File.ReadAllLines(s);

                //Load entries in file
                List<ShippingEntry> Entries = ParseEntries(Lines, DateTime.Now, false).OrderBy(o => o.CustomerCode).ToList();

                //set new file name
                string _customFilename = s.Replace(".CSV", ".shp");

                if (File.Exists(_customFilename))
                    continue;

                ExportFile(_customFilename, FTPMethods.SortFile(Entries));
            }
        }

        private void PopulateCompanies()
        {

        }

        public bool PopulateFiles(string location)
        {
            string[] _files = Directory.GetFiles(location);
            LoadedFiles.Clear();

            try
            {
                foreach (string s in _files)
                {

                    if (s.EndsWith("tmp")
                        || (shpVersion && s.ToLower().EndsWith("shp"))
                        || !shpVersion && s.ToLower().EndsWith("csv"))
                    {
                        LoadedFiles.Add(s);
                    }
                }

                LoadedFiles.Reverse();

                //if (shpVersion)
                //    LoadedFiles = _files.Where(o => o.ToLower().EndsWith("shp")).Reverse().ToList();
                //else
                //    LoadedFiles = _files.Where(o => o.ToLower().EndsWith("csv")).Reverse().ToList();
                return true;
            }
            catch
            {
                return false;
            }

        }

        public List<ShippingEntry> ParseEntries(string[] rawEntries, DateTime date, bool ship)
        {
            List<ShippingEntry> result = new List<ShippingEntry>();


            for (int i = 1; i < rawEntries.Length; i++)
            {
                if (rawEntries[i] != "")
                {
                    ShippingEntry entry = new ShippingEntry(date, rawEntries[i], ship);
                    if (entry != null)
                    {
                        result.Add(entry);
                    }
                }
            }

            return result;
        }

        public List<ShippingEntry> ParseEntries(string[] rawEntries, DateTime date)
        {
            List<ShippingEntry> result = new List<ShippingEntry>();


            for (int i = 1; i < rawEntries.Length; i++)
            {
                if (rawEntries[i] != "")
                {
                    ShippingEntry entry = new ShippingEntry(date, rawEntries[i], shpVersion);
                    if (entry != null)
                    {
                        result.Add(entry);
                    }
                }
            }

            return result;
        }

        public void UpdateListBox()
        {
            PopulateFiles(WorkingDir);

            lvDates.BeginUpdate();
            lvDates.Items.Clear();
            List<string> dates = new List<string>();

            foreach (string s in LoadedFiles)
            {
                ListViewItem Entry = new ListViewItem
                {
                    Name = s,
                    Text = Path.GetFileNameWithoutExtension(s),
                    ForeColor = Color.Black,
                };
                Entry.Text = ParseDate(s).ToString("ddd dd MMMM yyyy");

                if (!dates.Contains(Entry.Text))
                {
                    dates.Add(Entry.Text);

                    if (ContainsFilterCode(s))
                    {
                        Entry.ForeColor = Color.Blue;
                        lvDates.Items.Add(Entry);
                    }
                    else if (cbFilter.Checked == false)
                    {
                        lvDates.Items.Add(Entry);
                    }
                    else
                    {
                        Debug.WriteLine(s);
                    }
                }
            }
            lvDates.EndUpdate();
        }

        public void UpdateLVEntries()
        {
            // Clear old entries.
            lvEntries.BeginUpdate();
            lvEntries.Clear();
            Entries.Clear();

            // Set shp and tmp filenames
            int pos = FileLocation.IndexOf('.');
            string tmp = $"{FileLocation.Substring(0, pos)}.tmp";
            string shp = $"{FileLocation.Substring(0, pos)}.shp";

            // Create Entries
            DateTime date = ParseDate(shp);
            if (File.Exists(shp) || shp == "")
            {
                string[] Lines = File.ReadAllLines(shp);
                Entries.AddRange(ParseEntries(Lines, date).OrderBy(o => o.CustomerCode).ToList());
            }

            // Add in user entries
            if (File.Exists(tmp))
            {
                string[] tempFiles = File.ReadAllLines(tmp);
                foreach (string s in tempFiles)
                {
                    Entries.Add(new ShippingEntry(date, s, shpVersion));
                }
            }

            Entries = Entries.OrderBy(o => o.CustomerCode).ToList();

            // Create and add in columns.
            string[] ColumnsToAdd = new string[]
            {
                "Date",
                "Courier",
                "Shipment Code",
                "Customer Code",
                "Customer Name",
                "Reference 1",
                "Reference 2",
                "Reference 3",
                "Reference 4",
                "Reference 5",
                "Package PIN",
                "Shipment/Piece/Item (S/P/I)",
                "Total Cost with Tax",
                "Total Cost Before Tax",
                "GST Amount",
                "HST Amount",
                "QST Amount",
                "Base Cost",
                "Residential Area Charge",
                "Fuel Surcharge",
                "ExpCheq Surcharge",
                "Reference Per Piece"
            };

            if (!shpVersion)
            {
                _ = lvEntries.Columns.Add("Courier");
            }

            foreach (string s in ColumnsToAdd)
            {
                lvEntries.Columns.Add(s);
            }

            // Temporary assigning of checks to vars to see what is good or not
            bool entrybool;
            bool orderbool;
            bool BOLbool;

            // Set alternating colour.
            bool light = false;

            foreach (ShippingEntry entry in Entries)
            {
                ListViewItem lvi = entry.ToListViewItem();
                entrybool = entry.CustomerCode.ToLower().Contains(FilterCode);
                orderbool = entry.ContainsOrder(OrderNo);
                BOLbool = entry.PackagePIN.Contains(FilterBOL);

                if ((FilterCode == "" && OrderNo == "" && FilterBOL == "") || (entrybool && orderbool && BOLbool))
                {
                    if (entry.SPI == "S")
                    {
                        light = !light;
                    }

                    if (light)
                    {
                        lvi.BackColor = Color.LightGray;
                    }
                    else
                    {
                        lvi.BackColor = Color.White;
                    }

                    lvEntries.Items.Add(lvi);
                }
            }

            // Cleanup.
            lvEntries.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            lvEntries.Columns[0].Width = 5;
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
            List<ShippingEntry> Entries;
            if (Lines.Length == 1)
            {
                Entries = new List<ShippingEntry>() { new ShippingEntry(DateTime.Now, Lines[0], shpVersion) };
            }
            else
            {
                Entries = ParseEntries(Lines, DateTime.Now).OrderBy(o => o.CustomerCode).ToList();
            }

            // Cycle through each entry to check against FilterCode.
            foreach (ShippingEntry entry in Entries)
            {
                // If CustomerCode matches FilterCode, end method and return.
                if (entry.CustomerCode.ToLower().Contains(FilterCode) && entry.ContainsOrder(OrderNo) && entry.PackagePIN.Contains(FilterBOL))
                {
                    //Debug.WriteLine($"ContainsFilterCode match: {entry.CustomerCode}");
                    return true;
                }
            }

            // Nothing matched.
            //Debug.WriteLine($"No match founder for {FilterCode}");
            return false;
        }

        public bool CheckDirectories()
        {
            if (Directory.Exists(txtLocation.Text))
            {
                WorkingDir = txtLocation.Text;
                return true;
            }

            if (Directory.Exists(Settings.Default.BaseDir))
            {
                WorkingDir = Settings.Default.BaseDir;
                txtLocation.Text = Settings.Default.BaseDir;
                return true;
            }


            return false;
        }

        #region ExportFile
        public bool ExportFile(string location, List<string> lines)
        {
            bool result = false;

            string columns = @"Courier,Shipment Code,Customer Code,Customer Name,Reference 1,Reference 2,Reference 3,Reference 4,Reference 5,Package PIN,Shipment/Piece/Item (S/P/I),Total Cost with Tax,Total Cost Before Tax,GST Amount,HST Amount,QST Amount,Base Cost,Residential Area Charge,Fuel Surcharge,ExpCheq Surcharge,Reference Per Piece";

            //need to test in a safe environment            
            if (File.Exists(location))
            {
                File.Delete(location);
                result = true;
            }

            using (StreamWriter writer = File.CreateText(location))
            {
                writer.WriteLine(columns);
                foreach (string s in lines)
                {
                    writer.WriteLine(s);
                }
            }

            return result;
        }

        public bool ExportFile(string location, List<ShippingEntry> _Entries)
        {
            bool result = false;

            string columns = @"Courier,Shipment Code,Customer Code,Customer Name,Reference 1,Reference 2,Reference 3,Reference 4,Reference 5,Package PIN,Shipment/Piece/Item (S/P/I),Total Cost with Tax,Total Cost Before Tax,GST Amount,HST Amount,QST Amount,Base Cost,Residential Area Charge,Fuel Surcharge,ExpCheq Surcharge,Reference Per Piece";

            //need to test in a safe environment            
            if (File.Exists(location))
            {
                File.Delete(location);
                result = true;
            }

            using (StreamWriter writer = File.CreateText(location))
            {
                writer.WriteLine(columns);
                foreach (ShippingEntry Entry in _Entries)
                {
                    writer.WriteLine(Entry.ToStringLong());
                }
            }

            return result;
        }
        public bool ExportFile(string location, List<ShippingEntry> _Entries, bool headers)
        {
            bool result = false;

            string columns = @"Courier,Shipment Code,Customer Code,Customer Name,Reference 1,Reference 2,Reference 3,Reference 4,Reference 5,Package PIN,Shipment/Piece/Item (S/P/I),Total Cost with Tax,Total Cost Before Tax,GST Amount,HST Amount,QST Amount,Base Cost,Residential Area Charge,Fuel Surcharge,ExpCheq Surcharge,Reference Per Piece";

            //need to test in a safe environment            
            if (File.Exists(location))
            {
                File.Delete(location);
                result = true;
            }

            using (StreamWriter writer = File.CreateText(location))
            {
                if (headers)
                    writer.WriteLine(columns);

                foreach (ShippingEntry Entry in _Entries)
                {
                    writer.WriteLine(Entry.ToStringLong());
                }
            }

            return result;
        }
        #endregion

        #endregion

        #region Events
        private void LvDates_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvDates.SelectedItems.Count < 1 || lvDates.SelectedItems[0].ToString() == "")
                return;


            FileLocation = lvDates.SelectedItems[0].Name.ToString();
            UpdateLVEntries();
        }

        private void TxtFilter_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                BtnFilter_Click(sender, e);

        }

        private void TxtBOL_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                BtnFilter_Click(sender, e);
        }

        private void TxtLocation_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                BtnPick_Click(sender, e);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Debug.WriteLine($"Form1_Load");
            BtnPick_Click(sender, e);
        }

        private void BtnPick_Click(object sender, EventArgs e)
        {
            try
            {
                FTPMethods.DownloadNewFiles();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            if (!CheckDirectories())
            {
                if (fbdFolder.ShowDialog() == DialogResult.OK)
                {
                    WorkingDir = fbdFolder.SelectedPath;
                    txtLocation.Text = fbdFolder.SelectedPath;
                }
                else
                {
                    return;
                }
            }

            if (shpVersion)
                ConvertAll();

            UpdateListBox();
        }

        private void BtnFilter_Click(object sender, EventArgs e)
        {
            FilterCode = txtCustomer.Text.ToLower();
            OrderNo = txtOrder.Text.ToLower();
            FilterBOL = txtBOL.Text.ToLower();

            UpdateListBox();
            UpdateLVEntries();
        }

        private void BtnNew_Click(object sender, EventArgs e)
        {
            if (lvDates.SelectedItems.Count < 1)
                return;

            // Set shp and tmp filenames
            int pos = FileLocation.IndexOf('.');
            string tmp = $"{FileLocation.Substring(0, pos)}.tmp";
            string shp = $"{FileLocation.Substring(0, pos)}.shp";

            DebugWindow debugWindow = new DebugWindow();
            if (debugWindow.ShowDialog() == DialogResult.OK)
            {
                ShippingEntry ent = debugWindow.Entry;
                if (Entries.Count > 0 && ent.Date == Entries[0].Date)
                {
                    Entries.Add(ent);

                    ExportFile(tmp, Entries.Where(o => o.CourierCompany != "Purolator").ToList(), false);
                    ExportFile(shp, Entries.Where(o => o.CourierCompany == "Purolator").ToList());
                }
                else
                {
                    string s = $"{WorkingDir}\\{ent.Date:yyyy_MM_dd}.tmp";

                    // Create new date file, or add to end of existing one.
                    if (!File.Exists(tmp))
                    {
                        _ = ExportFile(s, new List<ShippingEntry> { ent }, false);
                    }
                    else
                    {
                        File.AppendAllLines(s, new string[] { ent.ToStringLong() });
                    }
                }

                UpdateLVEntries();
            }

        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (lvEntries.SelectedItems.Count < 1)
                return;

            int index = lvEntries.SelectedIndices[0];
            Color shading = lvEntries.SelectedItems[0].BackColor;

            ShippingEntry Entry = Entries[index];
            DebugWindow debugWindow = new DebugWindow(Entry);

            if (debugWindow.ShowDialog() == DialogResult.OK)
            {
                Entry.LoadString(debugWindow.Entry.ToString());

                lvEntries.Items.RemoveAt(index);
                ListViewItem item = debugWindow.Entry.ToListViewItem();
                item.BackColor = shading;
                lvEntries.Items.Insert(index, item);

                Entries.RemoveAt(index);
                Entries.Insert(index, debugWindow.Entry);

                // Set shp and tmp filenames
                int pos = FileLocation.IndexOf('.');
                string tmp = $"{FileLocation.Substring(0, pos)}.tmp";
                string shp = $"{FileLocation.Substring(0, pos)}.shp";

                ExportFile(tmp, Entries.Where(o => o.CourierCompany != "Purolator").ToList(), false);
                ExportFile(shp, Entries.Where(o => o.CourierCompany == "Purolator").ToList());
            }

        }

        private void BtnConvert_Click(object sender, EventArgs e)
        {
            ConvertAll();
        }

        private void LvEntries_DoubleClick(object sender, EventArgs e)
        {
            if (lvEntries.SelectedItems.Count < 1 || lvEntries.SelectedItems[0].SubItems.Count < 10 || lvEntries.SelectedItems[0].SubItems[10].Text == "")
                return;

            int index = lvEntries.SelectedIndices[0];

            ShippingEntry ent = Entries[index];

            switch (ent.CourierCompany.ToLower())
            {
                case "purolator":
                    _ = Process.Start(ent.PuroLink);
                    break;

                case "loomis":
                    _ = Process.Start($"https://www.loomisexpress.com/loomship/Track/TrackStatus?wbs={ent.PackagePIN}");
                    break;

                case "corporate":
                    goto default;

                case "novex":
                    goto default;

                case "k&h":
                    _ = Process.Start($"http://cc.khdispatch.com/ccweb1/OrderDetails.aspx?uid={ent.PackagePIN}");
                    break;

                case "strait":
                    goto default;

                default:
                    MessageBox.Show("Courier company does not have auto-tracking yet.");
                    break;
            }

            //Process.Start($"https://www.purolator.com/en/shipping/tracker?pin={lvEntries.SelectedItems[0].SubItems[10].Text}");
            //Process.Start($"https://www.purolator.com/en/ship-track/tracking-details.page?pin={lvEntries.SelectedItems[0].SubItems[10].Text}");
        }

        private void LvEntries_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvEntries.SelectedItems.Count < 1)
                return;

            int index = lvEntries.SelectedIndices[0];

            ShippingEntry entry = Entries[index];
            btnOpenLink.Text = entry.CourierCompany;
        }

        #region ToolStrip
        private void SortEntriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string _new = $"{Settings.Default.BaseDir}\\Archive\\{Path.GetFileName(FileLocation)}";
            string _folder = Path.GetDirectoryName(_new);
            string _customFilename = FileLocation.Replace(".CSV", ".shp");

            if (!File.Exists(_new))
            {
                Directory.CreateDirectory(_folder);
                File.Move(FileLocation, _new);
            }

            ExportFile(_customFilename, FTPMethods.SortFile(Entries));
            //ExportFile(FileLocation, FTPMethods.SortFile(Entries));
        }

        private void OpenFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(FileLocation);
        }

        private void AddNewEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void DeleteEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lvEntries.SelectedItems.Count < 1)
                return;

            int index = lvEntries.SelectedIndices[0];

            if (MessageBox.Show($"Delete entry for {Entries[index].CustomerCode}?", $"Delete {Entries[index].CustomerCode}", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                return;

            lvEntries.Items.RemoveAt(index);
            Entries.RemoveAt(index);

            // Set shp and tmp filenames
            int pos = FileLocation.IndexOf('.');
            string tmp = $"{FileLocation.Substring(0, pos)}.tmp";
            string shp = $"{FileLocation.Substring(0, pos)}.shp";

            ExportFile(tmp, Entries.Where(o => o.CourierCompany != "Purolator").ToList(), false);
            ExportFile(shp, Entries.Where(o => o.CourierCompany == "Purolator").ToList());
            //ExportFile(FileLocation, Entries);

        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
        }

        #endregion

        #endregion

    }

    public class ShippingEntry
    {
        public DateTime Date = DateTime.Now;
        public string CourierCompany = "Purolator";
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

        public string PuroLink => $"https://www.purolator.com/en/ship-track/tracking-details.page?pin={PackagePIN}";

        public bool ContainsOrder(string OrderNo)
        {
            if (Reference1.ToLower().Contains(OrderNo.ToLower()))
                return true;

            if (Reference2.ToLower().Contains(OrderNo.ToLower()))
                return true;

            if (Reference3.ToLower().Contains(OrderNo.ToLower()))
                return true;

            if (Reference4.ToLower().Contains(OrderNo.ToLower()))
                return true;

            if (Reference5.ToLower().Contains(OrderNo.ToLower()))
                return true;

            return false;
        }

        public ShippingEntry() { }

        public ShippingEntry(DateTime date, string item, bool SHP)
        {
            Date = date;

            if (SHP)
                LoadStringLong(item);
            else
                LoadString(item);
        }

        public ListViewItem ToListViewItem()
        {
            ListViewItem result = new ListViewItem(Date.ToShortDateString());

            result.SubItems.Add(CourierCompany);
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

        public override string ToString()
        {
            return $"{ShipmentCode},{CustomerCode},{CustomerName}," +
                $"{Reference1},{Reference2},{Reference3},{Reference4},{Reference5}," +
                $"{PackagePIN},{SPI},{TotalCostwithTax},{TotalCostBeforeTax}," +
                $"{GSTAmount},{HSTAmount},{QSTAmount},{BaseCost}," +
                $"{ResidentialAreaCharge},{FuelSurcharge},{ExpCheqSurcharge},{ReferencePerPiece}";

        }

        public string ToStringLong()
        {
            return $"{CourierCompany},{ShipmentCode},{CustomerCode},{CustomerName}," +
                $"{Reference1},{Reference2},{Reference3},{Reference4},{Reference5}," +
                $"{PackagePIN},{SPI},{TotalCostwithTax},{TotalCostBeforeTax}," +
                $"{GSTAmount},{HSTAmount},{QSTAmount},{BaseCost}," +
                $"{ResidentialAreaCharge},{FuelSurcharge},{ExpCheqSurcharge},{ReferencePerPiece}";

        }


        public void LoadString(string item)
        {
            if (!item.Contains(','))
                return;

            string[] split = item.Split(',');

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

        public void LoadStringLong(string item)
        {
            if (!item.Contains(','))
                return;

            string[] split = item.Split(',');

            CourierCompany = split[0];
            ShipmentCode = split[1];
            CustomerCode = split[2];
            CustomerName = split[3];
            Reference1 = split[4];
            Reference2 = split[5];
            Reference3 = split[6];
            Reference4 = split[7];
            Reference5 = split[8];

            if (split.Length < 22)
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
            else
            {
                PackagePIN = split[10];
                SPI = split[11];
                TotalCostwithTax = split[12];
                TotalCostBeforeTax = split[13];
                GSTAmount = split[14];
                HSTAmount = split[15];
                QSTAmount = split[16];
                BaseCost = split[17];
                ResidentialAreaCharge = split[18];
                FuelSurcharge = split[19];
                ExpCheqSurcharge = split[20];
                ReferencePerPiece = split[21];
            }
        }
    }

    public static class FTPMethods
    {
        public static string[] GetDirectoryListing()
        {
            // Get the object used to communicate with the server.
            FtpWebRequest directoryListRequest = (FtpWebRequest)WebRequest.Create($"{Settings.Default.FTPAddress}:{Settings.Default.FTPPort}");
            directoryListRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;

            // This example assumes the FTP site uses anonymous logon.
            directoryListRequest.Credentials = new NetworkCredential(Settings.Default.FTPUser, Settings.Default.FTPPassword);
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

        public static string GetFTPFile(string address)
        {
            string result = "";

            // Get the object used to communicate with the server.
            FtpWebRequest directoryDownloadRequest = (FtpWebRequest)WebRequest.Create(address);
            directoryDownloadRequest.Method = WebRequestMethods.Ftp.DownloadFile;

            // This example assumes the FTP site uses anonymous logon.
            directoryDownloadRequest.Credentials = new NetworkCredential(Settings.Default.FTPUser, Settings.Default.FTPPassword);
            directoryDownloadRequest.UsePassive = false;

            FtpWebResponse response = (FtpWebResponse)directoryDownloadRequest.GetResponse();

            using (Stream responseStream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    result = reader.ReadToEnd();
                    Console.WriteLine(result);

                    Console.WriteLine($"Download Complete, status {response.StatusDescription}");
                }
            }
            response.Close();

            return result;
        }

        public static void DownloadNewFiles()
        {
            string[] files = GetDirectoryListing();
            string[] details;
            foreach (string s in files)
            {
                details = ParseTextString(s);

                string date = ParseDate(details[0]);
                if (Path.GetExtension(details[1]).Contains("CSV"))
                {
                    string localfile = $" {Settings.Default.BaseDir}\\{date}.CSV";
                    if (!File.Exists(localfile))
                    {
                        try
                        {
                            string info = GetFTPFile($"{Settings.Default.FTPAddress}/{details[1]}").Trim();
                            string[] split = info.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                            File.WriteAllLines(localfile, split);
                            string newFile = localfile.Replace("CSV", "shp");

                            //if ( File.Exists(newFile))
                            //{
                            //    for(int j = 1; j< split.Length; j++)
                            //    {
                            //        using (StreamWriter writer = File.AppendText(newFile))
                            //        {
                            //            writer.WriteLine(split[j]);
                            //        }
                            //    }
                            //}
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                        }
                    }
                }
            }
        }

        private static string ParseDate(string raw)
        {
            //string temp = "19 May 2020";
            string[] temp = raw.Split(' ');

            switch (temp[1])
            {
                case "Jan":
                    temp[1] = "01";
                    break;

                case "Feb":
                    temp[1] = "02";
                    break;

                case "Mar":
                    temp[1] = "03";
                    break;

                case "Apr":
                    temp[1] = "04";
                    break;

                case "May":
                    temp[1] = "05";
                    break;

                case "Jun":
                    temp[1] = "06";
                    break;

                case "Jul":
                    temp[1] = "07";
                    break;

                case "Aug":
                    temp[1] = "08";
                    break;

                case "Sep":
                    temp[1] = "09";
                    break;

                case "Oct":
                    temp[1] = "10";
                    break;

                case "Nov":
                    temp[1] = "11";
                    break;

                case "Dec":
                    temp[1] = "12";
                    break;

            }

            string result = $"{temp[2]}_{temp[1]}_{temp[0]}";

            return result;
        }

        private static string[] ParseTextString(string RAW)
        {
            //string temp = $" - rw-r--r--" +
            //    $"1" +
            //    $"ftp" +
            //    $"ftp" +
            //    $"" +
            //    $"" +
            //    $"" +
            //    $"" +
            //    $"" +
            //    $"" +
            //    $"" +
            //    $"" +
            //    $"" +
            //    $"" +
            //    $"3100" +
            //    $"May" +
            //    $"13" +
            //    $"22:44" +
            //    $"MAN0016.CSV";

            string[] result = RAW.Split(' ');

            return new string[] { $"{result[16]} {result[15]} {DateTime.Now.Year}", result[18] };
        }

        public static List<ShippingEntry> ParseRawString(string raw, DateTime Date, bool SHP)
        {
            string[] split = raw.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            List<ShippingEntry> Entries = new List<ShippingEntry>();

            for (int i = 1; i < split.Length; i++)
            {
                ShippingEntry Entry = new ShippingEntry(Date, split[i], SHP);
                Entries.Add(Entry);
            }

            return Entries;
        }

        public static List<string> SortFile(List<ShippingEntry> Entries)
        {
            List<string> results = new List<string>();


            foreach (ShippingEntry E in Entries.OrderBy(o => o.CustomerName))
            {
                //results.Add(E.ToString());
                results.Add(E.ToStringLong());
            }


            return results;
        }

    }

}