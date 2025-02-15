


using System.Text.Json;

class mockEventFactory : eventFactory
{
    public override Event create_event(eventDTO eventData, object mutex_lock)
    {
        return new mockEvent(eventData, mutex_lock);
    }
}

