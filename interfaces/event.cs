using System.Text.Json;
abstract class Event {
    protected eventDTO eventData; 
    protected int status; 
    protected object platform_lock; 
    protected database db;
    protected router requester; 

    public Event(eventDTO eventData, object platform_lock) {
        this.eventData = eventData;
        this.status = 0; 
        this.platform_lock = platform_lock; 
        
        this.db = database.Instance;
        this.requester = router.Instance;
    }

    public void execute() {
        try {
            this.protectedExecution();
        } catch (Exception e) {
            Console.WriteLine(this.eventData.getPlatformSpecificDetails().ToString() + " :::: " + e);
            this.status = 2;
        }
    }

    abstract protected void protectedExecution();

    protected void checkShutdown() {
        if (this.eventData.getStartTime() < DateTime.UtcNow) {
            this.status = 1; 
        }
    }

    public int getStatus() {
        return this.status;
    }

    public eventDTO getEventDetails() {
        return this.eventData;
    }
}