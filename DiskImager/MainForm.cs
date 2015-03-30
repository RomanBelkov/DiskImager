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
using System.Diagnostics;
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

            CreateRegistry();
            ChangeLanguage("en-US");

            useMBRToolStripMenuItem.Checked  = true;
            unmountDrivesToolStripMenuItem.Checked = true;

            //MessageBoxEx.Owner = Handle;

            toolStripStatusLabel1.Text = Resources.MainForm_MainForm_Initialised__Licensed_under_GPLv3__Use_at_own_risk_;

            saveFileDialog1.OverwritePrompt = false;
            saveFileDialog1.Filter = Resources.MainForm_MainForm_Image_Files__Choose;

            // Set version into title
            var version = Assembly.GetEntryAssembly().GetName().Version;
            Text += @" v" + version;

            PopulateDrives();
            if (checkedListBoxDrives.Items.Count > 0)
                EnableButtons();
            else
                DisableButtons(false);

            ReadRegistry();

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
            if (GetPathIfEmpty() == false)
                return;

            DisableButtons(true);

            Task.Factory.StartNew(() =>
            {
                DiskAccesses.Clear();
                _disks.Clear();

                var diskAccess = NewDiskAccess();
                var disk = new Disk(diskAccess);

                Thread.CurrentThread.CurrentUICulture = CurrentLocale;
                SendProgressToUI(disk);

                DiskAccesses.Add(diskAccess);
                _disks.Add(disk);

                var res = false;
                try
                {
                    res = disk.ReadDrive(drive, textBoxFileName.Text, _eCompType, unmountDrivesToolStripMenuItem.Checked);
                }
                catch (Exception ex)
                {
                    Invoke(new Action(() => MessageBox.Show(ex.Message, @"Exception at ReadDrive", MessageBoxButtons.OK, MessageBoxIcon.Error)));
                }

                if (!res && !disk.IsCancelling)
                {
                    Invoke(new Action ( () => MessageBox.Show(Resources.MainForm_ButtonReadClick_Problem_with_reading_from_disk_, Resources.MainForm_ButtonReadClick_Read_Error,
                        MessageBoxButtons.OK, MessageBoxIcon.Error)));
                }

                Invoke((MethodInvoker) EnableButtons);
                if (res)
                    Invoke((MethodInvoker) EndInfo);
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
            if (GetPathIfEmpty() == false) 
                return;

            if (!File.Exists(textBoxFileName.Text))
            {
                MessageBox.Show(Resources.MainForm_ButtonWriteClick_File_does_not_exist_, Resources.MainForm_ButtonWriteClick_I_O_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DisableButtons(true);

            Task.Factory.StartNew(() =>
            {
                DiskAccesses.Clear();
                _disks.Clear();

                var tasks = drives.Select(drive => Task.Factory.StartNew(() =>
                {
                    var diskAccess = NewDiskAccess();
                    var disk = new Disk(diskAccess);

                    Thread.CurrentThread.CurrentUICulture = CurrentLocale;
                    SendProgressToUI(disk);

                    DiskAccesses.Add(diskAccess);
                    _disks.Add(disk);

                    var res = false;
                    try
                    {
                        res = disk.WriteDrive(drive, textBoxFileName.Text, _eCompType, unmountDrivesToolStripMenuItem.Checked);
                    }
                    catch (Exception ex)
                    {
                        Invoke(new Action( () => MessageBox.Show(ex.Message, @"Exception at WriteDrive", MessageBoxButtons.OK, MessageBoxIcon.Error)));
                    }

                    if (!res && !disk.IsCancelling)
                    {
                        Invoke(new Action ( () => MessageBox.Show(Resources.MainForm_ButtonWriteClick_Problem_writing_to_disk__Is_it_write_protected_, Resources.MainForm_ButtonWriteClick_Write_Error,
                            MessageBoxButtons.OK, MessageBoxIcon.Error)));
                    }

                })).ToArray();

                Task.WaitAll(tasks);
                Invoke((MethodInvoker)EndInfo);
                Invoke((MethodInvoker)EnableButtons);
            });
        }

        private static void EndInfo()
        {
            System.Media.SystemSounds.Beep.Play();
            MessageBox.Show(Resources.MainForm_ButtonWriteClick_All_drives_are_ready, Resources.MainForm_ButtonWriteClick_All_done, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Called to persist registry values on closure so we can remember things like last file used
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainFormFormClosing(object sender, FormClosingEventArgs e)
        {
            const string registryPath = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Dynamic Devices Ltd\\DiskImager";

            Registry.SetValue(registryPath, "FileName", textBoxFileName.Text);
            Registry.SetValue(registryPath, "Language", CurrentLocale.Name);

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

        private void radioButtonCompXZ_CheckedChanged(object sender, EventArgs e)
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
            text = text.Replace(".xz", "");

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
            } else if (radioButtonCompGz.Checked)
            {
                text += ".gz";
                textBoxFileName.Text = text;
            } else if (radioButtonCompXZ.Checked)
            {
                text += ".xz";
                textBoxFileName.Text = text;
            }
        }

        /// <summary>
        /// Select the file for read / write and setup defaults for whether we're using compression based on extension
        /// </summary>
        private bool ChooseFile()
        {
            var dr = saveFileDialog1.ShowDialog();

            if (dr != DialogResult.OK)
                return false;
            
            textBoxFileName.Text = saveFileDialog1.FileName;
            TextBoxFileNameTextChanged(this, null);
            return true;
        }

        /// <summary>
        /// Before writing / reading we should check that FileName is not empty
        /// </summary>
        private bool GetPathIfEmpty()
        {
            return !string.IsNullOrEmpty(textBoxFileName.Text) || ChooseFile();
        }

        /// <summary>
        /// Shows on-going process in UI using created elements
        /// </summary>
        private void SendProgressToUI(Disk disk)
        {
            Invoke((MethodInvoker)delegate
            {
                var progressBar = new ProgressBar { Size = new Size(flowLayoutPanelProgressBars.Width - 10, 10) };
                var label = new Label { Size = new Size(flowLayoutPanelProgressLabels.Width - 10, 17) };
                flowLayoutPanelProgressBars.Controls.Add(progressBar);
                flowLayoutPanelProgressLabels.Controls.Add(label);

                disk.OnLogMsg += (o, message) => Invoke((MethodInvoker) delegate { label.Text = message; });
                disk.OnProgress +=
                    (o, progressPercentage) => Invoke((MethodInvoker) delegate { progressBar.Value = progressPercentage; });
            });
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
            else if (textBoxFileName.Text.ToLower().EndsWith(".xz"))
                radioButtonCompXZ.Checked = true;

            if (radioButtonCompNone.Checked)
                _eCompType = EnumCompressionType.None;
            else if (radioButtonCompTgz.Checked)
                _eCompType = EnumCompressionType.Targzip;
            else if (radioButtonCompGz.Checked)
                _eCompType = EnumCompressionType.Gzip;
            else if (radioButtonCompZip.Checked)
                _eCompType = EnumCompressionType.Zip;
            else if (radioButtonCompXZ.Checked)
                _eCompType = EnumCompressionType.XZ;
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
            buttonCancel.Enabled = running;
            checkedListBoxDrives.Enabled = false;
            textBoxFileName.Enabled = false;
            buttonChooseFile.Enabled = false;
            groupBoxCompression.Enabled = false;
            menuStripMain.Enabled = !running;
        }

        /// <summary>
        /// Updates UI to enable buttons
        /// </summary>
        private void EnableButtons()
        {
            buttonRead.Enabled = true;
            buttonWrite.Enabled = true;
            buttonCancel.Enabled = false;
            checkedListBoxDrives.Enabled = true;
            textBoxFileName.Enabled = true;
            buttonChooseFile.Enabled = true;
            groupBoxCompression.Enabled = true;
            menuStripMain.Enabled = true;
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

        #region Localization

        private void englishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeLanguage("en-US");
        }

        private void russianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeLanguage("ru-RU");
        }

        //change the language in real time 
        private void ChangeLanguage(string lang)
        {
            CurrentLocale = new CultureInfo(lang);
            Thread.CurrentThread.CurrentUICulture = CurrentLocale;

            var resources = new ComponentResourceManager(typeof (MainForm));
            foreach (Control c in Controls)
            {
                resources.ApplyResources(c, c.Name, CurrentLocale);
                RefreshFormResources(c, resources);
            }
            
            ChangeMenuStripLanguage(menuStripMain.Items, resources);
            ChangeToolTipLanguage(resources);

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

        private void ChangeToolTipLanguage(ComponentResourceManager resources)
        {
            toolTip.SetToolTip(buttonChooseFile, resources.GetString("buttonChooseFile.ToolTip"));
            toolTip.SetToolTip(groupBoxCompression, resources.GetString("groupBoxCompression.ToolTip"));
            toolTip.SetToolTip(checkedListBoxDrives, resources.GetString("checkedListBoxDrives.ToolTip"));
            //toolTip.SetToolTip(checkBoxUnmount, resources.GetString("checkBoxUnmount.ToolTip"));
            //toolTip.SetToolTip(checkBoxUseMBR, resources.GetString("checkBoxUseMBR.ToolTip"));
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

        #endregion

        #region Registry

        private const string RegistryPath = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Dynamic Devices Ltd\\DiskImager";

        private static void CreateRegistry()
        {
            if ((string) Registry.GetValue(RegistryPath, "Language", "") != null) return;
            Registry.SetValue(RegistryPath, "FileName", "");
            Registry.SetValue(RegistryPath, "Language", "en-US");
        }

        private void ReadRegistry()
        {
            var file = (string)Registry.GetValue(RegistryPath, "FileName", "");
            if (File.Exists(file))
                textBoxFileName.Text = file;

            var lang = (string)Registry.GetValue(RegistryPath, "Language", "en-US");
            if (lang != "en-US")
            {
                ChangeLanguage(lang);
            }
        }

        #endregion

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/RomanBelkov/DiskImager/wiki");
        }

    }
}
