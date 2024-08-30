using ClientDatabaseApp.Services.Repositories;
using ClientDatabaseApp.Services;
using Moq;
using Prism.Events;
using ClientDatabaseApp.Services.Events;
using ClientDatabaseApp.ViewModels;
using System.Linq;
using Xunit;
using System.Windows.Forms;
using System.IO;
using ClientDatabaseApp.Models;
using ClientDatabaseApp.Services.Exceptions;
using System.Threading.Tasks;

namespace UnitTest.ViewModelsTests
{
    public class LoadCSVViewModelTests
    {
        private readonly Mock<IClientRepo> _clientRepoMock;
        private readonly Mock<IDialogService> _dialogServiceMock;
        private readonly Mock<IEventAggregator> _eventAggregatorMock;
        private readonly LoadCSVViewModel _viewModel;

        public LoadCSVViewModelTests()
        {
            _clientRepoMock = new Mock<IClientRepo>();
            _dialogServiceMock = new Mock<IDialogService>();
            _eventAggregatorMock = new Mock<IEventAggregator>();

            _eventAggregatorMock.Setup(x => x.GetEvent<ActivityAddedToDatabaseEvent>())
                                .Returns(new Mock<ActivityAddedToDatabaseEvent>().Object);
            _eventAggregatorMock.Setup(x => x.GetEvent<ActivityRemovedFromDatabaseEvent>())
                                .Returns(new Mock<ActivityRemovedFromDatabaseEvent>().Object);
            _eventAggregatorMock.Setup(x => x.GetEvent<ActivityUpdatedInDatabaseEvent>())
                                .Returns(new Mock<ActivityUpdatedInDatabaseEvent>().Object);

            _viewModel = new LoadCSVViewModel(
                _clientRepoMock.Object,
                _dialogServiceMock.Object
            );
        }

        [Fact]
        public void GetClientsFromCSV_ShouldPopulatePreviewClients_WhenFileIsSelected()
        {
            // Arrange
            var filePath = Path.GetTempFileName();
            var path = Path.GetDirectoryName(filePath);
            var csvContent = "ClientName,Phonenumber,Email,City,Facebook,Instagram,PageURL,Data,Owner,Note\nJohn Doe,123123123,,Warszawa,,,,2023-11-15,Client,Note";
            File.WriteAllText(filePath, csvContent);

            _dialogServiceMock.Setup(ds => ds.OpenFileDialog(It.IsAny<string>(), It.IsAny<string>()))
                             .Returns(filePath);

            // Act
            _viewModel.GetClientsFromCSVCommand.Execute(null);
            
            // Assert
            Assert.Single(_viewModel.PreviewClients);
            Assert.Equal("John Doe", _viewModel.PreviewClients.First().ClientName);

            // Clean up
            File.Delete(filePath);
        }



        [Fact]
        public void AddToDatabase_ShouldClearPreviewClients_WhenClientsAddedSuccessfully()
        {
            // Arrange
            _viewModel.PreviewClients.Add(new Client { ClientName = "Jane Doe" });

            // Act
            _viewModel.AddToDatabaseCommand.Execute(null);

            // Assert
            Assert.Empty(_viewModel.PreviewClients);
            _clientRepoMock.Verify(repo => repo.AddClient(It.Is<Client>(c => c.ClientName == "Jane Doe")), Times.Once);
        }

        [Fact]
        public void AddToDatabase_ShouldShowErrorMessage_WhenClientAlreadyExists()
        {
            // Arrange
            var client = new Client { ClientName = "Existing Client" };
            _viewModel.PreviewClients.Add(client);

            _clientRepoMock.Setup(repo => repo.AddClient(It.IsAny<Client>()))
                      .ThrowsAsync(new ClientAlreadyExistsException("Client exists"));

            // Act
            _viewModel.AddToDatabaseCommand.Execute(null);

            // Assert
            _dialogServiceMock.Verify(ds => ds.ShowMessage(It.Is<string>(msg => msg.Contains("Wystąpił błąd"))), Times.Once);
        }

    }
}
