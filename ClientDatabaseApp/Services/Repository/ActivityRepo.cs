using ClientDatabaseApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientDatabaseApp.Service.Repository
{
    public interface IActivityRepo
    {
        Task CreateActivity(Client client, DateTime date, string Note);
        Task DeleteActivity(Activity activity);
    }
    public class ActivityRepo : IActivityRepo
    {
        private readonly PostgresContext _context;
        public ActivityRepo(PostgresContext context)
        {
            _context = context;
        }
        public async Task CreateActivity(Client client, DateTime date, string Note)
        {
            var activity = new Activity()
            {
                Client = client,
                DateOfAction = date,
                Note = Note
            };

            _context.Activities.Add(activity);
            await _context.SaveChangesAsync();
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
    }
}
