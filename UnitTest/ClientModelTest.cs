using ClientDatabaseApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest
{
    public class ClientTests
    {
        [Fact]
        public void ShouldHaveDefaultEmptyActivitiesList()
        {
            // Arrange
            var client = new Client();

            // Act & Assert
            Assert.NotNull(client.Activities);
            Assert.Empty(client.Activities);
        }

        [Fact]
        public void ShouldValidateMaxLengthAttributes()
        {
            // Arrange
            var client = new Client
            {
                ClientName = new string('A', 201), // Exceeds max length
                Phonenumber = new string('1', 31), // Exceeds max length
                Email = new string('E', 41),       // Exceeds max length
                City = new string('C', 51),        // Exceeds max length
                Facebook = new string('F', 1001),  // Exceeds max length
                Instagram = new string('I', 1001), // Exceeds max length
                PageURL = new string('P', 1001),   // Exceeds max length
                Owner = new string('O', 51),       // Exceeds max length
                Note = new string('N', 2001)       // Exceeds max length
            };

            // Act
            var context = new ValidationContext(client, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();

            // Validate
            var isValid = Validator.TryValidateObject(client, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.ErrorMessage.Contains("Client name must be 200 characters or less!"));
            Assert.Contains(results, r => r.ErrorMessage.Contains("Phonenumber must be 30 characters or less!"));
            Assert.Contains(results, r => r.ErrorMessage.Contains("Email must be 40 characters or less!"));
            Assert.Contains(results, r => r.ErrorMessage.Contains("City must be 50 characters or less!"));
            Assert.Contains(results, r => r.ErrorMessage.Contains("Facebook must be 1000 characters or less!"));
            Assert.Contains(results, r => r.ErrorMessage.Contains("Instagram must be 1000 characters or less!"));
            Assert.Contains(results, r => r.ErrorMessage.Contains("PageURL must be 1000 characters or less!"));
            Assert.Contains(results, r => r.ErrorMessage.Contains("Owner name must be 50 characters or less!"));
            Assert.Contains(results, r => r.ErrorMessage.Contains("Note must be 2000 characters or less!"));
        }

        [Fact]
        public void ShouldValidateCorrectLengthAttributes()
        {
            // Arrange
            var client = new Client
            {
                ClientName = new string('A', 200), // Within max length
                Phonenumber = new string('1', 30), // Within max length
                Email = new string('E', 40),       // Within max length
                City = new string('C', 50),        // Within max length
                Facebook = new string('F', 1000),  // Within max length
                Instagram = new string('I', 1000), // Within max length
                PageURL = new string('P', 1000),   // Within max length
                Owner = new string('O', 50),       // Within max length
                Note = new string('N', 2000)       // Within max length
            };

            // Act
            var context = new ValidationContext(client, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();

            // Validate
            var isValid = Validator.TryValidateObject(client, context, results, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(results);
        }

    }

}
