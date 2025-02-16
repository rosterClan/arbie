

using System;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using Npgsql;

class sqlDatabase : abstractDatabase
{
    private string host = "127.0.0.1";
    private string database = "arbie_4.2";
    private string user = "postgres";
    private string password = "test";
    private string connection_string;

    public sqlDatabase() {
        connection_string = $"Host={host};Port=5432;Username={user};Password={password};Database={database}";
    }

    private List<JsonDocument> executeQuery(string query) {
            var results = new List<JsonDocument>();

            using (var connection = new NpgsqlConnection(connection_string)) {
                connection.Open();

                using (var command = new NpgsqlCommand(query, connection)) {
                    try
                    {
                        using (var reader = command.ExecuteReader()) {
                            while (reader.Read())
                            {
                                var row = new Dictionary<string, object>();
                                for (int i = 0; i < reader.FieldCount; i++) {
                                    var columnName = reader.GetName(i);
                                    var columnValue = reader.IsDBNull(i) ? null : reader.GetValue(i);
                                    row[columnName] = columnValue;
                                }

                                string jsonString = JsonSerializer.Serialize(row);
                                results.Add(JsonDocument.Parse(jsonString));
                            }
                        }
                    }
                    catch (InvalidOperationException) {
                        command.ExecuteNonQuery();
                    }
                }
            }

            return results;
        }

    private bool existingMatch(eventDTO eventData) {
        string sql_query = $"SELECT race_id, track, start_time, round FROM race WHERE race_id='{eventData.getHashId()}';";
        List<JsonDocument> results = this.executeQuery(sql_query);
        return results.Count > 0;
    }

    private bool existingTrack(eventDTO eventData) {
        string sql_query = $"SELECT track_name FROM track WHERE track_name='{eventData.getVenueName()}';";
        List<JsonDocument> results = this.executeQuery(sql_query); 
        return results.Count > 0;
    }

    private void addTrack(eventDTO eventData) {
        string sql_query = $"INSERT INTO track(track_name) VALUES ('{eventData.getVenueName()}');";
        this.executeQuery(sql_query);
    }

    private void addRace(eventDTO eventData) {
        string time = eventData.getStartTime().ToString("o");
        string sql_query = $"INSERT INTO public.race(race_id, track, start_time, round) VALUES ('{eventData.getHashId()}', '{eventData.getVenueName()}', '{time}', {eventData.getRound()});";
        this.executeQuery(sql_query);
    }

    private bool existingHorse(entrantDTO entrant) {
        string sql_query = $"SELECT name FROM horse WHERE name='{entrant.getName()}';";
        List<JsonDocument> results = this.executeQuery(sql_query);
        return results.Count > 0;
    }

    private void addPlatform(eventDTO eventData) {
        string sql_query = $"INSERT INTO public.platforms(platform_name, theme_color) VALUES ('{eventData.getPlatformName()}', '');";
        this.executeQuery(sql_query);
    }

    private bool existingPlatform(eventDTO eventData) {
        string sql_query = $"SELECT platform_name, theme_color FROM platforms WHERE platform_name='{eventData.getPlatformName()}';";
        List<JsonDocument> results = this.executeQuery(sql_query);
        return results.Count > 0;
    }

    private void addHorse(entrantDTO entrant) {
        string sql_query = $"INSERT INTO public.horse(name) VALUES ('{entrant.getName()}');";
        this.executeQuery(sql_query);
    }

    private bool checkEntry(entrantDTO entrant, eventDTO eventData) {
        short scratched = 0;
        if (entrant.getScratched()) {
            scratched = 1;
        }

        string sql_query = $@"
            SELECT 1 
            FROM public.datrum
                WHERE track = '{eventData.getVenueName()}'
                AND horse = '{entrant.getName()}'
                AND race_id = '{eventData.getHashId()}'
                AND platform_name = '{eventData.getPlatformName()}'
                AND is_scratched = {scratched}
                AND odds = {entrant.getOdds()}
            ORDER BY record_time DESC
            LIMIT 1;";
        List<JsonDocument> results = this.executeQuery(sql_query);
        return results.Count > 0;
    }

    private void addEntry(entrantDTO entrant, eventDTO eventData) {
        string record_time = entrant.getRecordTime().ToString("o");

        short scratched = 0;
        if (entrant.getScratched()) {
            scratched = 1;
        }

        string sql_query = $"INSERT INTO public.datrum(track, horse, race_id, platform_name, is_scratched, record_time, odds) VALUES ('{eventData.getVenueName()}', '{entrant.getName()}', '{eventData.getHashId()}', '{eventData.getPlatformName()}', {scratched}, '{record_time}', {entrant.getOdds()});";
        this.executeQuery(sql_query);
    }

    private void print_update(eventDTO eventData, entrantDTO entrant) {
        Console.WriteLine($"{eventData.getVenueName()} {eventData.getRound()} {eventData.getPlatformName()}: {entrant.getName()} {entrant.getOdds()}");
    }

    public override bool execute(eventDTO eventData, entrantDTO entrant)
    {
        if (!existingMatch(eventData)) {
            if (!existingTrack(eventData)) {
                addTrack(eventData);
            }
            addRace(eventData);
        }

        if (!existingHorse(entrant)) {
            addHorse(entrant);
        }

        if (!existingPlatform(eventData)) {
            addPlatform(eventData);
        }

        this.addEntry(entrant, eventData);
        this.print_update(eventData, entrant);

        return true;
    }
}

