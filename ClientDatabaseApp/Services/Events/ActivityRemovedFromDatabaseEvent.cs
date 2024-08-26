using ClientDatabaseApp.Models;
using Prism.Events;


namespace ClientDatabaseApp.Services.Events
{
    public class ActivityRemovedFromDatabaseEvent : PubSubEvent<Activity>
    {
    }
}
