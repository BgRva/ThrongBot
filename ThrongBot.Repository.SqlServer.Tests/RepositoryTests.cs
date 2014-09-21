using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThrongBot.Common;
using ThrongBot.Common.Entities;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace ThrongBot.Repository.SqlServer.Tests
{
    [TestFixture]
    public class RepositoryTests
    {
        private IRepository _repo;

        [SetUp]
        public void CreateSchema()
        {
            var schemaUpdate = new SchemaUpdate(NHibernateHelper.Configuration);
            schemaUpdate.Execute(false, true);
        }

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
