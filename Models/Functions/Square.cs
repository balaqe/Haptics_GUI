using System;

namespace Haptics_GUI.Models.Functions;

public class Square(
    Format inFormat,
    double inStartFreq,
    double inEndFreq,
    double inDur,
    double inStartPhase)
    : Function(inFormat, inStartFreq, inEndFreq, inDur, inStartPhase)
{
    public override double Func(int i)
    {
        var phase = startPhase + 2 * Math.PI * i *
            (((endFreq - startFreq) * i) / (2 * format.SamplingRate * sampleCount) + startFreq / format.SamplingRate);
        endPhase = phase % (2 * Math.PI);
        return Math.Sign(Math.Sin(phase)) == 1 ? 1 : -1; // Formula for linear chirp
    }
}