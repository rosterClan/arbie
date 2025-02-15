class platform {
    private eventFactory factory; 
    private eventProvider provider;
    private Dictionary<string, Event> events; 
    private Dictionary<string, Thread> threads; 
    private readonly object mutex_lock = new object();

    public platform(eventFactory factory, eventProvider provider) {
        this.factory = factory;
        this.provider = provider;
        this.events = new Dictionary<string, Event>();
        this.threads = new Dictionary<string, Thread>();
    }

    private void initEvents() {
        List<eventDTO> eventData = new List<eventDTO>();
        try {
            eventData = this.provider.execute();
        } catch (Exception e) {
            Console.WriteLine(e);
        }

        for (int idx = 0; idx < eventData.Count; idx++) {
            eventDTO obj = eventData[idx];
            if (!this.events.ContainsKey(obj.getHashId()) && obj.getStartTime() > DateTime.UtcNow) {
                
                //if (obj.getPlatformSpecificDetails().GetProperty("raceId").ToString() == "77904208") {
                    this.create_event(obj);
                    //break;
                //}
            }
        }
    }

    private void create_event(eventDTO event_details) {
        Event n_event = factory.create_event(event_details, mutex_lock);
        Thread eventThread = new Thread(() => n_event.execute());
        threads.Add(event_details.getHashId(), eventThread);
        eventThread.Start();
        events.Add(event_details.getHashId(), n_event);
    }

    private void remove_event(Event d_event) {
        this.threads.Remove(d_event.getEventDetails().getHashId());
        this.events.Remove(d_event.getEventDetails().getHashId());
    }

    private void cleanUpEvents() {
        List<Event> shutdown_list = new List<Event>();
        List<Event> reboot_list = new List<Event>();

        foreach(Event item in this.events.Values)
        {
            switch (item.getStatus()) {
                case 1:
                    shutdown_list.Add(item);
                    break;
                case 2:
                    reboot_list.Add(item);                  
                    break;
            }
        }

        for (int i = 0; i < shutdown_list.Count; i++) {
            this.remove_event(shutdown_list[i]);
        }

        for (int i = 0; i < reboot_list.Count; i++) {
            this.remove_event(reboot_list[i]);
            this.create_event(reboot_list[i].getEventDetails());
        }
    }

    public void execute() {
        while (true) {
            lock (this.mutex_lock) {
                this.cleanUpEvents();
                this.initEvents();

                Monitor.Wait(this.mutex_lock);
            }
        }
    }
}