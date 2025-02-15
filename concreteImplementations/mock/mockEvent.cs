



using System.Text.Json;

class mockEvent : Event
{
    public mockEvent(eventDTO eventData, object platform_lock) : base(eventData, platform_lock) {}

    protected override void protectedExecution()
    {
        while (this.status == 0) {
            Thread.Sleep(5000);
            this.checkShutdown();
        }

        lock (this.platform_lock) {
            Monitor.Pulse(this.platform_lock);
        }
    }
}