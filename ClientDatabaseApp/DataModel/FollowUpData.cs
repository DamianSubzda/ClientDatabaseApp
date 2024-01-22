using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientDatabaseApp.DataModel
{
    class FollowUpData
    {
        private int day;
        private readonly List<string> task;

        public FollowUpData(int day, params string[] tasks)
        {
            Day = day;
            task = new List<string>();
            foreach (string task in tasks)
            {
                this.task.Add(task);
            }
        }

        public int Day
        {
            get => day;
            set => day = value;
        }
        public List<string> Task
        {
            get => task;
        }
    }
}
