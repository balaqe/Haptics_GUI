using System;

namespace Haptics_GUI.Models.Functions;

public class Square : Function
{
    int zeroCrossing= 0;
    int period;
    public Square(double inFreq, double inDur, int inChannel, int inSamplingRate, int inBitDepth)
    : base(inFreq, inDur, inChannel, inSamplingRate, inBitDepth)
    {
        // period = (int)(inFreq/inSamplingRate); // Calculate period
        period = (int)(inSamplingRate/inFreq);
    }

    public override double Func(double inFreq, int inSamplingRate, int i)
    {
        if (i % (period/2) == 0) zeroCrossing++;
        return Math.Pow(-1, zeroCrossing);
    }
}