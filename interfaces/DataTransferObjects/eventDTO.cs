

using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Text.Json;

class eventDTO { 

    private DateTime start_time; 
    private string venue; 
    private int round; 
    private string platform_name; 
    private string hash_id; 
    private JsonElement jsonObj;

    public eventDTO(string platform, string venue, int round, DateTime start_time, string time_zone, JsonElement jsonObj) {
        this.platform_name = RemoveSpecialCharacters(platform);
        this.venue = RemoveSpecialCharacters(venue);
        this.round = round;

        this.start_time = start_time;
        //this.start_time

        this.jsonObj = jsonObj;
        string str_datetime = this.start_time.ToString("d");
        this.hash_id = this.computeHashString($"{this.platform_name}{this.venue}{this.round}{str_datetime}");
    }

    private string RemoveSpecialCharacters(string input)
    {
        return Regex.Replace(input.ToLower(), "[^a-zA-Z0-9]", "");
    }

    private string computeHashString(string inputString) {
        StringBuilder sb = new StringBuilder();
        foreach (byte b in this.computeHash(inputString)) {
            sb.Append(b.ToString("X2"));
        }
        return sb.ToString();
    }

    private byte[] computeHash(string entry) {
        using (HashAlgorithm algorithm = SHA256.Create())
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(entry));
    }

    public string getHashId() {
        return this.hash_id;
    }

    public string getPlatformName() {
        return this.platform_name;
    }

    public string getVenueName() {
        return this.venue;
    }

    public int getRound() {
        return this.round;
    }

    public DateTime getStartTime() {
        return this.start_time;
    }

    public JsonElement getPlatformSpecificDetails() {
        return this.jsonObj;
    }

}