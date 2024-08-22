using ClientDatabaseApp.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ClientDatabaseApp.Service.Repository
{
    public interface IActivityRepo
    {
        Task<List<Activity>> GetActivitiesOfDay(int year, int month, int day);
        Task<List<(int Day, int Count)>> GetActivitiesCountOfMonth(int year, int month);
        Task CreateActivity(Client client, DateTime date, string note);
        Task DeleteActivity(Activity activity);
    }
    public class ActivityRepo : IActivityRepo
    {
        private readonly PostgresContext _context;
        public ActivityRepo(PostgresContext context)
        {
            _context = context;
        }
        public async Task CreateActivity(Client client, DateTime date, string note)
        {
            try
            {
                var activity = new Activity()
                {
                    ClientId = client.ClientId,
                    DateOfAction = date,
                    Note = note
                };

                _context.Activities.Add(activity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving activity: {ex.Message}");
            }
        }

        public async Task DeleteActivity(Activity activity)
        {
            if (activity == null)
            {
                throw new ArgumentNullException(nameof(activity));
            }

            _context.Activities.Remove(activity);
            await _context.SaveChangesAsync();

        }

        public async Task<List<(int Day, int Count)>> GetActivitiesCountOfMonth(int year, int month)
        {
            var activities = await _context.Activities
                .Where(line => line.DateOfAction.HasValue
                            && line.DateOfAction.Value.Year == year
                            && line.DateOfAction.Value.Month == month)
                .ToListAsync();

            var activitiesCount = activities
                .GroupBy(line => line.DateOfAction.Value.Day)
                .Select(group => (Day: group.Key, Count: group.Count()))
                .ToList();

            return activitiesCount;
        }

        public async Task<List<Activity>> GetActivitiesOfDay(int year, int month, int day)
        {
            var activity = await _context.Activities
                            .Where(line => line.DateOfAction.HasValue
                                    && line.DateOfAction.Value.Year == year
                                    && line.DateOfAction.Value.Month == month
                                    && line.DateOfAction.Value.Day == day)
                            .ToListAsync();
            return activity;
        }
    }
}
