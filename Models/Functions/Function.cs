namespace Haptics_GUI.Models.Functions;

public abstract class Function
{
    public double freq;
    public double dur;
    public uint channel;
    public uint start;
    public uint samplingRate;
    public uint bitDepth;
    public byte[] waveform;

    public abstract void Generator();
}