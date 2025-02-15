CREATE TABLE track (
    track_name VARCHAR(100) PRIMARY KEY
);

CREATE TABLE horse (
    name VARCHAR(50) PRIMARY KEY
);

CREATE TABLE race (
    race_id VARCHAR(255) PRIMARY KEY,
    track VARCHAR(100),
    start_time TIMESTAMP,
    round int,
    FOREIGN KEY (track) REFERENCES track(track_name)
);

CREATE TABLE platforms (
    platform_name varchar(80) PRIMARY KEY,
    theme_color character varying(14)
);

CREATE TABLE datrum (
    track VARCHAR(100),
    horse VARCHAR(50),
    race_id VARCHAR(255),
    platform_name varchar(80),

    is_scratched int,
    record_time TIMESTAMP,
    odds float,
    
    FOREIGN KEY (horse) REFERENCES horse(name),
    FOREIGN KEY (race_id) REFERENCES race(race_id),
    FOREIGN KEY (track) REFERENCES track(track_name),
    FOREIGN KEY (platform_name) REFERENCES platforms(platform_name),
    PRIMARY KEY (horse, track, race_id, platform_name, record_time)
);