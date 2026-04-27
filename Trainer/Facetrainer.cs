using Emgu.CV;
using Emgu.CV.Face;
using Emgu.CV.Structure;

namespace FaceAttendance.Trainer;

public class FaceTrainer
{
    public LBPHFaceRecognizer TrainModel()
    {
        var recognizer = new LBPHFaceRecognizer();

        var images = new List<Image<Gray, byte>>();
        var labels = new List<int>();

        if (!Directory.Exists("Faces"))
            return recognizer;

        foreach (var file in Directory.GetFiles("Faces", "*.jpg"))
        {
            var img = new Image<Gray, byte>(file);
            int id = int.Parse(Path.GetFileName(file).Split('_')[0]);

            images.Add(img);
            labels.Add(id);
        }

        if (images.Count > 0)
            recognizer.Train(images.ToArray(), labels.ToArray());

        return recognizer;
    }
}