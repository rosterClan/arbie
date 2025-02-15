using System.Text.RegularExpressions;

class entrantDTO {
    private string name;
    private double odds; 
    private bool scratched;
    private DateTime record_time; 

    public entrantDTO(string name, double odds, bool scratched) {
        this.record_time = DateTime.UtcNow;
        this.name = RemoveSpecialCharacters(name);
        this.odds = odds; 
        this.scratched = scratched;
    }
    private string RemoveSpecialCharacters(string input)
    {
        return Regex.Replace(input.ToLower(), "[^a-zA-Z0-9 ]", "");
    }
    
    public string getName() {
        return this.name; 
    }

    public double getOdds() {
        return this.odds; 
    }

    public DateTime getRecordTime() {
        return this.record_time;
    }

    public bool getScratched() {
        return this.scratched;
    }
    
}