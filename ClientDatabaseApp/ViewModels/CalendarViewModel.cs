using ClientDatabaseApp.Model;
using ClientDatabaseApp.Service;
using ClientDatabaseApp.Service.API;
using ClientDatabaseApp.Service.Repository;
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
        public ICommand PickActivityCommand { get; private set; }
        public ICommand DeleteActivityCommand { get; private set; }

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

        private ObservableCollection<Activity> _activity;
        public ObservableCollection<Activity> Activity
        {
            get { return _activity; }
            set
            {
                _activity = value;
                OnPropertyChanged(nameof(Activity));
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
        private Activity _selectedActivity;

        public Activity SelectedActivity
        {
            get => _selectedActivity;
            set
            {
                _selectedActivity = value;
                OnPropertyChanged(nameof(SelectedActivity));
            }
        }

        private readonly IActivityRepo _activityRepo;
        private readonly IClientRepo _clientRepo;
        private readonly IDialogService _dialogService;

        public CalendarViewModel(IActivityRepo activityRepo, IDialogService dialogService, IClientRepo clientRepo)
        {
            _activityRepo = activityRepo;
            _clientRepo = clientRepo;
            _dialogService = dialogService;
            Button_Click_PrevMonthCommand = new DelegateCommand<RoutedEventArgs>(Button_Click_PrevMonth);
            Button_Click_NextMonthCommand = new DelegateCommand<RoutedEventArgs>(Button_Click_NextMonth);
            PickActivityCommand = new DelegateCommand<RoutedEventArgs>(PickActivity);
            DeleteActivityCommand = new DelegateCommand<RoutedEventArgs>(DeleteActivity);
            MouseClickCommand = new DelegateCommand<string>(OnMouseClick);

            calendarModel.HeaderToDisplay = ((MonthEnum)DateTime.Now.Month).ToString() + " " + DateTime.Now.Year.ToString();
            calendarModel.DateToDisplay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            InitialCalendar();
            InitializeAsync();
            _clientRepo = clientRepo;
        }

        public async void InitializeAsync()
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
        private void PickActivity(RoutedEventArgs e)
        {
            if(SelectedActivity != null)
            {
                ShowActivity showActivity = new ShowActivity();
                ShowActivityViewModel showActivityViewModel = new ShowActivityViewModel(SelectedActivity, () => showActivity.Close(), _clientRepo);
                showActivity.DataContext = showActivityViewModel;
                showActivity.ShowDialog();
            }
        }
        private void DeleteActivity(RoutedEventArgs e)
        {
            if (SelectedActivity != null)
            {
                bool result = _dialogService.Confirm("Czy jesteś pewien, że chcesz usunąć te wydarzenie?");

                if(result)
                {
                    _activityRepo.DeleteActivity(SelectedActivity);
                    Activity.Remove(SelectedActivity);
                    SelectedActivity = null;
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
                    var activities = await _activityRepo.GetActivitiesOfDay(calendarModel.DateToDisplay.Year, calendarModel.DateToDisplay.Month, day_number);

                    Activity = new ObservableCollection<Activity>(activities);
                }
                catch (Exception ex)
                {
                    _dialogService.ShowMessage(ex.Message);
                }
            }
        }
        private async void GetDaysFromMonth()
        {
            var activitiesCount = (await _activityRepo.GetActivitiesCountOfMonth(calendarModel.DateToDisplay.Year, calendarModel.DateToDisplay.Month))
                .ToDictionary(ac => ac.Day, ac => ac.Count);

            int daysInMonth = DateTime.DaysInMonth(calendarModel.DateToDisplay.Year, calendarModel.DateToDisplay.Month);
            int firstDayOfWeek = (int)new DateTime(calendarModel.DateToDisplay.Year, calendarModel.DateToDisplay.Month, 1).DayOfWeek;

            if (firstDayOfWeek == 0)
                firstDayOfWeek = 7;

            List<DayInfo> listOfDays = new List<DayInfo>();

            for (int i = 1; i < firstDayOfWeek; i++)
            {
                listOfDays.Add(new DayInfo
                {
                    DayNumber = "",
                    ActivitiesCount = 0
                });
            }

            for (int day = 1; day <= daysInMonth; day++)
            {
                listOfDays.Add(new DayInfo
                {
                    DayNumber = day.ToString(),
                    ActivitiesCount = activitiesCount.TryGetValue(day, out int count) ? count : 0
                });
            }

            Days = new ObservableCollection<DayInfo>(listOfDays);
        }

    }


}
