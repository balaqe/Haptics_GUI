using System;
using Haptics_GUI.Models.Functions;

namespace Haptics_GUI.Models.Transitions;

public abstract class Transition(Format format, double dur, double startTime, double inStartVal, double inEndVal)
    : Function(format, dur)
{
    public double startVal = inStartVal;
    public double endVal = inEndVal;
    public int startTime = (int)(startTime * format.SamplingRate);
}