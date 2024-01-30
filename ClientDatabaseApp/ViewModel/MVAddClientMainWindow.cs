using ClientDatabaseApp.Model;
using ClientDatabaseApp.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace ClientDatabaseApp.ViewModel
{
    public class MVAddClientMainWindow
    {
        private ICommand AddClientToDatabaseCommand { get; set; }

        public MVAddClientMainWindow()
        {
            AddClientToDatabaseCommand = new DelegateCommand<RoutedEventArgs>(AddClientToDatabase);
        }

        private void InitializeComboBoxStatus()
        {
            var statusItems = StatusEnumValues.Select(status => new StatusItem
            {
                Description = GetEnumDescription(status),
                Value = status,
                Color = GetBackgroundForStatus(status)
            }).ToList();

            //statusCombobox.ItemsSource = statusItems;
        }
        public class StatusItem
        {
            public string Description { get; set; }
            public Status? Value { get; set; }
            public SolidColorBrush Color { get; set; }
        }
        public enum Status
        {
            [Description("Trwają rozmowy")]
            InProgress = 0,
            [Description("Umówione spotkanie")]
            ScheduledMeeting = 1,
            [Description("Nie chcą/mają kogoś")]
            NotInterested = 2,
            [Description("Nie odebrane")]
            MissedCall = 3,
            [Description("Akcja do zrobienia instant")]
            ImmediateAction = 4,
            [Description("Akcja do zrobienia w dłuższym przedziale czasu")]
            DeferredAction = 5,
            [Description("Akcja do zrobienia bardzo do przodu")]
            VeryLongTermAction = 6,
            [Description("Tak oznaczonych firm nie bierzemy pod uwagę w statystykach")]
            NotConsideredInStatistics = 7
        }

        public List<Status> StatusEnumValues { get; } = Enum.GetValues(typeof(Status)).Cast<Status>().ToList();

        private static string GetEnumDescription(Status value)
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
                case Status.InProgress:
                    brush = new SolidColorBrush(Color.FromRgb(0, 204, 255));
                    break;
                case Status.ScheduledMeeting:
                    brush = new SolidColorBrush(Color.FromRgb(110, 239, 152));
                    break;
                case Status.NotInterested:
                    brush = new SolidColorBrush(Color.FromRgb(230, 161, 196));
                    break;
                case Status.MissedCall:
                    brush = new SolidColorBrush(Color.FromRgb(237, 242, 143));
                    break;
                case Status.ImmediateAction:
                    brush = new SolidColorBrush(Color.FromRgb(70, 189, 198));
                    break;
                case Status.DeferredAction:
                    brush = new SolidColorBrush(Color.FromRgb(204, 51, 102));
                    break;
                case Status.VeryLongTermAction:
                    brush = new SolidColorBrush(Color.FromRgb(255, 111, 1));
                    break;
                case Status.NotConsideredInStatistics:
                    brush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                    break;
                default:
                    brush = new SolidColorBrush(Colors.White);
                    break;
            }

            return brush;


        }
        private void AddClientToDatabase(RoutedEventArgs e)
        {
            Client client = new Client();
            //client.ClientName = NipTextBox.Text;
            //client.PageURL = ClientPageTextBox.Text;
            //client.Phonenumber = PhoneTextBox.Text.Trim();
            //client.Facebook = FbTextBox.Text.Trim();
            //client.City = CityTextBox.Text;
            //client.Data = DateTime.Parse(ClientDataPicker.Text);
            //client.Owner = OwnerTextBox.Text;
            //client.Note = StringFromRichTextBox(NotesRichTextBox);

            //if (!CheckIfClientIsInDatabase(client))
            //{
            //    AddClientToDatabase(client);
            //}
            //else
            //{
            //    MessageBoxResult dialog = MessageBox.Show($"Klient {client.ClientName} o numerze telefonu " +
            //                                        $"{client.Phonenumber} bądź podanej stronie Facebook już " +
            //                                        $"istnieje w bazie! Na pewno chcesz go dodać?", "Uwaga!", MessageBoxButton.YesNo);
            //    if (dialog == MessageBoxResult.Yes)
            //    {
            //        AddClientToDatabase(client);
            //    }
            //}
        }

        string StringFromRichTextBox(RichTextBox rtb)
        {
            TextRange textRange = new TextRange(
                rtb.Document.ContentStart,
                rtb.Document.ContentEnd
            );
            return textRange.Text;
        }
    }
}
