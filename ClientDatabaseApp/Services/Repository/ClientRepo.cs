﻿using ClientDatabaseApp.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace ClientDatabaseApp.Service.Repository
{
    public interface IClientRepo
    {
        Task AddClient(Client client);
        Task UpdateClient(Client client);
        Task<Client> GetClient(int id);
        Task<List<Client>> GetAllClients();
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

        public async Task<List<Client>> GetAllClients()
        {
            return await _context.Clients.ToListAsync();
        }

        public async Task<Client> GetClient(int id)
        {
            return await _context.Clients.FirstOrDefaultAsync(c => c.ClientId == id);
        }

        public async Task UpdateClient(Client client)
        {
            var foundClient = await _context.Clients.FirstOrDefaultAsync(c => c.ClientId == client.ClientId);

            if (foundClient != null)
            {
                foundClient.ClientName = client.ClientName;
                foundClient.Phonenumber = client.Phonenumber;
                foundClient.Email = client.Email;
                foundClient.City = client.City;
                foundClient.Facebook = client.Facebook;
                foundClient.Instagram = client.Instagram;
                foundClient.PageURL = client.PageURL;
                foundClient.Data = client.Data;
                foundClient.Owner = client.Owner;
                foundClient.Note = client.Note;
                foundClient.Status = client.Status;

                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Client not found.");
            }
        }
    }
}
