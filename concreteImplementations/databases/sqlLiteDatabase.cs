

using System;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using Microsoft.Data.Sqlite;
using Npgsql;

class sqlLiteDatabase : abstractDatabase
{
    private SqliteConnection connection; 

    public sqlLiteDatabase() {
        this.connection = new SqliteConnection("Data Source=C:\\Users\\William\\Desktop\\PersonalProjects\\Arbie\\arbie 4.2\\Arbie\\concreteImplementations\\databases\\liteDB.db");
    }

    private bool existingMatch(eventDTO eventData) {
        var command = this.connection.CreateCommand();
        command.CommandText = $"SELECT race_id, track, start_time, round FROM race WHERE race_id='{eventData.getHashId()}';";

        using var reader = command.ExecuteReader();
        return reader.Read();
    }

    private bool existingTrack(eventDTO eventData) {
        var command = this.connection.CreateCommand();
        command.CommandText = $"SELECT track_name FROM track WHERE track_name='{eventData.getVenueName()}';";

        using (var reader = command.ExecuteReader()) {
            while (reader.Read()) {
                return true;
            }
        }
        return false;
    }

    private void addTrack(eventDTO eventData) {
        var command = this.connection.CreateCommand();
        command.CommandText = $"INSERT INTO track(track_name) VALUES ('{eventData.getVenueName()}');";
        command.ExecuteNonQuery();
    }

    private void addRace(eventDTO eventData) {
        string time = eventData.getStartTime().ToString("o");
        var command = this.connection.CreateCommand();

        command.CommandText = $"INSERT INTO race(race_id, track, start_time, round) VALUES ('{eventData.getHashId()}', '{eventData.getVenueName()}', '{time}', {eventData.getRound()});";
        command.ExecuteNonQuery();
    }

    private bool existingHorse(entrantDTO entrant) {
        var command = this.connection.CreateCommand();
        command.CommandText = $"SELECT name FROM horse WHERE name='{entrant.getName()}';";

        using (var reader = command.ExecuteReader()) {
            while (reader.Read()) {
                return true;
            }
        }
        return false;
    }

    private void addPlatform(eventDTO eventData) {
        var command = this.connection.CreateCommand();
        command.CommandText = $"INSERT INTO platforms(platform_name, theme_color) VALUES ('{eventData.getPlatformName()}', '');";
        command.ExecuteNonQuery();
    }

    private bool existingPlatform(eventDTO eventData) {
        var command = this.connection.CreateCommand();
        command.CommandText = $"SELECT platform_name, theme_color FROM platforms WHERE platform_name='{eventData.getPlatformName()}';";

        using (var reader = command.ExecuteReader()) {
            while (reader.Read()) {
                return true;
            }
        }
        return false;
    }

    private void addHorse(entrantDTO entrant) {
        var command = this.connection.CreateCommand();
        command.CommandText = $"INSERT INTO horse(name) VALUES ('{entrant.getName()}');";
        command.ExecuteNonQuery();
    }

    private void addEntry(entrantDTO entrant, eventDTO eventData) {
        string record_time = entrant.getRecordTime().ToString("o");

        short scratched = 0;
        if (entrant.getScratched()) {
            scratched = 1;
        }

        var command = this.connection.CreateCommand();
        command.CommandText = $"INSERT INTO datrum(track, horse, race_id, platform_name, is_scratched, record_time, odds) VALUES ('{eventData.getVenueName()}', '{entrant.getName()}', '{eventData.getHashId()}', '{eventData.getPlatformName()}', {scratched}, '{record_time}', {entrant.getOdds()});";
        command.ExecuteNonQuery();
    }

    private void print_update(eventDTO eventData, entrantDTO entrant) {
        Console.WriteLine($"{eventData.getVenueName()} {eventData.getRound()} {eventData.getPlatformName()}: {entrant.getName()} {entrant.getOdds()}");
    }

    public override bool execute(eventDTO eventData, entrantDTO entrant)
    {
        this.connection.Open();
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
        this.connection.Close();

        return true;
    }
}

