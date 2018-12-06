using System;
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

        private void btnHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, "Click the icon to swap between RuneScape 3 and OldSchool RuneScape.", "Help");
        }

        private void picIcon_Click(object sender, EventArgs e)
        {
            frmMain.osrs = !frmMain.osrs;
            frmMain.saveConfig();
            Application.Restart();
        }
    }
}
