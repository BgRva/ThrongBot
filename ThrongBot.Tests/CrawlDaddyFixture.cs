using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Abot.Poco;
using ThrongBot.Common;
using Moq;
using Xunit;

namespace ThrongBot.Tests
{
    public class CrawlDaddyFixture
    {
        [Fact]
        public void IsPageToBeProcessed_Returns_True_If_Status_Is_Ok_And_Url_Is_Not_Blacklisted_Or_Processed()
        {
            //Arrange
            var mockProvider = new Mock<ILogicProvider>();
            var mockRepo = new Mock<IRepository>();
            var uri = new Uri("http://www.x.com");
            var code = HttpStatusCode.OK;

            #region Set expectations

            mockRepo.Setup(m => m.IsBlackListed(uri.GetBaseDomain()))
                    .Returns(false);

            mockRepo.Setup(m => m.IsPageProcessed("blah"))
                    .Returns(false);

            #endregion

            //Act
            var processor = new CrawlDaddy(mockProvider.Object, mockRepo.Object);
            var result = processor.IsPageToBeProcessed(uri, code);

            //Assert
            Assert.True(result);
        }
        
        [Fact]
        public void IsPageToBeProcessed_Returns_False_If_Url_Is_Blacklisted()
        {
            //Arrange
            var mockProvider = new Mock<ILogicProvider>();
            var mockRepo = new Mock<IRepository>();
            var uri = new Uri("http://www.x.com");
            var code = HttpStatusCode.OK;

            #region Set expectations

            mockRepo.Setup(m => m.IsBlackListed(It.IsAny<string>()))
                    .Returns(true)
                    .Verifiable();

            #endregion

            //Act
            var processor = new CrawlDaddy(mockProvider.Object, mockRepo.Object);
            var result = processor.IsPageToBeProcessed(uri, code);

            //Assert
            Assert.False(result);
            mockRepo.Verify();
        }

        [Fact]
        public void IsPageToBeProcessed_Returns_False_If_Url_Is_Processed()
        {
            //Arrange
            var mockProvider = new Mock<ILogicProvider>();
            var mockRepo = new Mock<IRepository>();
            var uri = new Uri("http://www.x.com");
            var code = HttpStatusCode.OK;

            #region Set expectations

            mockRepo.Setup(m => m.IsBlackListed(It.IsAny<string>()))
                    .Returns(false)
                    .Verifiable();

            mockRepo.Setup(m => m.IsPageProcessed(It.IsAny<string>()))
                    .Returns(true)
                    .Verifiable();

            #endregion

            //Act
            var processor = new CrawlDaddy(mockProvider.Object, mockRepo.Object);
            var result = processor.IsPageToBeProcessed(uri, code);

            //Assert
            Assert.False(result);
            mockRepo.Verify();
        }

        [Fact]
        public void IsPageToBeProcessed_Returns_False_If_HttpStatusCode_Is_Not_Ok()
        {
            //Arrange
            var mockProvider = new Mock<ILogicProvider>();
            var mockRepo = new Mock<IRepository>();
            var uri = new Uri("http://www.x.com");
            var code = HttpStatusCode.PartialContent;

            #region Set expectations

            mockRepo.Setup(m => m.IsBlackListed(It.IsAny<string>()))
                    .Returns(false);

            mockRepo.Setup(m => m.IsPageProcessed(It.IsAny<string>()))
                    .Returns(false);

            #endregion

            //Act
            var processor = new CrawlDaddy(mockProvider.Object, mockRepo.Object);
            var result = processor.IsPageToBeProcessed(uri, code);

            //Assert
            Assert.False(result);
        }

    }
}

