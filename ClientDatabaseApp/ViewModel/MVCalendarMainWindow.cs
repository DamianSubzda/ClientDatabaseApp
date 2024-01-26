using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ClientDatabaseApp.Model;

namespace ClientDatabaseApp.ViewModel
{
    public class MVCalendarMainWindow : INotifyPropertyChanged
    {
        public ICommand MouseEnterCommand { get; private set; }
        public ICommand MouseLeaveCommand { get; private set; }
        public ICommand MouseClickCommand { get; private set; }
        public ICommand Button_Click_PrevMonthCommand { get; private set; }
        public ICommand Button_Click_NextMonthCommand { get; private set; }

        public MVCalendarMainWindow()
        {
            Button_Click_PrevMonthCommand = new DelegateCommand<RoutedEventArgs>(Button_Click_PrevMonth);
            Button_Click_NextMonthCommand = new DelegateCommand<RoutedEventArgs>(Button_Click_NextMonth);
            MouseEnterCommand = new DelegateCommand<MouseEventArgs>(OnMouseEnter);
            MouseLeaveCommand = new DelegateCommand<MouseEventArgs>(OnMouseLeave);
            MouseClickCommand = new DelegateCommand<MouseButtonEventArgs>(OnMouseClick);
        }
        private enum MonthEnum
        {
            Styczeń = 1, Luty, Marzec, Kwiecień, Maj, Czerwiec, Lipiec, Sierpień, Wrzesień, Październik, Listopad, Grudzień
        }

        private enum DaysEnum
        {
            Monday = 1, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday
        }

        private string month = ((MonthEnum)DateTime.Now.Month).ToString() + " " + DateTime.Now.Year.ToString();
        private DateTime monthToDisplay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        public event PropertyChangedEventHandler PropertyChanged;

        public string Month
        {
            get => month;
            set
            {
                if (month != value)
                {
                    month = value;
                    OnPropertyChanged(nameof(Month));
                }
            }
        }

        private List<DayInfo> days;
        public List<DayInfo> Days
        {
            get => days;
            set
            {
                if (days != value)
                {
                    days = value;
                    OnPropertyChanged(nameof(Days));
                }
            }
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
            monthToDisplay = monthToDisplay.AddMonths(amountOfMonthToChange);
            Month = (MonthEnum)(monthToDisplay.Month) + " " + monthToDisplay.Year.ToString();

            GetDaysFromMonth();
        }

        private void InitialCalendar()
        {
            GetDaysFromMonth();
        }

        private void GetDaysFromMonth()
        {
            var context = new DBContextHVAC();
            var followUps = context.FollowUpDBSet
                .Where(line => line.DateOfAction.Year == monthToDisplay.Year && line.DateOfAction.Month == monthToDisplay.Month)
                .ToList();

            var followUpCounts = followUps
                .GroupBy(line => line.DateOfAction.Day)
                .Select(group => new { Day = group.Key, Count = group.Count() })
                .ToList();

            int days = DateTime.DaysInMonth(monthToDisplay.Year, monthToDisplay.Month);
            DayOfWeek dayOfWeek = monthToDisplay.DayOfWeek;
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

        private void OnMouseEnter(MouseEventArgs e)
        {

            //StackPanel stackPanel = (StackPanel)sender;
            //Label label = FindChild<Label>(stackPanel, "DayNumber");

            //if (label != null && !string.IsNullOrEmpty((string)label.Content))
            //{
            //    stackPanel.Background = new SolidColorBrush(Colors.LightGray);
            //}
        }

        private void OnMouseLeave(MouseEventArgs e)
        {
            //StackPanel stackPanel = (StackPanel)sender;
            //stackPanel.Background = Brushes.Transparent;
        }

        private void OnMouseClick(MouseButtonEventArgs e)
        {
            //listbox_todolist.Items.Clear();
            //StackPanel stackPanel = (StackPanel)sender;
            //Label label = FindChild<Label>(stackPanel, "DayNumber");

            //if (label != null && !string.IsNullOrEmpty((string)label.Content))
            //{
            //    if (Days.Any(dayinfo => dayinfo.DayNumber == (string)label.Content))
            //    {
            //        try
            //        {
            //            int day_number = int.Parse((string)label.Content);
            //            var context = new DBContextHVAC();
            //            var followUp = context.FollowUpDBSet
            //                .Where(line => line.DateOfAction.Year == monthToDisplay.Year
            //                            && line.DateOfAction.Month == monthToDisplay.Month
            //                            && line.DateOfAction.Day == day_number)
            //                .ToList();

            //            if (!followUp.Any())
            //            {
            //                return;
            //            }

            //            foreach (var item in followUp)
            //            {
            //                //listbox_todolist.Items.Add(item.Note);
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            MessageBox.Show(ex.Message);
            //        }
            //    }
            //}
        }

        private T FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
        {
            if (parent == null)
            {
                return null;
            }

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                T childType = (T)child;

                if (childType == null)
                {
                    foundChild = FindChild<T>(child, childName);

                    if (foundChild != null)
                    {
                        break;
                    }
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    FrameworkElement frameworkElement = (FrameworkElement)child;
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }

    }
    public class DayInfo //Lista followUp'ów 
    {
        public string DayNumber { get; set; }
        public int FollowUpCount { get; set; }
    }

}
