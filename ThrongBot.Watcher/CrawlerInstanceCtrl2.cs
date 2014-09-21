using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ThrongBot.Common;
using System.Threading;
using ThrongBot.Watcher.Args;

namespace ThrongBot.Watcher
{
    public partial class CrawlerInstanceCtrl2 : UserControl, ICrawlView
    {
        private CrawlDaddyAsyncWrapper _wrapper = null;
        private ICrawlDaddy _crawl = null;
        private List<string> _externalLinks = null;
        private List<string> _linksCrawled = null;

        public CrawlerInstanceCtrl2()
        {
            InitializeComponent();

            txtSeedUrl.ReadOnly = true;
            btnStop.Enabled = false;
        }

        public void InitializeCrawl(ICrawlDaddy crawl)
        {
            _wrapper = new CrawlDaddyAsyncWrapper();
            _wrapper.ProgressChanged += _wrapper_ProgressChanged;
            _wrapper.CrawlCompleted += _wrapper_CrawlCompleted;

            _externalLinks = new List<string>();
            _linksCrawled = new List<string>();
            Reset();
            _crawl = crawl;

            lblCrawlerId.Text = _crawl.CrawlerId.ToString();
            txtSeedUrl.Text = _crawl.Seed.AbsoluteUri;
            lblBaseDomain.Text = _crawl.BaseDomain;
        }

        private void _wrapper_CrawlCompleted(object sender, CrawlDaddyCompletedEventArgs e)
        {
            lblEndTime.Text = e.Message;

            lblCrawlerId.ForeColor = Color.Black;
            lblCrawlerId.Font = new Font("Arial", lblCrawlerId.Font.Size, FontStyle.Regular);
        }

        private void _wrapper_ProgressChanged(ProgressChangedEventArgs e)
        {
            var args = e as CrawlDaddyProgressChangedEventArgs;
            if (args.CrawlerArgs is DomainCrawlStartedEventArgs)
            {
                ReceiveArgs(args.CrawlerArgs as DomainCrawlStartedEventArgs);
            }
            else if (args.CrawlerArgs is LinkCrawlCompletedArgs)
            {
                ReceiveArgs(args.CrawlerArgs as LinkCrawlCompletedArgs);
            }
            else if (args.CrawlerArgs is ExternalLinksFoundEventArgs)
            {
                ReceiveArgs(args.CrawlerArgs as ExternalLinksFoundEventArgs);
            }
        }

        private void ReceiveArgs(LinkCrawlCompletedArgs e)
        {
            _linksCrawled.Add(string.Format("{0} -> {1}", string.Copy(e.SourceUrl), string.Copy(e.TargetUrl)));
        }
        private void ReceiveArgs(ExternalLinksFoundEventArgs e)
        {
            _externalLinks.Add(string.Copy(e.PageUri.AbsoluteUri));
            OnExternalLinksFound(e.CrawlerId, e.PageUri);
        }
        private void ReceiveArgs(DomainCrawlStartedEventArgs e)
        {
            lblStartTime.Text = e.StartTime.ToString();
            btnStop.Enabled = true;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            _wrapper.CrawlAsync(_crawl);
            lblCrawlerId.ForeColor = Color.DarkRed;
            lblCrawlerId.Font = new Font("Arial", lblCrawlerId.Font.Size, FontStyle.Bold);
        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            lblCrawlerId.ForeColor = Color.Black;
            lblCrawlerId.Font = new Font("Arial", lblCrawlerId.Font.Size, FontStyle.Regular);
        }
        private void btnConfig_Click(object sender, EventArgs e)
        {
            OnShowSomething(new CrawlViewEventArgs("Config"));
        }
        private void btnDetails_Click(object sender, EventArgs e)
        {
            OnShowSomething(new CrawlViewEventArgs("Details"));
        }

        private void Reset()
        {
            lblStartTime.Text = null;
            lblEndTime.Text = null;
            lblDepth.Text = "0";
            lblCrawledCount.Text = "0";
            if (_externalLinks != null)
                _externalLinks.Clear();
            else
                _externalLinks = new List<string>();
            if (_linksCrawled != null)
                _linksCrawled.Clear();
            else
                _linksCrawled = new List<string>();
        }

        public string[] GetDetails()
        {
            List<string> details = new List<string>();
            details.Add(string.Format("External Links ({0})", _externalLinks.Count));
            foreach (var link in _externalLinks)
            {
                details.Add(string.Format("    {0}", link));
            }
            details.Add("");

            details.Add(string.Format("Links Crawled ({0})", _linksCrawled.Count));
            foreach (var link in _linksCrawled)
            {
                details.Add(string.Format("    {0}", link));
            }
            return details.ToArray();
        }


        public event EventHandler<CrawlViewExteranlLinksFoundEventArgs> ExternalLinksFound;
        #region OnExternalLinksFound
        protected virtual void OnExternalLinksFound(int crawlerId, Uri link)
        {
            var handler = ExternalLinksFound;
            if (handler != null)
            {
                handler(this, new CrawlViewExteranlLinksFoundEventArgs(crawlerId, link));
            }
        }
        #endregion
        public event EventHandler<CrawlViewEventArgs> ShowSomething;
        #region OnShowSomething
        protected virtual void OnShowSomething(CrawlViewEventArgs e)
        {
            var handler = ShowSomething;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion
    }
}
