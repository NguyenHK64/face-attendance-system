using System;
using System.Windows.Forms;
using FaceAttendance.Services;

namespace FaceAttendance;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        Database.Init(); // init DB

        Application.Run(new UI.Form1()); 
    }
}