using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;

class sportsbettProvider : eventProvider
{
    private string name = "sportsbett";
    private DateTime start_time = new DateTime().AddSeconds(25);
    public override List<eventDTO> execute()
    {
        List<eventDTO> eventDetails = new List<eventDTO>();

        JsonElement root = this.getSportsBetEventsAsync().GetAwaiter().GetResult();
        JsonElement dates = root.GetProperty("dates")[0];
        JsonElement sections = dates.GetProperty("sections");

        JsonElement horseItem = sections[0];
        for (int idx = 0; idx < sections.GetArrayLength(); idx++) {
            JsonElement item = sections[idx];
            if (item.GetProperty("displayName").ToString() == "Horses") {
                horseItem = item;
                break;
            }
        }

        JsonElement meetings = horseItem.GetProperty("meetings");
        for (int meetIdx = 0; meetIdx < meetings.GetArrayLength(); meetIdx++) {
            JsonElement meet = meetings[meetIdx];
            if (meet.GetProperty("isInternational").GetBoolean()) {
                continue;
            }

            string venue = meet.GetProperty("name").ToString();
            JsonElement events = meet.GetProperty("events");
            for (int eventIdx = 0; eventIdx < events.GetArrayLength(); eventIdx++) {
                int ID = (int)events[eventIdx].GetProperty("id").GetInt64();
                int race_number = (int)events[eventIdx].GetProperty("raceNumber").GetInt64();
                string http_link = events[eventIdx].GetProperty("httpLink").ToString();
                
                int time_stamp = (int)events[eventIdx].GetProperty("startTime").GetInt64();
                DateTime start_time = DateTimeOffset.FromUnixTimeSeconds(time_stamp).UtcDateTime; 

                eventDetails.Add(new eventDTO("sportsbett", venue, race_number, start_time, "Australia/Sydney", events[eventIdx]));
            }
        }

        return eventDetails;
    }

    private async Task<JsonElement> getSportsBetEventsAsync() {
        HttpClient client = new HttpClient();
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://www.sportsbet.com.au/apigw/sportsbook-racing/Sportsbook/Racing/AllRacing/"+DateTime.Now.ToString("yyyy-MM-dd"));

        request.Headers.Add("accept", "application/json");
        request.Headers.Add("accept-language", "en-US,en;q=0.9");
        request.Headers.Add("apptoken", "cxp-desktop-web");
        request.Headers.Add("channel", "cxp");
        request.Headers.Add("priority", "u=1, i");
        request.Headers.Add("referer", "https://www.sportsbet.com.au/racing-schedule");
        request.Headers.Add("sec-ch-ua", "\"Microsoft Edge\";v=\"131\", \"Chromium\";v=\"131\", \"Not_A Brand\";v=\"24\"");
        request.Headers.Add("sec-ch-ua-mobile", "?0");
        request.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
        request.Headers.Add("sec-fetch-dest", "empty");
        request.Headers.Add("sec-fetch-mode", "cors");
        request.Headers.Add("sec-fetch-site", "same-origin");
        request.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36 Edg/131.0.0.0");

        request.Content = new StringContent("");
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        return this.requestor.performRequest(request, name);
    }
}