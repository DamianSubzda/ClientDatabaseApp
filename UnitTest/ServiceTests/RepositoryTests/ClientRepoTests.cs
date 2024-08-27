using ClientDatabaseApp.Models;
using ClientDatabaseApp.Services.Events;
using ClientDatabaseApp.Services.Exceptions;
using ClientDatabaseApp.Services.Repositories;
using Moq;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest.ServiceTests.RepositoryTests
{
    public class ClientRepoTests
    {
        private readonly Mock<PostgresContext> _mockContext;
        private readonly Mock<IEventAggregator> _mockEventAggregator;
        private readonly Mock<DbSet<Client>> _mockClientDbSet;
        private readonly ClientRepo _clientRepo;

        public ClientRepoTests()
        {
            _mockContext = new Mock<PostgresContext>();
            _mockEventAggregator = new Mock<IEventAggregator>();
            _mockClientDbSet = new Mock<DbSet<Client>>();

            _clientRepo = new ClientRepo(_mockContext.Object, _mockEventAggregator.Object);

            // Ustawienie podstawowych mocków dla Event Aggregatora
            SetupEventAggregator();
        }

        private void SetupEventAggregator()
        {
            _mockEventAggregator.Setup(x => x.GetEvent<ClientAddedToDatabaseEvent>())
                .Returns(new Mock<ClientAddedToDatabaseEvent>().Object);
            _mockEventAggregator.Setup(x => x.GetEvent<ClientRemovedFromDatabaseEvent>())
                .Returns(new Mock<ClientRemovedFromDatabaseEvent>().Object);
            _mockEventAggregator.Setup(x => x.GetEvent<ClientUpdatedInDatabaseEvent>())
                .Returns(new Mock<ClientUpdatedInDatabaseEvent>().Object);
        }

        private void SetupQuery<T>(IQueryable<T> data) where T : class
        {
            var asyncData = new TestDbAsyncEnumerable<T>(data);

            _mockClientDbSet.As<IDbAsyncEnumerable<T>>()
                .Setup(m => m.GetAsyncEnumerator())
                .Returns(new TestDbAsyncEnumerator<T>(data.GetEnumerator()));
            _mockClientDbSet.As<IQueryable<T>>()
                .Setup(m => m.Provider)
                .Returns(new TestDbAsyncQueryProvider<T>(data.Provider));
            _mockClientDbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            _mockClientDbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _mockClientDbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            _mockContext.Setup(c => c.Clients).Returns(_mockClientDbSet.Object);
        }

        [Fact]
        public async Task AddClient_ShouldAddClientAndPublishEvent()
        {
            // Arrange
            var client = new Client { ClientId = 1, ClientName = "Test Client", Email = "test@example.com" };

            SetupQuery(Enumerable.Empty<Client>().AsQueryable());

            // Act
            await _clientRepo.AddClient(client);

            // Assert
            _mockClientDbSet.Verify(m => m.Add(It.Is<Client>(c => c.ClientName == client.ClientName)), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(), Times.Once);
            _mockEventAggregator.Verify(m => m.GetEvent<ClientAddedToDatabaseEvent>().Publish(It.Is<Client>(c => c.ClientName == client.ClientName)), Times.Once);
        }

        [Fact]
        public async Task AddClient_ShouldThrowExceptionIfClientExists()
        {
            // Arrange
            var client = new Client { ClientId = 1, ClientName = "Test Client", Email = "test@example.com" };

            var clients = new List<Client>
            {
                new Client { ClientId = 2, ClientName = "Test Client", Email = "test@example.com" }
            }.AsQueryable();

            SetupQuery(clients);

            // Act & Assert
            await Assert.ThrowsAsync<ClientAlreadyExistsException>(() => _clientRepo.AddClient(client));
        }

        [Fact]
        public async Task DeleteClient_ShouldRemoveClientAndPublishEvent()
        {
            // Arrange
            var client = new Client { ClientId = 1, ClientName = "Test Client", Email = "test@example.com" };

            SetupQuery(new List<Client> { client }.AsQueryable());

            // Act
            await _clientRepo.DeleteClient(client);

            // Assert
            _mockClientDbSet.Verify(m => m.Remove(It.Is<Client>(c => c.ClientId == client.ClientId)), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(), Times.Once);
            _mockEventAggregator.Verify(m => m.GetEvent<ClientRemovedFromDatabaseEvent>().Publish(It.Is<Client>(c => c.ClientId == client.ClientId)), Times.Once);
        }

        [Fact]
        public async Task CheckIfClientExists_ShouldReturnTrueIfClientExists()
        {
            // Arrange
            var client = new Client { ClientName = "Test Client", Email = "test@example.com" };

            var clients = new List<Client>
            {
                new Client { ClientName = "Test Client", Email = "test@example.com" }
            }.AsQueryable();

            SetupQuery(clients);

            // Act
            var result = await _clientRepo.CheckIfClientExists(client);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task GetAllClients_ShouldReturnListOfClients()
        {
            // Arrange
            var clients = new List<Client>
            {
                new Client { ClientId = 1, ClientName = "Test Client 1" },
                new Client { ClientId = 2, ClientName = "Test Client 2" }
            }.AsQueryable();

            SetupQuery(clients);

            // Act
            var result = await _clientRepo.GetAllClients();

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetClient_ShouldReturnClientById()
        {
            // Arrange
            var client = new Client { ClientId = 1, ClientName = "Test Client 1" };

            var clients = new List<Client> { client }.AsQueryable();

            SetupQuery(clients);

            // Act
            var result = await _clientRepo.GetClient(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(client.ClientId, result.ClientId);
            Assert.Equal(client.ClientName, result.ClientName);
        }

        [Fact]
        public async Task UpdateClient_ShouldUpdateClientAndPublishEvent()
        {
            // Arrange
            var existingClient = new Client { ClientId = 1, ClientName = "Old Name", Email = "old@example.com" };
            var updatedClient = new Client { ClientId = 1, ClientName = "New Name", Email = "new@example.com" };

            var clients = new List<Client> { existingClient }.AsQueryable();

            SetupQuery(clients);

            // Act
            await _clientRepo.UpdateClient(updatedClient);

            // Assert
            Assert.Equal(updatedClient.ClientName, existingClient.ClientName);
            Assert.Equal(updatedClient.Email, existingClient.Email);
            _mockContext.Verify(m => m.SaveChangesAsync(), Times.Once);
            _mockEventAggregator.Verify(m => m.GetEvent<ClientUpdatedInDatabaseEvent>().Publish(It.Is<Client>(c => c.ClientId == updatedClient.ClientId)), Times.Once);
        }
    }
}
