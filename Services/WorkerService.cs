using Microsoft.Data.Sqlite;
using FaceAttendance.Models;
using System.Collections.Generic;

namespace FaceAttendance.Services;

public class WorkerService
{
    string conn = "Data Source=attendance.db";

    public List<Worker> GetAll()
    {
        var list = new List<Worker>();

        using var con = new SqliteConnection(conn);
        con.Open();

        var cmd = con.CreateCommand();
        cmd.CommandText = "SELECT * FROM Workers";

        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            list.Add(new Worker
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Code = reader.GetString(2)
            });
        }

        return list;
    }

    public void Add(string name, string code)
    {
        using var con = new SqliteConnection(conn);
        con.Open();

        var cmd = con.CreateCommand();
        cmd.CommandText = "INSERT INTO Workers(Name, Code) VALUES(@n, @c)";
        cmd.Parameters.AddWithValue("@n", name);
        cmd.Parameters.AddWithValue("@c", code);

        cmd.ExecuteNonQuery();
    }
    
    public int GetLastId()
    {
        using var con = new SqliteConnection(conn);
        con.Open();

        var cmd = con.CreateCommand();
        cmd.CommandText = "SELECT last_insert_rowid()";

        return Convert.ToInt32(cmd.ExecuteScalar());
    }
}