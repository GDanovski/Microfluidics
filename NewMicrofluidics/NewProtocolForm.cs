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
    public partial class NewProtocolForm : Form
    {
        public NewProtocolForm()
        {
            InitializeComponent();
        }

        private void Create_button_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Hide();
        }
        public string GetName
        {
            get
            {
                return textBox1.Text;
            }
        }
    }
}
