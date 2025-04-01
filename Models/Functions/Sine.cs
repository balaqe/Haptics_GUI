using System;

namespace Haptics_GUI.Models.Functions;

public class Sine : Function
{
    public Sine(double inFreq, double inDur, int inChannel, int inSamplingRate, int inBitDepth)
    : base(inFreq, inDur, inChannel, inSamplingRate, inBitDepth)
    {}

    public override double Func(double inFreq, int inSamplingRate, int i)
    {
        double omega = 2*Math.PI*inFreq/(double)inSamplingRate;
        return Math.Sin(omega * i);
    }
}