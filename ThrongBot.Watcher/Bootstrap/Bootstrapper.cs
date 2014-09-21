using ThrongBot.Repository.SqlServer;
using Ninject;
using Ninject.Activation;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ThrongBot.Watcher.Bootstrap
{
    public class Bootstrapper 
    {
        private static IKernel _ninjectKernel;

        public static void Initialize(INinjectModule module)
        {
            _ninjectKernel = new StandardKernel(module);
        }

        public static T Resolve<T>()
        {
            return _ninjectKernel.Get<T>();
        }
    }

    public class ApplicationModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ThrongBot.Common.IRepository>().ToProvider(new RepositoryProvider());
            Bind<ThrongBot.Common.ILogicProvider>().To<ThrongBot.LogicProvider>();
            Bind<ThrongBot.Common.ICrawlDaddy>().To<ThrongBot.CrawlDaddy>();
        }
    }

    public class RepositoryProvider: Ninject.Activation.Provider<Repository.SqlServer.Repository>
    {
        protected override Repository.SqlServer.Repository CreateInstance(IContext context)
        {
            var connStr = System.Configuration.ConfigurationManager.ConnectionStrings["SqlServerRepository"].ConnectionString;
            var sessionFactory = NHibernateHelper.SessionFactory;
            var repo = new Repository.SqlServer.Repository(sessionFactory, connStr);
            return repo;
        }
    }
}
