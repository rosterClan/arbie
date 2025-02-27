using System.Text.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using System.Collections;
using System.Transactions;
using System.Net;
using System.Security.AccessControl;

class hydra : abstractRouter
{
    private List<connection> connections; 
    private static readonly object _padlock = new object();
    private Dictionary<string, List<connection>> valid_connections;
    private DateTime reset;
    public hydra() {
        this.reset = DateTime.Now.AddHours(1);

        this.valid_connections = new Dictionary<string, List<connection>>();
        this.connections = new List<connection>();
        string targetImage = "arbie:latest";

        using var client = new DockerClientConfiguration(new Uri("npipe://./pipe/docker_engine")).CreateClient();
        var containers = client.Containers.ListContainersAsync(new ContainersListParameters{All = true}).GetAwaiter().GetResult();
        var targetContainers = containers.Where(c => c.Image.Contains(targetImage, StringComparison.OrdinalIgnoreCase));

        bool length_error = true;

        foreach (var container in targetContainers) {
            string connectionId = container.ID;
            string loc = container.Names.FirstOrDefault();
            int container_port = -1;
            foreach (var port in container.Ports) {
                container_port = port.PrivatePort;
                break;
            }
            this.connections.Add(new remote_con(connectionId, container_port, loc));
            length_error = false; 
        }

        if (length_error) {
            throw new Exception("No proxy containers could be found. Please setup said proxy containers.");
        }
        this.connections.Add(new local_con("na", -1, "local"));
    }

    public static HttpRequestMessage CloneHttpRequestMessage(HttpRequestMessage req)
    {
        HttpRequestMessage clone = new HttpRequestMessage(req.Method, req.RequestUri);

        var ms = new MemoryStream();
        if (req.Content != null)
        {
            req.Content.CopyToAsync(ms).ConfigureAwait(false).GetAwaiter().GetResult();
            ms.Position = 0;
            clone.Content = new StreamContent(ms);
            foreach (var h in req.Content.Headers)
                clone.Content.Headers.Add(h.Key, h.Value);
        }

        clone.Version = req.Version;
        foreach (KeyValuePair<string, object?> option in req.Options)
            clone.Options.Set(new HttpRequestOptionsKey<object?>(option.Key), option.Value);

        foreach (KeyValuePair<string, IEnumerable<string>> header in req.Headers)
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

        return clone;
    }

    private void manage_connections(string platform) {
        lock (_padlock) {
            if (!this.valid_connections.ContainsKey(platform)) {
                this.valid_connections.Add(platform, new List<connection>(this.connections));
            }

            if (this.reset < DateTime.Now) {
                List<string> keys = new List<string>();
                foreach (KeyValuePair<string, List<connection>> entry in this.valid_connections) {
                    keys.Add(entry.Key);
                }
                for (int i = 0; i < keys.Count; i++) {
                    this.valid_connections[keys[i]] = new List<connection>(this.connections); 
                }
                this.reset = DateTime.Now.AddHours(1);
            }
        }
    }

    public JsonElement performRequest(HttpRequestMessage msg, string platform)
    {
        this.manage_connections(platform);

        while (this.valid_connections[platform].Count > 0) {
            HttpRequestMessage clonesMsg = CloneHttpRequestMessage(msg);
            List<connection> possible_connections = this.valid_connections[platform];
            connection conn = possible_connections[new Random().Next(0,possible_connections.Count-1)];
            
            try {
                HttpClient client = conn.produceClient();
                HttpResponseMessage response = client.SendAsync(clonesMsg).GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();
                string responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                return JsonDocument.Parse(responseBody).RootElement;
            } catch (HttpRequestException e) {
                lock (_padlock) {
                    this.valid_connections[platform].Remove(conn);
                }
                Console.WriteLine($"{platform} blacklisted from {conn.getLoc()}.");
            }
        }

        throw new Exception("No suitable connections");
    }

    public WebProxy getProxy(string platform)
    {
        this.manage_connections(platform);
        connection conn = this.valid_connections[platform][new Random().Next(0,this.valid_connections[platform].Count-1)];
        return conn.produceProxy();
    }
}