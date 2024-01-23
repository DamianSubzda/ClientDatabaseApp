using ClientDatabaseApp.DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ClientDatabaseApp
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

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

        private List<string> days;
        public List<string> Days
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

        private void Button_Click_NextMonth(object sender, RoutedEventArgs e)
        {
            int year = monthToDisplay.Year;
            int month = monthToDisplay.Month;

            if (monthToDisplay.Month == 12)
            {
                Month = "Styczeń" + " " + (year + 1).ToString();
                monthToDisplay = new DateTime(year + 1, 1, 1);
            }
            else
            {
                Month = ((MonthEnum)month + 1).ToString() + " " + year.ToString();
                monthToDisplay = new DateTime(year, month + 1, 1);
            }
            GetDaysFromMonth();
        }

        private void Button_Click_PrevMonth(object sender, RoutedEventArgs e)
        {
            int year = monthToDisplay.Year;
            int month = monthToDisplay.Month;

            if (monthToDisplay.Month == 1)
            {
                Month = "Grudzień" + " " + (year - 1).ToString();
                monthToDisplay = new DateTime(year - 1, 12, 1);
            }
            else
            {
                Month = ((MonthEnum)month - 1).ToString() + " " + year.ToString();
                monthToDisplay = new DateTime(year, month - 1, 1);
            }
            GetDaysFromMonth();
        }

        private void GetDaysFromMonth()
        {
            int days = DateTime.DaysInMonth(monthToDisplay.Year, monthToDisplay.Month);
            DayOfWeek dayOfWeek = monthToDisplay.DayOfWeek;
            int nr = (int)(DaysEnum)dayOfWeek;

            if (dayOfWeek == DayOfWeek.Sunday)
            {
                nr = 7;
            }

            List<string> listOfDays = new List<string>();

            for (int i = 1; i < nr; i++)
            {
                listOfDays.Add("");
            }

            for (int i = 1; i <= days; i++)
            {
                listOfDays.Add(i.ToString());
            }

            Days = listOfDays;
        }

        private void Calendar_StackPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            StackPanel stackPanel = (StackPanel)sender;
            Label label = FindChild<Label>(stackPanel, "DayNumber");

            if (label != null && !string.IsNullOrEmpty((string)label.Content))
            {
                stackPanel.Background = new SolidColorBrush(Colors.LightGray);
            }
        }

        private void Calendar_StackPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            StackPanel stackPanel = (StackPanel)sender;
            stackPanel.Background = Brushes.Transparent;
        }

        private void Calendar_StackPanel_Click(object sender, MouseButtonEventArgs e)
        {
            listbox_todolist.Items.Clear();
            StackPanel stackPanel = (StackPanel)sender;
            Label label = FindChild<Label>(stackPanel, "DayNumber");

            if (label != null && !string.IsNullOrEmpty((string)label.Content))
            {
                //AddListOfTasks
                if (Days.Contains((string)label.Content))
                {
                    try
                    {
                        int day_number = int.Parse((string)label.Content);
                        //FollowUpData followUp = followups[day_number - 1];
                        //if (followUp == null)
                        //{
                        //    return;
                        //}

                        //List<string> cos = followUp.Task;

                        //foreach (string item in cos)
                        //{
                        //    listbox_todolist.Items.Add(item);
                        //}
                    }
                    catch { }
                }
            }
        }


        private T FindChild<T>(DependencyObject parent, string childName)
    where T : DependencyObject
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
}
