using System;

namespace Haptics_GUI.Models.Transitions;

public class Linear : Transition
{
    // private double m;

    public Linear(byte[] inData, double inStartTime, double inEndTime, double inStartVal, double inEndVal,
        int inBitDepth, int inSampRate)
        : base(inData, inStartTime, inEndTime, inStartVal, inEndVal, inBitDepth, inSampRate)
    {
        // m = (endVal - startVal) / (endSample - startSample);
    }

    public override double Func(int i)
    {
        double m = (i - startSample) / (double)(endSample - startSample);
        return startVal + m * (endVal - startVal); 
    }
}