namespace FaceAttendance.Models;

public class AttendanceLog
{
    public int Id { get; set; }
    public int WorkerId { get; set; }
    public DateTime Time { get; set; }
}