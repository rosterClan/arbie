
using System.Text.Json;

class pointsbetFactory : eventFactory
{
    public override Event create_event(eventDTO eventData, object mutex_lock)
    {
        return new pointsbetEvent(eventData, mutex_lock);
    }
}
