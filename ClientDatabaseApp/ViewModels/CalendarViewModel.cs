using ClientDatabaseApp.Models;
using ClientDatabaseApp.Services;
using ClientDatabaseApp.Services.Repositories;
using ClientDatabaseApp.Services.Events;
using ClientDatabaseApp.Views;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace ClientDatabaseApp.ViewModels
{
    public class CalendarViewModel : BaseViewModel
    {
        public ICommand MouseClickCommand { get; private set; }
        public ICommand ButtonClickPrevMonthCommand { get; private set; }
        public ICommand ButtonClickNextMonthCommand { get; private set; }
        public ICommand PickActivityCommand { get; private set; }
        public ICommand DeleteActivityCommand { get; private set; }

        private CalendarDataModel _calendarModel;
        public CalendarDataModel CalendarModel
        {
            get => _calendarModel;
            set => SetField(ref _calendarModel, value, nameof(CalendarModel));
        }

        public class CalendarDataModel
        {
            public string HeaderToDisplay { get; set; }
            public ObservableCollection<DayInfo> DaysOfCurrentMonth { get; set; }
            public DateTime DateToDisplay { get; set; }
            public List<Activity> Activities { get; set; }
        }

        public class DayInfo
        {
            public string DayNumber { get; set; }
            public int ActivitiesCount { get; set; }
        }

        public enum MonthEnum
        {
            Styczeń = 1, Luty, Marzec, Kwiecień, Maj, Czerwiec, Lipiec, Sierpień, Wrzesień, Październik, Listopad, Grudzień
        }

        public enum DaysEnum
        {
            Monday = 1, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday
        }

        public ObservableCollection<DayInfo> Days
        {
            get => CalendarModel.DaysOfCurrentMonth;
            set
            {
                if (CalendarModel.DaysOfCurrentMonth != value)
                {
                    CalendarModel.DaysOfCurrentMonth = value;
                    OnPropertyChanged(nameof(Days));
                }
            }
        }

        public string DataToDisplay
        {
            get => CalendarModel.HeaderToDisplay;
            set
            {
                if (CalendarModel.HeaderToDisplay != value)
                {
                    CalendarModel.HeaderToDisplay = value;
                    OnPropertyChanged(nameof(DataToDisplay));
                }
            }
        }

        private ObservableCollection<Activity> _activity;
        public ObservableCollection<Activity> Activity
        {
            get => _activity;
            set => SetField(ref _activity, value, nameof(Activity));
        }

        private Activity _selectedActivity;

        public Activity SelectedActivity
        {
            get => _selectedActivity;
            set => SetField(ref _selectedActivity, value, nameof(SelectedActivity));
        }

        private readonly IActivityRepo _activityRepo;
        private readonly IClientRepo _clientRepo;
        private readonly IDialogService _dialogService;
        private readonly IEventAggregator _eventAggregator;

        public CalendarViewModel() { }

        public CalendarViewModel(IActivityRepo activityRepo, IDialogService dialogService, IClientRepo clientRepo, IEventAggregator eventAggregator)
        {
            _activityRepo = activityRepo;
            _clientRepo = clientRepo;
            _dialogService = dialogService;
            _eventAggregator = eventAggregator;

            _eventAggregator.GetEvent<ActivityAddedToDatabaseEvent>().Subscribe(OnActivityAdded);
            _eventAggregator.GetEvent<ActivityRemovedFromDatabaseEvent>().Subscribe(OnActivityRemoved);
            _eventAggregator.GetEvent<ActivityUpdatedInDatabaseEvent>().Subscribe(OnActivityUpdated);

            ButtonClickPrevMonthCommand = new DelegateCommand<RoutedEventArgs>(ButtonClickPrevMonth);
            ButtonClickNextMonthCommand = new DelegateCommand<RoutedEventArgs>(ButtonClickNextMonth);
            PickActivityCommand = new DelegateCommand<RoutedEventArgs>(PickActivity);
            DeleteActivityCommand = new DelegateCommand<RoutedEventArgs>(DeleteActivity);
            MouseClickCommand = new DelegateCommand<string>(OnMouseClick);

            CalendarModel = new CalendarDataModel
            {
                HeaderToDisplay = ((MonthEnum)DateTime.Now.Month).ToString() + " " + DateTime.Now.Year.ToString(),
                DateToDisplay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                DaysOfCurrentMonth = new ObservableCollection<DayInfo>()
            };

            InitializeCalendar();
        }

        private void OnActivityRemoved(Activity removedActivity)
        {
            GetDaysFromMonth();
        }

        private void OnActivityAdded(Activity addedActivity)
        {
            GetDaysFromMonth();
        }

        private void OnActivityUpdated(Activity updatedActivity)
        {
            var activities = new ObservableCollection<Activity>(Activity);
            var existingActivity = activities.FirstOrDefault(a => a.ActivityId == updatedActivity.ActivityId);
            if (existingActivity != null)
            {
                int index = activities.IndexOf(existingActivity);
                if (index >= 0)
                {
                    activities[index] = updatedActivity;
                }
            }

            Activity = activities;
            SelectedActivity = updatedActivity;
        }

        private void InitializeCalendar()
        {
            GetDaysFromMonth();
        }

        private void ButtonClickNextMonth(RoutedEventArgs e)
        {
            ChangeMonth(1);
        }

        private void ButtonClickPrevMonth(RoutedEventArgs e)
        {
            ChangeMonth(-1);
        }

        private void PickActivity(RoutedEventArgs e)
        {
            if (SelectedActivity != null)
            {
                ShowActivity showActivity = new ShowActivity();
                ShowActivityViewModel showActivityViewModel = new ShowActivityViewModel(SelectedActivity, () => showActivity.Close(), _clientRepo, _activityRepo);
                showActivity.DataContext = showActivityViewModel;
                showActivity.ShowDialog();
            }
        }

        private void DeleteActivity(RoutedEventArgs e)
        {
            if (SelectedActivity != null)
            {
                bool result = _dialogService.Confirm("Czy jesteś pewien, że chcesz usunąć te wydarzenie?");

                if (result)
                {
                    _activityRepo.DeleteActivity(SelectedActivity);
                    Activity.Remove(SelectedActivity);
                    SelectedActivity = null;
                }
            }
        }

        private void ChangeMonth(int amountOfMonthToChange)
        {
            CalendarModel.DateToDisplay = CalendarModel.DateToDisplay.AddMonths(amountOfMonthToChange);
            DataToDisplay = (MonthEnum)CalendarModel.DateToDisplay.Month + " " + CalendarModel.DateToDisplay.Year.ToString();

            GetDaysFromMonth();
        }

        private async void OnMouseClick(string dayNumber)
        {
            if (Days.Any(dayInfo => dayInfo.DayNumber == dayNumber))
            {
                try
                {
                    int day_number = int.Parse(dayNumber);
                    var activities = await _activityRepo.GetActivitiesOfDay(CalendarModel.DateToDisplay.Year, CalendarModel.DateToDisplay.Month, day_number);

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
            var activitiesCount = (await _activityRepo.GetActivitiesCountOfMonth(CalendarModel.DateToDisplay.Year, CalendarModel.DateToDisplay.Month))
                .ToDictionary(ac => ac.Day, ac => ac.Count);

            int daysInMonth = DateTime.DaysInMonth(CalendarModel.DateToDisplay.Year, CalendarModel.DateToDisplay.Month);
            int firstDayOfWeek = (int)new DateTime(CalendarModel.DateToDisplay.Year, CalendarModel.DateToDisplay.Month, 1).DayOfWeek;

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
