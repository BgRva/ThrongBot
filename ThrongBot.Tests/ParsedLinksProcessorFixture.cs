using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Abot.Poco;
using ThrongBot.Common;
using ThrongBot.Common.Entities;
using Moq;
using Xunit;
using Xunit.Extensions;

namespace ThrongBot.Tests
{
    public class ParsedLinksProcessorFixture
    {
        [Theory]
        [InlineData(new[] { "http://www.A.com/" }, "http://www.A.com", new[] { "http://www.A.com" })]
        [InlineData(new[] { "http://www.A.com/", "http://www.A.com/A", "http://www.a.com/B" }, "http://www.A.com", new[] { "http://www.A.com" })]
        [InlineData(new[] { "http://www.A.com/", "http://www.A.com/A", "http://www.a.com/B" }, "http://www.a.com/A", new[] { "http://www.a.com/A" })]
        [InlineData(new[] { "http://www.A.com/", "http://www.A.com/A", "http://www.a.com/B" }, "HTTP://WWW.A.COM/A", new[] { "HTTP://WWW.A.COM/A" })]
        public void ProcessLink_Adds_Duplicate_To_List_Of_Links_To_Bypass 
            (string[] currentLinksToCrawl, string duplicateLink, string[] expectedLinksToBypass)
        {
            //Arrange
            var page = new CrawledPage(new Uri("http://www.z.com"));
         //   page.PageBag.SessionId = 3;
        //    page.PageBag.CrawlerId = 4;
            var inputLinks = new List<Uri>();
            page.ParsedLinks = inputLinks;

            var targetUri = new Uri(duplicateLink);

            var mockProvider = new Mock<ILogicProvider>();
            var mockFactory = new Mock<IModelFactory>();
            var processor = new ParsedLinksProcessor(mockProvider.Object);
            processor.LinksToByPass = new List<CrawledLink>();
            processor.MapOfLinksToCrawl = new Dictionary<string, LinkToCrawl>();

            foreach (var url in currentLinksToCrawl)
            {
                var uri = new Uri(url);
                processor.MapOfLinksToCrawl.Add(uri.AbsoluteUri, new LinkToCrawl(){TargetUrl = url, TargetBaseDomain = uri.GetBaseDomain()});
            }

            #region Set expectations

            mockFactory.Setup(m => m.CreateCrawledLink(It.IsAny<Uri>(), It.IsAny<Uri>(), It.IsAny<int>(), It.IsAny<int>()))
                        .Returns(new CrawledLink() { TargetUrl = duplicateLink })
                        .Verifiable();

            #endregion

            //Act
            processor.ProcessLink(page, mockFactory.Object, targetUri, 3, 4);
            var results = processor.LinksToByPass;

            //Assert
            Assert.NotNull(results);
            Assert.Equal(expectedLinksToBypass.Length, results.Count);
            Assert.Equal(expectedLinksToBypass[0], results[0].TargetUrl);
            mockFactory.Verify();
        }

        [Theory]
        [InlineData(new[] { "http://www.A.com/" }, "mailto:X@A.com", new[] { "mailto:X@A.com" })]
        [InlineData(new[] { "http://www.A.com/", "http://www.A.com/A", "http://www.a.com/B" }, "mailto:X@A.com", new[] { "mailto:X@A.com" })]
        public void ProcessLink_Adds_MailTo_Links_To_List_Of_Links_To_Bypass
            (string[] currentLinksToCrawl, string mailToLink, string[] expectedLinksToBypass)
        {
            //Arrange
            var page = new CrawledPage(new Uri("http://www.z.com"));
            //   page.PageBag.SessionId = 3;
            //    page.PageBag.CrawlerId = 4;
            var inputLinks = new List<Uri>();
            page.ParsedLinks = inputLinks;

            var targetUri = new Uri(mailToLink);

            var mockProvider = new Mock<ILogicProvider>();
            var mockFactory = new Mock<IModelFactory>();
            var processor = new ParsedLinksProcessor(mockProvider.Object);
            processor.LinksToByPass = new List<CrawledLink>();
            processor.MapOfLinksToCrawl = new Dictionary<string, LinkToCrawl>();

            foreach (var url in currentLinksToCrawl)
            {
                var uri = new Uri(url);
                processor.MapOfLinksToCrawl.Add(uri.AbsoluteUri, new LinkToCrawl() { TargetUrl = url, TargetBaseDomain = uri.GetBaseDomain() });
            }

            #region Set expectations

            mockFactory.Setup(m => m.CreateCrawledLink(It.IsAny<Uri>(), It.IsAny<Uri>(), It.IsAny<int>(), It.IsAny<int>()))
                        .Returns(new CrawledLink() { TargetUrl = mailToLink })
                        .Verifiable();

            #endregion

            //Act
            processor.ProcessLink(page, mockFactory.Object, targetUri, 3, 4);
            var results = processor.LinksToByPass;

            //Assert
            Assert.NotNull(results);
            Assert.Equal(expectedLinksToBypass.Length, results.Count);
            Assert.Equal(expectedLinksToBypass[0], results[0].TargetUrl);
            mockFactory.Verify();
        }

        [Theory]
        [InlineData(new[] { "http://www.A.com/" }, "http://www.z.com", new[] { "http://www.z.com" })]
        [InlineData(new[] { "http://www.A.com/", "http://www.A.com/A", "http://www.a.com/B" }, "http://www.z.com", new[] { "http://www.z.com" })]
        [InlineData(new[] { "http://www.A.com/", "http://www.A.com/A", "http://www.a.com/B" }, "HTTP://WWW.Z.COM", new[] { "HTTP://WWW.Z.COM" })]
        public void ProcessLink_Adds_Exact_Self_Loops_To_List_Of_Links_To_Bypass
            (string[] currentLinksToCrawl, string exactSelfLoopLink, string[] expectedLinksToBypass)
        {
            //Arrange
            var page = new CrawledPage(new Uri("http://www.z.com"));
            //   page.PageBag.SessionId = 3;
            //    page.PageBag.CrawlerId = 4;
            var inputLinks = new List<Uri>();
            page.ParsedLinks = inputLinks;

            var targetUri = new Uri(exactSelfLoopLink);

            var mockProvider = new Mock<ILogicProvider>();
            var mockFactory = new Mock<IModelFactory>();
            var processor = new ParsedLinksProcessor(mockProvider.Object);
            processor.LinksToByPass = new List<CrawledLink>();
            processor.MapOfLinksToCrawl = new Dictionary<string, LinkToCrawl>();

            foreach (var url in currentLinksToCrawl)
            {
                var uri = new Uri(url);
                processor.MapOfLinksToCrawl.Add(uri.AbsoluteUri, new LinkToCrawl() { TargetUrl = url, TargetBaseDomain = uri.GetBaseDomain() });
            }

            #region Set expectations

            mockFactory.Setup(m => m.CreateCrawledLink(It.IsAny<Uri>(), It.IsAny<Uri>(), It.IsAny<int>(), It.IsAny<int>()))
                        .Returns(new CrawledLink() { TargetUrl = exactSelfLoopLink })
                        .Verifiable();

            #endregion

            //Act
            processor.ProcessLink(page, mockFactory.Object, targetUri, 3, 4);
            var results = processor.LinksToByPass;

            //Assert
            Assert.NotNull(results);
            Assert.Equal(expectedLinksToBypass.Length, results.Count);
            Assert.Equal(expectedLinksToBypass[0], results[0].TargetUrl);
            mockFactory.Verify();
        }


        [Theory]
        [InlineData(new[] { "http://www.A.com/" }, "http://www.A.com/A", new[] { "http://www.A.com", "http://www.A.com/A" })]
        [InlineData(new[] { "http://www.A.com/" }, "http://www.A.com/index.htm", new[] { "http://www.A.com", "http://www.A.com/index.htm" })]
        [InlineData(new[] { "http://www.A.com/" }, "http://www.A.com/b/c?blahlalla", new[] { "http://www.A.com", "http://www.A.com/b/c?blahlalla" })]
        public void ProcessLink_Adds_New_Links_To_List_Of_Links_To_Crawl
            (string[] currentLinksToCrawl, string targetLink, string[] expectedLinksToCrawl)
        {
            //Arrange
            var page = new CrawledPage(new Uri("http://www.z.com"));
            //   page.PageBag.SessionId = 3;
            //    page.PageBag.CrawlerId = 4;
            var inputLinks = new List<Uri>();
            page.ParsedLinks = inputLinks;

            var targetUri = new Uri(targetLink);

            var mockProvider = new Mock<ILogicProvider>();
            var mockFactory = new Mock<IModelFactory>();
            var processor = new ParsedLinksProcessor(mockProvider.Object);
            processor.MapOfLinksToCrawl = new Dictionary<string, LinkToCrawl>();

            foreach (var url in currentLinksToCrawl)
            {
                var uri = new Uri(url);
                processor.MapOfLinksToCrawl.Add(uri.AbsoluteUri, new LinkToCrawl() { TargetUrl = url, TargetBaseDomain = uri.GetBaseDomain() });
            }

            #region Set expectations

            mockFactory.Setup(m => m.CreateLinkToCrawl(It.IsAny<CrawledPage>(), It.IsAny<Uri>(), It.IsAny<int>()))
                        .Returns(new LinkToCrawl() { TargetUrl = targetLink })
                        .Verifiable();

            #endregion

            //Act
            processor.ProcessLink(page, mockFactory.Object, targetUri, 3, 4);
            var results = processor.MapOfLinksToCrawl;

            //Assert
            Assert.NotNull(results);
            Assert.Equal(expectedLinksToCrawl.Length, results.Count);
            Assert.True(results.ContainsKey(targetUri.AbsoluteUri));
            mockFactory.Verify();
        }



        [Theory]
        [InlineData(new[] { "http://www.A.com/" }, "http://www.A.com/A", new[] { "http://www.A.com", "http://www.A.com/A" })]
        [InlineData(new[] { "http://www.A.com/" }, "http://www.A.com/index.htm", new[] { "http://www.A.com", "http://www.A.com/index.htm" })]
        [InlineData(new[] { "http://www.A.com/" }, "http://www.A.com/b/c?blahlalla", new[] { "http://www.A.com", "http://www.A.com/b/c?blahlalla" })]
        public void ProcessLink_Sets_ExternalLinksFound_To_True_If_External_Link_Added_To_List_Of_Links_To_Crawl
            (string[] currentLinksToCrawl, string targetLink, string[] expectedLinksToCrawl)
        {
            //Arrange
            var page = new CrawledPage(new Uri("http://www.z.com"));
            //   page.PageBag.SessionId = 3;
            //    page.PageBag.CrawlerId = 4;
            var inputLinks = new List<Uri>();
            page.ParsedLinks = inputLinks;

            var targetUri = new Uri(targetLink);

            var mockProvider = new Mock<ILogicProvider>();
            var mockFactory = new Mock<IModelFactory>();
            var processor = new ParsedLinksProcessor(mockProvider.Object);
            processor.MapOfLinksToCrawl = new Dictionary<string, LinkToCrawl>();

            foreach (var url in currentLinksToCrawl)
            {
                var uri = new Uri(url);
                processor.MapOfLinksToCrawl.Add(uri.AbsoluteUri, new LinkToCrawl() { TargetUrl = url, TargetBaseDomain = uri.GetBaseDomain() });
            }

            #region Set expectations

            mockFactory.Setup(m => m.CreateLinkToCrawl(It.IsAny<CrawledPage>(), It.IsAny<Uri>(), It.IsAny<int>()))
                        .Returns(new LinkToCrawl() { TargetUrl = targetLink })
                        .Verifiable();

            #endregion

            //Act
            processor.ProcessLink(page, mockFactory.Object, targetUri, 3, 4);
            var results = processor.MapOfLinksToCrawl;

            //Assert
            Assert.True(processor.ExternalLinksFound);
            mockFactory.Verify();
        }



        [Theory]
        [InlineData(new[] { "http://www.A.com/" }, "http://www.A.com/A", new[] { "http://www.A.com", "http://www.A.com/A" })]
        [InlineData(new[] { "http://www.A.com/" }, "http://www.A.com/index.htm", new[] { "http://www.A.com", "http://www.A.com/index.htm" })]
        [InlineData(new[] { "http://www.A.com/" }, "http://www.A.com/b/c?blahlalla", new[] { "http://www.A.com", "http://www.A.com/b/c?blahlalla" })]
        public void ProcessLink_Sets_ExternalLinksFound_To_False_If_No_External_Links_Found
            (string[] currentLinksToCrawl, string targetLink, string[] expectedLinksToCrawl)
        {
            //Arrange
            var page = new CrawledPage(new Uri("http://www.a.com/X/Y/Z"));
            //   page.PageBag.SessionId = 3;
            //    page.PageBag.CrawlerId = 4;
            var inputLinks = new List<Uri>();
            page.ParsedLinks = inputLinks;

            var targetUri = new Uri(targetLink);

            var mockProvider = new Mock<ILogicProvider>();
            var mockFactory = new Mock<IModelFactory>();
            var processor = new ParsedLinksProcessor(mockProvider.Object);
            processor.MapOfLinksToCrawl = new Dictionary<string, LinkToCrawl>();

            foreach (var url in currentLinksToCrawl)
            {
                var uri = new Uri(url);
                processor.MapOfLinksToCrawl.Add(uri.AbsoluteUri, new LinkToCrawl() { TargetUrl = url, TargetBaseDomain = uri.GetBaseDomain() });
            }

            #region Set expectations

            mockFactory.Setup(m => m.CreateLinkToCrawl(It.IsAny<CrawledPage>(), It.IsAny<Uri>(), It.IsAny<int>()))
                        .Returns(new LinkToCrawl() { TargetUrl = targetLink })
                        .Verifiable();

            #endregion

            //Act
            processor.ProcessLink(page, mockFactory.Object, targetUri, 3, 4);
            var results = processor.MapOfLinksToCrawl;

            //Assert
            Assert.False(processor.ExternalLinksFound);
            mockFactory.Verify();
        }
    }
}

