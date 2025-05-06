using System;

namespace Haptics_GUI.Models.FreqSweepFunctions;

public class FreqSweepSine : FreqSweepFunction
{
    public FreqSweepSine(double inStartFreq, double inEndFreq, double inDur, double inStartPhase,
        int inSamplingRate, int inBitDepth, int inChannel)
    : base(inStartFreq, inEndFreq, inDur, inStartPhase, inSamplingRate, inBitDepth, inChannel)
    {}
    public override double Func(int i)
    { 
        var phase = startPhase + 2*Math.PI * i * (((endFreq - startFreq) * i) / (2 * samplingRate * sampleCount) + startFreq / samplingRate);
        endPhase = phase % (2 * Math.PI);
        return Math.Sin(phase); // Formula for linear chirp
    }
}