using ClientDatabaseApp.Services.Utilities;
using System;
using System.Linq;
using System.Windows.Media;
using Xunit;

namespace UnitTest.ServiceTests.UtilityTests
{
    public class ComboboxStatusTests
    {
        
        [Fact]
        public void ComboboxStatus_ShouldInitializeStatusItemsWithCorrectDescriptionsAndColors()
        {
            // Arrange
            var comboboxStatus = new ComboboxStatus();
            var expectedStatuses = Enum.GetValues(typeof(ComboboxStatus.Status)).Cast<ComboboxStatus.Status>().ToList();

            // Act
            var statusItems = comboboxStatus.GetStatusItems();

            // Assert
            Assert.Equal(expectedStatuses.Count, statusItems.Count);

            foreach (var status in expectedStatuses)
            {
                var expectedDescription = GetDescription(status);
                var expectedColor = GetExpectedColor(status);

                var item = statusItems.FirstOrDefault(si => si.Value == status);

                Assert.Equal(expectedDescription, item.Description);
                Assert.Equal(expectedColor.ToString(), item.Color.ToString());
            }
        }

        private string GetDescription(ComboboxStatus.Status status)
        {
            var field = status.GetType().GetField(status.ToString());
            var attribute = Attribute.GetCustomAttribute(field, typeof(System.ComponentModel.DescriptionAttribute)) as System.ComponentModel.DescriptionAttribute;

            return attribute != null ? attribute.Description : status.ToString();
        }

        private Color GetExpectedColor(ComboboxStatus.Status status)
        {
            switch (status)
            {
                case ComboboxStatus.Status.NewClient:
                    return Colors.White;
                case ComboboxStatus.Status.InProgress:
                    return Colors.DeepSkyBlue;
                case ComboboxStatus.Status.ScheduledMeeting:
                    return Colors.LightGreen;
                case ComboboxStatus.Status.NotInterested:
                    return Colors.Plum;
                case ComboboxStatus.Status.MissedCall:
                    return Colors.LightYellow;
                case ComboboxStatus.Status.ImmediateAction:
                    return Colors.MediumTurquoise;
                case ComboboxStatus.Status.DeferredAction:
                    return Colors.MediumVioletRed;
                case ComboboxStatus.Status.VeryLongTermAction:
                    return Colors.DarkOrange;
                case ComboboxStatus.Status.NotConsideredInStatistics:
                    return Colors.Red;
                default:
                    return Colors.Black;
            }
        }

        [Theory]
        [InlineData(0, "Nowy klient", "White")]
        [InlineData(1, "Trwają rozmowy", "DeepSkyBlue")]
        [InlineData(2, "Umówione spotkanie", "LightGreen")]
        [InlineData(3, "Nie chcą/mają kogoś", "Plum")]
        [InlineData(4, "Nie odebrane", "LightYellow")]
        [InlineData(5, "Akcja do zrobienia instant", "MediumTurquoise")]
        [InlineData(6, "Akcja do zrobienia w dłuższym przedziale czasu", "MediumVioletRed")]
        [InlineData(7, "Akcja do zrobienia bardzo do przodu", "DarkOrange")]
        [InlineData(8, "Tak oznaczonych firm nie bierzemy pod uwagę w statystykach", "Red")]
        public void GetBackgroundForStatus_ShouldReturnExpectedColor(int statusValue, string expectedDescription, string expectedColorName)
        {
            // Arrange
            var status = (ComboboxStatus.Status)statusValue;
            var expectedColor = (Color)ColorConverter.ConvertFromString(expectedColorName);
            var comboboxStatus = new ComboboxStatus();

            // Act
            var item = comboboxStatus.GetStatusItems().FirstOrDefault(si => si.Value == status);

            // Assert
            Assert.Equal(expectedDescription, item.Description);
            Assert.Equal(expectedColor, item.Color.Color);
        }
    }
}
