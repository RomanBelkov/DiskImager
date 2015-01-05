using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using DynamicDevices.DiskWriter.Detection;
using DynamicDevices.DiskWriter.Win32;
using Microsoft.Win32;
using System.Threading.Tasks;

namespace DynamicDevices.DiskWriter
{
    public partial class MainForm : Form
    {
        #region Fields

        private readonly List<Disk> _disks = new List<Disk>();
        internal readonly List<IDiskAccess> DiskAccesses = new List<IDiskAccess>();

        private  DriveDetector _watcher = new DriveDetector();

        private EnumCompressionType _eCompType;

        #endregion

        #region Constructor

        public MainForm()
        {
            InitializeComponent();

            checkBoxUseMBR.Checked = true;

            MessageBoxEx.Owner = Handle;

            toolStripStatusLabel1.Text = @"Initialised. Licensed under GPLv3. Use at own risk!";

            saveFileDialog1.OverwritePrompt = false;
            saveFileDialog1.Filter = @"Image Files (*.img,*.bin,*.sdcard)|*.img;*.bin;*.sdcard|Compressed Files (*.zip,*.gz,*tgz)|*.zip;*.gz;*.tgz|All files (*.*)|*.*";

            // Set version into title
            var version = Assembly.GetEntryAssembly().GetName().Version;
            Text += @" v" + version;

            // Set app icon (not working on Mono/Linux)
            if (Environment.OSVersion.Platform != PlatformID.Unix)
                Icon = Utility.GetAppIcon();

            PopulateDrives();
            if (checkedListBox1.Items.Count > 0)
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
            if (checkedListBox1.CheckedItems.Count != 1)
            {
                MessageBox.Show(
                    @"You can read from only one drive at a time. Please select only one drive from drive list.", 
                    @"*** WARNING ***", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var drive = (string)checkedListBox1.CheckedItems[0];

            ClearLayoutPanels();
            GetPathIfEmpty();

            DisableButtons(true);

            Task.Factory.StartNew(() =>
            {
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
                    MessageBoxEx.Show("Problem with reading from disk.", "Read Error",
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
            if (checkedListBox1.CheckedItems.Count == 0)
                return;

            var drives = checkedListBox1.CheckedItems.Cast<Object>().Select(d => d.ToString()).ToArray();

            if (drives.Any(d => d.ToUpper().StartsWith("C:")))
            {
                var dr =
                    MessageBox.Show(
                        @"C: is almost certainly your main hard drive. Writing to this will likely destroy your data, and brick your PC. Are you absolutely sure you want to do this?",
                        @"*** WARNING ***", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
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
                        MessageBoxEx.Show("Problem writing to disk. Is it write-protected?", "Write Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                })).ToArray();

                Task.WaitAll(tasks);

                EnableButtons();
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
            var pb = new ProgressBar { Size = new Size(flowLayoutPanel1.Width - 10, 10) };
            var lab = new Label { Size = new Size(flowLayoutPanel2.Width - 10, 17) };

            Invoke((MethodInvoker)delegate
            {
                flowLayoutPanel1.Controls.Add(pb);
                flowLayoutPanel2.Controls.Add(lab);
                disk.OnLogMsg += (o, message) => lab.Text = message;
                disk.OnProgress += (o, progressPercentage) => pb.Value = progressPercentage;
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
            flowLayoutPanel1.Controls.Clear();
            flowLayoutPanel2.Controls.Clear();
        }

        /// <summary>
        /// Create disk object for media accesses
        /// </summary>>
        private IDiskAccess NewDiskAccess()
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
            if (checkedListBox1.Items.Count > 0)
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

            checkedListBox1.Items.Clear();

            foreach (var drive in DriveInfo.GetDrives())
            {
                // Only display removable drives
                if (drive.DriveType == DriveType.Removable || displayAllDrivesToolStripMenuItem.Checked)
                {
                    checkedListBox1.Items.Add(drive.Name.TrimEnd('\\'));
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

            if (checkedListBox1.Items.Count > 0)
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
            checkedListBox1.Enabled = false;
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
            checkedListBox1.Enabled = true;
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
    }
}
