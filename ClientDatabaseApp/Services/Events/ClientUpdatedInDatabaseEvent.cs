using ClientDatabaseApp.Models;
using Prism.Events;


namespace ClientDatabaseApp.Services.Events
{
    public class ClientUpdatedInDatabaseEvent : PubSubEvent<Client>
    {
    }
}
