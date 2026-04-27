using Emgu.CV.Face;
using Emgu.CV.Structure;

namespace FaceAttendance.Services;

public class FaceRecognizerService
{
    private LBPHFaceRecognizer recognizer;

    public void Load(LBPHFaceRecognizer model)
    {
        recognizer = model;
    }

    public int Predict(Image<Gray, byte> face)
    {
        if (recognizer == null)
            return -1;

        var result = recognizer.Predict(face);

        if (result.Label == -1 || result.Distance > 80)
            return -1;

        return result.Label;
    }
}