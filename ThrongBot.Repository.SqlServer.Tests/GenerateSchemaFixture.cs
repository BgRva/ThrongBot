using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThrongBot.Common.Entities;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace ThrongBot.Repository.SqlServer.Tests
{
    [TestFixture]
    public class SchemaTest
    {
        [Test]
        public void Can_Generate_Schema()
        {
            //Arrange
            var cfg = new Configuration();
            cfg.Configure();
            cfg.AddAssembly(typeof(CrawlerRun).Assembly);

            //Act
            var schema = new SchemaExport(cfg);
            schema.Execute(false, true, false);
        }
    }
}