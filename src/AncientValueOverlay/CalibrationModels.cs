namespace AncientValueOverlay;

public sealed record CalibrationPreview(string Message, bool IsReady)
{
    public static CalibrationPreview NotStarted => new("Calibration is not implemented in the simple starter build.", false);
}
