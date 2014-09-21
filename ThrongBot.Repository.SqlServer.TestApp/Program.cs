using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThrongBot.Common.Entities;
using NHibernate;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate.Cfg;
using System.Data.SqlClient;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Driver;
using NHibernate.Dialect;
using System.Data;
using System.Reflection;

namespace ThrongBot.Repository.SqlServer.TestApp
{
    class Program
    {
        static void Main()
        {
            ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SqlServerRepository"].ConnectionString;

            Console.WriteLine("WARNING: Will Clear DB Before Tests!  (X to continue) ...");
            var key = Console.ReadKey();
            if (key.Key == ConsoleKey.X)
            {
                Console.WriteLine("Running tests ...");
                RunRepoTestsForSqlExpress2008();
            }
            else
                Console.WriteLine("tests skipped.");

            Console.ReadKey();
        }

        public static string ConnectionString { get; set; }

        private static void RunRepoTestsForSqlExpress2008()
        {
            var test = new RepoSqlExpressTest(ConnectionString);
            test.RunTests();
        }

        public static bool TestConnection(string connStr)
        {
            bool result = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    connection.Open();

                    connection.Close();
                }
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                throw;
            }
            return result;
        }
    }
}
