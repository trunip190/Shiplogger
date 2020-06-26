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
    public partial class Form1 : Form
    {
        private string FilterCode = "";
        private string OrderNo = "";
        private List<string> LoadedFiles = new List<string>();
        private List<ShippingEntry> Entries = new List<ShippingEntry>();
        private string FileLocation = "";
        private string WorkingDir;

        public Form1()
        {
            InitializeComponent();
            string s = $"{Environment.CurrentDirectory}\\purofiles\\";
            if ( Directory.Exists(s))
            {
                Settings.Default.BaseDir = s;
                Settings.Default.Save();
            }
        }

        #region Methods
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
                if (rawEntries[i] != "")
                {
                    ShippingEntry entry = new ShippingEntry(date, rawEntries[i]);
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

            foreach (string s in LoadedFiles)
            {
                ListViewItem Entry = new ListViewItem
                {
                    Name = s,
                    Text = Path.GetFileNameWithoutExtension(s),
                    ForeColor = Color.Black,
                };
                Entry.Text = ParseDate(s).ToString("ddd dd MMMM yyyy");

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
            //if (lvDates.SelectedItems.Count < 1)
            //    return;

            lvEntries.BeginUpdate();
            lvEntries.Clear();

            if (FileLocation == "")
                return; ;

            string[] Lines = File.ReadAllLines(FileLocation);

            Entries = ParseEntries(Lines, ParseDate(FileLocation)).OrderBy(o => o.CustomerCode).ToList();
            string[] ColumnsToAdd = Lines[0].Split(',');

            // Add in Date field.
            lvEntries.Columns.Add("Date");
            lvEntries.Columns.Add("Courier");
            foreach (string s in ColumnsToAdd)
            {
                lvEntries.Columns.Add(s);

            }

            //string currentCustomer = "";
            bool light = false;

            foreach (ShippingEntry entry in Entries)
            {
                ListViewItem lvi = entry.ToListViewItem();                              

                if ((FilterCode == "" && OrderNo == "")|| entry.CustomerCode.ToLower().Contains(FilterCode) && entry.ContainsOrder(OrderNo))
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
            List<ShippingEntry> Entries = ParseEntries(Lines, DateTime.Now).OrderBy(o => o.CustomerCode).ToList();

            // Cycle through each entry to check against FilterCode.
            foreach (ShippingEntry entry in Entries)
            {
                // If CustomerCode matches FilterCode, end method and return.
                if (entry.CustomerCode.ToLower().Contains(FilterCode) && entry.ContainsOrder(OrderNo))
                {
                    Debug.WriteLine($"ContainsFilterCode match: {entry.CustomerCode}");
                    return true;
                }
            }

            // Nothing matched.
            Debug.WriteLine($"No match founder for {FilterCode}");
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

        public bool ExportFile(string location, List<string> lines)
        {
            bool result = false;
            string columns = @"Shipment Code,Customer Code,Customer Name,Reference 1,Reference 2,Reference 3,Reference 4,Reference 5,Package PIN,Shipment/Piece/Item (S/P/I),Total Cost with Tax,Total Cost Before Tax,GST Amount,HST Amount,QST Amount,Base Cost,Residential Area Charge,Fuel Surcharge,ExpCheq Surcharge,Reference Per Piece";

            //need to test in a safe environment            
            if (File.Exists(location))
            {
                File.Delete(location);
                result = true;
            }
                
            using (StreamWriter writer = File.CreateText(location))
            {
                writer.WriteLine(columns);
                foreach ( string s in lines)
                {
                    writer.WriteLine(s);
                }
            }

            return result;
        }

        public bool ExportFile(string location, List<ShippingEntry> _Entries)
        {
            bool result = false;
            string columns = @"Shipment Code,Customer Code,Customer Name,Reference 1,Reference 2,Reference 3,Reference 4,Reference 5,Package PIN,Shipment/Piece/Item (S/P/I),Total Cost with Tax,Total Cost Before Tax,GST Amount,HST Amount,QST Amount,Base Cost,Residential Area Charge,Fuel Surcharge,ExpCheq Surcharge,Reference Per Piece";

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
                    writer.WriteLine(Entry.ToString());
                }
            }

            return result;
        }
        #endregion

        #region Events
        private void BtnFilter_Click(object sender, EventArgs e)
        {
            FilterCode = txtCustomer.Text.ToLower();
            OrderNo = txtOrder.Text.ToLower();

            UpdateListBox();

            UpdateLVEntries();
        }

        private void LvDates_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvDates.SelectedItems.Count <1 || lvDates.SelectedItems[0].ToString() == "")
                return;


            FileLocation = lvDates.SelectedItems[0].Name.ToString();
            UpdateLVEntries();
        }

        private void TxtFilter_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

            UpdateListBox();
        }

        private void LvEntries_DoubleClick(object sender, EventArgs e)
        {
            if (lvEntries.SelectedItems.Count < 1||lvEntries.SelectedItems[0].SubItems.Count < 10|| lvEntries.SelectedItems[0].SubItems[10].Text == "")
                return;

            Process.Start($"https://www.purolator.com/en/shipping/tracker?pin={lvEntries.SelectedItems[0].SubItems[10].Text}");
            //Process.Start($"https://www.purolator.com/en/ship-track/tracking-details.page?pin={lvEntries.SelectedItems[0].SubItems[10].Text}");
        }

        #region ToolStrip
        private void SortEntriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string _new = $"{Settings.Default.BaseDir}\\Archive\\{Path.GetFileName(FileLocation)}";
            string _folder = Path.GetDirectoryName(_new);

            if ( !File.Exists(_new))
            {
                Directory.CreateDirectory(_folder);
                File.Move(FileLocation, _new);
            }
            

            ExportFile(FileLocation, FTPMethods.SortFile(Entries));
            //ExportFile(FileLocation, FTPMethods.SortFile(Entries));
        }

        private void OpenFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(FileLocation);
        }

        private void EditEntryToolStripMenuItem_Click(object sender, EventArgs e)
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

                ExportFile(FileLocation, Entries);
            }

        }

        private void AddNewEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {

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

        public ShippingEntry(DateTime date, string item)
        {
            Date = date;

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
                            string info = GetFTPFile($"{Settings.Default.FTPAddress}/{details[1]}");
                            File.WriteAllLines(localfile, new string[] { info.Trim() });
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
                case "May":
                    temp[1] = "05";
                    break;

                case "Jun":
                    temp[1] = "06";
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

        public static List<ShippingEntry> ParseRawString(string raw, DateTime Date)
        {
            string[] split = raw.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            List<ShippingEntry> Entries = new List<ShippingEntry>();

            for (int i = 1; i < split.Length; i++)
            {
                ShippingEntry Entry = new ShippingEntry(Date, split[i]);
                Entries.Add(Entry);
            }

            return Entries;
        }

        public static List<string> SortFile(List<ShippingEntry> Entries)
        {
            List<string> results = new List<string>();


            foreach (ShippingEntry E in Entries.OrderBy(o=>o.CustomerName))
            {
                results.Add(E.ToString());
            }


            return results;
        }

    }

}