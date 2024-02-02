using ClientDatabaseApp.Model;
using ClientDatabaseApp.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;

namespace ClientDatabaseApp.ViewModel
{
    public class MVAddClient : INotifyPropertyChanged //Do zastanowienia - zamiast MessageBox dać 
    {
        public ICommand AddClientToDatabaseCommand { get; set; }

        private string _clientNameTextBox;
        private string _phonenumberTextBox;
        private string _emailTextBox;
        private string _cityTextBox;
        private string _facebookTextBox;
        private string _instagramTextBox;
        private string _pageURLTextBox;
        private DateTime _dateTextBox;
        private string _ownerTextBox;
        private string _richTextContent;
        private StatusItem _selectedStatus;

        private ObservableCollection<StatusItem> _statusItems;
        public event PropertyChangedEventHandler PropertyChanged;

        public string ClientNameTextBox
        {
            get => _clientNameTextBox;
            set
            {
                if (value != null)
                {
                    _clientNameTextBox = value;
                    OnPropertyChanged(nameof(ClientNameTextBox));
                }
            }
        }
        public string PhonenumberTextBox
        {
            get => _phonenumberTextBox;
            set
            {
                if (value != null)
                {
                    _phonenumberTextBox = value;
                    OnPropertyChanged(nameof(PhonenumberTextBox));
                }
            }
        }
        public string EmailTextBox
        {
            get => _emailTextBox;
            set
            {
                if (value != null)
                {
                    _emailTextBox = value;
                    OnPropertyChanged(nameof(EmailTextBox));
                }
            }
        }
        public string CityTextBox
        {
            get => _cityTextBox;
            set
            {
                if (value != null)
                {
                    _cityTextBox = value;
                    OnPropertyChanged(nameof(CityTextBox));
                }
            }
        }
        public string FacebookTextBox
        {
            get => _facebookTextBox;
            set
            {
                if (value != null)
                {
                    _facebookTextBox = value;
                    OnPropertyChanged(nameof(FacebookTextBox));
                }
            }
        }
        public string InstagramTextBox
        {
            get => _instagramTextBox;
            set
            {
                if (value != null)
                {
                    _instagramTextBox = value;
                    OnPropertyChanged(nameof(InstagramTextBox));
                }
            }
        }
        public string PageURLTextBox
        {
            get => _pageURLTextBox;
            set
            {
                if (value != null)
                {
                    _pageURLTextBox = value;
                    OnPropertyChanged(nameof(PageURLTextBox));
                }
            }
        }
        public DateTime DateTextBox
        {
            get => _dateTextBox;
            set
            {
                if (value != null)
                {
                    _dateTextBox = value;
                    OnPropertyChanged(nameof(DateTextBox));
                }
            }
        }
        public string OwnerTextBox
        {
            get => _ownerTextBox;
            set
            {
                if (value != null)
                {
                    _ownerTextBox = value;
                    OnPropertyChanged(nameof(OwnerTextBox));
                }
            }
        }
        public string RichTextContent
        {
            get => _richTextContent;
            set
            {
                _richTextContent = value;
                OnPropertyChanged(nameof(RichTextContent));
            }
        }
        public ObservableCollection<StatusItem> StatusItems
        {
            get => _statusItems;

            set
            {
                _statusItems = value;
                OnPropertyChanged(nameof(StatusItems));
            }
        }
        public StatusItem SelectedStatus
        {
            get => _selectedStatus;

            set
            {
                _selectedStatus = value;
                OnPropertyChanged(nameof(SelectedStatus));
            }
        }

        public List<Status> StatusEnumValues { get; } = Enum.GetValues(typeof(Status)).Cast<Status>().ToList();

        public MVAddClient()
        {
            DateTextBox = DateTime.Now;
            AddClientToDatabaseCommand = new DelegateCommand<RoutedEventArgs>(AddClient);
            InitializeComboBoxStatus();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void InitializeComboBoxStatus()
        {
            StatusItems = new ObservableCollection<StatusItem>(StatusEnumValues.Select(status => new StatusItem
            {
                Description = GetEnumDescription(status),
                Value = status,
                Color = GetBackgroundForStatus(status)
            }).ToList());
            SelectedStatus = StatusItems[0];
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
        private void AddClient(RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ClientNameTextBox))
            {
                _ = MessageBox.Show("Brak nazwy klienta!");
                return;
            }

            Client client = new Client();
            client.ClientName = ClientNameTextBox;
            client.Phonenumber = PhonenumberTextBox?.Trim();
            client.Email = EmailTextBox?.Trim();
            client.City = CityTextBox;
            client.Facebook = FacebookTextBox;
            client.Instagram = InstagramTextBox;
            client.PageURL = PageURLTextBox;
            client.Data = DateTextBox;
            client.Owner = OwnerTextBox;
            client.Status = (int)SelectedStatus.Value;

            if (string.IsNullOrEmpty(RichTextContent))
            {
                client.Note = "";
            }
            else
            {
                try
                {
                    XDocument document = XDocument.Parse(RichTextContent);
                    XNamespace ns = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
                    client.Note = string.Join(" ", document.Descendants(ns + "Run")
                                               .Select(run => run.Value));
                }
                catch
                {
                    client.Note = "";
                }

            }

            DatabaseQuery query = new DatabaseQuery();

            var clients = new ObservableCollection<Client> { client };
            List<(string, string)> exceptions = query.TryAddClients(clients);
        }



    }
}
