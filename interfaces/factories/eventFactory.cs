
using System.Text.Json;

abstract class eventFactory {
    public eventFactory() {
        
    }
    
    public abstract Event create_event(eventDTO eventData, object mutex_lock);

}


