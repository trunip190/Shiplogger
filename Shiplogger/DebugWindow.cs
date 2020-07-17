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
        public ShippingEntry Entry;
        public Dictionary<string, string> Companies = new Dictionary<string, string>();

        public DebugWindow()
        {
            InitializeComponent();
            Entry = new ShippingEntry();

        }

        public DebugWindow(ShippingEntry _Entry)
        {
            InitializeComponent();
            Entry = _Entry;
        }

        private void DebugWindow_Load(object sender, EventArgs e)
        {
            dtpDate.Value = Entry.Date;
            txtCourier.Text = Entry.CourierCompany;
            txtName.Text = Entry.CustomerName;
            txtID.Text = Entry.CustomerCode;
            txtBOL.Text = Entry.PackagePIN;
            if (Entry.SPI == "S")
                cbLeadpin.Checked = true;

            txtRefs.Text = $"{Entry.Reference1} {Entry.Reference2} {Entry.Reference3} {Entry.Reference4} {Entry.Reference5}";

            AutocompleteID();
        }

        private void AutocompleteID()
        {
            txtID.AutoCompleteMode = AutoCompleteMode.Suggest;
            txtID.AutoCompleteSource = AutoCompleteSource.CustomSource;

            AutoCompleteStringCollection collection = new AutoCompleteStringCollection();
            collection.AddRange(Companies.Keys.ToArray());
            txtID.AutoCompleteCustomSource = collection;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            SaveEntry();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void SaveEntry()
        {
            Entry.CourierCompany = txtCourier.Text;
            Entry.Date = dtpDate.Value;
            Entry.CustomerName = txtName.Text;
            Entry.CustomerCode = txtID.Text;
            Entry.PackagePIN = txtBOL.Text;

            if (cbLeadpin.Checked == true)
                Entry.SPI = "S";
            else
                Entry.SPI = "P";

            string[] refs = txtRefs.Text.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < refs.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        Entry.Reference1 = refs[i];
                        break;

                    case 1:
                        Entry.Reference2 = refs[i];
                        break;

                    case 2:
                        Entry.Reference3 = refs[i];
                        break;

                    case 3:
                        Entry.Reference4 = refs[i];
                        break;

                    case 4:
                        Entry.Reference5 = refs[i];
                        break;

                    default:
                        Entry.Reference5 += $" {refs[i]}";
                        break;
                }
            }

        }

        private void TxtID_Leave(object sender, EventArgs e)
        {
            if ( Companies.ContainsKey(txtID.Text))
            {
                txtName.Text = Companies[txtID.Text];
            }
        }
    }
}
