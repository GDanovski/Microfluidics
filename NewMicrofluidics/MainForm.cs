using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NewMicrofluidics
{
    public partial class MainForm : Form
    {
        private COMClient serialPort;
        private delegate void SetTextDeleg(string text);
        private Protocols protocols1;
        public MainForm()
        {
            InitializeComponent();
            this.protocols1 = new Protocols();
            this.protocols1.Dock = DockStyle.Fill;
            this.panel4.Controls.Add(this.protocols1);

            this.Load += MainForm_Load;            
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            this.New_toolStripButton.Click += newToolStripMenuItem_Click;
            this.Open_toolStripButton.Click += openToolStripMenuItem_Click;
            this.Save_toolStripButton.Click += Save_toolStripButton_Click;
            this.SaveAs_toolStripButton.Click += SaveAs_toolStripButton_Click;
            this.Delete_toolStripButton.Click += Delete_toolStripButton_Click;
            this.protocols1.OnProtocolChange += OnProtocolChanging;
            this.Load_toolStripButton.Click += Load_toolStripButton_Click;
            this.Run_toolStripButton.Click += ManualControlBtn_Click;
            this.Stop_toolStripButton.Click += ManualControlBtn_Click;
            this.button_Info.Click += Button_Info_CLick;

            CopyrightsLabel.Alignment = ToolStripItemAlignment.Right;
            this.button_COM_Connect.Click += button_COM_Connect_Click;

            this.btn_Drain_Open.Click += ManualControlBtn_Click;
            this.btn_Drain_Closed.Click += ManualControlBtn_Click;
            this.btn_Valve1_Open.Click += ManualControlBtn_Click;
            this.btn_Valve1_Closed.Click += ManualControlBtn_Click;
            this.btn_Valve2_Open.Click += ManualControlBtn_Click;
            this.btn_Valve2_Closed.Click += ManualControlBtn_Click;
            this.btn_Valve3_Open.Click += ManualControlBtn_Click;
            this.btn_Valve3_Closed.Click += ManualControlBtn_Click;
            this.btn_Valve4_Open.Click += ManualControlBtn_Click;
            this.btn_Valve4_Closed.Click += ManualControlBtn_Click;
            this.btn_Valve5_Open.Click += ManualControlBtn_Click;
            this.btn_Valve5_Closed.Click += ManualControlBtn_Click;
            this.btn_Valve6_Open.Click += ManualControlBtn_Click;
            this.btn_Valve6_Closed.Click += ManualControlBtn_Click;
            this.btn_Valve7_Open.Click += ManualControlBtn_Click;
            this.btn_Valve7_Closed.Click += ManualControlBtn_Click;
            this.btn_Valve8_Open.Click += ManualControlBtn_Click;
            this.btn_Valve8_Closed.Click += ManualControlBtn_Click;

            int COMPort = Properties.Settings.Default.COMPort;
            this.textBox_COMPort.Text = COMPort.ToString();
            try
            {
                this.serialPort = new COMClient("COM" + COMPort);
                this.serialPort.OnDataRecieved += COMport_OnDataRecieved;
                CommandToLog("Set COM port to: COM" + COMPort);
                this.StatusLabel.Text = "Connected to COM" + COMPort;
            }
            catch
            {
                this.StatusLabel.Text = "Not connected";
            }
        }
        private void OnProtocolChanging(object sender, EventArgs e)
        {
            if (protocols1.SelectedProtocolDir != null)
            {
                try
                {
                    richTextBox_Console.Text = System.IO.File.ReadAllText(protocols1.SelectedProtocolDir);
                    CommandToLog("-------------------------------------");

                    CommandToLog("File opened: " + System.IO.Path.GetFileName(protocols1.SelectedProtocolDir));
                }
                catch
                {
                    CommandToLog("!!! File reading error: " + System.IO.Path.GetFileName(protocols1.SelectedProtocolDir));
                }
            }
        }
        private void CommandToLog(string str)
        {
            richTextBox_Log.AppendText(str + "\n");
            // set the current caret position to the end
            richTextBox_Log.SelectionStart = richTextBox_Log.Text.Length;
            // scroll it automatically
            richTextBox_Log.ScrollToCaret();
        }
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (NewProtocolForm dialog = new NewProtocolForm())
            {
                var result = dialog.ShowDialog();
                if (result == DialogResult.OK && dialog.GetName != "")
                {
                    string myPath = protocols1.Path + "\\" + dialog.GetName;
                    if (!myPath.EndsWith(".txt")) myPath += ".txt";

                    if (System.IO.File.Exists(myPath))
                    {
                        MessageBox.Show("Protocol with that name is already existing!");
                        return;
                    }
                    else
                    {
                        protocols1.SelectedProtocolDir = myPath;
                        System.IO.File.WriteAllText(myPath, "");
                    }
                }
                dialog.Dispose();
            }
        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = @"D:\",
                Title = "Browse Text Files",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "txt",
                Filter = "txt files (*.txt)|*.txt",
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string inputFile = openFileDialog1.FileName;
                System.IO.File.Copy(inputFile, protocols1.Path + "\\" + System.IO.Path.GetFileName(inputFile));
            }
        }

        private void Delete_toolStripButton_Click(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(protocols1.SelectedProtocolDir))
            {
                try
                {
                    System.IO.File.Delete(protocols1.SelectedProtocolDir);
                    CommandToLog("Protocol deleted");
                }
                catch
                {
                    CommandToLog("!!! Protocol deleting error");
                }
            }
        }

        private void Save_toolStripButton_Click(object sender, EventArgs e)
        {
            string dir = protocols1.SelectedProtocolDir;
            if (dir != null)
                try
                {
                    System.IO.File.WriteAllText(dir, richTextBox_Console.Text);
                    CommandToLog("Protocol saved");
                }
                catch
                {
                    CommandToLog("!!! Protocol saving error");
                }
        }

        private void SaveAs_toolStripButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog openFileDialog1 = new SaveFileDialog
            {
                InitialDirectory = @"D:\",
                Title = "Browse Text Files",

                DefaultExt = "txt",
                Filter = "txt files (*.txt)|*.txt",
                FilterIndex = 2,
                RestoreDirectory = true
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string inputFile = openFileDialog1.FileName;
                try
                {
                    System.IO.File.WriteAllText(inputFile, richTextBox_Console.Text);
                    CommandToLog("Protocol saved to: " + inputFile);
                }
                catch
                {
                    CommandToLog("!!! Protocol saving error: " + inputFile);
                }
            }
        }
        private void Load_toolStripButton_Click(object sender, EventArgs e)
        {
            if (serialPort != null)
            {
                string cmd = richTextBox_Console.Text;
                string error = serialPort.SentData(cmd,true);

                if (error != null)
                    CommandToLog(error);
            }
            else
            {
                CommandToLog("!!! COM port not found");
                this.StatusLabel.Text = "Not connected";
            }
        }
        private void ManualControlBtn_Click(object sender, EventArgs e)
        {
            if (serialPort != null)
            {
                string cmd = "";
                if(sender is Button)
                    cmd = (string)((Button)sender).Tag;
                else if (sender is ToolStripButton)
                    cmd = (string)((ToolStripButton)sender).Tag;

                string error = serialPort.SentData(cmd);

                if (error != null)
                    CommandToLog(error);
            }
            else
            {
                CommandToLog("!!! COM port not found");
                this.StatusLabel.Text = "Not connected";
            }
        }
        private void Button_Info_CLick(object sender, EventArgs e)
        {
            string[] ports = COMClient.GetPortNames();
            CommandToLog("Avaliable ports: " + string.Join(", ", ports));
        }

        private void button_COM_Connect_Click(object sender, EventArgs e)
        {
            if (this.serialPort != null)
            {
                this.serialPort.Dispose();
                this.serialPort = null;
            }

            int COMPort = -1;
            if (!int.TryParse(this.textBox_COMPort.Text, out COMPort))
            {
                MessageBox.Show("The value must be integer!");
            }
            if (COMPort != -1)
            {
                try
                {
                    this.serialPort = new COMClient("COM" + COMPort);
                    this.serialPort.OnDataRecieved += COMport_OnDataRecieved;

                    Properties.Settings.Default.COMPort = COMPort;
                    Properties.Settings.Default.Save();
                    CommandToLog("Set COM port to: COM" + COMPort);
                    this.StatusLabel.Text = "Connected to COM" + COMPort;
                }
                catch
                {
                    MessageBox.Show("Wrong port name!");
                    this.StatusLabel.Text = "Not connected";
                }
            }
        }
        private void COMport_OnDataRecieved(object sender, EventArgs e)
        {
            if (this.serialPort != null)
            {
                string cmd = this.serialPort.GetRecievedData;
                this.BeginInvoke(new SetTextDeleg(si_DataReceived), new object[] { cmd });
            }
        }
        private void si_DataReceived(string data)
        {
            CommandToLog("{" + string.Format("{0:HH:mm:ss:fff}", DateTime.Now) + "} " + COMClient.TranslateFromESP(data.Trim()));
        }
        
    }
}
