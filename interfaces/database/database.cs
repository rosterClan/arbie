
class database : abstractDatabase
{
    private Dictionary<string, Dictionary<string,entrantDTO>> entrant_map = new Dictionary<string, Dictionary<string,entrantDTO>>();
    private abstractDatabase _innerDatabase;
    private static database _instance;
    private static readonly object _padlock = new object();

    private DateTime reset_cache;
    private DateTime current; 

    private database() { 
        reset_cache = new DateTime().AddDays(1);
    }

    public static database Instance
    {
        get
        {
            lock (_padlock)
            {
                if (_instance == null)
                {
                    _instance = new database();
                }
                return _instance;
            }
        }
    }

    public void SetInnerDatabase(abstractDatabase db)
    {
        if (db == null)
        {
            throw new ArgumentNullException(nameof(db), "Inner database cannot be null.");
        }
        lock (_padlock)
        {
            _innerDatabase = db;
        }
    }

    public override bool execute(eventDTO eventData, entrantDTO entrant)
    {
        if (_innerDatabase != null)
        {
            current = new DateTime();

            lock (_padlock)
            {
                if ((reset_cache - current).TotalDays > 1) {
                    entrant_map = new Dictionary<string, Dictionary<string,entrantDTO>>();
                    reset_cache = new DateTime().AddDays(1);
                }

                if (!entrant_map.ContainsKey(eventData.getHashId())) {
                    Dictionary<string, entrantDTO> race_cache = new Dictionary<string,entrantDTO>();
                    entrant_map.Add(eventData.getHashId(), race_cache);
                }

                if (!entrant_map[eventData.getHashId()].ContainsKey(entrant.getName())) {
                    entrant_map[eventData.getHashId()].Add(entrant.getName(), entrant);
                    return _innerDatabase.execute(eventData, entrant);
                } else if (entrant_map[eventData.getHashId()][entrant.getName()].getOdds() != entrant.getOdds()) {
                    entrant_map[eventData.getHashId()].Remove(entrant.getName());
                    entrant_map[eventData.getHashId()].Add(entrant.getName(), entrant);
                    return _innerDatabase.execute(eventData, entrant);
                }

                return false; 
            }
        }
        return false;
    }
}
