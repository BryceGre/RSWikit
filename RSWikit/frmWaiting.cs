using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RSWikit
{
    public partial class frmWaiting : Form
    {
        public frmWaiting()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
        }

        public void setText(string text)
        {
            lblInfo.Text = text;
        }

        public void showHelp()
        {
            this.Height = 128;
            btnHelp.Visible = true;
            btnClose.Visible = true;
            picIcon.Visible = true;
            this.AllowDrop = true;
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, "Drag your RuneScape desktop icon (RS3 or OSRS) onto this window to get started.", "Help");
        }

        private void frmWaiting_DragDrop(object sender, DragEventArgs e)
        {
            frmMain.dragDrop(e);
        }

        private void frmWaiting_DragEnter(object sender, DragEventArgs e)
        {
            frmMain.dragEnter(e);
        }
    }
}
