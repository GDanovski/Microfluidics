using System;
using System.Windows.Forms;
using System.IO;

namespace NewMicrofluidics
{
    public partial class Protocols : UserControl
    {
        public event EventHandler OnProtocolChange;
        private TreeNode mySelectedNode;
        private FileSystemWatcher watcher;
        private string _Path;
        private string _SelectedProtocolDir;
        public Protocols()
        {
            InitializeComponent();

            this.Load += MyControl_Load;
        }
        public override string Text
        {
            set
            {
                label1.Text = value;
            }
            get
            {
                return label1.Text;
            }
        }
        public string Path
        {
            set
            {
                this._Path = value;
            }
            get
            {
                return this._Path;
            }
        }
        public string SelectedProtocolDir
        {
            set
            {
                if (this._SelectedProtocolDir != value)
                {
                    this._SelectedProtocolDir = value;
                    OnProtocolChange(this, new EventArgs());
                }
            }
            get
            {
                return this._SelectedProtocolDir;
            }
        }
        private void MyControl_Load(object sender, EventArgs e)
        {
            this.Path = Application.StartupPath + "\\Protocols";
            treeView1.HideSelection = false;
            treeView1.MouseDown += treeView1_MouseDown;
            treeView1.NodeMouseClick += menuItem1_Click;
            treeView1.AfterLabelEdit += treeView1_AfterLabelEdit;

            TreeView_LoadData();

            this.watcher = new FileSystemWatcher();
            watcher.Path = this.Path;
            watcher.SynchronizingObject = this;
            // Watch for changes in LastAccess and LastWrite times, and
            // the renaming of files or directories.
            watcher.NotifyFilter = NotifyFilters.LastAccess
                                   | NotifyFilters.LastWrite
                                   | NotifyFilters.FileName
                                   | NotifyFilters.DirectoryName;

            // Only watch text files.
            watcher.Filter = "*.txt";

            // Add event handlers.
            watcher.Changed += OnChanged;
            watcher.Created += OnChanged;
            watcher.Deleted += OnChanged;
            watcher.Renamed += OnChanged;

            // Begin watching.
            watcher.EnableRaisingEvents = true;
        }
        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            //Send to the main thred
            TreeView_LoadData();
        }
        private void TreeView_LoadData()
        {
            treeView1.SuspendLayout();
            treeView1.Nodes.Clear();

            foreach (string dir in Directory.GetFiles(this.Path, "*.txt"))
            {
                TreeNode node = new TreeNode();
                node.Tag = dir;
                node.Text = System.IO.Path.GetFileNameWithoutExtension(dir);
                treeView1.Nodes.Add(node);
            }

            if (this.SelectedProtocolDir != null && treeView1.Nodes.Count > 0)
            {
                if (File.Exists(this.SelectedProtocolDir))
                    foreach (TreeNode node in treeView1.Nodes)
                        if ((string)node.Tag == this.SelectedProtocolDir)
                            treeView1.SelectedNode = node;

            }
            if (this.SelectedProtocolDir == null && treeView1.Nodes.Count > 0)
            {
                treeView1.SelectedNode = treeView1.Nodes[0];
            }
            else
            {
                this.mySelectedNode = null;
            }

            if (mySelectedNode != null)
                this.SelectedProtocolDir = (string)mySelectedNode.Tag;
            else
                this.SelectedProtocolDir = null;

            treeView1.ResumeLayout(true);
        }
        /* Get the tree node under the mouse pointer and 
   save it in the mySelectedNode variable. */
        private void treeView1_MouseDown(object sender,
          System.Windows.Forms.MouseEventArgs e)
        {
            mySelectedNode = treeView1.GetNodeAt(e.X, e.Y);

            if (mySelectedNode != null)
                this.SelectedProtocolDir = (string)mySelectedNode.Tag;
        }

        private void menuItem1_Click(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && mySelectedNode != null)
            {
                treeView1.SelectedNode = mySelectedNode;
                treeView1.LabelEdit = true;
                if (!mySelectedNode.IsEditing)
                {
                    mySelectedNode.BeginEdit();
                }
            }
        }

        private void treeView1_AfterLabelEdit(object sender,
                 System.Windows.Forms.NodeLabelEditEventArgs e)
        {
            if (e.Label != null)
            {
                if (e.Label.Length > 0)
                {
                    string protocolsPath = System.IO.Path.GetDirectoryName((string)mySelectedNode.Tag) + "\\";
                    string newPath = protocolsPath + e.Label;
                    if (!newPath.EndsWith(".txt")) newPath += ".txt";
                    if (File.Exists(newPath))
                    {
                        /* Cancel the label edit action, inform the user, and 
                           place the node in edit mode again. */
                        e.CancelEdit = true;
                        MessageBox.Show("Protocol with that name is existing!");
                        e.Node.BeginEdit();
                    }
                    else if (e.Label.IndexOfAny(new char[] { '@', '.', ',', '!' }) == -1)
                    {
                        // Stop editing without canceling the label change.
                        e.Node.EndEdit(false);
                        this.SelectedProtocolDir = newPath;
                        File.Move((string)mySelectedNode.Tag, newPath);
                        //mySelectedNode.Tag = newPath;
                    }
                    else
                    {
                        /* Cancel the label edit action, inform the user, and 
                           place the node in edit mode again. */
                        e.CancelEdit = true;
                        MessageBox.Show("Invalid tree node label.\n" +
                           "The invalid characters are: '@','.', ',', '!'",
                           "Node Label Edit");
                        e.Node.BeginEdit();
                    }
                }
                else
                {
                    /* Cancel the label edit action, inform the user, and 
                       place the node in edit mode again. */
                    e.CancelEdit = true;
                    MessageBox.Show("Invalid tree node label.\nThe label cannot be blank",
                       "Node Label Edit");
                    e.Node.BeginEdit();
                }
            }
        }
    }
}
