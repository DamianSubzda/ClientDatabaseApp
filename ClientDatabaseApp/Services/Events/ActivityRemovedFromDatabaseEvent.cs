using ClientDatabaseApp.Model;
using Prism.Events;


namespace ClientDatabaseApp.Services.Events
{
    public class ActivityRemovedFromDatabaseEvent : PubSubEvent<Activity>
    {
    }
}
