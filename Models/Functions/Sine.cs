using System;

namespace Haptics_GUI.Models.Functions;

public class Sine : Function
{
    public Sine(double inFreq, double inDur, uint inChannel, uint inStart, uint inSamplingRate, uint inBitDepth)
    : base(inFreq, inDur, inChannel, inStart, inSamplingRate, inBitDepth)
    {}

    public override double Func(double inFreq, uint inSamplingRate, uint i)
    {
        double omega = 2*Math.PI*inFreq/(double)inSamplingRate;
        return Math.Sin(omega * i);
    }
}