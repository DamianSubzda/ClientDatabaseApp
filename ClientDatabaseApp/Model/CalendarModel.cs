using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ClientDatabaseApp.Model
{
    public class CalendarModel
    {
        public string HeaderToDisplay { get; set; }
        public ObservableCollection<DayInfo> DaysOfCurrentMonth { get; set; }
        public DateTime DateToDisplay { get; set; }
        public List<FollowUp> FollowUps { get; set; }
    }
    public class DayInfo
    {
        public string DayNumber { get; set; }
        public int FollowUpCount { get; set; }
    }

}
