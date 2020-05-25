namespace Shiplogger
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lvEntries = new System.Windows.Forms.ListView();
            this.lvDates = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.btnPick = new System.Windows.Forms.Button();
            this.fbdFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.txtCustomer = new System.Windows.Forms.TextBox();
            this.btnFilter = new System.Windows.Forms.Button();
            this.cbFilter = new System.Windows.Forms.CheckBox();
            this.pnDates = new System.Windows.Forms.Panel();
            this.pnEntries = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pnDates.SuspendLayout();
            this.pnEntries.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvEntries
            // 
            this.lvEntries.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvEntries.HideSelection = false;
            this.lvEntries.Location = new System.Drawing.Point(0, 0);
            this.lvEntries.Margin = new System.Windows.Forms.Padding(2);
            this.lvEntries.Name = "lvEntries";
            this.lvEntries.Size = new System.Drawing.Size(661, 468);
            this.lvEntries.TabIndex = 8;
            this.lvEntries.UseCompatibleStateImageBehavior = false;
            this.lvEntries.View = System.Windows.Forms.View.Details;
            // 
            // lvDates
            // 
            this.lvDates.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lvDates.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvDates.HideSelection = false;
            this.lvDates.Location = new System.Drawing.Point(5, 0);
            this.lvDates.Margin = new System.Windows.Forms.Padding(2);
            this.lvDates.Name = "lvDates";
            this.lvDates.Size = new System.Drawing.Size(119, 468);
            this.lvDates.TabIndex = 7;
            this.lvDates.UseCompatibleStateImageBehavior = false;
            this.lvDates.View = System.Windows.Forms.View.Details;
            this.lvDates.SelectedIndexChanged += new System.EventHandler(this.lvDates_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Date";
            this.columnHeader1.Width = 107;
            // 
            // txtAddress
            // 
            this.txtAddress.AllowDrop = true;
            this.txtAddress.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.txtAddress.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
            this.txtAddress.Location = new System.Drawing.Point(13, 10);
            this.txtAddress.Margin = new System.Windows.Forms.Padding(2);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(275, 20);
            this.txtAddress.TabIndex = 0;
            this.txtAddress.Text = "P:\\Public\\Shipping\\purofiles";
            this.txtAddress.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtAddress_PreviewKeyDown);
            // 
            // btnPick
            // 
            this.btnPick.Location = new System.Drawing.Point(291, 10);
            this.btnPick.Margin = new System.Windows.Forms.Padding(2);
            this.btnPick.Name = "btnPick";
            this.btnPick.Size = new System.Drawing.Size(83, 19);
            this.btnPick.TabIndex = 1;
            this.btnPick.Text = "Load Files";
            this.btnPick.UseVisualStyleBackColor = true;
            this.btnPick.Click += new System.EventHandler(this.btnPick_Click);
            // 
            // fbdFolder
            // 
            this.fbdFolder.RootFolder = System.Environment.SpecialFolder.MyComputer;
            this.fbdFolder.ShowNewFolderButton = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 36);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Customer Code";
            // 
            // txtCustomer
            // 
            this.txtCustomer.Location = new System.Drawing.Point(102, 36);
            this.txtCustomer.Margin = new System.Windows.Forms.Padding(2);
            this.txtCustomer.Name = "txtCustomer";
            this.txtCustomer.Size = new System.Drawing.Size(76, 20);
            this.txtCustomer.TabIndex = 2;
            this.txtCustomer.TextChanged += new System.EventHandler(this.btnFilter_Click);
            this.txtCustomer.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtCustomer_PreviewKeyDown);
            // 
            // btnFilter
            // 
            this.btnFilter.Location = new System.Drawing.Point(291, 34);
            this.btnFilter.Margin = new System.Windows.Forms.Padding(2);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(56, 19);
            this.btnFilter.TabIndex = 4;
            this.btnFilter.Text = "Filter";
            this.btnFilter.UseVisualStyleBackColor = true;
            this.btnFilter.Click += new System.EventHandler(this.btnFilter_Click);
            // 
            // cbFilter
            // 
            this.cbFilter.AutoSize = true;
            this.cbFilter.Checked = true;
            this.cbFilter.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbFilter.Location = new System.Drawing.Point(182, 38);
            this.cbFilter.Margin = new System.Windows.Forms.Padding(2);
            this.cbFilter.Name = "cbFilter";
            this.cbFilter.Size = new System.Drawing.Size(87, 17);
            this.cbFilter.TabIndex = 3;
            this.cbFilter.Text = "Filter results?";
            this.cbFilter.UseVisualStyleBackColor = true;
            this.cbFilter.CheckedChanged += new System.EventHandler(this.btnFilter_Click);
            // 
            // pnDates
            // 
            this.pnDates.Controls.Add(this.lvDates);
            this.pnDates.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnDates.Location = new System.Drawing.Point(0, 65);
            this.pnDates.Name = "pnDates";
            this.pnDates.Padding = new System.Windows.Forms.Padding(5, 0, 0, 5);
            this.pnDates.Size = new System.Drawing.Size(124, 473);
            this.pnDates.TabIndex = 7;
            // 
            // pnEntries
            // 
            this.pnEntries.Controls.Add(this.lvEntries);
            this.pnEntries.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnEntries.Location = new System.Drawing.Point(124, 65);
            this.pnEntries.Name = "pnEntries";
            this.pnEntries.Padding = new System.Windows.Forms.Padding(0, 0, 5, 5);
            this.pnEntries.Size = new System.Drawing.Size(666, 473);
            this.pnEntries.TabIndex = 8;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.txtAddress);
            this.panel1.Controls.Add(this.btnPick);
            this.panel1.Controls.Add(this.cbFilter);
            this.panel1.Controls.Add(this.txtCustomer);
            this.panel1.Controls.Add(this.btnFilter);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(790, 65);
            this.panel1.TabIndex = 9;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(790, 538);
            this.Controls.Add(this.pnEntries);
            this.Controls.Add(this.pnDates);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(492, 310);
            this.Name = "Form1";
            this.Text = "Shiplog";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.pnDates.ResumeLayout(false);
            this.pnEntries.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ListView lvEntries;
        private System.Windows.Forms.TextBox txtAddress;
        private System.Windows.Forms.Button btnPick;
        private System.Windows.Forms.FolderBrowserDialog fbdFolder;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtCustomer;
        private System.Windows.Forms.Button btnFilter;
        private System.Windows.Forms.ListView lvDates;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.CheckBox cbFilter;
        private System.Windows.Forms.Panel pnDates;
        private System.Windows.Forms.Panel pnEntries;
        private System.Windows.Forms.Panel panel1;
    }
}

