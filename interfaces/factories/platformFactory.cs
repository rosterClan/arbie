
abstract class platformFactory {
    protected Dictionary<string, eventFactory> eventFactories; 
    protected Dictionary<string, eventProvider> eventProviders;

    public platformFactory() {
        this.eventFactories = new Dictionary<string, eventFactory>();
        this.eventProviders = new Dictionary<string, eventProvider>();
    }

    public abstract platform createPlatform(string platform_name);

    public void createBinding(string name, eventFactory factory, eventProvider provider) {
        this.eventFactories.Add(name, factory);
        this.eventProviders.Add(name, provider);
    }

    public void removeBinding(string name) {
        this.eventFactories.Remove(name);
        this.eventProviders.Remove(name);
    }
}






