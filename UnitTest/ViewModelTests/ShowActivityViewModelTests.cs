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

namespace UnitTest.ViewModelsTests
{
    public class ShowActivityViewModelTests
    {
        private readonly Mock<IActivityRepo> _activityRepoMock;
        private readonly Mock<IClientRepo> _clientRepoMock;
        private readonly Mock<IDialogService> _dialogServiceMock;
        private readonly ShowActivityViewModel _viewModel;
        private readonly Activity _testActivity;
        private readonly Client _testClient;

        public ShowActivityViewModelTests()
        {
            _activityRepoMock = new Mock<IActivityRepo>();
            _clientRepoMock = new Mock<IClientRepo>();
            _dialogServiceMock = new Mock<IDialogService>();
            _testActivity = new Activity { ClientId = 1, Note = "Test Note", DateOfCreation = DateTime.Now };
            _testClient = new Client { ClientName = "John Doe" };

            _clientRepoMock.Setup(repo => repo.GetClient(It.IsAny<int>())).ReturnsAsync(_testClient);

            _viewModel = new ShowActivityViewModel(
                _testActivity,
                _clientRepoMock.Object,
                _activityRepoMock.Object, 
                _dialogServiceMock.Object
            );
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

        [StaFact]
        public void EditCommand_ShouldUpdateActivity_WhenNoteIsValid()
        {
            // Arrange

            _activityRepoMock.Setup(repo => repo.UpdateActivity(It.IsAny<Activity>())).Returns(Task.CompletedTask);

            var richTextBox = new RichTextBox { Document = new FlowDocument(new Paragraph(new Run("New Note"))) };

            // Act
            _viewModel.EditCommand.Execute(richTextBox);

            // Assert
            Assert.Equal("New Note", _viewModel.OriginalNote);
            _activityRepoMock.Verify(repo => repo.UpdateActivity(It.IsAny<Activity>()), Times.Once);
        }

        [StaFact]
        public void EditCommand_ShouldShowMessage_WhenUpdateFails()
        {
            // Arrange

            _activityRepoMock.Setup(repo => repo.UpdateActivity(It.IsAny<Activity>())).ThrowsAsync(new Exception("Update failed"));
            
            var richTextBox = new RichTextBox { Document = new FlowDocument(new Paragraph(new Run("New Note"))) };

            // Act
            _viewModel.EditCommand.Execute(richTextBox);

            // Assert
            _dialogServiceMock.Verify(ds => ds.ShowMessage("Wystąpił błąd podczas aktualizowania informacji o wydarzeniu! \nSprawdź wprowadzone dane!"), Times.Once);
        }

        [Fact]
        public void LoadClientNameAsync_ShouldSetClientName_WhenClientExists()
        {
            // Assert
            Assert.Equal("John Doe", _viewModel.ClientName);
        }


    }
}
