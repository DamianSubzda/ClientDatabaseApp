using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ClientDatabaseApp.Service
{
    public class ComboboxStatus
    {
        public enum Status
        {
            [Description("Nowy klient")]
            NewClient = 0,
            [Description("Trwają rozmowy")]
            InProgress = 1,
            [Description("Umówione spotkanie")]
            ScheduledMeeting = 2,
            [Description("Nie chcą/mają kogoś")]
            NotInterested = 3,
            [Description("Nie odebrane")]
            MissedCall = 4,
            [Description("Akcja do zrobienia instant")]
            ImmediateAction = 5,
            [Description("Akcja do zrobienia w dłuższym przedziale czasu")]
            DeferredAction = 6,
            [Description("Akcja do zrobienia bardzo do przodu")]
            VeryLongTermAction = 7,
            [Description("Tak oznaczonych firm nie bierzemy pod uwagę w statystykach")]
            NotConsideredInStatistics = 8
        }

        public ObservableCollection<StatusItem> StatusItems { get; set; }

        public struct StatusItem
        {
            public string Description { get; set; }
            public Status? Value { get; set; }
            public SolidColorBrush Color { get; set; }
        }
        private List<Status> StatusEnumValues { get; set; }
        public ComboboxStatus()
        {
            StatusEnumValues = Enum.GetValues(typeof(Status)).Cast<Status>().ToList();

            StatusItems = new ObservableCollection<StatusItem>(StatusEnumValues.Select(status => new StatusItem
            {
                Description = GetComboboxDescription(status),
                Value = status,
                Color = GetBackgroundForStatus(status)
            }).ToList());
        }
        
        private string GetComboboxDescription(Status value)
        {
            var field = value.GetType().GetField(value.ToString());
            object attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));

            if (attribute != null && attribute is DescriptionAttribute descriptionAttribute)
            {
                return descriptionAttribute.Description;
            }
            else
            {
                return value.ToString();
            }
        }

        private SolidColorBrush GetBackgroundForStatus(Status status)
        {

            SolidColorBrush brush;

            switch (status)
            {
                case Status.NewClient:
                    brush = new SolidColorBrush(Colors.White);
                    break;
                case Status.InProgress:
                    brush = new SolidColorBrush(Colors.DeepSkyBlue);
                    break;
                case Status.ScheduledMeeting:
                    brush = new SolidColorBrush(Colors.LightGreen);
                    break;
                case Status.NotInterested:
                    brush = new SolidColorBrush(Colors.Plum);
                    break;
                case Status.MissedCall:
                    brush = new SolidColorBrush(Colors.LightYellow);
                    break;
                case Status.ImmediateAction:
                    brush = new SolidColorBrush(Colors.MediumTurquoise);
                    break;
                case Status.DeferredAction:
                    brush = new SolidColorBrush(Colors.MediumVioletRed);
                    break;
                case Status.VeryLongTermAction:
                    brush = new SolidColorBrush(Colors.DarkOrange);
                    break;
                case Status.NotConsideredInStatistics:
                    brush = new SolidColorBrush(Colors.Red);
                    break;
                default:
                    brush = new SolidColorBrush(Colors.Black);
                    break;
            }

            return brush;

        }
    }
}
