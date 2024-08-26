using System;

namespace ClientDatabaseApp.Services.Exceptions
{
    internal class ClientAlreadyExistsException : Exception
    {
        public ClientAlreadyExistsException() 
            : base("The client already exists!") { }

        public ClientAlreadyExistsException(string clientName)
            : base($"The client already exists: {clientName}") { }
    }
}
