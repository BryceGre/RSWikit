using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using CefSharp;
using System.IO;
using System.Threading;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using CefSharp.WinForms.Internals;
using System.Text;

namespace RSWikit
{
    public partial class frmMain : Form
    {
        /****************/
        /***** Vars *****/
        /****************/
        public const string rs3_url = "rs-launch://www.runescape.com/jav_config.ws";
        public const string ors_url = "jagex-jav://oldschool.runescape.com/jav_config.ws";

        static Mutex mutex = new Mutex(true, "{F44E7DB8-2E07-4C53-8261-5965C935AD2E}");
        //width of sidebar
        public static int width = 500;
        //use old-school runescape wiki
        public static bool osrs = false;
        //start in fullscreen
        public static bool full = false;
        //list of open tabs
        public static List<string> tabs = new List<string>();

        bool resizing;
        FormWindowState lastWindowState = FormWindowState.Minimized;
        Rectangle lastBounds = Rectangle.Empty;

        //pointer to JagexClient window
        IntPtr clientWin;
        int clientID;

        /****************/
        /***** DLLs *****/
        /****************/

        /***** user32.dll *****/
        //used to start JagexClient and dock it into RS Wikit

        //methods
        [DllImport("user32.dll", EntryPoint = "GetWindowThreadProcessId", SetLastError = true,
             CharSet = CharSet.Unicode, ExactSpelling = true,
             CallingConvention = CallingConvention.StdCall)]
        private static extern long GetWindowThreadProcessId(long hWnd, long lpdwProcessId);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern long SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongA", SetLastError = true)]
        private static extern long GetWindowLong(IntPtr hwnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongA", SetLastError = true)]
        private static extern long SetWindowLong(IntPtr hwnd, int nIndex, long dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern long SetWindowPos(IntPtr hwnd, long hWndInsertAfter, long x, long y, long cx, long cy, long wFlags);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr hwnd, int x, int y, int cx, int cy, bool repaint);

        [DllImport("user32.dll", EntryPoint = "PostMessageA", SetLastError = true)]
        private static extern bool PostMessage(IntPtr hwnd, uint Msg, long wParam, long lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        //constants
        private const int SWP_NOOWNERZORDER = 0x200;
        private const int SWP_NOREDRAW = 0x8;
        private const int SWP_NOZORDER = 0x4;
        private const int SWP_SHOWWINDOW = 0x0040;
        private const int WS_EX_MDICHILD = 0x40;
        private const int SWP_FRAMECHANGED = 0x20;
        private const int SWP_NOACTIVATE = 0x10;
        private const int SWP_ASYNCWINDOWPOS = 0x4000;
        private const int SWP_NOMOVE = 0x2;
        private const int SWP_NOSIZE = 0x1;
        private const int GWL_STYLE = (-16);
        private const int WS_VISIBLE = 0x10000000;
        private const int WM_CLOSE = 0x10;
        private const UInt32 WS_CHILD = 0x40000000;
        private const UInt32 WS_POPUP = 0x80000000;

        /****************/
        /***** Func *****/
        /****************/
        public frmMain()
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                //initialize form components
                InitializeComponent();
                InitializeChromium();
                //try to load the saved settings
                loadConfig();
                //start JagexLauncher and dock it to RS Wikit window
                DockApp();
                //add tools page to sidebar
                MdiTabControl.TabPage tools = tabSide.TabPages.Add(new frmTools());
                tools.Icon = new Icon("ico/tools.ico"); //tools page icon
                tools.CloseButtonVisible = false; //disallow closing tools page

                //Resize game client
                UpdateSize();
                FinishSize();

                //Do fullscreen
                if (full)
                    setFullscreen(true);
            }
            else
            {
                MessageBox.Show("Multiple instances are not supported!");
                Environment.Exit(Environment.ExitCode);
            }
        }

        private void DockApp()
        {
            //start client handle at 0
            clientWin = IntPtr.Zero;

            //start JagexClient
            Process p = null;
            try
            {
                //stop any existing process that is not docked
                Process[] pr = Process.GetProcessesByName(osrs ? "JagexLauncher" : "rs2client");
                for (int i = 0; i < pr.Length; i++)
                {
                    pr[i].Kill();
                }

                //start the process
                Process.Start(osrs ? ors_url : rs3_url);

                int timeout = 10000;

                //now find the process
                while (p == null) {
                    //sleep
                    Thread.Sleep(100);

                    //handle timeout
                    timeout -= 100;
                    if (timeout <= 0)
                    {
                        //search for client process timed out
                        Console.WriteLine("Process not found");
                        //create the waiting form, which allows the user to switch between RS3 and OSRS
                        frmWaiting waiting = new frmWaiting();
                        //release "single instance" mutex
                        mutex.Close();
                        //show the waiting dialog
                        waiting.ShowDialog();
                        //stop here
                        return;
                    }

                    //check for the process
                    Process[] ps = Process.GetProcessesByName(osrs ? "JagexLauncher" : "rs2client");
                    for (int i = 0; i < ps.Length; i++) {
                        //if the process hasn't exited
                        if (!ps[i].HasExited) {
                            //save the process
                            p = ps[i];
                            break;
                        }
                    }
                }

                Console.WriteLine("Process found");

                //while the client is starting up
                while (clientWin == IntPtr.Zero)
                {
                    //wait for game client to be created and enter idle condition
                    p.WaitForInputIdle(1000);
                    p.Refresh();
                    //abort if the client finished before we got a handle
                    if (p.HasExited) {
                        Console.WriteLine("Process exited");
                        Environment.Exit(Environment.ExitCode);
                    }
                    //save game window and process id
                    clientWin = p.MainWindowHandle;  //store the client window handle
                    clientID = p.Id;
                    //set a handler to close this client when the game closes.
                    p.EnableRaisingEvents = true;
                    p.Exited += new EventHandler(onGameExit);
                }

                Console.WriteLine("Window found");
            }
            catch (Exception ex)
            {
                //some error
                MessageBox.Show(this, ex.Message, "Error");
                Environment.Exit(Environment.ExitCode);
            }

            //dock JagexClient window with RS Wikit form
            SetWindowLong(clientWin, GWL_STYLE, (GetWindowLong(clientWin, GWL_STYLE) & ~(WS_POPUP)) | WS_CHILD);
            SetParent(clientWin, this.Handle);

            //remove border and maximise
            SetWindowLong(clientWin, GWL_STYLE, WS_VISIBLE);
        }

        private void RestoreTabs()
        {
            Console.WriteLine("tabs.Count = " + tabs.Count);
            if (tabs.Count > 0 && MessageBox.Show("Restore your previous tabs?", "RSWikit", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                foreach (string tab in tabs)
                {
                    this.InvokeOnUiThreadIfRequired(() => tabSide.TabPages.Add(new frmWikia(tabSide.TabPages, tab)).Icon = new Icon("ico/icon.ico"));
                }
            }
            else
            {
                tabs.Clear();
            }
        }

        private void onGameExit(object sender, EventArgs e)
        {
            //if JagexClient is docked
            if (clientWin != IntPtr.Zero)
            {
                //clear the ID
                if (clientID > 0)
                    clientID = 0;

                //clear the handle
                clientWin = IntPtr.Zero;

                //exit with the game
                Console.WriteLine("Process exited");
                Environment.Exit(Environment.ExitCode);
            }
        }

        private void UpdateSize()
        {
            int ContentTop = tabSide.TabTop + tabSide.TabHeight;
            //update the buttons' size
            Size size = new Size(tabSide.TabHeight, tabSide.TabHeight);
            btnSwitch.Size = size;
            btnFullScreen.Size = size;
            btnNewTab.Size = size;
            //update the buttons' position
            btnSwitch.Top = tabSide.TabTop;
            btnSwitch.Left = ClientSize.Width - btnSwitch.Width;
            btnFullScreen.Top = tabSide.TabTop;
            btnFullScreen.Left = btnSwitch.Left - btnFullScreen.Width;
            btnNewTab.Top = tabSide.TabTop;
            btnNewTab.Left = btnFullScreen.Left - btnNewTab.Width;
            //update the sidebar size
            //note that the sidebar fills the page, but the right part is covered by the client
            //thus any tabs must be aware to use a width of frmMain.sideWidth
            //when arranging and alligning any components they have
            tabSide.Height = ClientSize.Height;
            tabSide.Width = btnNewTab.Left;
            tabSide.Left = 0;
            tabSide.Top = 0;
            //update BG panel
            pnlBG.Width = ClientSize.Width - tabSide.Width;
            pnlBG.Height = ContentTop;
            pnlBG.Left = tabSide.Width;
            pnlBG.Top = 0;
            //update the resize bar
            pnlResize.Left = width - 4;
            pnlResize.Top = ContentTop;
            pnlResize.MinimumSize = new Size(4, ClientSize.Height);

            size = new Size(width, Math.Max(ClientSize.Height - ContentTop, 0));
            foreach (MdiTabControl.TabPage tab in tabSide.TabPages)
            {
                Form frm = (Form)tab.Form;
                frm.MaximumSize = size;
                frm.MinimumSize = size;
            }

            //if JagexClient is docked
            if (clientWin != IntPtr.Zero)
            {
                //update client size and position
                MoveWindow(clientWin, width, ContentTop, ClientSize.Width - width, ClientSize.Height - ContentTop, true);
            }
        }

        private void FinishSize()
        {
            int ContentTop = tabSide.TabTop + tabSide.TabHeight;
            //if JagexClient is docked
            if (clientWin != IntPtr.Zero)
            {
                //repaint client
                MoveWindow(clientWin, width, ContentTop, ClientSize.Width - width, ClientSize.Height - ContentTop, true);
                //bring client to front
                SetForegroundWindow(clientWin);
            }
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (Cef.IsInitialized)
                Cef.Shutdown();

            //if JagexClient is docked
            if (clientWin != IntPtr.Zero)
            {
                if (clientID > 0)
                    clientID = 0;

                //tell JagexClient to close
                PostMessage(clientWin, WM_CLOSE, 0, 0);

                //wait for JagexClient to get the message
                Thread.Sleep(1000);

                //clear the handle
                clientWin = IntPtr.Zero;
            }

            base.OnHandleDestroyed(e);
        }

        private void frmMain_Shown(object sender, EventArgs e)
        {
            //change the size so that the client window is correctly arranged and displayed
            this.Width = this.Width + 1;
            //restore tabs if neccessary
            RestoreTabs();
        }

        private void frmMain_Move(object sender, EventArgs e)
        {
            UpdateSize();
        }

        private void frmMain_Resize(object sender, EventArgs e)
        {
            UpdateSize();
            if (FormBorderStyle == FormBorderStyle.None)
            {
                FinishSize();
            }
            else if (WindowState != lastWindowState)
            {
                FinishSize();
                lastWindowState = WindowState;
            }
        }

        private void frmMain_ResizeEnd(object sender, EventArgs e)
        {
            FinishSize();
        }
        
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            //If the close button is clicked and the game is running
            if (e.CloseReason != CloseReason.WindowsShutDown &&
                e.CloseReason != CloseReason.ApplicationExitCall &&
                e.CloseReason != CloseReason.TaskManagerClosing &&
                clientWin != IntPtr.Zero)
            {
                //Leave it up to the game to decide if it should close.
                e.Cancel = true;
                PostMessage(clientWin, WM_CLOSE, 0, 0);
            }
        }

        private void btnNewTab_Click(object sender, EventArgs e)
        {
            //add a new wiki tab to the sidebar
            tabSide.TabPages.Add(new frmWikia(tabSide.TabPages, "http://" + (osrs ? "oldschool" : "www") + ".runescape.wiki/")).Icon = new Icon("ico/icon.ico");
        }

        private void btnSwitch_Click(object sender, EventArgs e)
        {
            //switch between RS3 and OSRS
            DialogResult dialogResult = MessageBox.Show(osrs ? "Switch or RuneScape 3?" : "Switch to OldSchool RuneScape?", "RSWikit", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                osrs = !osrs;
                saveConfig();
                mutex.Close();
                Application.Restart();
            }
        }

        private void btnFullScreen_Click(object sender, EventArgs e)
        {
            //switch between fullscreen and save
            setFullscreen(!isFullscreen());
            saveConfig();
        }
        
        private void pnlResize_MouseDown(object sender, MouseEventArgs e)
        {
            //start resizing sidebar
            resizing = true;
        }

        private void pnlResize_MouseMove(object sender, MouseEventArgs e)
        {
            if (resizing == true)
            {
                //resize sidebar, snapping to 16px sizes
                width = Convert.ToInt32((pnlResize.Left + e.X) / 16.0f) * 16;
                UpdateSize();
            }
        }

        private void pnlResize_MouseUp(object sender, MouseEventArgs e)
        {
            if (resizing == true)
            {
                //save the new size of the sidebar
                saveConfig();

                //stop resizing
                resizing = false;
                FinishSize();
            }
        }

        private void pnlBG_Paint(object sender, PaintEventArgs e)
        {
            //paint a gradient to match the tab bar
            Rectangle rect = new Rectangle(0, 0, pnlBG.Width, pnlBG.Height);
            Brush brush = new LinearGradientBrush(rect, Color.DimGray, Color.Black, LinearGradientMode.Vertical);
            e.Graphics.FillRectangle(brush, rect);
        }

        private bool isFullscreen()
        {
            return (FormBorderStyle == FormBorderStyle.None);
        }

        private void setFullscreen(bool fullscreen)
        {
            if (fullscreen)
            {
                //Preserve lastWindowState
                FormWindowState lastState = WindowState;

                //Change state to normal
                WindowState = FormWindowState.Normal;
                lastWindowState = lastState;

                //Remove border
                FormBorderStyle = FormBorderStyle.None;

                //Fill screen
                lastBounds = Bounds;
                Bounds = Screen.PrimaryScreen.Bounds;
            }
            else
            {
                //Preserve lastWindowState
                FormWindowState lastState = lastWindowState;

                //Restore bounds
                Bounds = lastBounds;
                lastBounds = Rectangle.Empty;

                //Restore border
                FormBorderStyle = FormBorderStyle.Sizable;

                //Restore state
                lastWindowState = FormWindowState.Minimized;
                WindowState = lastState;
            }
            //set state
            full = fullscreen;
        }

        private static void loadConfig()
        {
            try
            {
                //open file
                StreamReader file = new StreamReader("config.ini");
                //read each line
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    //ini header, ignore
                    if (line[0] == '[') continue;
                    //split key/val
                    string[] vals = line.Split(new[] { '=' }, 2);
                    //read value
                    try
                    {
                        //skip invalid lines
                        if (vals.Length < 2 ||
                            vals[0] == null || vals[0].Length == 0 ||
                            vals[1] == null || vals[1].Length == 0)
                        {
                            //line is invalid
                            continue;
                        }
                        //read property
                        int tempi;
                        bool tempb;
                        switch (vals[0].ToLower())
                        {
                            case "width":
                                //width of sidebar
                                tempi = 0;
                                if (Int32.TryParse(vals[1], out tempi))
                                    width = tempi;
                                break;
                            case "osrs":
                                //oldschool, true/false
                                tempb = false;
                                if (Boolean.TryParse(vals[1], out tempb))
                                    osrs = tempb;
                                break;
                            case "full":
                                //start in fullscreen
                                tempb = false;
                                if (Boolean.TryParse(vals[1], out tempb))
                                    full = tempb;
                                break;
                            case "tabs":
                                //restore tabs
                                string[] list = vals[1].Split(' ');
                                tabs.AddRange(list);
                                tabs.Reverse();
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error");
                    }
                }

                file.Close();
            }
            catch (IOException ex)
            {
                //file does not exist, use defaults
                saveConfig(); //create file
            }
        }

        public static void saveConfig()
        {
            try
            {
                //open file
                StreamWriter file = new StreamWriter("config.ini");
                //write sidebar width
                file.WriteLine("width=" + width);
                //write osrs
                file.WriteLine("osrs=" + osrs);
                //write full
                file.WriteLine("full=" + full);
                //write tabs
                file.WriteLine("tabs=" + makeTabs());
                //close file
                file.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        public static void saveTabs(MdiTabControl.TabControl.TabPageCollection tabPages)
        {

            tabs.Clear();
            foreach (MdiTabControl.TabPage tab in tabPages)
            {
                frmWikia form = tab.Form as frmWikia;
                if (form != null)
                {
                    tabs.Add(form.getUrl());
                }
            }
            saveConfig();
        }

        private static string makeTabs()
        {

            StringBuilder str = new StringBuilder();

            foreach (string tab in tabs)
                str.Append(tab + " ");

            if (tabs.Count > 0)
                str.Length--;

            return str.ToString();
        }

        private static void InitializeChromium()
        {
            if (!Cef.IsInitialized)
            {
                //initialize chromium
                CefSettings settings = new CefSettings();
                settings.UserAgent = "Mozilla/5.0 (Linux; Android 4.0.4; Galaxy Nexus Build/IMM76B) AppleWebKit/535.19 (KHTML, like Gecko) Chrome/18.0.1025.133 Mobile Safari/535.19";
                settings.CachePath = "cache";
                settings.ResourcesDirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"lib\");
                Console.WriteLine(settings.ResourcesDirPath);
                settings.LocalesDirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"lib\locales\");
                settings.DisableGpuAcceleration();
                settings.BrowserSubprocessPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"lib\CefSharp.BrowserSubprocess.exe");
                // Initialize cef with the provided settings
                Cef.Initialize(settings, false, null);
            }
        }
    }
}
