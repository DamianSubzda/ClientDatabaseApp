using ClientDatabaseApp.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientDatabaseApp.Service.Repository
{
    public interface IClientRepo
    {
        Task AddClient(Client client);
        Task DeleteClient(Client client);
        Task<bool> CheckIfClientExists(Client client);

    }
    public class ClientRepo : IClientRepo
    {
        private readonly PostgresContext _context;
        public ClientRepo(PostgresContext context)
        {
            _context = context;
        }

        public async Task AddClient(Client client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            var result = await CheckIfClientExists(client);
            if (result)
            {
                throw new Exception("client exists");
            }

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteClient(Client client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            _context.Clients.Remove(client);

            await _context.SaveChangesAsync();
        }

        public async Task<bool> CheckIfClientExists(Client client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            return await _context.Clients.AnyAsync(c => c.ClientName == client.ClientName && c.Email == client.Email);
        }
    }
}
