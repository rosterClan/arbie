using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Globalization;
using System.Net.Http;

class pointsbetProvider : eventProvider
{
    private string name = "pointsbet";
    public override List<eventDTO> execute()
    {
        List<eventDTO> entities = new List<eventDTO>();

        JsonElement data = getRaces().GetAwaiter().GetResult()[0];
        JsonElement meetings = data.GetProperty("meetings");

        for (int i = 0; i < meetings.GetArrayLength(); i++) {
            JsonElement meet = meetings[i];
            string country = meet.GetProperty("countryCode").ToString();
            string venue = meet.GetProperty("venue").ToString();

            if (country.Equals("AUS")) {
                string start_time_str = meet.GetProperty("meetingStartDateTimeUtc").ToString();
                string race_type_str = meet.GetProperty("racingType").ToString();
                DateTime start_time = DateTime.Parse(start_time_str);

                if (!race_type_str.Equals("1")) {
                    continue;
                }

                JsonElement races = meet.GetProperty("races");
                for (int y = 0; y < races.GetArrayLength(); y++) {
                    JsonElement race = races[y];

                    string round_str = race.GetProperty("raceNumber").ToString();
                    int round = int.Parse(round_str);

                    entities.Add(new eventDTO("pointsbet", venue, round, start_time, race));
                }

            }
        }

        return entities;
    }

    private async Task<JsonElement> getRaces() {
        DateTime cur = DateTime.Now;
        DateTime end = cur.AddDays(1);

        string cur_str = cur.ToString("yyyy-MM-dd");
        string end_str = end.ToString("yyyy-MM-dd");

        HttpClient client = new HttpClient();

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://api.au.pointsbet.com/api/racing/v4/meetings?startDate="+cur_str+"T00:00:00.000Z&endDate="+end_str+"T00:00:00.000Z");

        request.Headers.Add("accept", "application/json, text/plain, */*");
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

        return this.requestor.performRequest(request, name);
    }
}