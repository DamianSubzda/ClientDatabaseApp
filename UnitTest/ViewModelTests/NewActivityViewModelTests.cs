using ClientDatabaseApp.Services.Repositories;
using ClientDatabaseApp.Services;
using Moq;
using ClientDatabaseApp.ViewModels;
using Xunit;
using System.Windows.Controls;
using ClientDatabaseApp.Models;
using System;

namespace UnitTest.ViewModelsTests
{
    public class NewActivityViewModelTests
    {
        private readonly Mock<IActivityRepo> _activityRepoMock;
        private readonly Mock<IDialogService> _dialogServiceMock;
        private readonly NewActivityViewModel _viewModel;
        private readonly Client _testClient;

        public NewActivityViewModelTests()
        {
            _activityRepoMock = new Mock<IActivityRepo>();
            _dialogServiceMock = new Mock<IDialogService>();
            _testClient = new Client { ClientName = "Test Client" };
            
            _viewModel = new NewActivityViewModel(
                _testClient, 
                _activityRepoMock.Object, 
                _dialogServiceMock.Object
            );
        }

        [StaFact]
        public void AddActivity_ShouldShowMessage_WhenNoteIsEmpty()
        {
            // Arrange
            RichTextBox richTextBox = new RichTextBox();

            // Act
            _viewModel.AddActivityCommand.Execute(richTextBox);

            // Assert
            _dialogServiceMock.Verify(ds => ds.ShowMessage("Brak podanej notatki!"), Times.Once);
        }

        [StaFact]
        public void AddActivity_ShouldCallCreateActivity_WhenNoteIsProvided()
        {
            // Arrange
            var richTextBox = new RichTextBox();
            richTextBox.AppendText("Sample Note");

            // Act
            _viewModel.AddActivityCommand.Execute(richTextBox);

            // Assert
            _activityRepoMock.Verify(ar => ar.CreateActivity(_testClient, _viewModel.SelectedDate, "Sample Note"), Times.Once);
            _dialogServiceMock.Verify(ds => ds.ShowMessage(It.IsAny<string>()), Times.Never);
        }

        [StaFact]
        public void AddActivity_ShouldShowErrorMessage_WhenCreateActivityFails()
        {
            // Arrange
            var richTextBox = new RichTextBox();
            richTextBox.AppendText("Sample Note");
            _activityRepoMock.Setup(ar => ar.CreateActivity(It.IsAny<Client>(), It.IsAny<DateTime>(), It.IsAny<string>()))
                             .Throws(new Exception());

            // Act
            _viewModel.AddActivityCommand.Execute(richTextBox);

            // Assert
            _dialogServiceMock.Verify(ds => ds.ShowMessage("Wystąpił błąd podczas próby stworzenia nowego wydarzenia! \nSprawdź wprowadzone dane!"), Times.Once);
        }

        [Fact]
        public void ExitWindow_ShouldInvokeCloseAction_WhenCloseActionIsSet()
        {
            // Arrange
            var closeActionCalled = false;
            _viewModel.CloseAction = () => closeActionCalled = true;

            // Act
            _viewModel.ExitWindow(null);

            // Assert
            Assert.True(closeActionCalled);
        }

        [Fact]
        public void ExitWindow_ShouldNotThrowException_WhenCloseActionIsNull()
        {
            // Arrange
            _viewModel.CloseAction = null;

            // Act
            var exception = Record.Exception(() => _viewModel.ExitWindow(null));

            // Assert
            Assert.Null(exception);
        }

    }
}
