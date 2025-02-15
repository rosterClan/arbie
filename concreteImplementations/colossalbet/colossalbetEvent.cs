
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Net.Http;

using System.Net.Http.Headers;
using System.Collections;
using System.ComponentModel.DataAnnotations;

using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


class colossalbetEvent : Event
{
    private const string ClientKey = "colossalbet";
    private const string BaseUrl = "https://apicob.generationweb.com.au/GWBetService";
    private const string SecretKey = "xnoh8dbr2pelqeuflv";
    public colossalbetEvent(eventDTO eventData, object platform_lock) : base(eventData, platform_lock) {}

    protected override void protectedExecution()
    {
        JsonElement additional_details = this.eventData.getPlatformSpecificDetails();
        string raceId = additional_details.GetProperty("raceId").ToString();
        while (this.status == 0)
        {
            string timestamp = ((DateTimeOffset.UtcNow.AddMilliseconds(1100)).ToUnixTimeSeconds()).ToString();

            JsonElement data = GetData(timestamp, raceId).GetAwaiter().GetResult();
            List<entrantDTO> entrants = processResponce(data);

            foreach (entrantDTO entrant in entrants) {
                this.db.execute(this.eventData, entrant);
            }

            Thread.Sleep(5000);
            this.checkShutdown();
        }
    }

    private List<entrantDTO> processResponce(JsonElement json) {
        JsonElement json_entrants = json.GetProperty("rnnr");
        List<entrantDTO> entrants = new List<entrantDTO>();
        for (int i = 0; i < json_entrants.GetArrayLength(); i++) {
            JsonElement entrant_json = json_entrants[i];
            string entrant_name = entrant_json.GetProperty("rn").ToString();
            string odds_str = "";

            bool is_scratched = false; 
            if (entrant_json.TryGetProperty("re", out JsonElement nameElement)) {
                is_scratched = true; 
                odds_str = "-1";
            } else {
                is_scratched = false; 
                odds_str = entrant_json.GetProperty("FWIN").ToString();
            }

            double win_odds = double.Parse(odds_str);
            
            entrantDTO n_entrant = new entrantDTO(entrant_name, win_odds, is_scratched);
            entrants.Add(n_entrant);
        }
        return entrants;
    }

    private string GenerateAuth(string timestamp)
    {
        string key = $"{SecretKey}{timestamp}";
        string resource = BaseUrl;

        using (var hmac = new HMACSHA1(Encoding.UTF8.GetBytes(key)))
        {
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(resource));
            string signature = BitConverter.ToString(hash).Replace("-", "").ToLower();

            string auth = "clientKey="+ClientKey+"&timestamp="+timestamp+"&signature="+signature;

            return auth;
        }
    }

    private async Task<JsonElement> GetData(string timestamp, string raceId)
    {
        string authHeader = GenerateAuth(timestamp);

        HttpClient client = new HttpClient();

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"https://apicob.generationweb.com.au/GWBetService/r/b/GetEventRace/{raceId}/RunnerNum?rand={timestamp}");

        request.Headers.Add("accept", "application/json, text/plain, */*");
        request.Headers.Add("accept-language", "en-US,en;q=0.9");
        request.Headers.TryAddWithoutValidation("authorization", authHeader);
        request.Headers.Add("origin", "https://www.colossalbet.com.au");
        request.Headers.Add("priority", "u=1, i");
        request.Headers.Add("referer", "https://www.colossalbet.com.au/");
        request.Headers.Add("sec-ch-ua", "\"Microsoft Edge\";v=\"131\", \"Chromium\";v=\"131\", \"Not_A Brand\";v=\"24\"");
        request.Headers.Add("sec-ch-ua-mobile", "?0");
        request.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
        request.Headers.Add("sec-fetch-dest", "empty");
        request.Headers.Add("sec-fetch-mode", "cors");
        request.Headers.Add("sec-fetch-site", "cross-site");
        request.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36 Edg/131.0.0.0");

        return this.requester.performRequest(request, ClientKey);
    }
}