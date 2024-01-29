using System;
using System.Collections.Generic;

namespace ClientDatabaseApp.Model
{
    public class MCalendar
    {
        public string HeaderToDisplay { get; set; }
        public List<DayInfo> DaysOfCurrentMonth { get; set; }
        public DateTime DateToDisplay { get; set; }
        public List<FollowUp> FollowUps { get; set; }
    }
    public class DayInfo
    {
        public string DayNumber { get; set; }
        public int FollowUpCount { get; set; }
    }

}
