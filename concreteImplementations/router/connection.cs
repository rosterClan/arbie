using System.Text.Json;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Net;
using System.Dynamic;


abstract class connection {
    private Dictionary<string, int> hits; 
    private Dictionary<string, bool> blacklist; 
    private string containerId;
    private int port; 
    private string ip; 
    private string loc; 
    private bool is_active; 

    private HttpClient client;

    public connection(string containerId, int port, string loc) {
        this.containerId = containerId; 
        this.port = port; 
        this.loc = loc; 

        this.client = null; 
        this.hits = new Dictionary<string, int>();
        this.blacklist = new Dictionary<string, bool>();

        this.checkOnline();
    }

    private void checkOnline() {
        try{
            HttpClient local_client = this.produceClient();
            string responseBody = local_client.GetStringAsync("https://api.ipify.org?format=json").GetAwaiter().GetResult();
            JsonDocument document = JsonDocument.Parse(responseBody);

            this.ip = document.RootElement.GetProperty("ip").ToString();
            this.is_active = true;
        } catch (Exception e) {
            Console.WriteLine(e);
            this.is_active = false;
        }
    }

    virtual public WebProxy produceProxy() {
        return new WebProxy("http://127.0.0.1:"+this.port.ToString());
    }

    virtual public HttpClient produceClient() {
        if (this.client == null) {
            var proxy = new WebProxy("http://127.0.0.1:"+this.port.ToString());
            var handler = new HttpClientHandler{Proxy = proxy, UseProxy = true};
            this.client = new HttpClient(handler);
        }
        return this.client;
    }

    public bool isActive() {
        return this.is_active; 
    }

    public string getIp() {
        return this.ip; 
    }

    public int getPort() {
        return this.port; 
    }

    public string getLoc() {
        return this.loc; 
    }

    public string getContainerID() {
        return this.containerId; 
    }
}