

class mockDatabase : abstractDatabase
{
    public override bool execute(eventDTO eventData, entrantDTO entrant)
    {
        Console.WriteLine($"platformName: {eventData.getPlatformName()}, Horse: {entrant.getName()}, Odds: {entrant.getOdds()}, record_time: {entrant.getRecordTime()}");
        return true;
    }
}