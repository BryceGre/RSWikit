using System;
using System.Drawing;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using CefSharp.WinForms.Internals;

namespace RSWikit
{
    public partial class frmWikia : Form
    {
        private MdiTabControl.TabControl.TabPageCollection tabPages;
        private string initUrl;

        public frmWikia() : this(null, "http://" + (frmMain.osrs ? "oldschool" : "www") + ".runescape.wiki/") {}
        public frmWikia(MdiTabControl.TabControl.TabPageCollection tabs, String url)
        {

            webWikia = new ChromiumWebBrowser(url);
            this.SuspendLayout();
            webWikia.Dock = DockStyle.None;
            webWikia.BackColor = Color.Aqua;
            webWikia.Location = new Point(0, 0);
            webWikia.Name = "webWikia";
            webWikia.Size = new Size(400, 768);
            webWikia.MenuHandler = new CustomMenuHandler(this);
            webWikia.TitleChanged += webWikia_TitleChanged;
            Controls.Add(webWikia);
            this.ResumeLayout(false);

            //initialize components
            InitializeComponent();

            tabPages = tabs;
            initUrl = url;

            resize();
        }

        private void frmWikia_SizeChanged(object sender, EventArgs e)
        {
            resize();
        }

        private void webWikia_TitleChanged(object sender, TitleChangedEventArgs args)
        {
            //update the form (tab) title when the document title changes
            String title = args.Title;
            if (title == "") return;

            if (title.IndexOf(" - ") >= 0)
                title = title.Substring(0, title.IndexOf(" - "));
            if (title.IndexOf(" | ") >= 0)
                title = title.Substring(0, title.IndexOf(" | "));

            this.InvokeOnUiThreadIfRequired(() => Text = title);
        }

        private void resize()
        {
            //update browser control location and size
            this.Width = frmMain.width;

            webWikia.Top = 0;
            webWikia.Left = 0;
            webWikia.Width = Width;
            webWikia.Height = Height;
        }

        private class CustomMenuHandler : IContextMenuHandler
        {
            frmWikia wikiaForm;

            public CustomMenuHandler(frmWikia form) : base()
            {
                wikiaForm = form;
            }

            public void OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
            {
                if (parameters.LinkUrl != "")
                {
                    model.InsertSeparatorAt(0);
                    model.InsertItemAt(0, (CefMenuCommand)26502, "Copy link address");
                    model.InsertSeparatorAt(0);
                    model.InsertItemAt(0, (CefMenuCommand)26501, "Open in new tab");
                }
            }

            public bool OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
            {
                if (commandId == (CefMenuCommand)26501)
                {
                    string url = parameters.LinkUrl;
                    if (!parameters.LinkUrl.Contains("://"))
                        url = "http://runescape.wikia.com" + url; //full URL link

                    wikiaForm.InvokeOnUiThreadIfRequired(() => wikiaForm.tabPages.Add(new frmWikia(wikiaForm.tabPages, url)).Icon = new Icon("img/icon.ico"));
                    return true;
                }
                else if (commandId == (CefMenuCommand)26502)
                {
                    string url = parameters.UnfilteredLinkUrl;
                    if (!parameters.LinkUrl.Contains("://"))
                        url = "http://runescape.wikia.com" + url; //full URL link
                    Clipboard.SetText(url);
                }
                return false;
            }

            public void OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame)
            {

            }

            public bool RunContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
            {
                return false;
            }
        }
    }
}
