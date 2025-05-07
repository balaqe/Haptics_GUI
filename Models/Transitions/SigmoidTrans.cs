using System;

namespace Haptics_GUI.Models.Transitions;

public class Sigmoid(Format format, double dur, int startTime, double inStartVal, double inEndVal) : Transition(format, dur, startTime, inStartVal, inEndVal)
{
    private readonly double stretchCoeff = -2*Math.Log(1 / 0.99 - 1, Math.E); // Set to reach 99% of the final value by endTime

    public override double Func(int i)
    {
        double normI = i / (double)(dur * format.SamplingRate);
        double sigmoidValue = 1.0 / (1.0 + Math.Exp(-stretchCoeff * (normI - 0.5)));
    
        return startVal + sigmoidValue * (endVal - startVal);
    }
}