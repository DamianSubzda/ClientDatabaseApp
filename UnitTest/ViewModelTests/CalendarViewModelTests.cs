using ClientDatabaseApp.Services.Repositories;
using ClientDatabaseApp.Services;
using Moq;
using Xunit;
using Prism.Events;
using ClientDatabaseApp.ViewModels;
using ClientDatabaseApp.Models;
using System.Collections.Generic;
using ClientDatabaseApp.Services.Events;
using ClientDatabaseApp.Services.Utilities;
using System.Collections.ObjectModel;
using System;
using static ClientDatabaseApp.ViewModels.CalendarViewModel;

namespace UnitTest.ViewModelsTests
{
    public class CalendarViewModelTests
    {
        private readonly Mock<IClientRepo> _clientRepoMock;
        private readonly Mock<IActivityRepo> _activityRepoMock;
        private readonly Mock<IDialogService> _dialogServiceMock;
        private readonly Mock<IEventAggregator> _eventAggregatorMock;
        private readonly CalendarViewModel _viewModel;

        public CalendarViewModelTests()
        {
            _clientRepoMock = new Mock<IClientRepo>();
            _activityRepoMock = new Mock<IActivityRepo>();
            _dialogServiceMock = new Mock<IDialogService>();
            _eventAggregatorMock = new Mock<IEventAggregator>();

            _eventAggregatorMock.Setup(x => x.GetEvent<ActivityAddedToDatabaseEvent>())
                                .Returns(new Mock<ActivityAddedToDatabaseEvent>().Object);
            _eventAggregatorMock.Setup(x => x.GetEvent<ActivityRemovedFromDatabaseEvent>())
                                .Returns(new Mock<ActivityRemovedFromDatabaseEvent>().Object);
            _eventAggregatorMock.Setup(x => x.GetEvent<ActivityUpdatedInDatabaseEvent>())
                                .Returns(new Mock<ActivityUpdatedInDatabaseEvent>().Object);

            var activitiesCount = new List<(int Day, int Count)>
            {
                (1, 2),
                (5, 1),
                (10, 3)
            };

            _activityRepoMock
                .Setup(repo => repo.GetActivitiesCountOfMonth(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(activitiesCount);

            _viewModel = new CalendarViewModel(
                _activityRepoMock.Object,
                _dialogServiceMock.Object,
                _clientRepoMock.Object,
                _eventAggregatorMock.Object
            );
        }

        [Fact]
        public void CalendarViewModel_Initialization_ShouldSetDefaultValues()
        {
            // Assert
            Assert.NotNull(_viewModel.CalendarModel);
            Assert.Equal(((MonthEnum)DateTime.Now.Month).ToString() + " " + DateTime.Now.Year.ToString(), _viewModel.DataToDisplay);
        }

        [Fact]
        public void ButtonClickNextMonth_ShouldChangeMonthByOne()
        {
            // Arrange
            var currentMonth = _viewModel.CalendarModel.DateToDisplay.Month;

            // Act
            _viewModel.ButtonClickNextMonthCommand.Execute(null);

            // Assert
            Assert.Equal(currentMonth + 1, _viewModel.CalendarModel.DateToDisplay.Month);
        }

        [Fact]
        public void ButtonClickPrevMonth_ShouldChangeMonthByOne()
        {
            // Arrange
            var currentMonth = _viewModel.CalendarModel.DateToDisplay.Month;

            // Act
            _viewModel.ButtonClickPrevMonthCommand.Execute(null);

            // Assert
            Assert.Equal(currentMonth - 1, _viewModel.CalendarModel.DateToDisplay.Month);
        }

        [Fact]
        public void OnActivityAdded_ShouldUpdateDaysOfMonth()
        {
            // Arrange
            var newActivity = new Activity { ActivityId = 1, DateOfAction = DateTime.Now.Date };

            // Act
            _viewModel.OnActivityAdded(newActivity);

            // Assert
            Assert.Contains(_viewModel.Days, day => day.DayNumber == newActivity.DateOfAction.Value.Day.ToString());
        }

        [Fact]
        public void DeleteActivity_ShouldRemoveSelectedActivity()
        {
            // Arrange
            var activity = new Activity { ActivityId = 1 };
            _viewModel.Activity = new ObservableCollection<Activity> { activity };
            _viewModel.SelectedActivity = activity;

            _dialogServiceMock.Setup(ds => ds.Confirm(It.IsAny<string>())).Returns(true);
            _activityRepoMock.Setup(repo => repo.DeleteActivity(activity)).Verifiable();

            // Act
            _viewModel.DeleteActivityCommand.Execute(null);

            // Assert
            Assert.DoesNotContain(_viewModel.Activity, a => a.ActivityId == activity.ActivityId);
            _activityRepoMock.Verify(repo => repo.DeleteActivity(activity), Times.Once);
        }

        [Fact]
        public void OnMouseClick_ShouldUpdateActivities()
        {
            // Arrange
            var activities = new List<Activity> { new Activity { ActivityId = 1, DateOfAction = DateTime.Now.Date } };
            _activityRepoMock.Setup(repo => repo.GetActivitiesOfDay(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                            .ReturnsAsync(activities);

            // Act
            _viewModel.OnMouseClick("1");

            // Assert
            Assert.Single(_viewModel.Activity);
        }

    }
}
