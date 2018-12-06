using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace RSWikit
{
    public partial class frmTools : Form
    {
        /****************/
        /***** Vars *****/
        /****************/

        //skip saving notes text if true
        bool skipSave = false;
        //store seconds since last GE update
        int GESeconds = 0;
        //tenths of seconds on stopwatch
        int SWSeconds = 0;
        int SWLastLap = 0;
        int CCount = 0;

        /****************/
        /***** Func *****/
        /****************/
        public frmTools()
        {
            //initialize form components
            InitializeComponent();
            //try to load the saved notes text
            try
            {
                skipSave = true; //it's redundant to save loaded text
                StreamReader file = new StreamReader("notes.txt");
                txtNotes.Text = file.ReadToEnd();
                file.Close();
            }
            catch (IOException ex)
            {
                //file does not exist, load no text
                txtNotes.Text = "";
            }

            //retrieve the GE last update time and start the GE timers
            getTime();
            tmrGE.Start(); //every second, tick time forward 1 second
            tmrUpdate.Start(); //every 5 minutes, retrieve a new GE time
        }

        private void txtNotes_TextChanged(object sender, EventArgs e)
        {
            //if the new text was entered by the user (and not loaded from file)
            if (skipSave == false)
            {
                //try to save the notes text
                try
                {
                    System.IO.StreamWriter file = new System.IO.StreamWriter("notes.txt");
                    file.WriteLine(txtNotes.Text);
                    file.Close();
                }
                catch (IOException ex)
                {
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.YesNo);
                }
                
            }
            skipSave = false;
        }

        private void getTime()
        {
            //get the time since the last GE update
            String time = "";
            //currently using rscript.org
            using (var client = new System.Net.WebClient())
            {
                //load the script data
                time = client.DownloadString("http://www.rscript.org/lookup.php?type=geupdate");
            }
            //if script still exists
            if (time != "")
            {
                //get a substring containing seconds since last update
                int start = time.IndexOf("UPDATEAGO:") + 11;
                int length = time.IndexOf("\n", start) - start;
                time = time.Substring(start, length);

                //parse substring into int
                GESeconds = int.Parse(time);
                //update time display
                updateTime();
            }
        }

        private void tmrGE_Tick(object sender, EventArgs e)
        {
            //timer ticks every second, so add one to seconds since last update
            GESeconds++;
            //update time display
            updateTime();
        }

        private void tmrUpdate_Tick(object sender, EventArgs e)
        {
            //retrieve the GE last update time
            getTime();
        }

        private void updateTime()
        {
            //determine the days, hours, minutes, and seconds since last GE update
            int days = (int)Math.Floor((double)GESeconds / (60 * 60 * 24));
            int daysRemainder = GESeconds % (60 * 60 * 24);
            int hours = (int)Math.Floor((double)daysRemainder / (60 * 60));
            int minutesRemainder = daysRemainder % (60 * 60);
            int minutes = (int)Math.Floor((double)minutesRemainder / 60);
            int seconds = minutesRemainder % 60;
            //update the label with the new info
            lblGETime.Text = days + " days, " + hours + " hours, " + minutes + " mins, " + seconds + " secs.";
        }

        private void frmTools_SizeChanged(object sender, EventArgs e)
        {
            int top = (int)(ClientSize.Height * 0.02);
            int left = (int)(ClientSize.Width * 0.1);
            int height = (int)(ClientSize.Height * 0.025);
            int width = (int)(ClientSize.Width * 0.8);

            //GE time
            lblGE.Top = top;
            lblGE.Height = height;
            lblGE.Left = left;
            lblGE.Width = width;

            top += height;

            lblGETime.Top = top;
            lblGETime.Height = height;
            lblGETime.Left = left;
            lblGETime.Width = width;

            //Horizontal Rule
            height = (int)(ClientSize.Height * 0.02);
            top += height;

            lblHr1.Top = top + (int)(ClientSize.Height * 0.02);
            lblHr1.Height = 2;
            lblHr1.Left = 0;
            lblHr1.Width = ClientSize.Width;

            //Size/Pos of Calc text
            height = (int)(ClientSize.Height * 0.05);
            width = (int)(ClientSize.Width * 0.60);
            top += height;
            left = (int)(ClientSize.Width * 0.2);

            //Calc Text
            txtCalc.Top = top;
            txtCalc.Height = height;
            txtCalc.Left = left;
            txtCalc.Width = width;

            //Size of Calc buttons
            height = (int)(ClientSize.Height * 0.05);
            width = (int)(ClientSize.Width * 0.12);

            //First row of Calc buttons
            top += height;
            left = (int)(ClientSize.Width * 0.2);

            calcMS.Top = top;
            calcMS.Height = height;
            calcMS.Left = left;
            calcMS.Width = width;

            left += width;

            calcMR.Top = top;
            calcMR.Height = height;
            calcMR.Left = left;
            calcMR.Width = width;

            left += width;

            calcC.Top = top;
            calcC.Height = height;
            calcC.Left = left;
            calcC.Width = width * 2;

            left += width * 2;

            calcCE.Top = top;
            calcCE.Height = height;
            calcCE.Left = left;
            calcCE.Width = width;

            //Second row of Calc buttons
            top += height;
            left = (int)(ClientSize.Width * 0.2);

            calc7.Top = top;
            calc7.Height = height;
            calc7.Left = left;
            calc7.Width = width;

            left += width;

            calc8.Top = top;
            calc8.Height = height;
            calc8.Left = left;
            calc8.Width = width;

            left += width;

            calc9.Top = top;
            calc9.Height = height;
            calc9.Left = left;
            calc9.Width = width;

            left += width;

            calcDiv.Top = top;
            calcDiv.Height = height;
            calcDiv.Left = left;
            calcDiv.Width = width;

            left += width;

            calcK.Top = top;
            calcK.Height = height;
            calcK.Left = left;
            calcK.Width = width;

            //Third row of Calc buttons
            top += height;
            left = (int)(ClientSize.Width * 0.2);

            calc4.Top = top;
            calc4.Height = height;
            calc4.Left = left;
            calc4.Width = width;

            left += width;

            calc5.Top = top;
            calc5.Height = height;
            calc5.Left = left;
            calc5.Width = width;


            left += width;

            calc6.Top = top;
            calc6.Height = height;
            calc6.Left = left;
            calc6.Width = width;

            left += width;

            calcMul.Top = top;
            calcMul.Height = height;
            calcMul.Left = left;
            calcMul.Width = width;

            left += width;

            calcM.Top = top;
            calcM.Height = height;
            calcM.Left = left;
            calcM.Width = width;

            //Fourth row of Calc buttons
            top += height;
            left = (int)(ClientSize.Width * 0.2);

            calc1.Top = top;
            calc1.Height = height;
            calc1.Left = left;
            calc1.Width = width;

            left += width;

            calc2.Top = top;
            calc2.Height = height;
            calc2.Left = left;
            calc2.Width = width;

            left += width;

            calc3.Top = top;
            calc3.Height = height;
            calc3.Left = left;
            calc3.Width = width;

            left += width;

            calcSub.Top = top;
            calcSub.Height = height;
            calcSub.Left = left;
            calcSub.Width = width;

            left += width;

            calcEqu.Top = top;
            calcEqu.Height = height * 2;
            calcEqu.Left = left;
            calcEqu.Width = width;

            //Fifth row of Calc buttons
            top += height;
            left = (int)(ClientSize.Width * 0.2);

            calc0.Top = top;
            calc0.Height = height;
            calc0.Left = left;
            calc0.Width = width * 2;

            left += width * 2;

            calcDot.Top = top;
            calcDot.Height = height;
            calcDot.Left = left;
            calcDot.Width = width;

            left += width;

            calcAdd.Top = top;
            calcAdd.Height = height;
            calcAdd.Left = left;
            calcAdd.Width = width;


            //Horizontal Rule
            height = (int)(ClientSize.Height * 0.04);
            top += height;

            lblHr2.Top = top + (int)(ClientSize.Height * 0.02);
            lblHr2.Height = 2;
            lblHr2.Left = 0;
            lblHr2.Width = ClientSize.Width;

            //Stopwatch Size
            top += height;
            left = (int)(ClientSize.Width * 0.1);
            width = (int)(ClientSize.Width * 0.2);
            height = (int)(ClientSize.Height * 0.08);

            //Stopwatch buttons
            btnStart.Top = top;
            btnStart.Height = height;
            btnStart.Left = left;
            btnStart.Width = width;

            left += width;
            
            btnStop.Top = top;
            btnStop.Height = height;
            btnStop.Left = left;
            btnStop.Width = width;

            left += width;

            int oldLeft = left;

            //Stopwatch Time size
            width = (int)(ClientSize.Width * 0.2);
            height = (int)(ClientSize.Height * 0.04);

            //Stopwatch Time
            txtTime.Top = top;
            txtTime.Height = height;
            txtTime.Left = left;
            txtTime.Width = width;

            left += width;

            //Counter Size
            width = (int)(ClientSize.Width * 0.05);
            height = (int)(ClientSize.Height * 0.04);

            //Vertical Rule
            lblVr1.Top = top;
            lblVr1.Height = height;
            lblVr1.Left = left + (int)(ClientSize.Width * 0.02);
            lblVr1.Width = 2;

            left += width;

            //Coutner Buttons
            btnCountDown.Top = top;
            btnCountDown.Height = height;
            btnCountDown.Left = left;
            btnCountDown.Width = width;

            left += width;

            txtCount.Top = top;
            txtCount.Height = height;
            txtCount.Left = left;
            txtCount.Width = width;

            left += width;

            btnCountUp.Top = top;
            btnCountUp.Height = height;
            btnCountUp.Left = left;
            btnCountUp.Width = width;

            //Lap Size
            width = (int)(ClientSize.Width * 0.3);
            height = (int)(ClientSize.Height * 0.04);
            top += height;
            left = oldLeft;

            //Lap Text
            txtLap.Top = top;
            txtLap.Height = height;
            txtLap.Left = left;
            txtLap.Width = width;

            left += width;
            width = (int)(ClientSize.Width * 0.05);

            //Vertical Rule
            lblVr2.Top = top;
            lblVr2.Height = height;
            lblVr2.Left = left + (int)(ClientSize.Width * 0.02);
            lblVr2.Width = 2;

            //Counter Buttons
            left += width;

            btnCountZero.Top = top;
            btnCountZero.Height = height;
            btnCountZero.Left = left;
            btnCountZero.Width = width;

            top += height;

            //Horizontal Rule
            lblHr3.Top = top + (int)(ClientSize.Height * 0.02);
            lblHr3.Height = 2;
            lblHr3.Left = 0;
            lblHr3.Width = ClientSize.Width;

            top += height;
            left = (int)(ClientSize.Width * 0.1);
            width = (int)(ClientSize.Width * 0.5);
            height = (int)(ClientSize.Height * 0.04);

            chkCalc.Top = top;
            chkCalc.Height = height;
            chkCalc.Left = left;
            chkCalc.Width = width;

            left += width;
            width = (int)(ClientSize.Width * 0.3);

            chkLap.Top = top;
            chkLap.Height = height;
            chkLap.Left = left;
            chkLap.Width = width;

            top += height;

            txtNotes.Top = top;
            txtNotes.Height = ClientSize.Height - top;
            txtNotes.Left = (int)(ClientSize.Width * 0.1);
            txtNotes.Width = (int)(ClientSize.Width * 0.8);

            txtLap.Font = new Font(txtLap.Font.FontFamily, 12f, txtLap.Font.Style);
            while (TextRenderer.MeasureText(txtLap.Text, txtLap.Font).Width > txtLap.Width)
            {
                txtLap.Font = new Font(txtLap.Font.FontFamily, txtLap.Font.Size - 0.1f, txtLap.Font.Style);
            }
        }

        /**********************/
        /***** Calculator *****/
        /**********************/

        private String c1 = "0";
        private String c2 = "0";
        private String c0 = "0";
        private String cOp = "+";
        private String cMem = "0";
        private bool cReset = true;
        private char[] cMods = { 'K', 'M' };

        private void calcNumber(int num)
        {
            if (cReset == true)
            {
                c1 = num.ToString();
                c0 = "0";
                cReset = false;
            }
            else
            {
                appendC1(num.ToString());
            }
            txtCalc.Text = c1;
        }

        private void calcDot_Click(object sender, EventArgs e)
        {
            if (cReset == true)
            {
                c1 = "0.";
                cReset = false;
            } else {
                if (!c1.Contains("."))
                    appendC1(".");
            }
            txtCalc.Text = c1;
        }

        private void calcOp(String op)
        {
            if (cReset == true)
                c2 = "0";
            else
            {
                calculate();
            }
            cOp = op;
            c2 = c1;
            cReset = true;
        }

        private void calcMod(String mod)
        {
            c1 = c1.TrimEnd(cMods);
            c1 = c1 + mod;
            txtCalc.Text = c1;
        }

        private void calcC_Click(object sender, EventArgs e)
        {
            c1 = "0";
            c2 = "0";
            c0 = "0";
            cOp = "+";
            cReset = true;
            txtCalc.Text = c1;
        }

        private void calcCE_Click(object sender, EventArgs e)
        {
            c1 = "0";
            cReset = true;
            txtCalc.Text = c1;
        }
        
        private void calcMS_Click(object sender, EventArgs e)
        {
            cMem = c1;
            if (chkCalc.Checked)
                txtNotes.Text += "\r\n" + cMem;
            cReset = true;
        }

        private void calcMR_Click(object sender, EventArgs e)
        {
            c1 = cMem;
            txtCalc.Text = c1;
        }

        private void calculate()
        {

            if (c0 == "0")
                c0 = c1;
            int mod1 = 1;
            if (c0.EndsWith("K"))
                mod1 = 1000;
            else if (c0.EndsWith("M"))
                mod1 = 1000000;
            int mod2 = 1;
            if (c2.EndsWith("K"))
                mod2 = 1000;
            else if (c2.EndsWith("M"))
                mod2 = 1000000;

            double n1 = double.Parse(c2.TrimEnd(cMods), System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
            double n2 = double.Parse(c0.TrimEnd(cMods), System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
            switch (cOp)
            {
                case "+":
                    c1 = ((n1 * mod2) + (n2 * mod1)).ToString();
                    break;
                case "-":
                    c1 = ((n1 * mod2) - (n2 * mod1)).ToString();
                    break;
                case "*":
                    c1 = ((n1 * mod2) * (n2 * mod1)).ToString();
                    break;
                case "/":
                    c1 = ((n1 * mod2) / (n2 * mod1)).ToString();
                    break;
            }
            
            if (c1 != "0")
            {
                double nX = double.Parse(c1, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                if (nX % 100000 == 0 && nX >= 1000000)
                {
                    c1 = (nX / 1000000).ToString() + "M";
                }
                else if (nX % 100 == 0 && nX >= 1000)
                {
                    c1 = (nX / 1000).ToString() + "K";
                }
            }

            txtCalc.Text = c1;
            c2 = c1;
            cReset = true;
        }

        private void appendC1(string append)
        {
            foreach (char c in cMods)
            {
                if (c1.EndsWith(c.ToString()))
                {
                    c1 = c1.Remove(c1.Length - 1, 1);
                    c1 = c1 + append;
                    c1 = c1 + c.ToString();
                    return;
                }
            }
            c1 = c1 + append;
        }

        private void calc0_Click(object sender, EventArgs e)
        {
            calcNumber(0);
        }

        private void calc1_Click(object sender, EventArgs e)
        {
            calcNumber(1);
        }

        private void calc2_Click(object sender, EventArgs e)
        {
            calcNumber(2);
        }

        private void calc3_Click(object sender, EventArgs e)
        {
            calcNumber(3);
        }

        private void calc4_Click(object sender, EventArgs e)
        {
            calcNumber(4);
        }

        private void calc5_Click(object sender, EventArgs e)
        {
            calcNumber(5);
        }

        private void calc6_Click(object sender, EventArgs e)
        {
            calcNumber(6);
        }

        private void calc7_Click(object sender, EventArgs e)
        {
            calcNumber(7);
        }

        private void calc8_Click(object sender, EventArgs e)
        {
            calcNumber(8);
        }

        private void calc9_Click(object sender, EventArgs e)
        {
            calcNumber(9);
        }

        private void calcK_Click(object sender, EventArgs e)
        {
            calcMod("K");
        }

        private void calcM_Click(object sender, EventArgs e)
        {
            calcMod("M");
        }

        private void calcDiv_Click(object sender, EventArgs e)
        {
            calcOp("/");
        }

        private void calcMul_Click(object sender, EventArgs e)
        {
            calcOp("*");
        }

        private void calcSub_Click(object sender, EventArgs e)
        {
            calcOp("-");
        }

        private void calcAdd_Click(object sender, EventArgs e)
        {
            calcOp("+");
        }

        private void calcEqu_Click(object sender, EventArgs e)
        {
            calculate();
        }

        private void tmrStopWatch_Tick(object sender, EventArgs e)
        {
            SWSeconds++;
            txtTime.Text = formatSW(SWSeconds);
        }

        private string last2(String str)
        {
            return str.Substring(str.Length - 2);
        }

        private string formatSW(int swSec)
        {
            int tenths = swSec % 10;
            int seconds = (int)Math.Floor(swSec / 10.0) % 60;
            int minutes = (int)Math.Floor(swSec / 60.0 / 10.0) % 60;
            int hours = (int)Math.Floor(swSec / 60.0 / 60.0 / 10.0);

            return hours + ":" + last2("0" + minutes) + ":" + last2("0" + seconds) + "." + tenths;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (tmrTime.Enabled)
            {
                tmrTime.Enabled = false;
            }
            else
            {
                tmrTime.Enabled = true;
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {

            if (tmrTime.Enabled)
            {
                txtLap.Text = txtTime.Text + " (" + formatSW(SWSeconds - SWLastLap) + ")";
                if (chkLap.Checked)
                    txtNotes.Text += "\r\n" + txtLap.Text;
                SWLastLap = SWSeconds;
            }
            else
            {
                SWSeconds = 0;
                txtTime.Text = "0:00:00.0";
                SWLastLap = 0;
                txtLap.Text = "0:00:00.0 (0:00:00.0)";
            }
        }

        private void btnCountZero_Click(object sender, EventArgs e)
        {
            CCount = 0;
            txtCount.Text = CCount.ToString();
        }

        private void btnCountDown_Click(object sender, EventArgs e)
        {
            CCount--;
            txtCount.Text = CCount.ToString();
        }

        private void btnCountUp_Click(object sender, EventArgs e)
        {
            CCount++;
            txtCount.Text = CCount.ToString();
        }

        private void btnEnter(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            btn.BackgroundImage = Properties.Resources.btn_highlight;
        }

        private void btnLeave(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            btn.BackgroundImage = Properties.Resources.btn_normal;
        }

        /**********************/
        /*****  End Calc  *****/
        /**********************/
    }
}
