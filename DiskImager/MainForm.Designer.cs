﻿namespace DynamicDevices.DiskWriter
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
            this.buttonRead = new System.Windows.Forms.Button();
            this.buttonWrite = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.labelDriveTitle = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
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
            this.label2 = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.statusStrip1.SuspendLayout();
            this.menuStripMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonRead
            // 
            resources.ApplyResources(this.buttonRead, "buttonRead");
            this.buttonRead.Name = "buttonRead";
            this.toolTip.SetToolTip(this.buttonRead, resources.GetString("buttonRead.ToolTip"));
            this.buttonRead.UseVisualStyleBackColor = true;
            this.buttonRead.Click += new System.EventHandler(this.ButtonReadClick);
            // 
            // buttonWrite
            // 
            resources.ApplyResources(this.buttonWrite, "buttonWrite");
            this.buttonWrite.Name = "buttonWrite";
            this.toolTip.SetToolTip(this.buttonWrite, resources.GetString("buttonWrite.ToolTip"));
            this.buttonWrite.UseVisualStyleBackColor = true;
            this.buttonWrite.Click += new System.EventHandler(this.ButtonWriteClick);
            // 
            // statusStrip1
            // 
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Name = "statusStrip1";
            this.toolTip.SetToolTip(this.statusStrip1, resources.GetString("statusStrip1.ToolTip"));
            // 
            // toolStripStatusLabel1
            // 
            resources.ApplyResources(this.toolStripStatusLabel1, "toolStripStatusLabel1");
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "img";
            resources.ApplyResources(this.saveFileDialog1, "saveFileDialog1");
            // 
            // labelDriveTitle
            // 
            resources.ApplyResources(this.labelDriveTitle, "labelDriveTitle");
            this.labelDriveTitle.Name = "labelDriveTitle";
            this.toolTip.SetToolTip(this.labelDriveTitle, resources.GetString("labelDriveTitle.ToolTip"));
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.Name = "buttonCancel";
            this.toolTip.SetToolTip(this.buttonCancel, resources.GetString("buttonCancel.ToolTip"));
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.ButtonCancelClick);
            // 
            // menuStripMain
            // 
            resources.ApplyResources(this.menuStripMain, "menuStripMain");
            this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.menuStripMain.Name = "menuStripMain";
            this.menuStripMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolTip.SetToolTip(this.menuStripMain, resources.GetString("menuStripMain.ToolTip"));
            // 
            // optionsToolStripMenuItem
            // 
            resources.ApplyResources(this.optionsToolStripMenuItem, "optionsToolStripMenuItem");
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.displayAllDrivesToolStripMenuItem,
            this.useMBRToolStripMenuItem,
            this.unmountDrivesToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            // 
            // displayAllDrivesToolStripMenuItem
            // 
            resources.ApplyResources(this.displayAllDrivesToolStripMenuItem, "displayAllDrivesToolStripMenuItem");
            this.displayAllDrivesToolStripMenuItem.CheckOnClick = true;
            this.displayAllDrivesToolStripMenuItem.Name = "displayAllDrivesToolStripMenuItem";
            this.displayAllDrivesToolStripMenuItem.CheckedChanged += new System.EventHandler(this.DisplayAllDrivesToolStripMenuItemCheckedChanged);
            // 
            // useMBRToolStripMenuItem
            // 
            resources.ApplyResources(this.useMBRToolStripMenuItem, "useMBRToolStripMenuItem");
            this.useMBRToolStripMenuItem.Checked = true;
            this.useMBRToolStripMenuItem.CheckOnClick = true;
            this.useMBRToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.useMBRToolStripMenuItem.Name = "useMBRToolStripMenuItem";
            // 
            // unmountDrivesToolStripMenuItem
            // 
            resources.ApplyResources(this.unmountDrivesToolStripMenuItem, "unmountDrivesToolStripMenuItem");
            this.unmountDrivesToolStripMenuItem.Checked = true;
            this.unmountDrivesToolStripMenuItem.CheckOnClick = true;
            this.unmountDrivesToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.unmountDrivesToolStripMenuItem.Name = "unmountDrivesToolStripMenuItem";
            // 
            // aboutToolStripMenuItem
            // 
            resources.ApplyResources(this.aboutToolStripMenuItem, "aboutToolStripMenuItem");
            this.aboutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.languageToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            // 
            // languageToolStripMenuItem
            // 
            resources.ApplyResources(this.languageToolStripMenuItem, "languageToolStripMenuItem");
            this.languageToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.englishToolStripMenuItem,
            this.russianToolStripMenuItem});
            this.languageToolStripMenuItem.Name = "languageToolStripMenuItem";
            // 
            // englishToolStripMenuItem
            // 
            resources.ApplyResources(this.englishToolStripMenuItem, "englishToolStripMenuItem");
            this.englishToolStripMenuItem.Name = "englishToolStripMenuItem";
            this.englishToolStripMenuItem.Click += new System.EventHandler(this.englishToolStripMenuItem_Click);
            // 
            // russianToolStripMenuItem
            // 
            resources.ApplyResources(this.russianToolStripMenuItem, "russianToolStripMenuItem");
            this.russianToolStripMenuItem.Name = "russianToolStripMenuItem";
            this.russianToolStripMenuItem.Click += new System.EventHandler(this.russianToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            resources.ApplyResources(this.helpToolStripMenuItem, "helpToolStripMenuItem");
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
            // 
            // checkedListBoxDrives
            // 
            resources.ApplyResources(this.checkedListBoxDrives, "checkedListBoxDrives");
            this.checkedListBoxDrives.FormattingEnabled = true;
            this.checkedListBoxDrives.Name = "checkedListBoxDrives";
            this.checkedListBoxDrives.Sorted = true;
            this.toolTip.SetToolTip(this.checkedListBoxDrives, resources.GetString("checkedListBoxDrives.ToolTip"));
            // 
            // flowLayoutPanelProgressBars
            // 
            resources.ApplyResources(this.flowLayoutPanelProgressBars, "flowLayoutPanelProgressBars");
            this.flowLayoutPanelProgressBars.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flowLayoutPanelProgressBars.Name = "flowLayoutPanelProgressBars";
            this.toolTip.SetToolTip(this.flowLayoutPanelProgressBars, resources.GetString("flowLayoutPanelProgressBars.ToolTip"));
            // 
            // flowLayoutPanelProgressLabels
            // 
            resources.ApplyResources(this.flowLayoutPanelProgressLabels, "flowLayoutPanelProgressLabels");
            this.flowLayoutPanelProgressLabels.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flowLayoutPanelProgressLabels.Name = "flowLayoutPanelProgressLabels";
            this.toolTip.SetToolTip(this.flowLayoutPanelProgressLabels, resources.GetString("flowLayoutPanelProgressLabels.ToolTip"));
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            this.toolTip.SetToolTip(this.label1, resources.GetString("label1.ToolTip"));
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 15000;
            this.toolTip.InitialDelay = 500;
            this.toolTip.ReshowDelay = 100;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            this.toolTip.SetToolTip(this.label2, resources.GetString("label2.ToolTip"));
            // 
            // openFileDialog1
            // 
            resources.ApplyResources(this.openFileDialog1, "openFileDialog1");
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.flowLayoutPanelProgressLabels);
            this.Controls.Add(this.flowLayoutPanelProgressBars);
            this.Controls.Add(this.checkedListBoxDrives);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.labelDriveTitle);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStripMain);
            this.Controls.Add(this.buttonWrite);
            this.Controls.Add(this.buttonRead);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MainMenuStrip = this.menuStripMain;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.toolTip.SetToolTip(this, resources.GetString("$this.ToolTip"));
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFormFormClosing);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStripMain.ResumeLayout(false);
            this.menuStripMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonRead;
        private System.Windows.Forms.Button buttonWrite;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Label labelDriveTitle;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.MenuStrip menuStripMain;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem displayAllDrivesToolStripMenuItem;
        private System.Windows.Forms.CheckedListBox checkedListBoxDrives;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelProgressBars;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelProgressLabels;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ToolStripMenuItem useMBRToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unmountDrivesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem languageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem englishToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem russianToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}

