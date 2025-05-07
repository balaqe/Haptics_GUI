namespace Haptics_GUI.Models;

public class PhaseOffset(double phase, int timestamp)
{
    public double Phase = phase;
    public int Timestamp = timestamp;

    public PhaseOffset() : this(0.0, 0)
    {}
}