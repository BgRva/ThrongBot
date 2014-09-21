namespace ThrongBot.Watcher
{
    partial class CrawlerInstanceCtrl2
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    if (_crawl != null)
                    {
                        _crawl.Dispose();
                        _crawl = null;
                    }
                }
                // your clean up code here
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.lblCrawlerId = new System.Windows.Forms.Label();
            this.txtSeedUrl = new System.Windows.Forms.TextBox();
            this.lblCrawledCount = new System.Windows.Forms.Label();
            this.lblDepth = new System.Windows.Forms.Label();
            this.lblEndTime = new System.Windows.Forms.Label();
            this.lblStartTime = new System.Windows.Forms.Label();
            this.lblBaseDomain = new System.Windows.Forms.Label();
            this.btnDetails = new System.Windows.Forms.Button();
            this.btnConfig = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(63, 4);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(27, 25);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = ">";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(93, 4);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(28, 25);
            this.btnStop.TabIndex = 1;
            this.btnStop.Text = "X";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // lblCrawlerId
            // 
            this.lblCrawlerId.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCrawlerId.Location = new System.Drawing.Point(125, 6);
            this.lblCrawlerId.Name = "lblCrawlerId";
            this.lblCrawlerId.Size = new System.Drawing.Size(88, 22);
            this.lblCrawlerId.TabIndex = 2;
            this.lblCrawlerId.Text = "000000";
            this.lblCrawlerId.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtSeedUrl
            // 
            this.txtSeedUrl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSeedUrl.Location = new System.Drawing.Point(213, 6);
            this.txtSeedUrl.Name = "txtSeedUrl";
            this.txtSeedUrl.Size = new System.Drawing.Size(230, 22);
            this.txtSeedUrl.TabIndex = 3;
            this.txtSeedUrl.Text = "http://www.blah.com/X/Y.htm";
            // 
            // lblCrawledCount
            // 
            this.lblCrawledCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCrawledCount.Location = new System.Drawing.Point(950, 6);
            this.lblCrawledCount.Name = "lblCrawledCount";
            this.lblCrawledCount.Size = new System.Drawing.Size(88, 22);
            this.lblCrawledCount.TabIndex = 4;
            this.lblCrawledCount.Text = "0000000000";
            this.lblCrawledCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblDepth
            // 
            this.lblDepth.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblDepth.Location = new System.Drawing.Point(1038, 6);
            this.lblDepth.Name = "lblDepth";
            this.lblDepth.Size = new System.Drawing.Size(88, 22);
            this.lblDepth.TabIndex = 5;
            this.lblDepth.Text = "0000000000";
            this.lblDepth.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblEndTime
            // 
            this.lblEndTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblEndTime.Location = new System.Drawing.Point(766, 6);
            this.lblEndTime.Name = "lblEndTime";
            this.lblEndTime.Size = new System.Drawing.Size(184, 22);
            this.lblEndTime.TabIndex = 6;
            this.lblEndTime.Text = "03-03-2013 1200:00:00:00";
            this.lblEndTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblStartTime
            // 
            this.lblStartTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblStartTime.Location = new System.Drawing.Point(580, 6);
            this.lblStartTime.Name = "lblStartTime";
            this.lblStartTime.Size = new System.Drawing.Size(186, 22);
            this.lblStartTime.TabIndex = 7;
            this.lblStartTime.Text = "03-03-2013 1200:00:00:00";
            this.lblStartTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblBaseDomain
            // 
            this.lblBaseDomain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblBaseDomain.Location = new System.Drawing.Point(443, 6);
            this.lblBaseDomain.Name = "lblBaseDomain";
            this.lblBaseDomain.Size = new System.Drawing.Size(137, 22);
            this.lblBaseDomain.TabIndex = 8;
            this.lblBaseDomain.Text = "blah.com";
            this.lblBaseDomain.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnDetails
            // 
            this.btnDetails.Location = new System.Drawing.Point(33, 4);
            this.btnDetails.Name = "btnDetails";
            this.btnDetails.Size = new System.Drawing.Size(28, 25);
            this.btnDetails.TabIndex = 10;
            this.btnDetails.Text = "?";
            this.btnDetails.UseVisualStyleBackColor = true;
            this.btnDetails.Click += new System.EventHandler(this.btnDetails_Click);
            // 
            // btnConfig
            // 
            this.btnConfig.Location = new System.Drawing.Point(3, 4);
            this.btnConfig.Name = "btnConfig";
            this.btnConfig.Size = new System.Drawing.Size(27, 25);
            this.btnConfig.TabIndex = 9;
            this.btnConfig.Text = "*";
            this.btnConfig.UseVisualStyleBackColor = true;
            this.btnConfig.Click += new System.EventHandler(this.btnConfig_Click);
            // 
            // CrawlerInstanceCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnDetails);
            this.Controls.Add(this.btnConfig);
            this.Controls.Add(this.lblDepth);
            this.Controls.Add(this.lblCrawledCount);
            this.Controls.Add(this.lblEndTime);
            this.Controls.Add(this.lblStartTime);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.lblBaseDomain);
            this.Controls.Add(this.txtSeedUrl);
            this.Controls.Add(this.lblCrawlerId);
            this.Name = "CrawlerInstanceCtrl";
            this.Size = new System.Drawing.Size(1174, 35);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label lblCrawlerId;
        private System.Windows.Forms.TextBox txtSeedUrl;
        private System.Windows.Forms.Label lblCrawledCount;
        private System.Windows.Forms.Label lblDepth;
        private System.Windows.Forms.Label lblEndTime;
        private System.Windows.Forms.Label lblStartTime;
        private System.Windows.Forms.Label lblBaseDomain;
        private System.Windows.Forms.Button btnDetails;
        private System.Windows.Forms.Button btnConfig;
    }
}
