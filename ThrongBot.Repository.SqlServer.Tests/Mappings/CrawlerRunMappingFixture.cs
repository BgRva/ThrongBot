using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace ThrongBot.Repository.SqlServer.Tests.Mappings
{
    [TestFixture]
    public class CrawlerRunMappingFixture
    {
        //[Test]
        //public void CanGenerateXmlMapping()
        //{
        //    //Arrange
        //    var mapper = new ModelMapper();
        //    mapper.AddMapping<CrawlerRunMapping>();

        //    var mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();
        //    var xmlSerializer = new XmlSerializer(mapping.GetType());

        //    var writer = new StringWriter();

        //    //Act
        //    xmlSerializer.Serialize(writer, mapping);

        //    //Assert
        //    var result = writer.ToString();
        //    Assert.NotNull(result);
        //}
    }
}
