using Microsoft.Data.Sqlite;
using System;

namespace FaceAttendance.Services;

public class AttendanceService
{
    string conn = "Data Source=attendance.db";

    public void Log(int workerId)
    {
        using var con = new SqliteConnection(conn);
        con.Open();

        var cmd = con.CreateCommand();
        cmd.CommandText = "INSERT INTO AttendanceLogs(WorkerId, Time) VALUES(@id, @t)";
        cmd.Parameters.AddWithValue("@id", workerId);
        cmd.Parameters.AddWithValue("@t", DateTime.Now.ToString("s"));

        cmd.ExecuteNonQuery();
    }

    public (DateTime?, DateTime?, double) GetWorkingHours(int workerId)
    {
        using var con = new SqliteConnection(conn);
        con.Open();

        var cmd = con.CreateCommand();
        cmd.CommandText = @"
            SELECT MIN(Time), MAX(Time)
            FROM AttendanceLogs
            WHERE WorkerId = @id";

        cmd.Parameters.AddWithValue("@id", workerId);

        using var reader = cmd.ExecuteReader();

        if (reader.Read())
        {
            DateTime? start = reader.IsDBNull(0) ? (DateTime?)null : DateTime.Parse(reader.GetString(0));
            DateTime? end = reader.IsDBNull(1) ? (DateTime?)null : DateTime.Parse(reader.GetString(1));

            double hours = 0;
            if (start != null && end != null)
                hours = (end.Value - start.Value).TotalHours;

            return (start, end, hours);
        }

        return (null, null, 0);
    }
}