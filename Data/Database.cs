using Microsoft.Data.Sqlite;

namespace FaceAttendance;

public class Database
{
    static string conn = "Data Source=attendance.db";

    public static void Init()
    {
        using var con = new SqliteConnection(conn);
        con.Open();

        var cmd = con.CreateCommand();

        cmd.CommandText = @"
        CREATE TABLE IF NOT EXISTS Workers(
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Name TEXT,
            Code TEXT
        )";
        cmd.ExecuteNonQuery();

        cmd.CommandText = @"
        CREATE TABLE IF NOT EXISTS AttendanceLogs(
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            WorkerId INTEGER,
            Time TEXT
        )";
        cmd.ExecuteNonQuery();
    }
}