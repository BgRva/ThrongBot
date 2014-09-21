using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DomainCrawler.Common;

namespace DomainCrawler.Watcher
{
    public interface ICrawlView
    {
        event EventHandler<CrawlViewEventArgs> ShowSomething;
        string[] GetDetails();
    }

    public partial class CrawlerInstanceCtrl : UserControl
    {
        private ICrawlDaddy _crawl = null;
        private List<string> _externalLinks = null;
        private List<string> _linksCrawled = null;

        public CrawlerInstanceCtrl()
        {
            InitializeComponent();
            btnStop.Enabled = false;
        }

        public void InitializeCrawl(ICrawlDaddy crawl)
        {
            _externalLinks = new List<string>();
            _linksCrawled = new List<string>();
            Reset();
            _crawl = crawl;
            _crawl.DomainCrawlStarted += _crawl_DomainCrawlStarted;
            _crawl.DomainCrawlEnded += _crawl_DomainCrawlEnded;
            _crawl.ExternalLinksFound += _crawl_ExternalLinksFound;
            _crawl.LinkCrawlCompleted += _crawl_LinkCrawlCompleted;

            lblCrawlerId.Text = _crawl.CrawlerId.ToString();
            txtSeedUrl.Text = _crawl.Seed.AbsoluteUri;
            lblBaseDomain.Text = _crawl.BaseDomain;
        }
        public int CrawlerId
        {
            get
            {
                return _crawl.CrawlerId;
            }
        }
        private void _crawl_LinkCrawlCompleted(object sender, LinkCrawlCompletedArgs e)
        {
            _externalLinks.Add(string.Format("{0} -> {1}", string.Copy(e.SourceUrl), string.Copy(e.TargetUrl)));
        }
        private void _crawl_ExternalLinksFound(object sender, ExternalLinksFoundEventArgs e)
        {
            _externalLinks.Add(string.Copy(e.PageUri.AbsoluteUri));
        }
        private void _crawl_DomainCrawlEnded(object sender, DomainCrawlEndedEventArgs e)
        {
            lblEndTime.Text = e.EndTime.ToString();
        }
        private void _crawl_DomainCrawlStarted(object sender, DomainCrawlStartedEventArgs e)
        {
            lblStartTime.Text = e.StartTime.ToString();
            btnStop.Enabled = true;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            _crawl.StartCrawl();
            btnStart.Enabled = false;
        }
        private void btnStop_Click(object sender, EventArgs e)
        {

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

    public class CrawlViewEventArgs : EventArgs
    {
        public CrawlViewEventArgs()
        { }

        public CrawlViewEventArgs(string something)
        {
            Something = something;
        }

        public string Something { get; set; }
    }
}
