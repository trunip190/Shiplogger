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
            this.btnLoad = new System.Windows.Forms.Button();
            this.lvEntries = new System.Windows.Forms.ListView();
            this.lbDate = new System.Windows.Forms.ListBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.btnPick = new System.Windows.Forms.Button();
            this.fbdFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.txtCustomer = new System.Windows.Forms.TextBox();
            this.btnFilter = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(501, 12);
            this.btnLoad.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(119, 23);
            this.btnLoad.TabIndex = 0;
            this.btnLoad.Text = "Open Location";
            this.btnLoad.UseVisualStyleBackColor = true;
            // 
            // lvEntries
            // 
            this.lvEntries.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvEntries.HideSelection = false;
            this.lvEntries.Location = new System.Drawing.Point(0, 0);
            this.lvEntries.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.lvEntries.Name = "lvEntries";
            this.lvEntries.Size = new System.Drawing.Size(607, 167);
            this.lvEntries.TabIndex = 1;
            this.lvEntries.UseCompatibleStateImageBehavior = false;
            this.lvEntries.View = System.Windows.Forms.View.Details;
            // 
            // lbDate
            // 
            this.lbDate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbDate.FormattingEnabled = true;
            this.lbDate.ItemHeight = 16;
            this.lbDate.Location = new System.Drawing.Point(0, 0);
            this.lbDate.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.lbDate.Name = "lbDate";
            this.lbDate.Size = new System.Drawing.Size(607, 75);
            this.lbDate.TabIndex = 2;
            this.lbDate.SelectedValueChanged += new System.EventHandler(this.listBox1_SelectedValueChanged);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(13, 70);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lbDate);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lvEntries);
            this.splitContainer1.Size = new System.Drawing.Size(607, 246);
            this.splitContainer1.SplitterDistance = 75;
            this.splitContainer1.TabIndex = 3;
            // 
            // txtAddress
            // 
            this.txtAddress.Location = new System.Drawing.Point(131, 15);
            this.txtAddress.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(365, 22);
            this.txtAddress.TabIndex = 4;
            this.txtAddress.Text = "C:\\Users\\chrisw\\purofiles";
            // 
            // btnPick
            // 
            this.btnPick.Location = new System.Drawing.Point(13, 14);
            this.btnPick.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnPick.Name = "btnPick";
            this.btnPick.Size = new System.Drawing.Size(111, 23);
            this.btnPick.TabIndex = 5;
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
            this.label1.Location = new System.Drawing.Point(13, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 17);
            this.label1.TabIndex = 6;
            this.label1.Text = "Customer Code";
            // 
            // txtCustomer
            // 
            this.txtCustomer.Location = new System.Drawing.Point(131, 43);
            this.txtCustomer.Name = "txtCustomer";
            this.txtCustomer.Size = new System.Drawing.Size(100, 22);
            this.txtCustomer.TabIndex = 7;
            // 
            // btnFilter
            // 
            this.btnFilter.Location = new System.Drawing.Point(545, 42);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(75, 23);
            this.btnFilter.TabIndex = 8;
            this.btnFilter.Text = "Filter";
            this.btnFilter.UseVisualStyleBackColor = true;
            this.btnFilter.Click += new System.EventHandler(this.btnFilter_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(635, 336);
            this.Controls.Add(this.btnFilter);
            this.Controls.Add(this.txtCustomer);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnPick);
            this.Controls.Add(this.txtAddress);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.btnLoad);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MinimumSize = new System.Drawing.Size(650, 373);
            this.Name = "Form1";
            this.Text = "Form1";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.ListView lvEntries;
        private System.Windows.Forms.ListBox lbDate;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox txtAddress;
        private System.Windows.Forms.Button btnPick;
        private System.Windows.Forms.FolderBrowserDialog fbdFolder;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtCustomer;
        private System.Windows.Forms.Button btnFilter;
    }
}

