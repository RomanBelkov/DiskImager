namespace DynamicDevices.DiskWriter
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.textBoxFileName = new System.Windows.Forms.TextBox();
            this.buttonRead = new System.Windows.Forms.Button();
            this.buttonWrite = new System.Windows.Forms.Button();
            this.buttonChooseFile = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.labelFileName = new System.Windows.Forms.Label();
            this.labelDriveTitle = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBoxCompression = new System.Windows.Forms.GroupBox();
            this.radioButtonCompXZ = new System.Windows.Forms.RadioButton();
            this.radioButtonCompNone = new System.Windows.Forms.RadioButton();
            this.radioButtonCompTgz = new System.Windows.Forms.RadioButton();
            this.radioButtonCompGz = new System.Windows.Forms.RadioButton();
            this.radioButtonCompZip = new System.Windows.Forms.RadioButton();
            this.menuStripMain = new System.Windows.Forms.MenuStrip();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.displayAllDrivesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.useMBRToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unmountDrivesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.languageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.englishToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.russianToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkedListBoxDrives = new System.Windows.Forms.CheckedListBox();
            this.flowLayoutPanelProgressBars = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanelProgressLabels = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.statusStrip1.SuspendLayout();
            this.groupBoxCompression.SuspendLayout();
            this.menuStripMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxFileName
            // 
            resources.ApplyResources(this.textBoxFileName, "textBoxFileName");
            this.textBoxFileName.Name = "textBoxFileName";
            this.textBoxFileName.TextChanged += new System.EventHandler(this.TextBoxFileNameTextChanged);
            // 
            // buttonRead
            // 
            resources.ApplyResources(this.buttonRead, "buttonRead");
            this.buttonRead.Name = "buttonRead";
            this.buttonRead.UseVisualStyleBackColor = true;
            this.buttonRead.Click += new System.EventHandler(this.ButtonReadClick);
            // 
            // buttonWrite
            // 
            resources.ApplyResources(this.buttonWrite, "buttonWrite");
            this.buttonWrite.Name = "buttonWrite";
            this.buttonWrite.UseVisualStyleBackColor = true;
            this.buttonWrite.Click += new System.EventHandler(this.ButtonWriteClick);
            // 
            // buttonChooseFile
            // 
            resources.ApplyResources(this.buttonChooseFile, "buttonChooseFile");
            this.buttonChooseFile.Name = "buttonChooseFile";
            this.toolTip.SetToolTip(this.buttonChooseFile, resources.GetString("buttonChooseFile.ToolTip"));
            this.buttonChooseFile.UseVisualStyleBackColor = true;
            this.buttonChooseFile.Click += new System.EventHandler(this.ButtonChooseFileClick);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Name = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            resources.ApplyResources(this.toolStripStatusLabel1, "toolStripStatusLabel1");
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.AddExtension = false;
            this.saveFileDialog1.DefaultExt = "img";
            resources.ApplyResources(this.saveFileDialog1, "saveFileDialog1");
            // 
            // labelFileName
            // 
            resources.ApplyResources(this.labelFileName, "labelFileName");
            this.labelFileName.Name = "labelFileName";
            // 
            // labelDriveTitle
            // 
            resources.ApplyResources(this.labelDriveTitle, "labelDriveTitle");
            this.labelDriveTitle.Name = "labelDriveTitle";
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.ButtonCancelClick);
            // 
            // groupBoxCompression
            // 
            this.groupBoxCompression.Controls.Add(this.radioButtonCompXZ);
            this.groupBoxCompression.Controls.Add(this.radioButtonCompNone);
            this.groupBoxCompression.Controls.Add(this.radioButtonCompTgz);
            this.groupBoxCompression.Controls.Add(this.radioButtonCompGz);
            this.groupBoxCompression.Controls.Add(this.radioButtonCompZip);
            resources.ApplyResources(this.groupBoxCompression, "groupBoxCompression");
            this.groupBoxCompression.Name = "groupBoxCompression";
            this.groupBoxCompression.TabStop = false;
            this.toolTip.SetToolTip(this.groupBoxCompression, resources.GetString("groupBoxCompression.ToolTip"));
            // 
            // radioButtonCompXZ
            // 
            resources.ApplyResources(this.radioButtonCompXZ, "radioButtonCompXZ");
            this.radioButtonCompXZ.Name = "radioButtonCompXZ";
            this.radioButtonCompXZ.TabStop = true;
            this.radioButtonCompXZ.UseVisualStyleBackColor = true;
            this.radioButtonCompXZ.CheckedChanged += new System.EventHandler(this.radioButtonCompXZ_CheckedChanged);
            // 
            // radioButtonCompNone
            // 
            resources.ApplyResources(this.radioButtonCompNone, "radioButtonCompNone");
            this.radioButtonCompNone.Checked = true;
            this.radioButtonCompNone.Name = "radioButtonCompNone";
            this.radioButtonCompNone.TabStop = true;
            this.radioButtonCompNone.UseVisualStyleBackColor = true;
            this.radioButtonCompNone.CheckedChanged += new System.EventHandler(this.RadioButtonCompNoneCheckedChanged);
            // 
            // radioButtonCompTgz
            // 
            resources.ApplyResources(this.radioButtonCompTgz, "radioButtonCompTgz");
            this.radioButtonCompTgz.Name = "radioButtonCompTgz";
            this.radioButtonCompTgz.UseVisualStyleBackColor = true;
            this.radioButtonCompTgz.CheckedChanged += new System.EventHandler(this.RadioButtonCompTgzCheckedChanged);
            // 
            // radioButtonCompGz
            // 
            resources.ApplyResources(this.radioButtonCompGz, "radioButtonCompGz");
            this.radioButtonCompGz.Name = "radioButtonCompGz";
            this.radioButtonCompGz.UseVisualStyleBackColor = true;
            this.radioButtonCompGz.CheckedChanged += new System.EventHandler(this.RadioButtonCompGzCheckedChanged);
            // 
            // radioButtonCompZip
            // 
            resources.ApplyResources(this.radioButtonCompZip, "radioButtonCompZip");
            this.radioButtonCompZip.Name = "radioButtonCompZip";
            this.radioButtonCompZip.UseVisualStyleBackColor = true;
            this.radioButtonCompZip.CheckedChanged += new System.EventHandler(this.RadioButtonCompZipCheckedChanged);
            // 
            // menuStripMain
            // 
            this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem,
            this.aboutToolStripMenuItem});
            resources.ApplyResources(this.menuStripMain, "menuStripMain");
            this.menuStripMain.Name = "menuStripMain";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.displayAllDrivesToolStripMenuItem,
            this.useMBRToolStripMenuItem,
            this.unmountDrivesToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            resources.ApplyResources(this.optionsToolStripMenuItem, "optionsToolStripMenuItem");
            // 
            // displayAllDrivesToolStripMenuItem
            // 
            this.displayAllDrivesToolStripMenuItem.CheckOnClick = true;
            this.displayAllDrivesToolStripMenuItem.Name = "displayAllDrivesToolStripMenuItem";
            resources.ApplyResources(this.displayAllDrivesToolStripMenuItem, "displayAllDrivesToolStripMenuItem");
            this.displayAllDrivesToolStripMenuItem.CheckedChanged += new System.EventHandler(this.DisplayAllDrivesToolStripMenuItemCheckedChanged);
            // 
            // useMBRToolStripMenuItem
            // 
            this.useMBRToolStripMenuItem.Checked = true;
            this.useMBRToolStripMenuItem.CheckOnClick = true;
            this.useMBRToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.useMBRToolStripMenuItem.Name = "useMBRToolStripMenuItem";
            resources.ApplyResources(this.useMBRToolStripMenuItem, "useMBRToolStripMenuItem");
            // 
            // unmountDrivesToolStripMenuItem
            // 
            this.unmountDrivesToolStripMenuItem.Checked = true;
            this.unmountDrivesToolStripMenuItem.CheckOnClick = true;
            this.unmountDrivesToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.unmountDrivesToolStripMenuItem.Name = "unmountDrivesToolStripMenuItem";
            resources.ApplyResources(this.unmountDrivesToolStripMenuItem, "unmountDrivesToolStripMenuItem");
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.languageToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            resources.ApplyResources(this.aboutToolStripMenuItem, "aboutToolStripMenuItem");
            // 
            // languageToolStripMenuItem
            // 
            this.languageToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.englishToolStripMenuItem,
            this.russianToolStripMenuItem});
            this.languageToolStripMenuItem.Name = "languageToolStripMenuItem";
            resources.ApplyResources(this.languageToolStripMenuItem, "languageToolStripMenuItem");
            // 
            // englishToolStripMenuItem
            // 
            this.englishToolStripMenuItem.Name = "englishToolStripMenuItem";
            resources.ApplyResources(this.englishToolStripMenuItem, "englishToolStripMenuItem");
            this.englishToolStripMenuItem.Click += new System.EventHandler(this.englishToolStripMenuItem_Click);
            // 
            // russianToolStripMenuItem
            // 
            this.russianToolStripMenuItem.Name = "russianToolStripMenuItem";
            resources.ApplyResources(this.russianToolStripMenuItem, "russianToolStripMenuItem");
            this.russianToolStripMenuItem.Click += new System.EventHandler(this.russianToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            resources.ApplyResources(this.helpToolStripMenuItem, "helpToolStripMenuItem");
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
            // 
            // checkedListBoxDrives
            // 
            this.checkedListBoxDrives.FormattingEnabled = true;
            resources.ApplyResources(this.checkedListBoxDrives, "checkedListBoxDrives");
            this.checkedListBoxDrives.Name = "checkedListBoxDrives";
            this.checkedListBoxDrives.Sorted = true;
            this.toolTip.SetToolTip(this.checkedListBoxDrives, resources.GetString("checkedListBoxDrives.ToolTip"));
            // 
            // flowLayoutPanelProgressBars
            // 
            this.flowLayoutPanelProgressBars.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.flowLayoutPanelProgressBars, "flowLayoutPanelProgressBars");
            this.flowLayoutPanelProgressBars.Name = "flowLayoutPanelProgressBars";
            // 
            // flowLayoutPanelProgressLabels
            // 
            this.flowLayoutPanelProgressLabels.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.flowLayoutPanelProgressLabels, "flowLayoutPanelProgressLabels");
            this.flowLayoutPanelProgressLabels.Name = "flowLayoutPanelProgressLabels";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 15000;
            this.toolTip.InitialDelay = 500;
            this.toolTip.ReshowDelay = 100;
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.flowLayoutPanelProgressLabels);
            this.Controls.Add(this.flowLayoutPanelProgressBars);
            this.Controls.Add(this.checkedListBoxDrives);
            this.Controls.Add(this.groupBoxCompression);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.labelDriveTitle);
            this.Controls.Add(this.labelFileName);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStripMain);
            this.Controls.Add(this.buttonChooseFile);
            this.Controls.Add(this.buttonWrite);
            this.Controls.Add(this.buttonRead);
            this.Controls.Add(this.textBoxFileName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MainMenuStrip = this.menuStripMain;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFormFormClosing);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBoxCompression.ResumeLayout(false);
            this.groupBoxCompression.PerformLayout();
            this.menuStripMain.ResumeLayout(false);
            this.menuStripMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxFileName;
        private System.Windows.Forms.Button buttonRead;
        private System.Windows.Forms.Button buttonWrite;
        private System.Windows.Forms.Button buttonChooseFile;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Label labelFileName;
        private System.Windows.Forms.Label labelDriveTitle;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.GroupBox groupBoxCompression;
        private System.Windows.Forms.RadioButton radioButtonCompNone;
        private System.Windows.Forms.RadioButton radioButtonCompTgz;
        private System.Windows.Forms.RadioButton radioButtonCompGz;
        private System.Windows.Forms.RadioButton radioButtonCompZip;
        private System.Windows.Forms.MenuStrip menuStripMain;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem displayAllDrivesToolStripMenuItem;
        private System.Windows.Forms.CheckedListBox checkedListBoxDrives;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelProgressBars;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelProgressLabels;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton radioButtonCompXZ;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ToolStripMenuItem useMBRToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unmountDrivesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem languageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem englishToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem russianToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
    }
}

