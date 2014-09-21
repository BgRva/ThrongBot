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
    public class ProviderFixture
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

        [Fact]
        public void GetInstanceOf_Returns_Instance_Of_ParsedLinksProcessor_When_T_Is_ILinksProcessor()
        {
            //Arrange
            var provider = new LogicProvider();

            //Act
            var result = provider.GetInstanceOf<ILinksProcessor>();

            //Assert
            Assert.NotNull(result);
            Assert.IsType<ParsedLinksProcessor>(result);
        }

        [Fact]
        public void GetInstanceOf_Returns_Instance_Of_ModelFactory_When_T_Is_IModelFactory()
        {
            //Arrange
            var provider = new LogicProvider();

            //Act
            var result = provider.GetInstanceOf<IModelFactory>();

            //Assert
            Assert.NotNull(result);
            Assert.IsType<ModelFactory>(result);
        }

        [Fact]
        public void GetInstanceOf_Returns_Null_For_Unsupported_Types()
        {
            //Arrange
            var provider = new LogicProvider();

            //Act
            var result = provider.GetInstanceOf<DummyLogic>();

            //Assert
            Assert.Null(result);
        }

        //Helpers
        private class DummyLogic : ILogic
        {

        }
    }

}
