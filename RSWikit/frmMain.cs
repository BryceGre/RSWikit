using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using CefSharp;
using System.IO;
using System.Threading;

namespace RSWikit
{
    public partial class frmMain : Form
    {
        /****************/
        /***** Vars *****/
        /****************/
        static Mutex mutex = new Mutex(true, "{F44E7DB8-2E07-4C53-8261-5965C935AD2E}");

        //width of sidebar
        public static int width = 500;
        //runescape client launch url
        public static string url = "rs-launch://www.runescape.com/k=5/l=$(Language:0)/jav_config.ws";
        //use old-school runescape wiki
        public static bool osrs = false;

        bool resizing;
        FormWindowState lastWindowState = FormWindowState.Minimized;

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
            }
            else
            {
                MessageBox.Show("Multiple instances are not supported!");
                Environment.Exit(Environment.ExitCode);
            }
        }

        private void DockApp()
        {
            frmWaiting waiting = new frmWaiting();
            waiting.setText("Searching for Client...");
            waiting.Show();

            //start client handle at 0
            clientWin = IntPtr.Zero;

            //start JagexClient
            Process p = null;
            try
            {
                //stop any existing process that is not docked
                Process[] pr = Process.GetProcessesByName("rs2client");
                for (int i = 0; i < pr.Length; i++)
                {
                    pr[i].Kill();
                }

                //start the process
                Process.Start(url);

                int timeout = 10000;

                //now find the process
                while (p == null) {
                    Thread.Sleep(100);
                    timeout -= 100;
                    if (timeout <= 0)
                    {
                        Console.WriteLine("Process not found");
                        waiting.setText("Could not find client");
                        waiting.showHelp();
                        return;
                    }

                    Process[] ps = Process.GetProcessesByName("rs2client");
                    for (int i = 0; i < ps.Length; i++) {
                        if (!ps[i].HasExited) { //&& Array.IndexOf(pids, ps[i].Id) < 0) {
                            p = ps[i];
                            break;
                        }
                    }
                }

                Console.WriteLine("Process found");

                //while the client is starting up
                while (clientWin == IntPtr.Zero)
                {
                    // Wait for client to be created and enter idle condition
                    p.WaitForInputIdle(1000);
                    p.Refresh();
                    if (p.HasExited) {
                        Console.WriteLine("Process exited");
                        Environment.Exit(Environment.ExitCode); //abort if the client finished before we got a handle
                    }
                    clientWin = p.MainWindowHandle;  //store the client window handle
                    clientID = p.Id;
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

            waiting.Close();
        }

        private void onGameExit(object sender, EventArgs e)
        {
            //if JagexClient is docked
            if (clientWin != IntPtr.Zero)
            {
                if (clientID > 0)
                    clientID = 0;

                //clear the handle
                clientWin = IntPtr.Zero;

                Console.WriteLine("Process exited");
                Environment.Exit(Environment.ExitCode);
            }
        }

        private void UpdateSize()
        {
            //update the sidebar size
            //note that the sidebar fills the page, but the right part is covered by the client
            //thus any tabs must be aware to use a width of frmMain.sideWidth, rather than this.Width
            //when arranging and alligning any components they have
            tabSide.Height = this.Height - 48;
            tabSide.Width = this.Width - 48;
            //update the New Tab button's position
            btnHelp.Left = this.Width - 48;
            btnNewTab.Left = this.Width - btnHelp.Width - 48;
            //update the resize bar
            pnlResize.Left = width - 4;
            pnlResize.Top = tabSide.TabHeight + 4;
            pnlResize.MinimumSize = new Size(4, this.Height);

            Size size = new Size(width, Math.Max(this.Height - 48, 0));
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
                MoveWindow(clientWin, width, tabSide.TabHeight + 4, this.Width - width - 16, this.Height - tabSide.TabHeight - 44, true);
            }
        }

        private void FinishSize()
        {
            //if JagexClient is docked
            if (clientWin != IntPtr.Zero)
            {
                //repaint client
                MoveWindow(clientWin, width, tabSide.TabHeight + 4, this.Width - width - 16, this.Height - tabSide.TabHeight - 44, true);
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

        private void btnNewTab_Click(object sender, EventArgs e)
        {
            //add a new wiki tab to the sidebar
            tabSide.TabPages.Add(new frmWikia(tabSide.TabPages, "http://" + (osrs ? "oldschool" : "www") + ".runescape.wiki/")).Icon = new Icon("ico/icon.ico");
        }

        private void frmMain_Shown(object sender, EventArgs e)
        {
            //change the size so that the client window is correctly arranged and displayed
            this.Width = this.Width + 1;
        }

        private void frmMain_ResizeEnd(object sender, EventArgs e)
        {
            FinishSize();
        }

        private void frmMain_Resize(object sender, EventArgs e)
        {
            UpdateSize();
            if (WindowState != lastWindowState)
            {
                FinishSize();
                lastWindowState = WindowState;
            }
        }

        private void frmMain_Move(object sender, EventArgs e)
        {
            UpdateSize();
        }

        private void pnlResize_MouseDown(object sender, MouseEventArgs e)
        {
            resizing = true;
        }

        private void pnlResize_MouseMove(object sender, MouseEventArgs e)
        {
            if (resizing == true)
            {
                width = Convert.ToInt32((pnlResize.Left + e.X) / 16.0f) * 16;
                UpdateSize();
            }
        }

        private void pnlResize_MouseUp(object sender, MouseEventArgs e)
        {
            if (resizing == true)
            {
                saveConfig();

                resizing = false;
                FinishSize();
            }
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, "Don't see RS? Drag your RuneScape desktop icon (RS3 or OSRS) onto this client to get started.", "Help");
        }

        private void frmMain_DragDrop(object sender, DragEventArgs e)
        {
            dragDrop(e);
        }

        private void frmMain_DragEnter(object sender, DragEventArgs e)
        {
            dragEnter(e);
        }

        public static void dragDrop(DragEventArgs e)
        {
            // Handle FileDrop data.
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string filename in files)
                {
                    if (Path.GetExtension(filename) == ".url")
                    {
                        StreamReader file = new StreamReader(filename);

                        //read each line
                        string line;
                        while ((line = file.ReadLine()) != null)
                        {
                            if (line.StartsWith("URL=", true, null))
                            {
                                string temps = line.Substring(4);
                                if (temps.StartsWith("rs-launch://"))
                                {
                                    url = temps;
                                    osrs = (temps.Contains("oldschool"));

                                    saveConfig();
                                    Application.Restart();
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void dragEnter(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Link;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
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
                        switch (vals[0].ToLower())
                        {
                            case "width":
                                //width of sidebar
                                int tempi = 0;
                                if (Int32.TryParse(vals[1], out tempi))
                                    width = tempi;
                                break;
                            case "url":
                                //runescape launch url
                                Console.WriteLine("\"" + vals[1] + "\"");
                                if (vals[1].StartsWith("rs-launch://"))
                                    url = vals[1];
                                break;
                            case "osrs":
                                //oldschool, true/false
                                bool tempb = false;
                                if (Boolean.TryParse(vals[1], out tempb))
                                    osrs = tempb;
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

        private static void saveConfig()
        {
            try
            {
                //open file
                StreamWriter file = new StreamWriter("config.ini");
                //write sidebar width
                file.WriteLine("width=" + width);
                //write runescape url
                file.WriteLine("url=" + url);
                //write osrs
                file.WriteLine("osrs=" + osrs);
                //close file
                file.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
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
                settings.LocalesDirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"lib\locales\");
                settings.DisableGpuAcceleration();
                settings.BrowserSubprocessPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"lib\CefSharp.BrowserSubprocess.exe");
                // Initialize cef with the provided settings
                Cef.Initialize(settings, false, null);
            }
        }
    }
}
