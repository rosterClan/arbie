
using System.Text.Json;

class boombetFactory : eventFactory
{
    public override Event create_event(eventDTO eventData, object mutex_lock)
    {
        return new boombetEvent(eventData, mutex_lock);
    }
}
