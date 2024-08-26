using ClientDatabaseApp.Models;
using ClientDatabaseApp.Services.Events;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace ClientDatabaseApp.Services.Repositories
{
    public interface IActivityRepo
    {
        Task UpdateActivity(Activity activity);
        Task<List<Activity>> GetActivitiesOfDay(int year, int month, int day);
        Task<List<(int Day, int Count)>> GetActivitiesCountOfMonth(int year, int month);
        Task CreateActivity(Client client, DateTime date, string note);
        Task DeleteActivity(Activity activity);
    }

    public class ActivityRepo : IActivityRepo
    {
        private readonly PostgresContext _context;
        private readonly IEventAggregator _eventAggregator;

        public ActivityRepo(PostgresContext context, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
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
                _eventAggregator.GetEvent<ActivityAddedToDatabaseEvent>().Publish(activity);
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
            _eventAggregator.GetEvent<ActivityRemovedFromDatabaseEvent>().Publish(activity);
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

        public async Task UpdateActivity(Activity activity)
        {
            var foundActivity = await _context.Activities.FirstOrDefaultAsync(a => a.ActivityId == activity.ActivityId);

            if (foundActivity != null)
            {
                foundActivity.ActivityId = activity.ActivityId;
                foundActivity.ClientId = activity.ClientId;
                foundActivity.DateOfCreation = activity.DateOfCreation;
                foundActivity.DateOfAction = activity.DateOfAction;
                foundActivity.Note = activity.Note;

                await _context.SaveChangesAsync();
                _eventAggregator.GetEvent<ActivityUpdatedInDatabaseEvent>().Publish(activity);
            }
            else
            {
                throw new Exception("Client not found.");
            }
        }


    }
}
