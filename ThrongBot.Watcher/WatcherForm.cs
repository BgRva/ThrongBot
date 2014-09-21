using ThrongBot.Common;
using ThrongBot.Common.Entities;
using ThrongBot.Watcher.Args;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ThrongBot.Watcher
{
    public partial class WatcherForm : Form
    {
        private IList<SessionConfiguration> _sessions = null;

        public WatcherForm()
        {
            InitializeComponent();
            menuSessionsAsync.DropDownItemClicked += menuSessionsAsync_DropDownItemClicked;
        }

        private void WatcherForm_Load(object sender, EventArgs e)
        {
            _sessions = GetSessions();

            foreach (var session in _sessions)
            {
                var menuAsync = new ToolStripMenuItem() { Name = string.Format("menuSession{0}", session.SessionId) };
                menuAsync.Tag = session.SessionId;
            //    menuAsync.Text = string.Format("Session {0} ({1})", session.SessionId, session.Definitions.Count);
                this.menuSessionsAsync.DropDownItems.Add(menuAsync);
            }
        }

        private void menuSessionsAsync_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            int id = (int)e.ClickedItem.Tag;
            var q = from s in _sessions
                    where s.SessionId == id
                    select s;

            var session = q.FirstOrDefault();

            if (session != null)
            {
                ClearSession();
                //foreach (var def in session.Definitions)
                //{
                //    // var daddy = Bootstrap.Bootstrapper.Resolve<ICrawlDaddy>();
                //    var daddy = new Fake.FakeCrawlDaddy();
                //  //  if (!IsCrawlerDefined(session.SessionId, def.CrawlerId))
                //    {
                //        if (daddy.InitializeCrawler(def.SeedUrl, session.SessionId, def.CrawlerId))
                //        {
                //            var ctrl = new CrawlerInstanceCtrl2();
                //            ctrl.InitializeCrawl(daddy);
                //            this.splitContainer1.Panel1.Controls.Add(ctrl);
                //            ctrl.Dock = DockStyle.Top;
                //            ctrl.ShowSomething += crawlView_ShowSomething;
                //            ctrl.ExternalLinksFound += crawlView_ExternalLinksFound;
                //            ctrl.BackColor = Color.Thistle;
                //        }
                //        else
                //        {
                //            Log(string.Format("Init failed: a crawl is already defined with sessionId: {0} and crawlerId : {1}", session.SessionId, def.CrawlerId));
                //        }
                //    }
                //}
            }
            Cursor = Cursors.Default;
        }

        private void crawlView_ExternalLinksFound(object sender, CrawlViewExteranlLinksFoundEventArgs e)
        {
            Log(string.Format("External Link: {0}: {1}", e.CrawlerId, e.Link.AbsoluteUri));
        }
        private void crawlView_ShowSomething(object sender, CrawlViewEventArgs e)
        {
            var instance = sender as ICrawlView;
            this.txtDetails.Lines = instance.GetDetails();
        }

        private void ClearSession()
        {
            if (splitContainer1.Panel1.Controls.Count == 0)
                return;
            foreach (var ctrl in splitContainer1.Panel1.Controls)
            {
                var instance = ctrl as ICrawlView;
                instance.ShowSomething -= crawlView_ShowSomething;
                instance.ExternalLinksFound -= crawlView_ExternalLinksFound;
            }
            splitContainer1.Panel1.Controls.Clear();
            txtDetails.Clear();
        }


        private void Log(string mssg)
        {
            txtDetails.AppendText(mssg);
            txtDetails.AppendText(Environment.NewLine);
        }
        //------------------

        private bool IsCrawlerDefined(int sessionId, int crawlerId)
        {
            return false;
        }
        private List<SessionConfiguration> GetSessions()
        {
            var sessions = new List<SessionConfiguration>();
            
            var cfg = new SessionConfiguration(3301);

            //var def1 = new CrawlerRunDefinition();
            //def1.CrawlerId = 22;
            //def1.SeedUrl = "http://www.bluespiders.net";
            //cfg.Definitions.Add(def1);

            //sessions.Add(cfg);

            //cfg = new SessionConfiguration(505);

            //def1 = new CrawlerRunDefinition();
            //def1.CrawlerId = 54;
            //def1.SeedUrl = "http://www.sharipastorephotography.com";
            //cfg.Definitions.Add(def1);

            //sessions.Add(cfg);

            return sessions;
        }

        private void menuLogClear_Click(object sender, EventArgs e)
        {
            txtDetails.Clear();
        }

        private void menuLoadSessionFile_Click(object sender, EventArgs e)
        {

        }
    }
}
