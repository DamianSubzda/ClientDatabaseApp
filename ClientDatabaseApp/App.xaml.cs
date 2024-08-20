using ClientDatabaseApp.Service.Repository;
using ClientDatabaseApp.View;
using System.Data.Entity;
using System.Windows;
using Unity.Lifetime;
using Unity;

namespace ClientDatabaseApp
{
    /// <summary>
    /// Logika interakcji dla klasy App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IUnityContainer Container { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Container = new UnityContainer();
            Container.RegisterType<DbContext, PostgresContext>(new HierarchicalLifetimeManager());
            Container.RegisterType<IClientRepo, ClientRepo>();
            Container.RegisterType<IActivityRepo, ActivityRepo>();

            var mainWindow = Container.Resolve<MainWindow>();
            mainWindow.Show();
        }
    }
}
