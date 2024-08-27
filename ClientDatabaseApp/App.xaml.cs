using ClientDatabaseApp.Services.Repositories;
using ClientDatabaseApp.Views;
using System.Data.Entity;
using System.Windows;
using Unity.Lifetime;
using Unity;
using ClientDatabaseApp.Services;
using Prism.Events;
using ClientDatabaseApp.Services.APIClients;

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
            Container.RegisterType<IDialogService, DialogService>();
            Container.RegisterType<IEventAggregator, EventAggregator>(new ContainerControlledLifetimeManager());

            Container.RegisterType<IIpifyAPIConnector, IpifyAPIConnector>();
            Container.RegisterType<IGeolocationAPIConnector, GeolocationAPIConnector>();
            Container.RegisterType<IOpenweatherAPIConnector, OpenweatherAPIConnector>();

            var mainWindow = Container.Resolve<MainWindow>();
            mainWindow.Show();
        }
    }
}
