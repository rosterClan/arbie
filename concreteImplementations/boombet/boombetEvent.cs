
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Net.Http;

using System.Net.Http.Headers;
using System.Collections;
using System.ComponentModel.DataAnnotations;

using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


class boombetEvent : Event
{
    private const string platform = "boombet";
    private string race_id = "";

    public boombetEvent(eventDTO eventData, object platform_lock) : base(eventData, platform_lock) {
        this.race_id = eventData.getPlatformSpecificDetails().GetProperty("eventId").ToString();
    }

    protected override void protectedExecution()
    {
        while (this.status == 0) {
            JsonElement data = getUpdatedOdds().GetAwaiter().GetResult();
            List<entrantDTO> entrants = this.processData(data);

            for (int i = 0; i < entrants.Count; i++) {
                this.db.execute(this.eventData, entrants[i]);
            }

            this.checkShutdown();
            Thread.Sleep(10000);
        }
    }

    private List<entrantDTO> processData(JsonElement data) {
        List<entrantDTO> entrants = new List<entrantDTO>();

        JsonElement runners = data.GetProperty("runners");
        for (int i = 0; i < runners.GetArrayLength(); i++) {
            JsonElement odds = runners[i].GetProperty("odds");
            string horse_name = runners[i].GetProperty("name").ToString();

            if (horse_name == "Justhandy") {
                Console.WriteLine("Break");
            }

            string scratched_str = runners[i].GetProperty("scratchedDateTime").ToString();
            double win_odds = -1;
            bool scratched = !scratched_str.Equals("");

            for (int y = 0; y < odds.GetArrayLength(); y++) {
                JsonElement product = odds[y].GetProperty("product");
                string bet_type = product.GetProperty("betType").ToString();
                

                if (bet_type.Equals("Win")) {
                    string win_odds_str = odds[y].GetProperty("value").ToString();
                    win_odds = double.Parse(win_odds_str);
                    break;
                }
            }

            entrants.Add(new entrantDTO(horse_name, win_odds, scratched));
        }

        return entrants;
    }

    private async Task<JsonElement> getUpdatedOdds() {
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://sb-saturn.azurefd.net/api/v3/race/event/"+this.race_id+"?checkHotBet=false&includeForm=true");

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

        JsonElement data = this.requester.performRequest(request, platform);
        return data; 
    }
}