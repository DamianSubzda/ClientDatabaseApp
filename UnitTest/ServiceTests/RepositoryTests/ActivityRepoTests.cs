using ClientDatabaseApp.Models;
using ClientDatabaseApp.Services.Events;
using ClientDatabaseApp.Services.Repositories;
using Moq;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest.ServiceTests.RepositoryTests
{
    public class ActivityRepoTests
    {
        private readonly Mock<PostgresContext> _mockContext;
        private readonly Mock<IEventAggregator> _mockEventAggregator;
        private readonly Mock<DbSet<Activity>> _mockActivityDbSet;
        private readonly ActivityRepo _activityRepo;

        public ActivityRepoTests()
        {
            _mockContext = new Mock<PostgresContext>();
            _mockEventAggregator = new Mock<IEventAggregator>();
            _mockActivityDbSet = new Mock<DbSet<Activity>>();

            // Dane testowe
            var activities = new List<Activity>().AsQueryable();

            // Konfiguracja DbSet z asynchronicznym providerem i enumeratorem
            _mockActivityDbSet.As<IDbAsyncEnumerable<Activity>>()
                .Setup(m => m.GetAsyncEnumerator())
                .Returns(new TestDbAsyncEnumerator<Activity>(activities.GetEnumerator()));
            _mockActivityDbSet.As<IQueryable<Activity>>()
                .Setup(m => m.Provider)
                .Returns(new TestDbAsyncQueryProvider<Activity>(activities.Provider));
            _mockActivityDbSet.As<IQueryable<Activity>>().Setup(m => m.Expression).Returns(activities.Expression);
            _mockActivityDbSet.As<IQueryable<Activity>>().Setup(m => m.ElementType).Returns(activities.ElementType);
            _mockActivityDbSet.As<IQueryable<Activity>>().Setup(m => m.GetEnumerator()).Returns(activities.GetEnumerator());

            // Mockowanie metod Add i Remove
            _mockActivityDbSet.Setup(m => m.Add(It.IsAny<Activity>())).Callback<Activity>(a => activities.ToList().Add(a));
            _mockActivityDbSet.Setup(m => m.Remove(It.IsAny<Activity>())).Callback<Activity>(a => activities.ToList().Remove(a));

            // Mockowanie SaveChangesAsync
            _mockContext.Setup(m => m.SaveChangesAsync()).ReturnsAsync(1);

            // Przypisanie DbSet do kontekstu
            _mockContext.Setup(m => m.Activities).Returns(_mockActivityDbSet.Object);

            // Mockowanie Event Aggregatora
            _mockEventAggregator.Setup(x => x.GetEvent<ActivityRemovedFromDatabaseEvent>())
                .Returns(new Mock<ActivityRemovedFromDatabaseEvent>().Object);
            _mockEventAggregator.Setup(x => x.GetEvent<ActivityAddedToDatabaseEvent>())
                .Returns(new Mock<ActivityAddedToDatabaseEvent>().Object);
            _mockEventAggregator.Setup(x => x.GetEvent<ActivityUpdatedInDatabaseEvent>())
                .Returns(new Mock<ActivityUpdatedInDatabaseEvent>().Object);

            _activityRepo = new ActivityRepo(_mockContext.Object, _mockEventAggregator.Object);
        }

        [Fact]
        public async Task CreateActivity_ShouldAddActivityAndPublishEvent()
        {
            // Arrange
            var client = new Client { ClientId = 1 };
            var date = DateTime.Now;
            var note = "Test Note";

            // Act
            await _activityRepo.CreateActivity(client, date, note);

            // Assert
            _mockActivityDbSet.Verify(m => m.Add(It.Is<Activity>(a => a.ClientId == client.ClientId && a.DateOfAction == date && a.Note == note)), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteActivity_ShouldRemoveActivityAndPublishEvent()
        {
            // Arrange
            var activity = new Activity { ActivityId = 1 };

            // Act
            await _activityRepo.DeleteActivity(activity);

            // Assert
            _mockActivityDbSet.Verify(m => m.Remove(It.Is<Activity>(a => a.ActivityId == activity.ActivityId)), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(), Times.Once);
            _mockEventAggregator.Verify(m => m.GetEvent<ActivityRemovedFromDatabaseEvent>().Publish(It.Is<Activity>(a => a.ActivityId == activity.ActivityId)), Times.Once);
        }

        [Fact]
        public async Task GetActivitiesOfDay_ShouldReturnActivitiesForSpecifiedDay()
        {
            // Arrange
            var activities = new List<Activity>
            {
                new Activity { ActivityId = 1, DateOfAction = new DateTime(2023, 8, 25) },
                new Activity { ActivityId = 2, DateOfAction = new DateTime(2023, 8, 25) }
            }.AsQueryable();

            _mockActivityDbSet.As<IDbAsyncEnumerable<Activity>>()
                .Setup(m => m.GetAsyncEnumerator())
                .Returns(new TestDbAsyncEnumerator<Activity>(activities.GetEnumerator()));
            _mockActivityDbSet.As<IQueryable<Activity>>()
                .Setup(m => m.Provider)
                .Returns(new TestDbAsyncQueryProvider<Activity>(activities.Provider));
            _mockActivityDbSet.As<IQueryable<Activity>>().Setup(m => m.Expression).Returns(activities.Expression);
            _mockActivityDbSet.As<IQueryable<Activity>>().Setup(m => m.ElementType).Returns(activities.ElementType);
            _mockActivityDbSet.As<IQueryable<Activity>>().Setup(m => m.GetEnumerator()).Returns(activities.GetEnumerator());

            // Act
            var result = await _activityRepo.GetActivitiesOfDay(2023, 8, 25);

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetActivitiesCountOfMonth_ShouldReturnActivityCountsForMonth()
        {
            // Arrange
            var activities = new List<Activity>
            {
                new Activity { ActivityId = 1, DateOfAction = new DateTime(2023, 8, 25) },
                new Activity { ActivityId = 2, DateOfAction = new DateTime(2023, 8, 26) },
                new Activity { ActivityId = 3, DateOfAction = new DateTime(2023, 8, 25) }
            }.AsQueryable();

            _mockActivityDbSet.As<IDbAsyncEnumerable<Activity>>()
                .Setup(m => m.GetAsyncEnumerator())
                .Returns(new TestDbAsyncEnumerator<Activity>(activities.GetEnumerator()));
            _mockActivityDbSet.As<IQueryable<Activity>>()
                .Setup(m => m.Provider)
                .Returns(new TestDbAsyncQueryProvider<Activity>(activities.Provider));
            _mockActivityDbSet.As<IQueryable<Activity>>().Setup(m => m.Expression).Returns(activities.Expression);
            _mockActivityDbSet.As<IQueryable<Activity>>().Setup(m => m.ElementType).Returns(activities.ElementType);
            _mockActivityDbSet.As<IQueryable<Activity>>().Setup(m => m.GetEnumerator()).Returns(activities.GetEnumerator());

            // Act
            var result = await _activityRepo.GetActivitiesCountOfMonth(2023, 8);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal((25, 2), result[0]);
            Assert.Equal((26, 1), result[1]);
        }

        [Fact]
        public async Task UpdateActivity_ShouldUpdateExistingActivityAndPublishEvent()
        {
            // Arrange
            var existingActivity = new Activity { ActivityId = 1, ClientId = 1, DateOfCreation = DateTime.Now };
            var updatedActivity = new Activity { ActivityId = 1, ClientId = 2, DateOfCreation = DateTime.Now, DateOfAction = DateTime.Now.AddDays(1), Note = "Updated Note" };

            var activities = new List<Activity> { existingActivity }.AsQueryable();

            _mockActivityDbSet.As<IDbAsyncEnumerable<Activity>>()
                .Setup(m => m.GetAsyncEnumerator())
                .Returns(new TestDbAsyncEnumerator<Activity>(activities.GetEnumerator()));
            _mockActivityDbSet.As<IQueryable<Activity>>()
                .Setup(m => m.Provider)
                .Returns(new TestDbAsyncQueryProvider<Activity>(activities.Provider));
            _mockActivityDbSet.As<IQueryable<Activity>>().Setup(m => m.Expression).Returns(activities.Expression);
            _mockActivityDbSet.As<IQueryable<Activity>>().Setup(m => m.ElementType).Returns(activities.ElementType);
            _mockActivityDbSet.As<IQueryable<Activity>>().Setup(m => m.GetEnumerator()).Returns(activities.GetEnumerator());

            // Mockowanie wywołania Find
            _mockActivityDbSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
                .ReturnsAsync(existingActivity);

            // Act
            await _activityRepo.UpdateActivity(updatedActivity);

            // Assert
            Assert.Equal(updatedActivity.ClientId, existingActivity.ClientId);
            Assert.Equal(updatedActivity.DateOfAction, existingActivity.DateOfAction);
            Assert.Equal(updatedActivity.Note, existingActivity.Note);
            _mockContext.Verify(m => m.SaveChangesAsync(), Times.Once);
        }
    }
}
