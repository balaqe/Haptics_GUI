using System;

namespace Haptics_GUI.Models.Transitions;

public class Sigmoid: Transition
{
    private double stretchCoeff;

    public Sigmoid(byte[] inData, double inStartTime, double inEndTime, double inStartVal, double inEndVal,
        int inBitDepth, int inSampRate)
        : base(inData, inStartTime, inEndTime, inStartVal, inEndVal, inBitDepth, inSampRate)
    {
        stretchCoeff = -2*Math.Log(1 / 0.99 - 1, Math.E); // Set to reach 99% of the final value by endTime
    }

    public override double Func(int i)
    {
        double normI = (i - startSample) / (double)(endSample - startSample);
        double sigmoidValue = 1.0 / (1.0 + Math.Exp(-stretchCoeff * (normI - 0.5)));
    
        return startVal + sigmoidValue * (endVal - startVal);
    }
}