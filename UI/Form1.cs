using System;
using System.Drawing;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using FaceAttendance.Services;
using FaceAttendance.Models;

namespace FaceAttendance.UI;

public class Form1 : Form
{
    // UI
    TextBox txtName = new TextBox();
    TextBox txtCode = new TextBox();
    Button btnAdd = new Button();
    Button btnCalc = new Button();
    DataGridView grid = new DataGridView();
    PictureBox pic = new PictureBox();
    Label lblInfo = new Label();

    // Services
    WorkerService workerService = new WorkerService();
    AttendanceService attendanceService = new AttendanceService();
    FaceTrainer trainer = new FaceTrainer();
    FaceRecognizerService recognizer = new FaceRecognizerService();
    Dictionary<int, DateTime> lastLog = new();

    // Camera
    VideoCapture cam;
    CascadeClassifier face;

    public Form1()
    {
        InitUI();
        LoadData();
        InitCamera();
    }

    void InitUI()
    {
        this.Text = "Face Attendance System";
        this.Width = 900;
        this.Height = 600;

        // Inputs
        txtName.SetBounds(20, 20, 150, 25);
        txtCode.SetBounds(180, 20, 150, 25);

        btnAdd.Text = "Add Worker";
        btnAdd.SetBounds(350, 20, 120, 25);
        btnAdd.Click += BtnAdd_Click;

        btnCalc.Text = "Calculate Hours";
        btnCalc.SetBounds(500, 20, 150, 25);
        btnCalc.Click += BtnCalc_Click;

        // Grid
        grid.SetBounds(20, 60, 600, 250);
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        // Camera
        pic.SetBounds(650, 60, 200, 150);

        // Info
        lblInfo.SetBounds(650, 220, 200, 100);

        Controls.AddRange(new Control[]
        {
            txtName, txtCode, btnAdd, btnCalc,
            grid, pic, lblInfo
        });
    }

    void LoadData()
    {
        grid.DataSource = workerService.GetAll();
    }

    void InitCamera()
    {
        face = new CascadeClassifier("haarcascade_frontalface_default.xml");

        cam = new VideoCapture(0);
        cam.ImageGrabbed += ProcessFrame;

        var model = trainer.TrainModel();   
        recognizer.Load(model);             
        cam.Start();
    }

    void ProcessFrame(object sender, EventArgs e)
    {
        try
        {
            Mat frame = new Mat();
            cam.Retrieve(frame);

            var img = frame.ToImage<Bgr, byte>();
            var gray = img.Convert<Gray, byte>();

            var faces = face.DetectMultiScale(gray, 1.1, 5);

            foreach (var f in faces)
            {
                img.Draw(f, new Bgr(Color.Red), 2);

                var faceImg = gray.Copy(f)
                    .Resize(200, 200, Emgu.CV.CvEnum.Inter.Linear);

                int workerId = recognizer.Predict(faceImg);

                if (workerId != -1)
                {
                    if (!lastLog.ContainsKey(workerId) ||
                        (DateTime.Now - lastLog[workerId]).TotalSeconds > 10)
                    {
                        attendanceService.Log(workerId);
                        lastLog[workerId] = DateTime.Now;
                    }

                    lblInfo.Text = $"Recognized: {workerId}";
                }
                else
                {
                    lblInfo.Text = "Unknown";
                }
            }

            pic.Image = img.ToBitmap();
        }
        catch { }
    }

    private void BtnAdd_Click(object sender, EventArgs e)
    {
        workerService.Add(txtName.Text, txtCode.Text);
        LoadData();

        int workerId = workerService.GetLastId();

        CaptureFace(workerId);

        var model = trainer.TrainModel();
        recognizer.Load(model);
    }

    private void BtnCalc_Click(object sender, EventArgs e)
    {
        if (grid.CurrentRow == null) return;

        int workerId = (int)grid.CurrentRow.Cells["Id"].Value;

        var r = attendanceService.GetWorkingHours(workerId);

        MessageBox.Show(
            $"Check-in: {r.Item1}\n" +
            $"Check-out: {r.Item2}\n" +
            $"Hours: {r.Item3:F2}");
    }

    void CaptureFace(int workerId)
    {
        Mat frame = new Mat();
        cam.Retrieve(frame);

        var img = frame.ToImage<Bgr, byte>();
        var gray = img.Convert<Gray, byte>();

        var faces = face.DetectMultiScale(gray, 1.1, 5);

        if (faces.Length == 0)
        {
            MessageBox.Show("No face detected!");
            return;
        }

        Directory.CreateDirectory("Faces");

        //  chụp nhiều ảnh (tối đa 3)
        for (int i = 0; i < Math.Min(3, faces.Length); i++)
        {
            var faceImg = gray.Copy(faces[i])
                .Resize(200, 200, Emgu.CV.CvEnum.Inter.Linear);

            faceImg.Save($"Faces/{workerId}_{i}.jpg");
        }

        MessageBox.Show("Captured multiple face images!");

        MessageBox.Show("Face captured!");
    }
}