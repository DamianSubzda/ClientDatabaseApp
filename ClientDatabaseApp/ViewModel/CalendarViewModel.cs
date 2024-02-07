using ClientDatabaseApp.Model;
using ClientDatabaseApp.Service;
using ClientDatabaseApp.Service.API;
using ClientDatabaseApp.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ClientDatabaseApp.ViewModel
{
    public class CalendarViewModel : INotifyPropertyChanged
    {

        private CalendarModel calendarModel = new CalendarModel();
        public ICommand MouseClickCommand { get; private set; }
        public ICommand Button_Click_PrevMonthCommand { get; private set; }
        public ICommand Button_Click_NextMonthCommand { get; private set; }
        public ICommand PickFollowUpCommand { get; private set; }
        public ICommand DeleteFollowUpCommand { get; private set; }

        public enum MonthEnum
        {
            Styczeń = 1, Luty, Marzec, Kwiecień, Maj, Czerwiec, Lipiec, Sierpień, Wrzesień, Październik, Listopad, Grudzień
        }

        public enum DaysEnum
        {
            Monday = 1, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<DayInfo> Days
        {
            get => calendarModel.DaysOfCurrentMonth;
            set
            {
                if (calendarModel.DaysOfCurrentMonth != value)
                {
                    calendarModel.DaysOfCurrentMonth = value;
                    OnPropertyChanged(nameof(Days));
                }
            }
        }

        public string DataToDisplay
        {
            get => calendarModel.HeaderToDisplay;
            set
            {
                if (calendarModel.HeaderToDisplay != value)
                {
                    calendarModel.HeaderToDisplay = value;
                    OnPropertyChanged(nameof(DataToDisplay));
                }
            }
        }

        private ObservableCollection<FollowUp> _followUp;
        public ObservableCollection<FollowUp> FollowUp
        {
            get { return _followUp; }
            set
            {
                _followUp = value;
                OnPropertyChanged(nameof(FollowUp));
            }
        }

        private string _temperature;
        private string _temperatureFeel;
        private string _wind;
        private string _weather;
        private string _city;

        public string Temperature
        {
            get => _temperature;
            set
            {
                _temperature = value;
                OnPropertyChanged(nameof(Temperature));
            }
        }
        public string TemperatureFeel
        {
            get => _temperatureFeel;
            set
            {
                _temperatureFeel = value;
                OnPropertyChanged(nameof(TemperatureFeel));
            }
        }
        public string Wind
        {
            get => _wind;
            set
            {
                _wind = value;
                OnPropertyChanged(nameof(Wind));
            }
        }
        public string Weather
        {
            get => _weather;
            set
            {
                _weather = value;
                OnPropertyChanged(nameof(Weather));
            }
        }
        public string City
        {
            get => _city;
            set
            {
                _city = value;
                OnPropertyChanged(nameof(City));
            }
        }
        private FollowUp _selectedFollowUp;

        public FollowUp SelectedFollowUp
        {
            get => _selectedFollowUp;
            set
            {
                _selectedFollowUp = value;
                OnPropertyChanged(nameof(SelectedFollowUp));
            }
        }

        public CalendarViewModel()
        {

            Button_Click_PrevMonthCommand = new DelegateCommand<RoutedEventArgs>(Button_Click_PrevMonth);
            Button_Click_NextMonthCommand = new DelegateCommand<RoutedEventArgs>(Button_Click_NextMonth);
            PickFollowUpCommand = new DelegateCommand<RoutedEventArgs>(PickFollowUp);
            DeleteFollowUpCommand = new DelegateCommand<RoutedEventArgs>(DeleteFollowUp);
            MouseClickCommand = new DelegateCommand<string>(OnMouseClick);

            calendarModel.HeaderToDisplay = ((MonthEnum)DateTime.Now.Month).ToString() + " " + DateTime.Now.Year.ToString();
            calendarModel.DateToDisplay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            InitialCalendar();
            _ = InitializeAsync();
        }

        public async Task InitializeAsync()
        {
            IpifyAPIConnector ipify = new IpifyAPIConnector();
            await ipify.GetIp();
            GeolocationAPIConnector geolocation = new GeolocationAPIConnector(ipify.IPAddress);
            await geolocation.GetCoorAsync();
            OpenweatherAPIConnector openweather = new OpenweatherAPIConnector(geolocation.Lat, geolocation.Lon);
            await openweather.GetWeather();
            Temperature = Math.Round(openweather.temperature, 1).ToString() + " C.";
            TemperatureFeel = Math.Round(openweather.temperatureFeel, 1).ToString() + " C.";
            Wind = Math.Round(openweather.wind * 36 / 10, 1).ToString() + " km/h.";
            Weather = openweather.weather + '.';
            City = geolocation.City + '.';
        }

        private void InitialCalendar()
        {
            GetDaysFromMonth();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Button_Click_NextMonth(RoutedEventArgs e)
        {
            ChangeMonth(1);
        }

        private void Button_Click_PrevMonth(RoutedEventArgs e)
        {
            ChangeMonth(-1);
        }
        private void PickFollowUp(RoutedEventArgs e)
        {
            if(SelectedFollowUp != null)
            {
                ShowFollowUp showFollowUp = new ShowFollowUp();
                ShowFollowUpViewModel showFollowUpViewModel = new ShowFollowUpViewModel(SelectedFollowUp, () => showFollowUp.Close());
                showFollowUp.DataContext = showFollowUpViewModel;
                showFollowUp.ShowDialog();
            }
        }
        private void DeleteFollowUp(RoutedEventArgs e)
        {
            if (SelectedFollowUp != null)
            {
                MessageBoxResult result = MessageBox.Show("Czy jesteś pewien, że chcesz usunąć ten followUp?", "Uwaga", MessageBoxButton.YesNo);
                if(result == MessageBoxResult.Yes)
                {
                    DatabaseQuery query = new DatabaseQuery();
                    query.DeleteFollowUp(SelectedFollowUp);
                    FollowUp.Remove(SelectedFollowUp);
                    SelectedFollowUp = null;
                }
            }
        }


        private void ChangeMonth(int amountOfMonthToChange)
        {
            calendarModel.DateToDisplay = calendarModel.DateToDisplay.AddMonths(amountOfMonthToChange);
            DataToDisplay = (MonthEnum)calendarModel.DateToDisplay.Month + " " + calendarModel.DateToDisplay.Year.ToString();

            GetDaysFromMonth();
        }



        private async void OnMouseClick(string dayNumber)
        {
            if (Days.Any(dayinfo => dayinfo.DayNumber == dayNumber))
            {
                try
                {
                    int day_number = int.Parse(dayNumber);
                    var context = new DBContextHVAC();
                    var followUp = await context.FollowUpDBSet
                        .Where(line => line.DateOfAction.Year == calendarModel.DateToDisplay.Year
                                    && line.DateOfAction.Month == calendarModel.DateToDisplay.Month
                                    && line.DateOfAction.Day == day_number)
                        .ToListAsync();

                    FollowUp = new ObservableCollection<FollowUp>(followUp);
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show(ex.Message);
                }

            }
        }
        private void GetDaysFromMonth()
        {
            var context = new DBContextHVAC();
            var followUps = context.FollowUpDBSet
                .Where(line => line.DateOfAction.Year == calendarModel.DateToDisplay.Year &&
                               line.DateOfAction.Month == calendarModel.DateToDisplay.Month)
                .ToList();

            var followUpCounts = followUps
                .GroupBy(line => line.DateOfAction.Day)
                .Select(group => new { Day = group.Key, Count = group.Count() })
                .ToList();

            int days = DateTime.DaysInMonth(calendarModel.DateToDisplay.Year, calendarModel.DateToDisplay.Month);
            DayOfWeek dayOfWeek = calendarModel.DateToDisplay.DayOfWeek;
            int nr = (int)(DaysEnum)dayOfWeek;

            if (dayOfWeek == DayOfWeek.Sunday)
            {
                nr = 7;
            }

            List<DayInfo> listOfDays = new List<DayInfo>();

            for (int i = 1; i < nr; i++)
            {
                listOfDays.Add(new DayInfo()
                {
                    DayNumber = "",
                    FollowUpCount = 0
                });
            }

            for (int i = 1; i <= days; i++)
            {
                var followUpCount = followUpCounts.FirstOrDefault(fc => fc.Day == i)?.Count ?? 0;

                listOfDays.Add(new DayInfo()
                {
                    DayNumber = i.ToString(),
                    FollowUpCount = followUpCount
                });
            }

            Days = new ObservableCollection<DayInfo>(listOfDays);
        }

    }


}
