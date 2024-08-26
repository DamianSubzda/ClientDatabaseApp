﻿using ClientDatabaseApp.Services.Repositories;
using ClientDatabaseApp.Services;
using System.Linq;
using Moq;
using Xunit;
using Prism.Events;
using ClientDatabaseApp.ViewModels;
using ClientDatabaseApp.Models;
using System.Collections.Generic;
using ClientDatabaseApp.Services.Events;

namespace UnitTest
{
    public class ClientDatabaseViewModelTests
    {
        private readonly Mock<IClientRepo> _clientRepoMock;
        private readonly Mock<IActivityRepo> _activityRepoMock;
        private readonly Mock<IDialogService> _dialogServiceMock;
        private readonly Mock<IEventAggregator> _eventAggregatorMock;
        private readonly ClientDatabaseViewModel _viewModel;

        public ClientDatabaseViewModelTests()
        {
            _clientRepoMock = new Mock<IClientRepo>();
            _activityRepoMock = new Mock<IActivityRepo>();
            _dialogServiceMock = new Mock<IDialogService>();
            _eventAggregatorMock = new Mock<IEventAggregator>();

            _eventAggregatorMock.Setup(x => x.GetEvent<ClientAddedToDatabaseEvent>())
                                .Returns(new Mock<ClientAddedToDatabaseEvent>().Object);
            _eventAggregatorMock.Setup(x => x.GetEvent<ClientUpdatedInDatabaseEvent>())
                                .Returns(new Mock<ClientUpdatedInDatabaseEvent>().Object);
            _eventAggregatorMock.Setup(x => x.GetEvent<ClientRemovedFromDatabaseEvent>())
                                .Returns(new Mock<ClientRemovedFromDatabaseEvent>().Object);

            _viewModel = new ClientDatabaseViewModel(
                _clientRepoMock.Object,
                _activityRepoMock.Object,
                _dialogServiceMock.Object,
                _eventAggregatorMock.Object
            );
        }


        [Fact]
        public void ShouldInitializeClientsView()
        {
            // Arrange
            var expectedClients = new List<Client> { new Client { ClientName = "Test Client" } };
            _clientRepoMock.Setup(repo => repo.GetAllClients()).ReturnsAsync(expectedClients);

            // Act
            _viewModel.LoadClients();
            var actualClients = _viewModel.ClientsView;

            // Assert
            Assert.NotNull(actualClients);
            Assert.Equal(expectedClients.Count, actualClients.Cast<object>().Count());
        }
    }
}
