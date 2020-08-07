using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using Shiplogger.Properties;
using System.Net;
using System.Windows.Forms;

namespace Shiplogger
{
    public partial class MainWindow : Form
    {
        #region variables
        private string FilterCode = "";
        private string OrderNo = "";
        private string FilterBOL = "";
        private readonly List<string> LoadedFiles = new List<string>();
        private List<ShippingEntry> Entries = new List<ShippingEntry>();
        private string FileLocation = "";
        private string WorkingDir;
        private string LogFile => $"{WorkingDir}\\log.txt";
        private readonly bool shpVersion = false;
        private readonly Dictionary<string, string> companies = new Dictionary<string, string>();
        private readonly bool FullColumns = false;
        private string User => Environment.UserName;
        private string Machine => Environment.MachineName;
        #endregion

        #region Methods
        public MainWindow()
        {
            Debug.WriteLine($"Form1");
            InitializeComponent();
            shpVersion = Settings.Default.SHPMode;
            BtnConvert.Enabled = Settings.Default.DebugMode;
            FullColumns = Settings.Default.FullColumns;
            string s = $"{Environment.CurrentDirectory}\\purofiles\\";
            if (Directory.Exists(s))
            {
                Settings.Default.BaseDir = s;
                Settings.Default.Save();
            }

        }

        private void Log(string message)
        {
            if (!Settings.Default.Log)
            {
                return;
            }

            try
            {
                using (StreamWriter writer = File.AppendText(LogFile))
                {
                    writer.WriteLine($"[{DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()} {Machine} - {User}] {message}");
                }
            }
            catch { }
            finally { }
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
            foreach (string s in LoadedFiles)
            {
                if (!File.Exists(s))
                {
                    return;
                }

                List<ShippingEntry> ents = ParseEntries(File.ReadAllLines(s), ParseDate(s), shpVersion);

                foreach (ShippingEntry se in ents)
                {
                    if (!companies.ContainsKey(se.CustomerCode))
                    {
                        companies.Add(se.CustomerCode, se.CustomerName);
                    }
                }
            }
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
            //List<string> dates = new List<string>();
            Dictionary<string, int> dates = new Dictionary<string, int>();


            // New week variables.
            DateTime lastDate = ParseDate(LoadedFiles[0]);
            DateTime newDate;
            Color backColour = Color.White;


            foreach (string s in LoadedFiles)
            {
                ListViewItem Entry = new ListViewItem
                {
                    Name = s,
                    Text = Path.GetFileNameWithoutExtension(s),
                    ForeColor = Color.Black,
                };

                // Format weekly.
                newDate = ParseDate(s);
                Entry.Text = newDate.ToString("ddd dd MMMM yyyy");

                if ( NextWeek(lastDate, newDate))
                {
                    lastDate = newDate;
                    backColour = backColour == Color.LightGray ? Color.White : Color.LightGray;
                }
                Entry.BackColor = backColour;

                // Add if new entry.

                if (!dates.ContainsKey(Entry.Text))
                {
                    if (ContainsFilterCode(s))
                    {
                        Entry.ForeColor = Color.Blue;
                        lvDates.Items.Add(Entry);
                        dates.Add(Entry.Text, lvDates.Items.Count - 1);
                    }
                    else if (cbFilter.Checked == false)
                    {
                        lvDates.Items.Add(Entry);
                        dates.Add(Entry.Text, lvDates.Items.Count - 1);
                    }
                    else
                    {
                        Debug.WriteLine(s);
                    }
                }
                else
                {
                    if (ContainsFilterCode(s))
                    {
                        int i = dates[Entry.Text];
                        lvDates.Items[i].ForeColor = Color.Blue;
                    }
                        //lvDates.Items[i].ForeColor =Color.Blue; 
                }
            }
            
            lvDates.EndUpdate();
        }

        public bool NextWeek(DateTime oldDate, DateTime newDate)
        {
            return (newDate - oldDate).TotalDays < -5;
        }

        public void UpdateLVEntries()
        {
            // Clear old entries.
            lvEntries.BeginUpdate();
            lvEntries.Clear();
            Entries.Clear();

            if (FileLocation == "")
                return;

            // Set shp and tmp filenames
            int pos = FileLocation.IndexOf('.');
            string tmp = $"{FileLocation.Substring(0, pos)}.tmp";
            string shp = $"{FileLocation.Substring(0, pos)}.shp";
            Debug.WriteLine(tmp);

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

            string[] ColumnsToAdd;
            if (FullColumns)
            {
                ColumnsToAdd = new string[]
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
            }
            else
            {
                ColumnsToAdd = new string[]
                {
                    "Date",
                    "Courier",
                    "Shipment Code",
                    "Customer Code",
                    "Customer Name",
                    "Order 1",
                    "Order 2",
                    "Order 3",
                    "Order 4",
                    "Order 5+",
                    "Package PIN",
                    "Shipment/Piece/Item (S/P/I)",
                    "Reference Per Piece",
                    "Weight",
                    //"Length",
                    //"Width",
                    //"Height"
                };

                lvEntries.Columns.Add("Date", 5);
                lvEntries.Columns.Add("Courier", 66);
                lvEntries.Columns.Add("Shipment Code", 50);
                lvEntries.Columns.Add("Customer Code", 124);
                lvEntries.Columns.Add("Customer Name", 244);
                lvEntries.Columns.Add("Orders", 65);
                //lvEntries.Columns.Add("Order 2", 65);
                //lvEntries.Columns.Add("Order 3", 65);
                //lvEntries.Columns.Add("Order 4", 65);
                //lvEntries.Columns.Add("Order 5+", 65);
                lvEntries.Columns.Add("Package PIN", 96);
                lvEntries.Columns.Add("Shipment/Piece/Item (S/P/I)", 50);
                lvEntries.Columns.Add("Reference Per Piece", 50);
                lvEntries.Columns.Add("Weight", 61);
                
            }



            if (!shpVersion)
            {
                _ = lvEntries.Columns.Add("Courier");
            }

            //foreach (string s in ColumnsToAdd)
            //{
            //    lvEntries.Columns.Add(s);
            //}

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

                if ((FilterCode == "" && OrderNo == "" && FilterBOL == "") || (entrybool && orderbool && BOLbool) ||!cbFilter.Checked)
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
            //lvEntries.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            //lvEntries.Columns[0].Width = 5;
            lvEntries.Columns[5].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            lvEntries.EndUpdate();
            lvEntries.Update();
            
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

        private void ExportAll(string tmp, string shp)
        {
            if (Entries.Where(o => o.CourierCompany != "Purolator").ToList().Count > 0)
            {
                ExportFile(tmp, Entries.Where(o => o.CourierCompany != "Purolator").ToList(), false);
            }
            else
            {
                if (File.Exists(tmp) && MessageBox.Show("Delete Purolator entries?", "Delete?", MessageBoxButtons.YesNo) == DialogResult.OK)
                {
                    File.Delete(tmp);
                }
            }

            if (Entries.Where(o => o.CourierCompany == "Purolator").ToList().Count > 0)
            {
                ExportFile(shp, Entries.Where(o => o.CourierCompany == "Purolator").ToList());
            }
            else
            {
                if (File.Exists(shp) && MessageBox.Show("Delete other couriers entries?", "Delete?", MessageBoxButtons.YesNo) == DialogResult.OK)
                {
                    File.Delete(shp);
                }
            }
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

            //Log
            Log("Logging in");

            lvDates.SelectedIndices.Add(0);
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

            DebugWindow debugWindow = new DebugWindow
            {
                Companies = companies
            };

            if (debugWindow.ShowDialog() == DialogResult.OK)
            {
                ShippingEntry ent = debugWindow.Entry;
                if (Entries.Count > 0 && ent.Date == Entries[0].Date)
                {
                    Entries.Add(ent);

                    ExportAll(tmp, shp);
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
            DebugWindow debugWindow = new DebugWindow(Entry)
            {
                Companies = companies
            };

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

                ExportAll(tmp, shp);
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


            switch (lvEntries.SelectedItems[0].SubItems[1].Text.ToLower())
            {
                case "purolator":
                    //_ = Process.Start(lvEntries.SelectedItems[0].SubItems[10].Text);
                    Process.Start($"https://www.purolator.com/en/shipping/tracker?pin={lvEntries.SelectedItems[0].SubItems[10].Text}");
            
                    break;

                case "loomis":
                    _ = Process.Start($"https://www.loomisexpress.com/loomship/Track/TrackStatus?wbs={lvEntries.SelectedItems[0].SubItems[10].Text}");
                    break;

                case "corporate":
                    goto default;

                case "novex":
                    goto default;

                case "k&h":
                    _ = Process.Start($"http://cc.khdispatch.com/ccweb1/OrderDetails.aspx?uid={lvEntries.SelectedItems[0].SubItems[10].Text}");
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

            
            btnOpenLink.Text = lvEntries.SelectedItems[0].SubItems[1].Text;
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

        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
        }

        #endregion

        #endregion

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Log($"{e.CloseReason}");
        }

        private void CompaniesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PopulateCompanies();
        }

        private void checkFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] lines;
            string[] _files = Directory.GetFiles(WorkingDir, "*.csv");
            string line;
            int last = 0;
            int count = 0;
            foreach (string _file in _files)
            {
                lines = File.ReadAllLines(_file);
                //foreach (string line in lines)
                for (int i = 1; i < lines.Length; i++)
                {
                    line = lines[i];
                    count = line.Count(o => o == ',');
                    if (count != last)
                    {
                        last = count;
                        Debug.WriteLine($"{new ShippingEntry(DateTime.Now, line, false).CustomerName} +  {_file}");
                        break;
                    }
                }
            }
        }

        private void lvEntries_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {

            Debug.WriteLine($"lvEntries.Columns.Add({lvEntries.Columns[e.ColumnIndex].Text}, {lvEntries.Columns[e.ColumnIndex].Width}");
        }
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
        public string Weight;
        public string Length;
        public string Width;
        public string Height;


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

            // All orders combined into one field for the moment.
            result.SubItems.Add($"{Reference1} {Reference2} {Reference3} {Reference4} {Reference5}");
            //result.SubItems.Add(Reference2);
            //result.SubItems.Add(Reference3);
            //result.SubItems.Add(Reference4);
            //result.SubItems.Add(Reference5);

            result.SubItems.Add(PackagePIN);
            result.SubItems.Add(SPI);

            // Ignore all of the pricing stuff
            //result.SubItems.Add(TotalCostwithTax);
            //result.SubItems.Add(TotalCostBeforeTax);
            //result.SubItems.Add(GSTAmount);
            //result.SubItems.Add(HSTAmount);
            //result.SubItems.Add(QSTAmount);
            //result.SubItems.Add(BaseCost);
            //result.SubItems.Add(ResidentialAreaCharge);
            //result.SubItems.Add(FuelSurcharge);
            //result.SubItems.Add(ExpCheqSurcharge);

            result.SubItems.Add(ReferencePerPiece);
            result.SubItems.Add(Weight);
            result.SubItems.Add(Length);
            result.SubItems.Add(Width);
            result.SubItems.Add(Height);

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
                $"{ResidentialAreaCharge},{FuelSurcharge},{ExpCheqSurcharge}," +
                $"{ReferencePerPiece},{Weight},{Length},{Width},{Height} ";

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

            if (split.Length < 21)
            {

                ExpCheqSurcharge = split[18];
                ReferencePerPiece = split[19];
            }
            else
            {
                //PackagePIN = split[9];
                //SPI = split[10];
                //TotalCostwithTax = split[11];
                //TotalCostBeforeTax = split[12];
                //GSTAmount = split[13];
                //HSTAmount = split[14];
                //QSTAmount = split[15];
                //BaseCost = split[16];
                //ResidentialAreaCharge = split[17];

                //FuelSurcharge = split[18];

                ReferencePerPiece = split[18];
                Weight = split[19];
                Length = split[20];
                Width = split[21];
                Height = split[22];
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

            PackagePIN = split[9];
            SPI = split[10];
            TotalCostwithTax = split[11];
            TotalCostBeforeTax = split[12];
            GSTAmount = split[13];
            HSTAmount = split[14];
            QSTAmount = split[15];
            BaseCost = split[16];
            ResidentialAreaCharge = split[17];
            if (split.Length < 23)
            {
                FuelSurcharge = split[18];
                ExpCheqSurcharge = split[19];
                ReferencePerPiece = split[20];
            }
            else
            {
                ResidentialAreaCharge = split[18];
                FuelSurcharge = split[19];

                ReferencePerPiece = split[20];
                Weight = split[21];
                Length = split[22];
                Width = split[23];
                Height = split[24];
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