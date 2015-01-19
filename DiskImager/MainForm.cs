/**
 *  DiskImager - a tool for writing / reading images on SD cards
 *
 *  Copyright 2015 by Roman Belkov <romanbelkov@gmail.com>
 *  Copyright 2013, 2014 by Alex J. Lennon <ajlennon@dynamicdevices.co.uk>
 *
 *  Licensed under GNU General Public License 3.0 or later. 
 *  Some rights reserved. See LICENSE, AUTHORS.
 *
 * @license GPL-3.0+ <http://www.gnu.org/licenses/gpl-3.0.en.html>
 */

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using DynamicDevices.DiskWriter.Detection;
using DynamicDevices.DiskWriter.Win32;
using Microsoft.Win32;
using System.Threading.Tasks;
using DynamicDevices.DiskWriter.Properties;

namespace DynamicDevices.DiskWriter
{
    public partial class MainForm : Form
    {
        #region Fields

        private readonly List<Disk> _disks = new List<Disk>();
        internal readonly List<IDiskAccess> DiskAccesses = new List<IDiskAccess>();

        private DriveDetector _watcher = new DriveDetector();
        private EnumCompressionType _eCompType;
        
        private CultureInfo CurrentLocale { get; set; }

        #endregion

        #region Constructor

        public MainForm()
        {
            InitializeComponent();

            checkBoxUseMBR.Checked = true;

            MessageBoxEx.Owner = Handle;

            toolStripStatusLabel1.Text = Resources.MainForm_MainForm_Initialised__Licensed_under_GPLv3__Use_at_own_risk_;

            saveFileDialog1.OverwritePrompt = false;
            saveFileDialog1.Filter = Resources.MainForm_MainForm_Image_Files__Choose;

            // Set version into title
            var version = Assembly.GetEntryAssembly().GetName().Version;
            Text += @" v" + version;

            // Set app icon (not working on Mono/Linux)
            if (Environment.OSVersion.Platform != PlatformID.Unix)
                Icon = Utility.GetAppIcon();

            PopulateDrives();
            if (checkedListBoxDrives.Items.Count > 0)
                EnableButtons();
            else
                DisableButtons(false);

            // Read registry values
            var key = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Dynamic Devices Ltd\\DiskImager");
            if (key != null)
            {
                var file = (string)key.GetValue("FileName", "");
                if (File.Exists(file))
                    textBoxFileName.Text = file;

                Globals.CompressionLevel = (int)key.GetValue("CompressionLevel", Globals.CompressionLevel);
                Globals.MaxBufferSize = (int)key.GetValue("MaxBufferSize", Globals.MaxBufferSize);

                key.Close();
            }
            
            // Detect insertions / removals
            _watcher.DeviceArrived += OnDriveArrived;
            _watcher.DeviceRemoved += OnDriveRemoved;
            StartListenForChanges();
        }

        #endregion

        public override sealed string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        #region Disk access event handlers


        /// <summary>
        /// Close the application
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e"></param>
        private void ButtonExitClick(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Select a file for read/write from/to removable media
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonChooseFileClick(object sender, EventArgs e)
        {
            ChooseFile();
        }

        /// <summary>
        /// Read from removable media to file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonReadClick(object sender, EventArgs e)
        {
            if (checkedListBoxDrives.CheckedItems.Count != 1)
            {
                MessageBox.Show(
                    Resources.MainForm_ButtonReadClick_You_can_read_from_only_one_drive_at_a_time, 
                    Resources.MainForm_ButtonReadClick_____WARNING____, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var drive = (string)checkedListBoxDrives.CheckedItems[0];

            ClearLayoutPanels();
            GetPathIfEmpty();

            DisableButtons(true);

            Task.Factory.StartNew(() =>
            {
                DiskAccesses.Clear();
                _disks.Clear();

                var diskAccess = NewDiskAccess();
                var disk = new Disk(diskAccess);

                SendProgressToUI(disk);

                var res = false;
                try
                {
                    res = disk.ReadDrive(drive, textBoxFileName.Text, _eCompType, checkBoxUseMBR.Checked);
                }
                catch (Exception ex)
                {
                    toolStripStatusLabel1.Text = ex.Message;
                }
                if (!res && !disk.IsCancelling)
                {
                    MessageBoxEx.Show(Resources.MainForm_ButtonReadClick_Problem_with_reading_from_disk_, Resources.MainForm_ButtonReadClick_Read_Error,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                EnableButtons();
            });
        }

        /// <summary>
        /// Write to removable media from file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonWriteClick(object sender, EventArgs e)
        {
            if (checkedListBoxDrives.CheckedItems.Count == 0)
                return;

            var drives = checkedListBoxDrives.CheckedItems.Cast<Object>().Select(d => d.ToString()).ToArray();

            if (drives.Any(d => d.ToUpper().StartsWith("C:")))
            {
                var dr =
                    MessageBox.Show(
                        Resources.MainForm_ButtonWriteClick_C__is_almost_certainly_your_main_HDD,
                        Resources.MainForm_ButtonReadClick_____WARNING____, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dr != DialogResult.Yes)
                    return;
            }

            ClearLayoutPanels();
            GetPathIfEmpty();

            DisableButtons(true);

            Task.Factory.StartNew(() =>
            {
                DiskAccesses.Clear();
                _disks.Clear();

                var tasks = drives.Select(drive => Task.Factory.StartNew(() =>
                {
                    var diskAccess = new Win32DiskAccess();
                    var disk = new Disk(diskAccess);

                    SendProgressToUI(disk);

                    DiskAccesses.Add(diskAccess);
                    _disks.Add(disk);

                    var res = false;
                    try
                    {
                        res = disk.WriteDrive(drive, textBoxFileName.Text, _eCompType);
                    }
                    catch (Exception ex)
                    {
                        toolStripStatusLabel1.Text = ex.Message;
                    }
                    if (!res && !disk.IsCancelling)
                    {
                        MessageBoxEx.Show(Resources.MainForm_ButtonWriteClick_Problem_writing_to_disk__Is_it_write_protected_, Resources.MainForm_ButtonWriteClick_Write_Error,
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                })).ToArray();

                Task.WaitAll(tasks);

                Invoke((MethodInvoker) EnableButtons);
            });
            
        }

        /// <summary>
        /// Called to persist registry values on closure so we can remember things like last file used
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainFormFormClosing(object sender, FormClosingEventArgs e)
        {
            var key = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Dynamic Devices Ltd\\DiskImager");
            if (key != null)
            {
                key.SetValue("FileName", textBoxFileName.Text);
                key.Close();
            }

            _watcher.DeviceArrived -= OnDriveArrived;
            _watcher.DeviceRemoved -= OnDriveRemoved;
            StopListenForChanges();
        }

        /// <summary>
        /// Cancels an ongoing read/write
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonCancelClick(object sender, EventArgs e)
        {
            foreach (var disk in _disks)
            {
                disk.IsCancelling = true;
            }
        }

        private void RadioButtonCompZipCheckedChanged(object sender, EventArgs e)
        {
            UpdateFileNameText();
        }

        private void RadioButtonCompTgzCheckedChanged(object sender, EventArgs e)
        {
            UpdateFileNameText();
        }

        private void RadioButtonCompGzCheckedChanged(object sender, EventArgs e)
        {
            UpdateFileNameText();
        }

        private void RadioButtonCompNoneCheckedChanged(object sender, EventArgs e)
        {
            UpdateFileNameText();
        }

        #endregion

        #region Implementation

        private void UpdateFileNameText()
        {
            
            var text = textBoxFileName.Text;

         
            text = text.Replace(".tar.gz", "");
            text = text.Replace(".tgz", "");
            text = text.Replace(".tar", "");
            text = text.Replace(".gzip", "");
            text = text.Replace(".gz", "");
            text = text.Replace(".zip", "");

            if (radioButtonCompNone.Checked)
            {
                textBoxFileName.Text = text;
            } else if(radioButtonCompZip.Checked)
            {
                text += ".zip";
                textBoxFileName.Text = text;                
            } else if(radioButtonCompTgz.Checked)
            {
                text += ".tgz";
                textBoxFileName.Text = text;
            }
            else if (radioButtonCompGz.Checked)
            {
                text += ".gz";
                textBoxFileName.Text = text;
            }
        }

        /// <summary>
        /// Select the file for read / write and setup defaults for whether we're using compression based on extension
        /// </summary>
        private void ChooseFile()
        {
            var dr = saveFileDialog1.ShowDialog();

            if (dr != DialogResult.OK)
                return;
            
            textBoxFileName.Text = saveFileDialog1.FileName;
                TextBoxFileNameTextChanged(this, null);
        }

        /// <summary>
        /// Shows on-going process in UI using created elements
        /// </summary>
        private void SendProgressToUI(Disk disk)
        {
            var pb = new ProgressBar { Size = new Size(flowLayoutPanelProgressBars.Width - 10, 10) };
            var lab = new Label { Size = new Size(flowLayoutPanelProgressLabels.Width - 10, 17) };

            Invoke((MethodInvoker)delegate
            {
                flowLayoutPanelProgressBars.Controls.Add(pb);
                flowLayoutPanelProgressLabels.Controls.Add(lab);
                disk.OnLogMsg += (o, message) => Invoke((MethodInvoker) delegate { lab.Text = message; });
                disk.OnProgress +=
                    (o, progressPercentage) => Invoke((MethodInvoker) delegate { pb.Value = progressPercentage; });
            });
        }

        /// <summary>
        /// Before writing / reading we should check that FileName is not empty
        /// </summary>
        private void GetPathIfEmpty()
        {
            if (string.IsNullOrEmpty(textBoxFileName.Text))
                ChooseFile();
        }

        /// <summary>
        /// Flushes all existing controls from layout panels
        /// </summary>
        private void ClearLayoutPanels()
        {
            flowLayoutPanelProgressBars.Controls.Clear();
            flowLayoutPanelProgressLabels.Controls.Clear();
        }

        /// <summary>
        /// Create disk object for media accesses
        /// </summary>>
        private static IDiskAccess NewDiskAccess()
        {
            return (Environment.OSVersion.Platform == PlatformID.Unix) ? new LinuxDiskAccess() as IDiskAccess : new Win32DiskAccess();
        }

        private void TextBoxFileNameTextChanged(object sender, EventArgs e)
        {
            if (textBoxFileName.Text.ToLower().EndsWith(".tar.gz") || textBoxFileName.Text.ToLower().EndsWith(".tgz"))
                radioButtonCompTgz.Checked = true;
            else if (textBoxFileName.Text.ToLower().EndsWith(".gz"))
                radioButtonCompGz.Checked = true;
            else if (textBoxFileName.Text.ToLower().EndsWith(".zip"))
                radioButtonCompZip.Checked = true;
            else if (textBoxFileName.Text.ToLower().EndsWith(".img") || textBoxFileName.Text.ToLower().EndsWith(".bin") || textBoxFileName.Text.ToLower().EndsWith(".sdcard"))
                radioButtonCompNone.Checked = true;

            if (radioButtonCompNone.Checked)
                _eCompType = EnumCompressionType.None;
            else if (radioButtonCompTgz.Checked)
                _eCompType = EnumCompressionType.Targzip;
            else if (radioButtonCompGz.Checked)
                _eCompType = EnumCompressionType.Gzip;
            else if (radioButtonCompZip.Checked)
                _eCompType = EnumCompressionType.Zip;
        }

        private void DisplayAllDrivesToolStripMenuItemCheckedChanged(object sender, EventArgs e)
        {
            PopulateDrives();
            if (checkedListBoxDrives.Items.Count > 0)
                EnableButtons();
            else
                DisableButtons(false);
        }

        /// <summary>
        /// Load in the drives
        /// </summary>
        private void PopulateDrives()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(PopulateDrives));
                return;
            }

            checkedListBoxDrives.Items.Clear();

            foreach (var drive in DriveInfo.GetDrives())
            {
                // Only display removable drives
                if (drive.DriveType == DriveType.Removable || displayAllDrivesToolStripMenuItem.Checked)
                {
                    checkedListBoxDrives.Items.Add(drive.Name.TrimEnd('\\'));
                }
            }

#if false
            //import the System.Management namespace at the top in your "using" statement.
            var searcher = new ManagementObjectSearcher(
                 "SELECT * FROM Win32_DiskDrive WHERE InterfaceType='USB'");
            foreach (var disk in searcher.Get())
            {
                var props = disk.Properties;
                foreach(var p in props)
                    Console.WriteLine(p.Name + " = " + p.Value);
            }
#endif

            //if (comboBoxDrives.Items.Count > 0)
            //    comboBoxDrives.SelectedIndex = 0;
        }

        /// <summary>
        /// Callback when removable media is inserted or removed, repopulates the drive list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WatcherEventArrived(object sender, EventArgs e)
        {
            if(InvokeRequired)
            {
                Invoke(new EventHandler(WatcherEventArrived));
                return;
            }

            PopulateDrives();

            if (checkedListBoxDrives.Items.Count > 0)
                EnableButtons();
            else
                DisableButtons(false);
        }

        /// <summary>
        /// Updates UI to disable buttons
        /// </summary>
        /// <param name="running">Whether read/write process is running</param>
        private void DisableButtons(bool running)
        {
            buttonRead.Enabled = false;
            buttonWrite.Enabled = false;
            buttonExit.Enabled = !running;
            buttonCancel.Enabled = running;
            checkedListBoxDrives.Enabled = false;
            textBoxFileName.Enabled = false;
            buttonChooseFile.Enabled = false;
            groupBoxCompression.Enabled = false;
            groupBoxTruncation.Enabled = false;
        }

        /// <summary>
        /// Updates UI to enable buttons
        /// </summary>
        private void EnableButtons()
        {
            buttonRead.Enabled = true;
            buttonWrite.Enabled = true;
            buttonExit.Enabled = true;
            buttonCancel.Enabled = false;
            checkedListBoxDrives.Enabled = true;
            textBoxFileName.Enabled = true;
            buttonChooseFile.Enabled = true;
            groupBoxCompression.Enabled = true;
            groupBoxTruncation.Enabled = true;
        }

        #endregion

        #region Disk Change Handling

        public bool StartListenForChanges()
        {
            _watcher.DeviceArrived += OnDriveArrived;
            _watcher.DeviceRemoved += OnDriveRemoved;
            return true;
        }

        public void StopListenForChanges()
        {
            if (_watcher != null)
            {
                _watcher.Dispose();
                _watcher = null;
            }
        }

        void OnDriveArrived(object sender, DriveDetectorEventArgs e)
        {
            WatcherEventArrived(sender, e);
        }

        void OnDriveRemoved(object sender, DriveDetectorEventArgs e)
        {
            WatcherEventArrived(sender, e);
        }

        #endregion

        private void englishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            ChangeLanguage("en-US");
        }

        private void russianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");
            ChangeLanguage("ru-RU");
        }

        //change the language in real time 
        private void ChangeLanguage(string lang)
        {
            CurrentLocale = new CultureInfo(lang);
            var resources = new ComponentResourceManager(typeof (MainForm));
            foreach (Control c in Controls)
            {
                resources.ApplyResources(c, c.Name, CurrentLocale);
                RefreshFormResources(c, resources);
            }
            
            ChangeMenuStripLanguage(menuStripMain.Items, resources);
            toolStripStatusLabel1.Text = Resources.MainForm_MainForm_Initialised__Licensed_under_GPLv3__Use_at_own_risk_;
            saveFileDialog1.Filter = Resources.MainForm_MainForm_Image_Files__Choose;
        }

        //refresh all menu strip controls recursively
        private void ChangeMenuStripLanguage(ToolStripItemCollection collection, ComponentResourceManager resources)
        {
            foreach (ToolStripItem m in collection)
            {
                resources.ApplyResources(m, m.Name, CurrentLocale);
                var item = m as ToolStripDropDownItem;
                if (item != null)
                    ChangeMenuStripLanguage(item.DropDownItems, resources);
            }
        }

        //refresh all the sub-controls of the form recursively
        private void RefreshFormResources(Control ctrl, ComponentResourceManager res)
        {
            ctrl.SuspendLayout();
            res.ApplyResources(ctrl, ctrl.Name, CurrentLocale);
            foreach (Control c in ctrl.Controls)
                RefreshFormResources(c, res); // recursion
            ctrl.ResumeLayout(false);
        }

    }
}
