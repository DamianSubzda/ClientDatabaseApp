using ClientDatabaseApp.Services.Repositories;
using ClientDatabaseApp.Services;
using System.Linq;
using Moq;
using Xunit;
using Prism.Events;
using ClientDatabaseApp.ViewModels;
using ClientDatabaseApp.Models;
using System.Collections.Generic;
using ClientDatabaseApp.Services.Events;
using ClientDatabaseApp.Services.Utilities;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Data;
using static ClientDatabaseApp.Services.Utilities.ComboboxStatus;

namespace UnitTest.ViewModelsTests
{
    public class ClientDatabaseViewModelTests
    {
        private readonly Mock<IClientRepo> _clientRepoMock;
        private readonly Mock<IActivityRepo> _activityRepoMock;
        private readonly Mock<IDialogService> _dialogServiceMock;
        private readonly Mock<IEventAggregator> _eventAggregatorMock;
        private readonly Mock<IComboboxStatus> _comboboxStatus;
        private readonly ClientDatabaseViewModel _viewModel;

        public ClientDatabaseViewModelTests()
        {
            _clientRepoMock = new Mock<IClientRepo>();
            _activityRepoMock = new Mock<IActivityRepo>();
            _dialogServiceMock = new Mock<IDialogService>();
            _eventAggregatorMock = new Mock<IEventAggregator>();
            _comboboxStatus = new Mock<IComboboxStatus>();

            _eventAggregatorMock.Setup(x => x.GetEvent<ClientAddedToDatabaseEvent>())
                                .Returns(new Mock<ClientAddedToDatabaseEvent>().Object);
            _eventAggregatorMock.Setup(x => x.GetEvent<ClientUpdatedInDatabaseEvent>())
                                .Returns(new Mock<ClientUpdatedInDatabaseEvent>().Object);
            _eventAggregatorMock.Setup(x => x.GetEvent<ClientRemovedFromDatabaseEvent>())
                                .Returns(new Mock<ClientRemovedFromDatabaseEvent>().Object);
            var statusItems = new ObservableCollection<StatusItem>
            {
                new StatusItem { Value = 0 },
            };
            _comboboxStatus.Setup(x => x.GetStatusItems()).Returns(statusItems);

            _viewModel = new ClientDatabaseViewModel(
                _clientRepoMock.Object,
                _activityRepoMock.Object,
                _dialogServiceMock.Object,
                _eventAggregatorMock.Object,
                _comboboxStatus.Object
            );
        }

        private Client CreateClient(int id) => new Client { ClientId = id, ClientName = "Client " + id };

        private ObservableCollection<Client> CreateClientCollection(int count)
        {
            var clients = new ObservableCollection<Client>();
            for (int i = 1; i <= count; i++)
            {
                clients.Add(CreateClient(i));
            }
            return clients;
        }

        [Fact]
        public async Task ShouldInitializeClientsView()
        {
            // Arrange
            var expectedClients = new List<Client> { new Client { ClientName = "Test Client" } };
            _clientRepoMock.Setup(repo => repo.GetAllClients()).ReturnsAsync(expectedClients);

            // Act
            await _viewModel.LoadClientsAsync();
            var actualClients = _viewModel.ClientsView;

            // Assert
            Assert.NotNull(actualClients);
            Assert.Equal(expectedClients.Count, actualClients.Cast<object>().Count());
        }

        [Fact]
        public void ShowMoreDetailsCommand_ShouldInvokeShowMoreDetailsMethod()
        {
            // Arrange
            var client = CreateClient(1);
            _viewModel.SelectedClient = client;
            var command = _viewModel.ShowMoreDetailsCommand as DelegateCommand<object>;
            
            // Act
            command.Execute(null);

            // Assert
            _dialogServiceMock.Verify(ds => ds.ShowMessage(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void RemoveSelectedCommand_ShouldRemoveClient()
        {
            // Arrange
            var client = CreateClient(1);
            var clients = CreateClientCollection(1);
            _viewModel.ClientsView = CollectionViewSource.GetDefaultView(clients);
            _viewModel.SelectedClient = client;

            _dialogServiceMock.Setup(ds => ds.Confirm(It.IsAny<string>())).Returns(true);
            _clientRepoMock.Setup(cr => cr.DeleteClient(It.IsAny<Client>())).Returns(Task.CompletedTask).Verifiable();

            // Act
            var command = _viewModel.RemoveSelectedCommand as DelegateCommand<object>;
            command.Execute(null);

            // Assert
            Assert.DoesNotContain(client, clients);
            _clientRepoMock.Verify(cr => cr.DeleteClient(client), Times.Once);
        }



        [Fact]
        public void AddActivityCommand_ShouldInvokeAddActivityMethod()
        {
            // Arrange
            var client = CreateClient(1);
            _viewModel.SelectedClient = client;
            var command = _viewModel.AddActivityCommand as DelegateCommand<object>;

            // Act
            command.Execute(null);

            // Assert
            _dialogServiceMock.Verify(ds => ds.ShowMessage(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void FilterCommand_ShouldApplyFilter()
        {
            // Arrange
            _viewModel.FilterText = "Client 1";
            var command = _viewModel.FilterCommand as DelegateCommand<object>;

            var statusItems = new ObservableCollection<ComboboxStatus.StatusItem>
            {
                new ComboboxStatus.StatusItem { Description = "Nowy klient", Value = ComboboxStatus.Status.NewClient, Color = new SolidColorBrush(Colors.White) }
            };

            _comboboxStatus.Setup(c => c.GetStatusItems()).Returns(statusItems);
            _viewModel.StatusItems = statusItems;

            // Act
            command.Execute(null);

            // Assert
            Assert.NotNull(_viewModel.ClientsView.Filter);
        }

    }
}
