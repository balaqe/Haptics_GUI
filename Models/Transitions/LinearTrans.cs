using System;

namespace Haptics_GUI.Models.Transitions;

public class Linear : Transition
{
    public Linear(byte[] inData, double inStartTime, double inEndTime, double inStartVal, double inEndVal, int inBitDepth, int inSampRate)
    : base(inData, inStartTime, inEndTime, inStartVal, inEndVal, inBitDepth, inSampRate)
    {}

    public override double Func(int i)
    {
        int startSample = (int)(startTime * samplesPerSecond);
        int endSample = (int)(endTime * samplesPerSecond);
        double normalizedTime = (i - startSample) / (double)(endSample - startSample);

        return startVal + normalizedTime * (endVal - startVal); 
    }
}