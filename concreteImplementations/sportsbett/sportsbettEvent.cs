

using System.Text.Json.Nodes;
using System.Text.Json;
using System.Net.Http;

using System.Net.Http.Headers;
using System.Collections;
using System.ComponentModel.DataAnnotations;

class sportsbettEvent : Event
{
    private string poll_code = "!!!!!!!!!!";
    private Dictionary<string, entrantDTO> entrant_map = new Dictionary<string, entrantDTO>();
    public sportsbettEvent(eventDTO eventData, object platform_lock) : base(eventData, platform_lock) {}
    private async Task<JsonDocument> getInitialRaceCardAsync(string http_link) {
        HttpClient client = new HttpClient();
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"https://www.sportsbet.com.au/apigw/sportsbook-racing/{http_link}");

        request.Headers.Add("accept", "application/json");
        request.Headers.Add("accept-language", "en-US,en;q=0.9");
        request.Headers.Add("apptoken", "cxp-desktop-web");
        request.Headers.Add("channel", "cxp");
        request.Headers.Add("cookie", "segment-80-20=B; segment-50-50=B; _fbp=fb.2.1730452522916.981875860927998873; _tt_enable_cookie=1; _ga=GA1.1.1567653050.1730452523; _scid=qJOmyI6Z8RE0NnQBuP8errGoYkJy2U1E; _gcl_au=1.1.1055001613.1730452526; _cc=AbZdq18aPxuGv1FVEQD2qSr7; _cid_cc=AbZdq18aPxuGv1FVEQD2qSr7; tl_clid=1; tl_cmpid=351; tl_evtid=8683246; AKA_A2=A; sb_partner_mid=QnbbQus4dcUWqcfzuvZcQGNd7ZgqdRLk12498; isLoggedIn=false; _scid_r=thOmyI6Z8RE0NnQBuP8errGoYkJy2U1Ec-cr4g; _ttp=sQjB3sHXxtqW20W7e7HLEn62HOL.tt.2; _ScCbts=%5B%5D; _sctr=1%7C1732971600000; KP_UIDz-ssn=02ma5WjUAEmFnXwqabN0iEy4gg4TGlTpYjTIckvgD8JhR7L6Gek2STFgnMVGcGaIz9nli7f263ssy6ZjRqkBeCzdMxb6USAXepu8fY7ZcendXFQ9M7dp66ZHdNZXZrjRttIIqbSmH8PwejiYihrgQDH2evsLmHB8caQvIW; KP_UIDz=02ma5WjUAEmFnXwqabN0iEy4gg4TGlTpYjTIckvgD8JhR7L6Gek2STFgnMVGcGaIz9nli7f263ssy6ZjRqkBeCzdMxb6USAXepu8fY7ZcendXFQ9M7dp66ZHdNZXZrjRttIIqbSmH8PwejiYihrgQDH2evsLmHB8caQvIW; _ga_RKBY49TXN5=GS1.1.1733012878.8.1.1733012896.42.0.0; utag_main=v_id:0192e701fc25004fbe7561784d680507d011107500979$_sn:8$_ss:0$_st:1733014696277$dc_visit:8$ses_id:1733012869371%3Bexp-session$_pn:2%3Bexp-session$_prevpage:%2Ftoday%3Bexp-1733016496280$dc_event:3%3Bexp-session$dc_region:ap-southeast-2%3Bexp-session$dleUpToDate:true%3Bexp-session; _uetsid=1d741c30af7b11efb7979d3ced455354; _uetvid=d2fb1450983111efa8f1513a71d60128; _uetmsclkid=_uetde75ae184597150d5b6ac93cd9bc909f; breakpoint=narrow; RT=\"z=1&dm=www.sportsbet.com.au&si=f9a6b3a9-805d-42ba-81d5-111ae108bf79&ss=m44v4bam&sl=0&tt=0&bcn=%2F%2F684d0d45.akstat.io%2F&nu=3stmad3r&cl=1qte6\"");
        request.Headers.Add("priority", "u=1, i");
        request.Headers.Add("referer", "https://www.sportsbet.com.au/horse-racing/australia-nz/warrnambool/race-1-8746027");
        request.Headers.Add("sec-ch-ua", "\"Microsoft Edge\";v=\"131\", \"Chromium\";v=\"131\", \"Not_A Brand\";v=\"24\"");
        request.Headers.Add("sec-ch-ua-mobile", "?0");
        request.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
        request.Headers.Add("sec-fetch-dest", "empty");
        request.Headers.Add("sec-fetch-mode", "cors");
        request.Headers.Add("sec-fetch-site", "same-origin");
        request.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36 Edg/131.0.0.0");
        request.Headers.Add("x-request-id", "eacbbd9c479643bd9238a2ae7ff400fb");

        request.Content = new StringContent("");
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();

        JsonDocument document = JsonDocument.Parse(responseBody);
        
        return document;
    }

    private async Task<String> performLongPoll(string data) {
        HttpClient client = new HttpClient();
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://www.sportsbet.com.au/push");

        request.Headers.Add("accept", "*/*");
        request.Headers.Add("accept-language", "en-AU,en-US;q=0.9,en-GB;q=0.8,en;q=0.7");
        request.Headers.Add("cache-control", "no-cache");
        request.Headers.Add("dnt", "1");
        request.Headers.Add("origin", "https://www.sportsbet.com.au");
        request.Headers.Add("pragma", "no-cache");
        request.Headers.Add("priority", "u=1, i");
        request.Headers.Add("sec-ch-ua", "\"Google Chrome\";v=\"131\", \"Chromium\";v=\"131\", \"Not_A Brand\";v=\"24\"");
        request.Headers.Add("sec-ch-ua-mobile", "?0");
        request.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
        request.Headers.Add("sec-fetch-dest", "empty");
        request.Headers.Add("sec-fetch-mode", "cors");
        request.Headers.Add("sec-fetch-site", "same-origin");
        request.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36");

        request.Content = new StringContent(data);
        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("text/plain;charset=UTF-8");

        HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();

        return responseBody;
    }

    private List<entrantDTO> parseInitialRaceCard(JsonDocument initialCommunication) {
        List<entrantDTO> entrant_objs = new List<entrantDTO>();

        JsonElement root = initialCommunication.RootElement;
        JsonElement markets = root.GetProperty("markets");

        JsonElement proper_market = markets[0];
        for (int idx = 0; idx < markets.GetArrayLength(); idx++) {
            JsonElement item = markets[idx];
            if (item.GetProperty("name").ToString() == "Win or Place") {
                proper_market = item;
                break;
            }
        }

        JsonElement entrants = proper_market.GetProperty("selections");
        for (int idx = 0; idx < entrants.GetArrayLength(); idx++) {
            JsonElement entrant_json = entrants[idx];
            
            string name = entrant_json.GetProperty("name").ToString();
            bool scratched = !(entrant_json.GetProperty("statusCode").ToString().Equals("A"));

            JsonElement prices = entrant_json.GetProperty("prices");
            JsonElement win_price = prices[0];
            for (int priceIdx = 0; priceIdx < prices.GetArrayLength(); priceIdx++) {
                JsonElement price = prices[priceIdx];
                if (price.GetProperty("priceCode").ToString() == "L") {
                    win_price = price;
                    break;
                }
            }

            double odds = -1;
            if (win_price.TryGetProperty("winPrice", out JsonElement winPriceElement)) {
                odds = winPriceElement.GetDouble();
            }

            entrantDTO new_entrant = new entrantDTO(name, odds, scratched);
            entrant_map.Add(entrant_json.GetProperty("id").ToString(), new_entrant);
            entrant_objs.Add(new_entrant);
        }

        return entrant_objs;
    }

    protected override void protectedExecution()
    {   
        string http_link = this.eventData.getPlatformSpecificDetails().GetProperty("httpLink").ToString();
        List<entrantDTO> initial_entrants = this.parseInitialRaceCard(this.getInitialRaceCardAsync(http_link).GetAwaiter().GetResult());
        
        for (int idx = 0; idx < initial_entrants.Count; idx++) {
             this.db.execute(this.eventData, initial_entrants[idx]);
        }

        string platform_race_identifyer = this.eventData.getPlatformSpecificDetails().GetProperty("id").ToString();
        string long_poll_id = platform_race_identifyer.PadLeft(10, '0');
        string poll_prefix = $"CL0000S0002SEVENT{long_poll_id}sEVENT{long_poll_id}";

        string return_msg = this.performLongPoll($"{poll_prefix}{poll_code}").GetAwaiter().GetResult();

        while (this.status == 0) {
            if (return_msg.Length > 0) {
                List<List<String>> messages = parseMessage(return_msg);
                updateOdds(messages);
            }
            try {
                return_msg = this.performLongPoll($"{poll_prefix}{poll_code}").GetAwaiter().GetResult();
            } catch (Exception e) {
                long_poll_id = platform_race_identifyer.PadLeft(10, '0');
            }
            this.checkShutdown();
        }

        lock (this.platform_lock) {
            Monitor.Pulse(this.platform_lock);
        }
    }

    private void updateOdds(List<List<String>> messages) {
        List<List<object>> newEntrants = new List<List<object>>();

        for (int i = 0; i < messages.Count; i++) {
            List<String> message = messages[i];
            foreach (var entry in entrant_map) {
                if (message[0].Contains(entry.Key)) {
                    try {
                        JsonDocument odds_document = JsonDocument.Parse(message[1]);
                        JsonElement odds_json = odds_document.RootElement;

                        string horse_name = entry.Value.getName();

                        string lp_num_str = odds_json.GetProperty("lp_num").ToString();
                        string lp_den_str = odds_json.GetProperty("lp_den").ToString();

                        if (lp_den_str.Equals("") || lp_num_str.Equals("")) {
                            continue;
                        }

                        double lp_num = Convert.ToDouble(lp_num_str);
                        double lp_den = Convert.ToDouble(lp_den_str);

                        double odds_num = 1+(lp_num/lp_den);
                        bool status = odds_json.GetProperty("status").ToString() != "A";

                        entrantDTO new_entrant = new entrantDTO(horse_name, odds_num, status);
                        newEntrants.Add(new List<object> {entry.Key, new_entrant});   
                    } catch (Exception e) {
                        Console.WriteLine(e);
                        continue;
                    }
                }
            }
        }

        for (int i = 0; i < newEntrants.Count; i++) {
            this.db.execute(this.eventData, (entrantDTO)newEntrants[i][1]);
            this.entrant_map.Remove((string)newEntrants[i][0]);
            this.entrant_map.Add((string)newEntrants[i][0],(entrantDTO)newEntrants[i][1]);
        }
    }

    private List<List<String>> parseMessage(string msg) {
        List<List<String>> messages = new List<List<String>>();

        int index = 0; 
        while (true) {
            List<object> return_state = generateRealResponce(msg, index);

            int status = (int)return_state[0];
            if (status <= 0){
                return messages;
            }
            
            index = (int)return_state[1];
            String potential_new_token = (String)return_state[3];
            messages.Add(new List<string>{(String)return_state[4],(String)return_state[2]});

            //Console.WriteLine("New: " + potential_new_token);
            if (string.Compare(potential_new_token, poll_code, StringComparison.Ordinal) > 0) {
                poll_code = potential_new_token;
                //Console.WriteLine("Set: " + poll_code);
            }
        }
    }

    private int parseHexDigits(string msg, int start) {
        int length = 6; 
        if (start+length > msg.Length) {
            throw new ArgumentException("Input is too short"); 
        }
        string segment = msg.Substring(start, length);
        if (!System.Text.RegularExpressions.Regex.IsMatch(segment, @"^[0-9a-fA-F]+$")){
            throw new ArgumentException("Not a valid hexadecimal number");
        }
        if (!int.TryParse(segment, System.Globalization.NumberStyles.HexNumber, null, out int number)) {
            throw new ArgumentException("Invalid hexadecimal number");
        }
        return number;
    }

    private String parseMsgId(string str, int start) {
        if (start + 10 > str.Length) {
            throw new ArgumentException("Invalid");
        }
        string segment = str.Substring(start, 10);
        if (!System.Text.RegularExpressions.Regex.IsMatch(segment, @"^[!-u]+$")){
            throw new ArgumentException("Not a valid hexadecimal number");
        }

        return segment;
    }

    private List<object> generateRealResponce(string e, int t) {
        int r = e.Length;
        if (r-t < 56) {
            return new List<object> {0, null, null};
        }
        char a = e[t];
        t += 1;
        if (a != 'M') {
            return new List<object> {0, null, null};
        }

        int s = 1; 
        int l = t + 16 + 10 + s + 16 + 6;
        if (l >= r) {
            return new List<object> {0, null, null};
        }

        int hex_number;
        try{
            hex_number = parseHexDigits(e, l);
        } catch (Exception gg) {
            return new List<object> {0, null, null};
        }
        int u = hex_number;
        int d = l+6;

        if (r-d < u) {
            return new List<object> {0, null, null};
        }
        t += 16;

        String message_id = "";
        try {
            message_id = parseMsgId(e, t);
        } catch (Exception gg) {
            return new List<object> {0, null, null};
        }

        string id = e.Substring(d-29, 29);
        string f = e.Substring(d, u);

        return new List<object> {1, d+u, f, message_id, id};
    }
}