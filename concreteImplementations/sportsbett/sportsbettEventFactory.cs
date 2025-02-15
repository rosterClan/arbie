


using System.Text.Json;

class sportsbettEventFactory : eventFactory
{
    public override Event create_event(eventDTO eventData, object mutex_lock)
    {
        return new sportsbettEvent(eventData, mutex_lock);
    }
}

