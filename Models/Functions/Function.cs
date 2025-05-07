namespace Haptics_GUI.Models.Functions;

public abstract class Function
{
    protected Format format;
    
    protected double startFreq;
    protected double endFreq;
    
    protected readonly double dur;
    public double startPhase;
    public double endPhase; // Continuously updated by func
    
    protected int sampleCount;
    public double[] rawSamples;

    
    protected Function(Format inFormat, double inStartFreq, 
        double inEndFreq, double inDur, double inStartPhase)
    {
        format = inFormat;
        startFreq = inStartFreq;
        endFreq = inEndFreq;
        dur = inDur;
        startPhase = inStartPhase;
        endPhase = 0;
        sampleCount = (int)(format.SamplingRate * inDur);
        rawSamples = new double[sampleCount];
    }

    protected Function(Format inFormat, double inDur)
    {
        format = inFormat;
        startFreq = inDur;
        sampleCount = (int)(format.SamplingRate * inDur);
        rawSamples = new double[sampleCount];
    }

    public abstract double Func(int i);

    public void Generate()
    {
        for (int i = 0; i < sampleCount; i++)
        {
            rawSamples[i] = Func(i);
        }
    }
}