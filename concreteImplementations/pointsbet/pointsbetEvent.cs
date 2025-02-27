
using System.Text.Json;

using System.Net.Http.Headers;
using System.Text;
using System.Net.WebSockets;
using System.Reflection.Metadata;

class pointsbetEvent : Event
{
    private string name = "pointsbet";
    public pointsbetEvent(eventDTO eventData, object platform_lock) : base(eventData, platform_lock) {}

    protected override void protectedExecution()
    {
        this.getProcessInitalOdds();
        this.protectedExecutionAsync().GetAwaiter().GetResult();
    }

    private void getProcessInitalOdds() {
        Dictionary<string, JsonElement> cache = new Dictionary<string, JsonElement>();
        string race_id = this.eventData.getPlatformSpecificDetails().GetProperty("raceId").ToString();

        JsonElement data = pullInitialOdds(race_id).GetAwaiter().GetResult();
        
        JsonElement runners = data.GetProperty("runners");
        for (int i = 0; i < runners.GetArrayLength(); i++) {
            JsonElement entrant_json = runners[i];
            string runnerId = entrant_json.GetProperty("runnerId").ToString();
            cache.Add(runnerId, entrant_json);
        }

        JsonElement markets = data.GetProperty("markets");
        for (int i = 0; i < markets.GetArrayLength(); i++) {
            if (markets[i].GetProperty("marketType").ToString() == "FixedWin") {
                JsonElement winMarket = markets[i].GetProperty("selections");
                for (int z = 0; z < winMarket.GetArrayLength(); z++) {
                    JsonElement entrant = cache[winMarket[z].GetProperty("runnerId").ToString()];
                    double price = winMarket[z].GetProperty("price").GetDouble();
                    bool scratched = entrant.GetProperty("isScratched").GetBoolean();
                    string name = entrant.GetProperty("runnerName").ToString();

                    entrantDTO n_entrant = new entrantDTO(name, price, scratched);
                    this.db.execute(this.eventData, n_entrant);
                }
            }
        }
    }

    private async Task protectedExecutionAsync() {
        string preAuth = await getPreAuthToken();
        JsonElement socketAuth = await getWebsocketToken(preAuth);

        string connectionToken = socketAuth.GetProperty("connectionToken").ToString();
        string connectionId = socketAuth.GetProperty("connectionId").ToString();
        string raceId = this.eventData.getPlatformSpecificDetails().GetProperty("raceId").ToString();

        using (ClientWebSocket ws = new ClientWebSocket())
        {
            string uriUrl = $"wss://push.au.pointsbet.com/client/?hub=signalrhub&id={connectionToken}&access_token={preAuth}";
            while (true) {
                try {
                    var proxy = this.requester.getProxy(name);
                    ws.Options.Proxy = proxy;
                    Uri uri = new Uri(uriUrl);
                    await ws.ConnectAsync(uri, CancellationToken.None);
                    break;
                } catch (Exception e) {
                    Console.WriteLine(e);
                    continue;
                }
            }

            var initMessage = new ArraySegment<byte>(Encoding.UTF8.GetBytes("{\"protocol\":\"json\",\"version\":1}"));
            await ws.SendAsync(initMessage, WebSocketMessageType.Text, true, CancellationToken.None);
            await setSocketWatch(connectionId, raceId);

            while (this.status == 0){
                var pingMessage = new ArraySegment<byte>(Encoding.UTF8.GetBytes("{\"type\":6}\u001E"));
                await ws.SendAsync(pingMessage, WebSocketMessageType.Text, true, CancellationToken.None);

                var sb = new StringBuilder();
                WebSocketReceiveResult result;
                var buffer = new byte[4096];
                
                do {
                    result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Server closed", CancellationToken.None);
                        break;
                    }
                    
                    sb.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));

                } while (!result.EndOfMessage);

                List<char> completeMessage = sb.ToString().ToList();
                int i = 0;
                for (i = completeMessage.Count-1; i > 0; i--) {
                    if (completeMessage[i].Equals('}')) {
                        break;
                    }
                }
                string messageFiltered = "";
                for (int y=0; y <= i; y++) {
                    messageFiltered += completeMessage[y];
                }

                this.parseMessage(messageFiltered);
                this.checkShutdown();
            }
        }
    }

    private void parseMessage(string data) {
        if (data == "{}") return;
        
        JsonElement ele = JsonDocument.Parse(data).RootElement;
        if (ele.GetProperty("type").ToString() == "1") {
            JsonElement arguments = ele.GetProperty("arguments")[0];
            JsonElement payload = arguments.GetProperty("payload");
                
            if (payload.TryGetProperty("fixedOddsMarket", out JsonElement fixedMarket)) {
                JsonElement outcomes = fixedMarket.GetProperty("outcomes");
                for (int i = 0; i < outcomes.GetArrayLength(); i++) {
                    JsonElement item = outcomes[i];
                    
                    string name = item.GetProperty("outcomeName").ToString();
                    double price = -1; 

                    JsonElement prices = item.GetProperty("fixedPrices");
                    for (int y = 0; y < prices.GetArrayLength(); y++) {
                        if (prices[y].GetProperty("marketTypeCode").ToString() == "WIN") {
                            price = prices[y].GetProperty("price").GetDouble();
                            break;
                        }
                    }

                    bool is_scratched = true;
                    if (item.ToString().Contains("scratched")) {
                        is_scratched = item.GetProperty("scratched").GetBoolean();
                    }
                    
                    entrantDTO entrant = new entrantDTO(name, price, is_scratched);
                    this.db.execute(this.eventData, entrant);
                }
            }
        }
    }

    private async Task<JsonElement> pullInitialOdds(string race_id) {
        HttpClient client = new HttpClient();
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://api.au.pointsbet.com/api/racing/v3/races/"+race_id);

        request.Headers.Add("accept", "application/json, text/plain, */*");
        request.Headers.Add("accept-language", "en-AU,en-US;q=0.9,en-GB;q=0.8,en;q=0.7");
        request.Headers.Add("cache-control", "no-cache");
        request.Headers.Add("dnt", "1");
        request.Headers.Add("origin", "https://pointsbet.com.au");
        request.Headers.Add("pragma", "no-cache");
        request.Headers.Add("priority", "u=1, i");
        request.Headers.Add("referer", "https://pointsbet.com.au/");
        request.Headers.Add("sec-ch-ua", "\"Google Chrome\";v=\"131\", \"Chromium\";v=\"131\", \"Not_A Brand\";v=\"24\"");
        request.Headers.Add("sec-ch-ua-mobile", "?0");
        request.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
        request.Headers.Add("sec-fetch-dest", "empty");
        request.Headers.Add("sec-fetch-mode", "cors");
        request.Headers.Add("sec-fetch-site", "cross-site");
        request.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36");
        
        return this.requester.performRequest(request, name); 
    }
    
    private async Task setSocketWatch(string socket_tocken, string race_id) {
        HttpClient client = new HttpClient();
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://api.au.pointsbet.com/signalr/"+socket_tocken+"/batch-subscribe");

        request.Headers.Add("accept", "*/*");
        request.Headers.Add("accept-language", "en-US,en;q=0.9");
        request.Headers.Add("origin", "https://pointsbet.com.au");
        request.Headers.Add("priority", "u=1, i");
        request.Headers.Add("referer", "https://pointsbet.com.au/");
        request.Headers.Add("sec-ch-ua", "\"Microsoft Edge\";v=\"131\", \"Chromium\";v=\"131\", \"Not_A Brand\";v=\"24\"");
        request.Headers.Add("sec-ch-ua-mobile", "?0");
        request.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
        request.Headers.Add("sec-fetch-dest", "empty");
        request.Headers.Add("sec-fetch-mode", "cors");
        request.Headers.Add("sec-fetch-site", "cross-site");
        request.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36 Edg/131.0.0.0");

        request.Content = new StringContent("{\"GroupNames\":[\"/racing/fixedoddsmarket/"+race_id+"\",\"/inPlaySportsList\"]}");
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        HttpResponseMessage response = client.SendAsync(request).GetAwaiter().GetResult();
        response.EnsureSuccessStatusCode();
    }

    private async Task<JsonElement> getWebsocketToken(string preToken) {
        HttpClient client = new HttpClient();
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://push.au.pointsbet.com/client/negotiate?hub=signalrhub&negotiateVersion=1");

        request.Headers.Add("accept", "*/*");
        request.Headers.Add("accept-language", "en-US,en;q=0.9");
        request.Headers.Add("authorization", "Bearer " + preToken);
        request.Headers.Add("cache-control", "max-age=0");
        request.Headers.Add("origin", "https://pointsbet.com.au");
        request.Headers.Add("priority", "u=1, i");
        request.Headers.Add("referer", "https://pointsbet.com.au/");
        request.Headers.Add("sec-ch-ua", "\"Microsoft Edge\";v=\"131\", \"Chromium\";v=\"131\", \"Not_A Brand\";v=\"24\"");
        request.Headers.Add("sec-ch-ua-mobile", "?0");
        request.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
        request.Headers.Add("sec-fetch-dest", "empty");
        request.Headers.Add("sec-fetch-mode", "cors");
        request.Headers.Add("sec-fetch-site", "cross-site");
        request.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36 Edg/131.0.0.0");
        request.Headers.Add("x-requested-with", "XMLHttpRequest");
        request.Headers.Add("x-signalr-user-agent", "Microsoft SignalR/8.0 (8.0.7; Unknown OS; Browser; Unknown Runtime Version)");

        HttpResponseMessage response = client.SendAsync(request).GetAwaiter().GetResult();
        response.EnsureSuccessStatusCode();
        string responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

        JsonDocument document = JsonDocument.Parse(responseBody);
        return document.RootElement; 
    }

    private async Task<string> getPreAuthToken() {
        HttpClient client = new HttpClient();
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://api.au.pointsbet.com/signalr/negotiate");

        request.Headers.Add("accept", "*/*");
        request.Headers.Add("accept-language", "en-US,en;q=0.9");
        request.Headers.Add("origin", "https://pointsbet.com.au");
        request.Headers.Add("priority", "u=1, i");
        request.Headers.Add("referer", "https://pointsbet.com.au/");
        request.Headers.Add("sec-ch-ua", "\"Microsoft Edge\";v=\"131\", \"Chromium\";v=\"131\", \"Not_A Brand\";v=\"24\"");
        request.Headers.Add("sec-ch-ua-mobile", "?0");
        request.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
        request.Headers.Add("sec-fetch-dest", "empty");
        request.Headers.Add("sec-fetch-mode", "cors");
        request.Headers.Add("sec-fetch-site", "cross-site");
        request.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36 Edg/131.0.0.0");

        request.Content = new StringContent("{}");
        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("text/plain;charset=UTF-8");

        HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        string responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

        JsonDocument document = JsonDocument.Parse(responseBody);
        return document.RootElement.GetProperty("accessToken").ToString(); 
    }
}