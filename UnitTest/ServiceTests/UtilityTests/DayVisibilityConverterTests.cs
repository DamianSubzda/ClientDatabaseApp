using ClientDatabaseApp.Services.Utilities;
using System.Globalization;
using System.Windows;
using Xunit;

namespace UnitTest.ServiceTests.UtilityTests
{
    public class DayVisibilityConverterTests
    {
        private readonly DayVisibilityConverter _converter;

        public DayVisibilityConverterTests()
        {
            _converter = new DayVisibilityConverter();
        }

        [Theory]
        [InlineData(null, Visibility.Hidden)]
        [InlineData("", Visibility.Hidden)]
        [InlineData("2024-08-27", Visibility.Visible)]
        public void Convert_ShouldReturnExpectedVisibility(object input, Visibility expected)
        {
            // Act
            var result = _converter.Convert(input, typeof(Visibility), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.Equal(expected, result);
        }


    }
}
