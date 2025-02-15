using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Globalization;

class colossalbetProvider : eventProvider
{
    public string name = "colossalbet";
    public override List<eventDTO> execute()
    {
        List<eventDTO> eventDetails = new List<eventDTO>();
        JsonElement root =  this.getColossalbetAsync().GetAwaiter().GetResult();

        JsonElement meets = root.GetProperty("meeting");
        for (int i = 0; i < meets.GetArrayLength(); i++) {
            JsonElement meet = meets[i];
            JsonElement races = meet.GetProperty("races");
            string loc = meet.GetProperty("country").ToString();
            string sport_code = meet.GetProperty("sportCode").ToString();

            if (!loc.Equals("AU") || !sport_code.Equals("HORS")) {
                continue;
            }

            string venue_str = meet.GetProperty("meetingName").ToString();
            for (int y = 0; y < races.GetArrayLength(); y++) {
                JsonElement race = races[y];
                string round_str = race.GetProperty("raceNumber").ToString();
                string start_time_str = race.GetProperty("startTime").ToString();

                DateTime start_time = DateTime.ParseExact(start_time_str, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
                int round = int.Parse(round_str);

                eventDTO new_event = new eventDTO("colossalbet", venue_str, round, start_time, race);
                eventDetails.Add(new_event);
            }
        }
        
        return eventDetails;
    }

    private async Task<JsonElement> getColossalbetAsync() {
        HttpClient client = new HttpClient();

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://api.racebookhq.com/api/v1/genweb/events/short/"+DateTime.Now.ToString("yyyy-MM-dd"));

        request.Headers.Add("accept", "application/json, text/plain, */*");
        request.Headers.Add("accept-language", "en-US,en;q=0.9");
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

        return this.requestor.performRequest(request, name);
    }
}