using ClientDatabaseApp.Services.Repositories;
using ClientDatabaseApp.Services;
using Moq;
using ClientDatabaseApp.ViewModels;
using Xunit;
using System.Windows.Controls;
using ClientDatabaseApp.Models;
using System;
using System.Threading.Tasks;
using System.Windows.Documents;
using ClientDatabaseApp.Services.Utilities;
using static ClientDatabaseApp.Services.Utilities.ComboboxStatus;
using System.Collections.ObjectModel;

namespace UnitTest.ViewModelsTests
{
    public class ShowClientViewModelTests
    {
        private readonly Mock<IClientRepo> _clientRepoMock;
        private readonly Mock<IDialogService> _dialogServiceMock;
        private readonly Mock<IComboboxStatus> _comboboxStatusMock;
        private readonly ShowClientViewModel _viewModel;
        private readonly Client _testClient;

        public ShowClientViewModelTests()
        {
            _clientRepoMock = new Mock<IClientRepo>();
            _dialogServiceMock = new Mock<IDialogService>();
            _comboboxStatusMock = new Mock<IComboboxStatus>();
            _testClient = new Client
            {
                ClientName = "John Doe",
                Phonenumber = "123456789",
                Email = "john.doe@example.com",
                City = "New York",
                Facebook = "facebook.com/johndoe",
                Instagram = "instagram.com/johndoe",
                PageURL = "example.com",
                Data = new DateTime(2024, 1, 1),
                Owner = "Owner",
                Note = "Some note",
                Status = 0
            };
            _clientRepoMock.Setup(repo => repo.GetClient(It.IsAny<int>())).ReturnsAsync(_testClient);

            var statusItems = new ObservableCollection<StatusItem>
            {
                new StatusItem { Value = Status.NewClient, Description = "Nowy klient" },
                new StatusItem { Value = Status.InProgress, Description = "Trwają rozmowy" },
                new StatusItem { Value = Status.ScheduledMeeting, Description = "Umówione spotkanie" },
                new StatusItem { Value = Status.NotInterested, Description = "Nie chcą/mają kogoś" },
                new StatusItem { Value = Status.MissedCall, Description = "Nie odebrane" },
                new StatusItem { Value = Status.ImmediateAction, Description = "Akcja do zrobienia instant" },
                new StatusItem { Value = Status.DeferredAction, Description = "Akcja do zrobienia w dłuższym przedziale czasu" },
                new StatusItem { Value = Status.VeryLongTermAction, Description = "Akcja do zrobienia bardzo do przodu" },
                new StatusItem { Value = Status.NotConsideredInStatistics, Description = "Tak oznaczonych firm nie bierzemy pod uwagę w statystykach" }
            };

            _comboboxStatusMock.Setup(x => x.GetStatusItems()).Returns(statusItems);

            _viewModel = new ShowClientViewModel(
                _testClient,
                _clientRepoMock.Object,
                _dialogServiceMock.Object,
                _comboboxStatusMock.Object

            );
        }

        [Fact]
        public void EditDataCommand_ShouldSetIsEditingToTrue()
        {
            // Act
            _viewModel.EditDataCommand.Execute(null);

            // Assert
            Assert.True(_viewModel.IsEditing);
        }

        [StaFact]
        public void SaveDataCommand_ShouldUpdateClient_WhenNoteIsValid()
        {
            // Arrange
            _clientRepoMock.Setup(repo => repo.UpdateClient(It.IsAny<Client>())).Verifiable();

            var richTextBox = new RichTextBox { Document = new FlowDocument(new Paragraph(new Run("Valid Note"))) };

            // Act
            _viewModel.SaveDataCommand.Execute(richTextBox);

            // Assert
            _clientRepoMock.Verify(repo => repo.UpdateClient(It.IsAny<Client>()), Times.Once);
            Assert.False(_viewModel.IsEditing);
        }

        [StaFact]
        public void SaveDataCommand_ShouldShowMessage_WhenUpdateFails()
        {
            // Arrange
            _clientRepoMock.Setup(repo => repo.UpdateClient(It.IsAny<Client>())).Throws(new Exception("Update failed"));

            var richTextBox = new RichTextBox { Document = new FlowDocument(new Paragraph(new Run("Valid Note"))) };

            // Act
            _viewModel.SaveDataCommand.Execute(richTextBox);

            // Assert
            _dialogServiceMock.Verify(ds => ds.ShowMessage("Wystąpił błąd podczas aktualizowania informacji o kliencie! \nSprawdź wprowadzone dane!"), Times.Once);
            Assert.False(_viewModel.IsEditing);
        }

        [Fact]
        public void ExitCommand_ShouldInvokeCloseAction()
        {
            // Arrange
            var closeActionInvoked = false;
            _viewModel.CloseAction = () => closeActionInvoked = true;

            // Act
            _viewModel.ExitCommand.Execute(null);

            // Assert
            Assert.True(closeActionInvoked);
        }

        [Fact]
        public void Client_Setter_ShouldInitializeProperties()
        {
            // Act & Assert
            Assert.Equal("John Doe", _viewModel.ClientNameTextBox);
            Assert.Equal("123456789", _viewModel.PhonenumberTextBox);
            Assert.Equal("john.doe@example.com", _viewModel.EmailTextBox);
            Assert.Equal("New York", _viewModel.CityTextBox);
            Assert.Equal("facebook.com/johndoe", _viewModel.FacebookTextBox);
            Assert.Equal("instagram.com/johndoe", _viewModel.InstagramTextBox);
            Assert.Equal("example.com", _viewModel.PageURLTextBox);
            Assert.Equal(new DateTime(2024, 1, 1), _viewModel.DateTextBox);
            Assert.Equal("Owner", _viewModel.OwnerTextBox);
            Assert.Equal("Some note", _viewModel.RichTextContent);
            Assert.Equal("Nowy klient", _viewModel.SelectedStatus.Description);
        }


    }
}
