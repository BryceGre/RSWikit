using CefSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace RSWikit
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            CefLibraryHandle handle = new CefLibraryHandle(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"lib\libcef.dll"));
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
            handle.Dispose();
        }
    }
}
