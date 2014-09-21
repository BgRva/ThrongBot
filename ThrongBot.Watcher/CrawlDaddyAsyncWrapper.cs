using ThrongBot.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThrongBot.Watcher
{
    public delegate void ProgressChangedEventHandler(ProgressChangedEventArgs e);

    public delegate void CrawlDaddyCompletedEventHandler(object sender, CrawlDaddyCompletedEventArgs e);

    public class CrawlDaddyAsyncWrapper
    {
        private delegate void WorkerEventHandler(ICrawlDaddy crawl, AsyncOperation asyncOp);
        AsyncOperation _asyncOp = null;
        private bool _cancelPending = false;
        private SendOrPostCallback _onProgressReportDelegate;
        private SendOrPostCallback _onCompletedDelegate;

        public CrawlDaddyAsyncWrapper()
        {
            _onProgressReportDelegate = new SendOrPostCallback(RaiseCrawlProgress);
            _onCompletedDelegate = new SendOrPostCallback(RaiseCrawlCompleted);
        }

        public event ProgressChangedEventHandler ProgressChanged;
        #region OnProgressChanged

        protected void OnProgressChanged(ProgressChangedEventArgs e)
        {
            if (ProgressChanged != null)
            {
                ProgressChanged(e);
            }
        }

        #endregion

        public event CrawlDaddyCompletedEventHandler CrawlCompleted;
        #region OnCrawlCompleted

        protected void OnCrawlCompleted(CrawlDaddyCompletedEventArgs args)
        {
            if (CrawlCompleted != null)
            {
                CrawlCompleted(this, args);
            }
        }

        #endregion

        public void CrawlAsync(ICrawlDaddy crawl)
        {
            // Create an AsyncOperation for taskId.
            _asyncOp = AsyncOperationManager.CreateOperation(crawl.CrawlerId);

            crawl.DomainCrawlStarted += crawl_DomainCrawlStarted;
            crawl.DomainCrawlEnded += crawl_DomainCrawlEnded;
            crawl.LinkCrawlCompleted += crawl_LinkCrawlCompleted;
            crawl.ExternalLinksFound += crawl_ExternalLinksFound;

            // Start the asynchronous operation.
            WorkerEventHandler workerDelegate = new WorkerEventHandler(CrawlWorker);
            workerDelegate.BeginInvoke(crawl, _asyncOp, null, null);
        }
        public void CancelAsync(int crawlerId)
        {
            _cancelPending = true;
        }

        private void crawl_LinkCrawlCompleted(object sender, LinkCrawlCompletedArgs e)
        {
            if (_asyncOp != null && !_cancelPending)
            {
                var args = new CrawlDaddyProgressChangedEventArgs(e, 1, _asyncOp.UserSuppliedState);
                _asyncOp.Post(_onProgressReportDelegate, args);
            }
        }
        private void crawl_ExternalLinksFound(object sender, ExternalLinksFoundEventArgs e)
        {
            if (_asyncOp != null && !_cancelPending)
            {
                var args = new CrawlDaddyProgressChangedEventArgs(e, 1, _asyncOp.UserSuppliedState);
                _asyncOp.Post(_onProgressReportDelegate, args);
            }
        }
        private void crawl_DomainCrawlStarted(object sender, DomainCrawlStartedEventArgs e)
        {
            if (_asyncOp != null && !_cancelPending)
            {
                var args = new CrawlDaddyProgressChangedEventArgs(e, 1, _asyncOp.UserSuppliedState);
                _asyncOp.Post(_onProgressReportDelegate, args);
            }
        }
        private void crawl_DomainCrawlEnded(object sender, DomainCrawlEndedEventArgs e)
        {
            if (_asyncOp != null && !_cancelPending)
            {
                var args = new CrawlDaddyProgressChangedEventArgs(e, 1, _asyncOp.UserSuppliedState);
                _asyncOp.Post(_onProgressReportDelegate, args);
            }
        }

        private void CrawlWorker(ICrawlDaddy crawl, AsyncOperation asyncOp)
        {
            ProgressChangedEventArgs args = null;
            Exception e = null;

            // Check that the task is still active. 
            // The operation may have been canceled before 
            // the thread was scheduled. 
            if (!_cancelPending)
            {
                try
                {
                    crawl.StartCrawl();
                }
                catch (Exception ex)
                {
                    e = ex;
                }
            }

            CrawlEnded(crawl, e, _cancelPending, asyncOp);
        }
        private void CrawlEnded(ICrawlDaddy crawl, Exception exception, bool canceled, AsyncOperation asyncOp)
        {
            crawl.DomainCrawlStarted -= crawl_DomainCrawlStarted;
            crawl.DomainCrawlEnded -= crawl_DomainCrawlEnded;
            crawl.LinkCrawlCompleted -= crawl_LinkCrawlCompleted;
            crawl.ExternalLinksFound -= crawl_ExternalLinksFound;

            // Package the results of the operation in a  
            // CrawlDaddyCompletedEventArgs.
            var e = new CrawlDaddyCompletedEventArgs("DONE", exception, canceled, asyncOp.UserSuppliedState);

            // End the task. The asyncOp object is responsible  
            // for marshaling the call.
            asyncOp.PostOperationCompleted(_onCompletedDelegate, e);

            // Note that after the call to OperationCompleted,  
            // asyncOp is no longer usable, and any attempt to use it 
            // will cause an exception to be thrown.
        }

        //async op delegate methods
        private void RaiseCrawlCompleted(object operationState)
        {
            var e = operationState as CrawlDaddyCompletedEventArgs;

            OnCrawlCompleted(e);
        }
        private void RaiseCrawlProgress(object state)
        {
            var e = state as ProgressChangedEventArgs;
            OnProgressChanged(e);
        }
    }

    public class CrawlDaddyProgressChangedEventArgs : ProgressChangedEventArgs
    {
        public CrawlDaddyProgressChangedEventArgs(string mssg, int progressPercentage, object userToken) : 
            base(progressPercentage, userToken)
        {
            if (mssg != null)
                Message = string.Copy(mssg);
        }
        public CrawlDaddyProgressChangedEventArgs(EventArgs crawlerArgs, int progressPercentage, object userToken) :
            base(progressPercentage, userToken)
        {
            CrawlerArgs = crawlerArgs;
        }
        public string Message { get; set; }
        public EventArgs CrawlerArgs { get; set; }
    }

    public class CrawlDaddyCompletedEventArgs : AsyncCompletedEventArgs
    {
        public CrawlDaddyCompletedEventArgs(
            string mssg,
            Exception e,
            bool canceled,
            object state)
            : base(e, canceled, state)
        {
            if (mssg != null)
                Message = string.Copy(mssg);
        }

        public string Message { get; set; }
        public DateTime EndTime { get; set; }
    }
}
