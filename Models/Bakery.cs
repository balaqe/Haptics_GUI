using System;
using System.Collections.Generic;
using System.Linq;
using Haptics_GUI.Models.Functions;
using Haptics_GUI.Models.Transitions;

namespace Haptics_GUI.Models;

public class Bakery
{
    private readonly Format _format;
    private double[] rawSamples;
    
    public List<Transition> Transitions;
    public List<PhaseOffset> PhaseOffsets;

    public double[] BakedSamples;

    public Bakery(Format format)
    {
        _format = format;
        Transitions = new List<Transition>();
        PhaseOffsets = new List<PhaseOffset>();
        rawSamples = new double[0];
        BakedSamples = new double[0];
    }

    // SampleCount needs to be updated when adding a new pulse
    public void PadSamples(double totalLength)
    {
        var newSampleCount = (int)(totalLength * _format.SamplingRate);
        if (newSampleCount > rawSamples.Length)
        {
            // TODO MIGHT BE AN ISSUE, temp array might need  to be made
            Array.Resize(ref rawSamples, newSampleCount);
            for (int i = rawSamples.Length; i < newSampleCount; i++)
            {
                rawSamples[i] = 0;
            }
        }
    }
    
    
    public void PadSamples(int totalLengthSamples)
    {
        var newSampleCount = totalLengthSamples;
        if (newSampleCount > rawSamples.Length)
        {
            // TODO MIGHT BE AN ISSUE, temp array might need  to be made
            Array.Resize(ref rawSamples, newSampleCount);
            for (int i = rawSamples.Length; i < newSampleCount; i++)
            {
                rawSamples[i] = 0;
            }
        }
        
        newSampleCount = totalLengthSamples;
        if (newSampleCount > BakedSamples.Length)
        {
            // TODO MIGHT BE AN ISSUE, temp array might need  to be made
            Array.Resize(ref BakedSamples, newSampleCount);
            for (int i = BakedSamples.Length; i < newSampleCount; i++)
            {
                BakedSamples[i] = 0;
            }
        }
    }

    public void GenerateSigmoidTransition(double duration, int startTime, double inStartVal, double inEndVal)
    {
        Transitions.Add(new Sigmoid(_format, duration, startTime, inStartVal, inEndVal));
        Transitions.Last().Generate();
    }
    

    public void GenerateSine(double startTime, double duration, double startFreq, double endFreq)
    {
        var phaseOffsetList = PhaseOffsets.Where(p => p.Timestamp == (int)(startTime * _format.SamplingRate)).ToList();
        if (phaseOffsetList.Count > 1)
        {
            throw new Exception("Phase offset list has multiple elements at the same timestamp");
        }

        if (phaseOffsetList.Count == 0)
        {
            phaseOffsetList.Add(new PhaseOffset(0, 0));
        }
        var phaseOffset = phaseOffsetList[0];
        
        Function sine = new Sine(_format, startFreq, endFreq, duration, phaseOffset.Phase);
        sine.Generate();
        
        // Padding until end
        PadSamples((int)((startTime + duration) * _format.SamplingRate));
        
        PhaseOffsets.Add(new PhaseOffset(sine.endPhase, (int)((startTime + duration) * _format.SamplingRate)));

        var offset = (int)(startTime * _format.SamplingRate);
        Array.Copy(sine.rawSamples, 0, rawSamples, offset, sine.rawSamples.Length);
    }
    
    public void GenerateSquare()
    {}



    private void ApplyTransitions()
    {
        if (Transitions.Count == 0) return;

        foreach (var trans in Transitions)
        {
            if (BakedSamples.Length >= (trans.rawSamples.Length + trans.startTime))
            {
                for (int i = 0; i < trans.rawSamples.Length; i++)
                {
                    BakedSamples[trans.startTime + i] *= trans.rawSamples[i];
                }
            } 
        }
    }

    private void Cleanup()
    {
        // Only single phase offsets have to cleaned up
        var singlePhaseOffsets = PhaseOffsets.GroupBy(p => p.Timestamp).Where(g => g.Count() == 1).ToList();
        foreach (var phaseOffset in singlePhaseOffsets)
        {
            // Cannot be higher than rawSamples.Length if incremented by 1
            var index = phaseOffset.Key - 2;
            /*
            0_0 = 0
                
            +_0 = 1
            +_+ = 2
            -_0 = -1
            -_- = -2
                
            +0- = 0
            */
            // TODO TEST THIS STUFF
            bool signChange = false;
            while (index > 0 && signChange != true)
            {
                // If opposing signs +1 + (-1) = 0
                signChange = Math.Sign(BakedSamples[index + 1]) + Math.Sign(BakedSamples[index - 1]) == 0;

                BakedSamples[index] = 0;
                
                index--;
            }
        }
    }
    

    public void Bake()
    {
        ApplyTransitions();
        Cleanup();
    }
    
}