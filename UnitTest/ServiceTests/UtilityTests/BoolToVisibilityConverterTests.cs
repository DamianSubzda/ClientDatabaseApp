using ClientDatabaseApp.Services.Utilities;
using System.Globalization;
using System.Windows;
using Xunit;

namespace UnitTest.ServiceTests.UtilityTests
{
    public class BoolToVisibilityConverterTests
    {
        private readonly BoolToVisibilityConverter _converter;

        public BoolToVisibilityConverterTests()
        {
            _converter = new BoolToVisibilityConverter();
        }

        [Theory]
        [InlineData(true, Visibility.Visible)]
        [InlineData(false, Visibility.Hidden)]
        public void Convert_ShouldReturnExpectedVisibility(bool input, Visibility expected)
        {
            // Act
            var result = _converter.Convert(input, typeof(Visibility), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.Equal(expected, result);
        }


    }
}
