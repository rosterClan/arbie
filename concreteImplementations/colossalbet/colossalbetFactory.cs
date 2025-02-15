
using System.Text.Json;

class colossalbetFactory : eventFactory
{
    public override Event create_event(eventDTO eventData, object mutex_lock)
    {
        return new colossalbetEvent(eventData, mutex_lock);
    }
}
