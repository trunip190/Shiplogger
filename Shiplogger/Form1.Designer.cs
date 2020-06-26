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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.lvEntries = new System.Windows.Forms.ListView();
            this.lvDates = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ContextDays = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortEntriesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.txtLocation = new System.Windows.Forms.TextBox();
            this.btnPick = new System.Windows.Forms.Button();
            this.fbdFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.txtCustomer = new System.Windows.Forms.TextBox();
            this.cbFilter = new System.Windows.Forms.CheckBox();
            this.pnDates = new System.Windows.Forms.Panel();
            this.pnEntries = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.txtOrder = new System.Windows.Forms.TextBox();
            this.btnOpenLink = new System.Windows.Forms.Button();
            this.timerFTP = new System.Windows.Forms.Timer(this.components);
            this.workerFTP = new System.ComponentModel.BackgroundWorker();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnEdit = new System.Windows.Forms.Button();
            this.ContextEntries = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addNewEntryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editEntryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ContextDays.SuspendLayout();
            this.pnDates.SuspendLayout();
            this.pnEntries.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.ContextEntries.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvEntries
            // 
            this.lvEntries.ContextMenuStrip = this.ContextEntries;
            this.lvEntries.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvEntries.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvEntries.FullRowSelect = true;
            this.lvEntries.HideSelection = false;
            this.lvEntries.Location = new System.Drawing.Point(0, 0);
            this.lvEntries.Margin = new System.Windows.Forms.Padding(2);
            this.lvEntries.Name = "lvEntries";
            this.lvEntries.Size = new System.Drawing.Size(454, 275);
            this.lvEntries.TabIndex = 8;
            this.lvEntries.UseCompatibleStateImageBehavior = false;
            this.lvEntries.View = System.Windows.Forms.View.Details;
            this.lvEntries.DoubleClick += new System.EventHandler(this.LvEntries_DoubleClick);
            // 
            // lvDates
            // 
            this.lvDates.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lvDates.ContextMenuStrip = this.ContextDays;
            this.lvDates.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvDates.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvDates.HideSelection = false;
            this.lvDates.Location = new System.Drawing.Point(5, 0);
            this.lvDates.Margin = new System.Windows.Forms.Padding(0);
            this.lvDates.Name = "lvDates";
            this.lvDates.Size = new System.Drawing.Size(157, 275);
            this.lvDates.TabIndex = 7;
            this.lvDates.UseCompatibleStateImageBehavior = false;
            this.lvDates.View = System.Windows.Forms.View.Details;
            this.lvDates.SelectedIndexChanged += new System.EventHandler(this.LvDates_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Date";
            this.columnHeader1.Width = 150;
            // 
            // ContextDays
            // 
            this.ContextDays.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openFileToolStripMenuItem,
            this.sortEntriesToolStripMenuItem});
            this.ContextDays.Name = "cmSort";
            this.ContextDays.Size = new System.Drawing.Size(134, 48);
            this.ContextDays.Text = "Actions";
            // 
            // openFileToolStripMenuItem
            // 
            this.openFileToolStripMenuItem.Name = "openFileToolStripMenuItem";
            this.openFileToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.openFileToolStripMenuItem.Text = "Open File";
            this.openFileToolStripMenuItem.Click += new System.EventHandler(this.OpenFileToolStripMenuItem_Click);
            // 
            // sortEntriesToolStripMenuItem
            // 
            this.sortEntriesToolStripMenuItem.Name = "sortEntriesToolStripMenuItem";
            this.sortEntriesToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.sortEntriesToolStripMenuItem.Text = "Sort Entries";
            this.sortEntriesToolStripMenuItem.Click += new System.EventHandler(this.SortEntriesToolStripMenuItem_Click);
            // 
            // txtLocation
            // 
            this.txtLocation.AllowDrop = true;
            this.txtLocation.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.txtLocation.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
            this.txtLocation.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLocation.Location = new System.Drawing.Point(5, 2);
            this.txtLocation.Margin = new System.Windows.Forms.Padding(2);
            this.txtLocation.Name = "txtLocation";
            this.txtLocation.Size = new System.Drawing.Size(279, 23);
            this.txtLocation.TabIndex = 0;
            this.txtLocation.Text = "P:\\Public\\Shipping\\purofiles";
            this.txtLocation.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.TxtLocation_PreviewKeyDown);
            // 
            // btnPick
            // 
            this.btnPick.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPick.Location = new System.Drawing.Point(288, 2);
            this.btnPick.Margin = new System.Windows.Forms.Padding(2);
            this.btnPick.Name = "btnPick";
            this.btnPick.Size = new System.Drawing.Size(92, 24);
            this.btnPick.TabIndex = 1;
            this.btnPick.Text = "Load Files";
            this.btnPick.UseVisualStyleBackColor = true;
            this.btnPick.Click += new System.EventHandler(this.BtnPick_Click);
            // 
            // fbdFolder
            // 
            this.fbdFolder.RootFolder = System.Environment.SpecialFolder.MyComputer;
            this.fbdFolder.ShowNewFolderButton = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(5, 7);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 17);
            this.label1.TabIndex = 6;
            this.label1.Text = "Customer Code";
            // 
            // txtCustomer
            // 
            this.txtCustomer.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCustomer.Location = new System.Drawing.Point(114, 4);
            this.txtCustomer.Margin = new System.Windows.Forms.Padding(2);
            this.txtCustomer.Name = "txtCustomer";
            this.txtCustomer.Size = new System.Drawing.Size(76, 23);
            this.txtCustomer.TabIndex = 2;
            this.txtCustomer.TextChanged += new System.EventHandler(this.BtnFilter_Click);
            this.txtCustomer.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.TxtFilter_PreviewKeyDown);
            // 
            // cbFilter
            // 
            this.cbFilter.AutoSize = true;
            this.cbFilter.Checked = true;
            this.cbFilter.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbFilter.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbFilter.Location = new System.Drawing.Point(353, 6);
            this.cbFilter.Margin = new System.Windows.Forms.Padding(2);
            this.cbFilter.Name = "cbFilter";
            this.cbFilter.Size = new System.Drawing.Size(112, 21);
            this.cbFilter.TabIndex = 3;
            this.cbFilter.Text = "Filter results?";
            this.cbFilter.UseVisualStyleBackColor = true;
            this.cbFilter.CheckedChanged += new System.EventHandler(this.BtnFilter_Click);
            // 
            // pnDates
            // 
            this.pnDates.Controls.Add(this.lvDates);
            this.pnDates.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnDates.Location = new System.Drawing.Point(0, 39);
            this.pnDates.Name = "pnDates";
            this.pnDates.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.pnDates.Size = new System.Drawing.Size(162, 275);
            this.pnDates.TabIndex = 7;
            // 
            // pnEntries
            // 
            this.pnEntries.Controls.Add(this.lvEntries);
            this.pnEntries.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnEntries.Location = new System.Drawing.Point(162, 39);
            this.pnEntries.Name = "pnEntries";
            this.pnEntries.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.pnEntries.Size = new System.Drawing.Size(459, 275);
            this.pnEntries.TabIndex = 8;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.txtOrder);
            this.panel1.Controls.Add(this.btnOpenLink);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.cbFilter);
            this.panel1.Controls.Add(this.txtCustomer);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(621, 39);
            this.panel1.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(198, 7);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 17);
            this.label2.TabIndex = 9;
            this.label2.Text = "Order No.";
            // 
            // txtOrder
            // 
            this.txtOrder.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOrder.Location = new System.Drawing.Point(273, 5);
            this.txtOrder.Margin = new System.Windows.Forms.Padding(2);
            this.txtOrder.Name = "txtOrder";
            this.txtOrder.Size = new System.Drawing.Size(76, 23);
            this.txtOrder.TabIndex = 8;
            this.txtOrder.TextChanged += new System.EventHandler(this.BtnFilter_Click);
            this.txtOrder.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.TxtFilter_PreviewKeyDown);
            // 
            // btnOpenLink
            // 
            this.btnOpenLink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenLink.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOpenLink.Location = new System.Drawing.Point(462, 3);
            this.btnOpenLink.Name = "btnOpenLink";
            this.btnOpenLink.Size = new System.Drawing.Size(137, 25);
            this.btnOpenLink.TabIndex = 7;
            this.btnOpenLink.Text = "Open in Purolator";
            this.btnOpenLink.UseVisualStyleBackColor = true;
            this.btnOpenLink.Click += new System.EventHandler(this.LvEntries_DoubleClick);
            // 
            // timerFTP
            // 
            this.timerFTP.Interval = 3600000;
            this.timerFTP.Tick += new System.EventHandler(this.BtnPick_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnEdit);
            this.panel2.Controls.Add(this.txtLocation);
            this.panel2.Controls.Add(this.btnPick);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 314);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(621, 30);
            this.panel2.TabIndex = 10;
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEdit.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEdit.Location = new System.Drawing.Point(527, 2);
            this.btnEdit.Margin = new System.Windows.Forms.Padding(2);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(92, 24);
            this.btnEdit.TabIndex = 2;
            this.btnEdit.Text = "Edit Entry";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.EditEntryToolStripMenuItem_Click);
            // 
            // ContextEntries
            // 
            this.ContextEntries.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addNewEntryToolStripMenuItem,
            this.editEntryToolStripMenuItem});
            this.ContextEntries.Name = "contextMenuStrip1";
            this.ContextEntries.Size = new System.Drawing.Size(154, 48);
            // 
            // addNewEntryToolStripMenuItem
            // 
            this.addNewEntryToolStripMenuItem.Name = "addNewEntryToolStripMenuItem";
            this.addNewEntryToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.addNewEntryToolStripMenuItem.Text = "Add New Entry";
            this.addNewEntryToolStripMenuItem.Click += new System.EventHandler(this.AddNewEntryToolStripMenuItem_Click);
            // 
            // editEntryToolStripMenuItem
            // 
            this.editEntryToolStripMenuItem.Name = "editEntryToolStripMenuItem";
            this.editEntryToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.editEntryToolStripMenuItem.Text = "Edit Entry";
            this.editEntryToolStripMenuItem.Click += new System.EventHandler(this.EditEntryToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(621, 344);
            this.Controls.Add(this.pnEntries);
            this.Controls.Add(this.pnDates);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(637, 310);
            this.Name = "Form1";
            this.Text = "Shiplog";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ContextDays.ResumeLayout(false);
            this.pnDates.ResumeLayout(false);
            this.pnEntries.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ContextEntries.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ListView lvEntries;
        private System.Windows.Forms.TextBox txtLocation;
        private System.Windows.Forms.Button btnPick;
        private System.Windows.Forms.FolderBrowserDialog fbdFolder;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtCustomer;
        private System.Windows.Forms.ListView lvDates;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.CheckBox cbFilter;
        private System.Windows.Forms.Panel pnDates;
        private System.Windows.Forms.Panel pnEntries;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnOpenLink;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtOrder;
        private System.Windows.Forms.Timer timerFTP;
        private System.ComponentModel.BackgroundWorker workerFTP;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.ContextMenuStrip ContextDays;
        private System.Windows.Forms.ToolStripMenuItem sortEntriesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openFileToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip ContextEntries;
        private System.Windows.Forms.ToolStripMenuItem addNewEntryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editEntryToolStripMenuItem;
    }
}

