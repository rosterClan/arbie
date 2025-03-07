using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Globalization;

class boombetProvider : eventProvider
{
    private string name = "boombet";
    public override List<eventDTO> execute()
    {
        List<eventDTO> events = new List<eventDTO>();
        JsonElement meets = this.getBoombetAsync().GetAwaiter().GetResult().GetProperty("races");

        for (int i = 0; i < meets.GetArrayLength(); i++) {
            JsonElement venue = meets[i];
            
            string meeting_name = venue.GetProperty("meetingName").ToString();
            int race_type = int.Parse(venue.GetProperty("raceType").ToString());

            if (!meeting_name.Contains("-") && race_type == 4) {
                string round_str = venue.GetProperty("raceNumber").ToString();
                string start_time_int_str = venue.GetProperty("closesInSec").ToString();
                
                DateTime current_utc = DateTime.UtcNow;
                DateTime jump_time = current_utc.AddSeconds(int.Parse(start_time_int_str));
                int round = int.Parse(round_str);

                events.Add(new eventDTO(name, meeting_name, round, jump_time, "Australia/Sydney", venue));
            }
        }

        return events;
    }

    private async Task<JsonElement> getBoombetAsync() {
        HttpClient client = new HttpClient();

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://sb-saturn.azurefd.net/api/v3/race/getnexttojump?maxCount=1000");

        request.Headers.Add("accept", "application/json");
        request.Headers.Add("accept-language", "en-US,en;q=0.9");
        request.Headers.Add("authorization", "Bearer");
        request.Headers.Add("origin", "https://www.boombet.com.au");
        request.Headers.Add("priority", "u=1, i");
        request.Headers.Add("referer", "https://www.boombet.com.au/");
        request.Headers.Add("sec-ch-ua", "\"Microsoft Edge\";v=\"131\", \"Chromium\";v=\"131\", \"Not_A Brand\";v=\"24\"");
        request.Headers.Add("sec-ch-ua-mobile", "?0");
        request.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
        request.Headers.Add("sec-fetch-dest", "empty");
        request.Headers.Add("sec-fetch-mode", "cors");
        request.Headers.Add("sec-fetch-site", "cross-site");
        request.Headers.Add("sp-deviceid", "dev");
        request.Headers.Add("sp-platformid", "2");
        request.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36 Edg/131.0.0.0");

        JsonElement data = this.requestor.performRequest(request, name);
        return data; 
    }
}