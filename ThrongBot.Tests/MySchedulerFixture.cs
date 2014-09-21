using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThrongBot.Common;
using ThrongBot.Common.Entities;
using Xunit;

namespace ThrongBot.Tests
{
    public class MySchedulerFixture
    {
        [Fact]
        public void GetInstanceOf_Returns_Instance_Of_MyCrawledPageProcessor_When_T_Is_ICrawledPageProcessor()
        {
            //Arrange
            var provider = new LogicProvider();

            //Act
            var result = provider.GetInstanceOf<ICrawledPageProcessor>();

            //Assert
            Assert.NotNull(result);
            Assert.IsType<MyCrawledPageProcessor>(result);
        }
    }
}
