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
    public class ActivityTests
    {
        [Fact]
        public void ShouldSetDateOfCreationToNowOnInitialization()
        {
            // Arrange & Act
            var activity = new Activity();

            // Assert
            Assert.Equal(DateTime.Now.Date, activity.DateOfCreation.Date);
        }

        [Fact]
        public void ShouldValidateMaxLengthAttributes()
        {
            // Arrange
            var activity = new Activity
            {
                Note = new string('A', 2001) // Exceeds max length
            };

            // Act
            var context = new ValidationContext(activity, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();

            // Validate
            var isValid = Validator.TryValidateObject(activity, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.ErrorMessage.Contains("Must be 2000 characters or less!"));
        }

        [Fact]
        public void ShouldValidateCorrectLengthAttributes()
        {
            // Arrange
            var activity = new Activity
            {
                Note = new string('A', 2000) // Within max length
            };

            // Act
            var context = new ValidationContext(activity, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();

            // Validate
            var isValid = Validator.TryValidateObject(activity, context, results, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(results);
        }


    }

}
