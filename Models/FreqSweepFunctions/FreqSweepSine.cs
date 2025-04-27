using System;

namespace Haptics_GUI.Models.FreqSweepFunctions;

public class FreqSweepSine : FreqSweepFunction
{
    public FreqSweepSine(double inStartFreq, double inEndFreq, double inDur, double inStartPhase,
        int inSamplingRate, int inBitDepth, int inChannel)
    : base(inStartFreq, inEndFreq, inDur, inStartPhase, inSamplingRate, inBitDepth, inChannel)
    {}
    public override double Func(int numberOfSamples, int i)
    { 
        // uncomplete
        var arg = ((startFreq / samplingRate) + (((endFreq - startFreq) / samplingRate) / numberOfSamples) / 2 * i) 
                  * 2 * Math.PI * i 
                  + 2 * Math.PI * startPhase;
        endPhase = arg / 2 * Math.PI - ((startFreq / samplingRate) + (((endFreq - startFreq) / samplingRate) / numberOfSamples) * i) * i; // Modulo this to not have BIG number
        return Math.Sin(arg); // Formula for linear chirp
    }
}