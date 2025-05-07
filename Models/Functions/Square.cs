using System;

namespace Haptics_GUI.Models.Functions;

public class Square : Function
{
    int zeroCrossing= 0;
    int period;
    public Square(Format inFormat, double inStartFreq, 
                          double inDur)
    : base(inFormat, inStartFreq, inStartFreq, inDur, 0)
    {
        // period = (int)(inFreq/inSamplingRate); // Calculate period
        period = (int)(format.SamplingRate/startFreq);
    }

    public override double Func(int i)
    {
        if (i % (period/2) == 0) zeroCrossing++;
        return Math.Pow(-1, zeroCrossing);
    }
}