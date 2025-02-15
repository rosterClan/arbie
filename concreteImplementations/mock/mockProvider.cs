



class mockProvider : eventProvider
{
    private DateTime start_time = new DateTime().AddSeconds(25);
    public override List<eventDTO> execute()
    {
        List<eventDTO> temp = new List<eventDTO>();
        for (int idx = 0; idx < 10; idx++) {
            temp.Add(new eventDTO("mock", "Randwick", idx, this.start_time, new System.Text.Json.JsonElement()));
        }
        return temp;
    }
}