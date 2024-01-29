using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ClientDatabaseApp.Model;
using ClientDatabaseApp.Service;

namespace ClientDatabaseApp.ViewModel
{
    public class MVCalendarMainWindow : INotifyPropertyChanged
    {

        private MCalendar calendarModel = new MCalendar();
        public ICommand MouseClickCommand { get; private set; }
        public ICommand Button_Click_PrevMonthCommand { get; private set; }
        public ICommand Button_Click_NextMonthCommand { get; private set; }

        public enum MonthEnum
        {
            Styczeń = 1, Luty, Marzec, Kwiecień, Maj, Czerwiec, Lipiec, Sierpień, Wrzesień, Październik, Listopad, Grudzień
        }

        public enum DaysEnum
        {
            Monday = 1, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public List<DayInfo> Days
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

        private List<string> _followUp;
        public List<string> FollowUp
        {
            get { return _followUp; }
            set
            {
                _followUp = value;
                OnPropertyChanged(nameof(FollowUp));
            }
        }


        public MVCalendarMainWindow()
        {
            Button_Click_PrevMonthCommand = new DelegateCommand<RoutedEventArgs>(Button_Click_PrevMonth);
            Button_Click_NextMonthCommand = new DelegateCommand<RoutedEventArgs>(Button_Click_NextMonth);
            MouseClickCommand = new DelegateCommand<string>(OnMouseClick);

            calendarModel.HeaderToDisplay = ((MonthEnum)DateTime.Now.Month).ToString() + " " + DateTime.Now.Year.ToString();
            calendarModel.DateToDisplay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            InitialCalendar();
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

                    FollowUp = followUp.Select(line => line.Note).ToList();
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

            Days = listOfDays;
        }

    }


}
