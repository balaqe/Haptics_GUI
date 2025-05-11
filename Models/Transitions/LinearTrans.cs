using Haptics_GUI.Models.Functions;

namespace Haptics_GUI.Models.Transitions;

public class Linear(Format format, double inDur, double startTime, double inStartVal, double inEndVal) : Transition(format, inDur, startTime, inStartVal, inEndVal)
{
    public override double Func(int i)
    {
        double m = (double)i / (double)(sampleCount);
        return startVal + m * (endVal - startVal); 
    }
}