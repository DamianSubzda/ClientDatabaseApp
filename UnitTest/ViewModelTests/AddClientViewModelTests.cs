using ClientDatabaseApp.Models;
using ClientDatabaseApp.Services;
using ClientDatabaseApp.Services.Repositories;
using ClientDatabaseApp.Services.Utilities;
using ClientDatabaseApp.ViewModels;
using Moq;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using Xunit;
using static ClientDatabaseApp.Services.Utilities.ComboboxStatus;

namespace UnitTests.ViewModels.Tests
{
    public class AddClientViewModelTests
    {
        private readonly Mock<IClientRepo> _clientRepoMock;
        private readonly Mock<IDialogService> _dialogServiceMock;
        private readonly Mock<IComboboxStatus> _comboboxStatusMock;
        private readonly AddClientViewModel _viewModel;

        public AddClientViewModelTests()
        {
            _clientRepoMock = new Mock<IClientRepo>();
            _dialogServiceMock = new Mock<IDialogService>();
            _comboboxStatusMock = new Mock<IComboboxStatus>();

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

            _viewModel = new AddClientViewModel(
                _clientRepoMock.Object,
                _dialogServiceMock.Object,
                _comboboxStatusMock.Object
            );
        }

        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            // Assert
            Assert.NotNull(_viewModel.StatusItems);
            Assert.Equal(9, _viewModel.StatusItems.Count);
            Assert.Equal(_viewModel.StatusItems[0], _viewModel.SelectedStatus);
            Assert.Equal(DateTime.Now.Date, _viewModel.DateTextBox.Date);
        }

        [StaFact]
        public void SaveRichTextContent_ShouldUpdateRichTextContent()
        {
            // Arrange
            var richTextBox = new RichTextBox();
            richTextBox.AppendText("Some content");

            // Act
            _viewModel.SaveRichTextContent(richTextBox);

            // Assert
            Assert.Equal("Some content", _viewModel.RichTextContent);
        }

        [StaFact]
        public void AddClientAsync_ShouldAddClient_WhenValidData()
        {
            // Arrange
            var richTextBox = new RichTextBox();
            richTextBox.AppendText("Note content");

            _viewModel.ClientNameTextBox = "Client Name";
            _viewModel.EmailTextBox = "client@example.com";
            _viewModel.SelectedStatus = _viewModel.StatusItems[0];

            _clientRepoMock.Setup(cr => cr.AddClient(It.IsAny<Client>())).Returns(Task.CompletedTask);

            // Act
            _viewModel.AddClientAsync(richTextBox);

            // Assert
            _clientRepoMock.Verify(cr => cr.AddClient(It.Is<Client>(c =>
                c.ClientName == "Client Name" &&
                c.Email == "client@example.com" &&
                c.Note == "Note content" &&
                c.Status == 0
            )), Times.Once);

            Assert.Empty(_viewModel.ClientNameTextBox);
            Assert.Empty(_viewModel.EmailTextBox);
            Assert.Empty(_viewModel.RichTextContent);
            Assert.Equal(_viewModel.StatusItems[0], _viewModel.SelectedStatus);
        }

        [StaFact]
        public void AddClientAsync_ShouldShowMessage_WhenClientNameIsEmpty()
        {
            // Arrange
            var richTextBox = new RichTextBox();
            _viewModel.ClientNameTextBox = "";

            // Act
            _viewModel.AddClientAsync(richTextBox);

            // Assert
            _dialogServiceMock.Verify(ds => ds.ShowMessage("Brak nazwy klienta!"), Times.Once);
        }

        [StaFact]
        public void AddClientAsync_ShouldShowMessage_WhenExceptionOccurs()
        {
            // Arrange
            var richTextBox = new RichTextBox();
            _viewModel.ClientNameTextBox = "Client Name";

            _clientRepoMock.Setup(cr => cr.AddClient(It.IsAny<Client>())).ThrowsAsync(new Exception());

            // Act
            _viewModel.AddClientAsync(richTextBox);

            // Assert
            _dialogServiceMock.Verify(ds => ds.ShowMessage(It.Is<string>(s => s.Contains("Wystąpił błąd"))), Times.Once);
        }
    }
}
