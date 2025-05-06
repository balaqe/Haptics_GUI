using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Haptics_GUI.Models.FreqSweepFunctions;
using Haptics_GUI.Models.Functions;
using Haptics_GUI.Models.Transitions;
using NAudio.Wave;

namespace Haptics_GUI.Models;

public class FreqSweepWaveformGen
{
    public int SamplingRate;
    public byte BitDepth;
    public List<byte[]> ByteStreams;
    public List<double[]> rawSamples;

    public List<List<PhaseOffset>> PhaseOffsets;
    
    public WaveFormat WaveFormat;
    private double longestTrack;
    
    public class PhaseOffset
    {
        public double Phase;
        public int Timestamp;

        public PhaseOffset()
        {
            Phase = 0.0;
            Timestamp = 0;
        }

        public PhaseOffset(double phase, int timestamp)
        {
            Phase = phase;
            Timestamp = timestamp;
        }
    }

    public FreqSweepWaveformGen() // Default constructor
    {
        SamplingRate = 44100;
        BitDepth = 16;
        longestTrack = 0;
        ByteStreams = new List<byte[]>();
        rawSamples = new List<double[]>();
        PhaseOffsets = new List<List<PhaseOffset>>();
    }

    public FreqSweepWaveformGen(int samplingRate, int bitDepth) // Custom constructor
    {
        SamplingRate = samplingRate;
        BitDepth = (byte)bitDepth;
        longestTrack = 0;
        ByteStreams = new List<byte[]>();
        rawSamples = new List<double[]>();
        PhaseOffsets = new List<List<PhaseOffset>>();
    }
    
    private void SpawnChannel (int channelNo, double dur, double start)
    {
        var noOfSamples = (int)((double)SamplingRate * (dur + start));
        var  byteLength = noOfSamples * BitDepth / 8;
        
        while (ByteStreams.Count <= channelNo) // Check if channel exists
        {
            ByteStreams.Add(new byte[byteLength]); // Create channels if needed
            rawSamples.Add(new double[noOfSamples]);
            WaveFormat = new WaveFormat(SamplingRate, BitDepth, ByteStreams.Count);
            PhaseOffsets.Add(new List<PhaseOffset>());
        }

        if (ByteStreams[channelNo].Length < byteLength) {
            var temp = ByteStreams[channelNo];
            Array.Resize(ref temp, byteLength);
            ByteStreams[channelNo] = temp;
        }
        
        if (rawSamples[channelNo].Length < noOfSamples) {
            var temp = rawSamples[channelNo];
            Array.Resize(ref temp, noOfSamples);
            rawSamples[channelNo] = temp;
        }
        
    }
    
    private void PadChannels (double dur)
    {
        if (dur > longestTrack) longestTrack = dur;
        int newSize = (int)(SamplingRate * longestTrack * BitDepth) / 8;

        for (int i=0; i<ByteStreams.Count; i++) // Not foreach because we need the index
        {
            if (ByteStreams[i].Length < newSize)
            {
                byte[] temp = ByteStreams[i];
                Array.Resize(ref temp, newSize); // Pad the end of the array with zeroes (tested)
                ByteStreams[i] = temp;
            }
        }
        
        newSize = (int)(SamplingRate * longestTrack);
        for (int i=0; i<rawSamples.Count; i++) // Not foreach because we need the index
        {
            if (rawSamples[i].Length < newSize)
            {
                double[] temp = rawSamples[i];
                Array.Resize(ref temp, newSize); // Pad the end of the array with zeroes (tested)
                rawSamples[i] = temp;
            }
        }
    }
    
    public void Sine (double startFreq, double endFreq, double dur, int channelNo, double start)
    {
        SpawnChannel(channelNo, dur, start);
        
        // Finding phase of last waveform
        var phaseOffsetList = PhaseOffsets[channelNo].Where(p => p.Timestamp == (int)(start * SamplingRate)).ToList();
        if (phaseOffsetList.Count > 1)
        {
            throw new Exception("Phase offset list has multiple elements at the same timestamp");
        }

        if (phaseOffsetList.Count == 0)
        {
            phaseOffsetList.Add(new PhaseOffset(0, 0));
        }
        var phaseOffset = phaseOffsetList[0];
        
        FreqSweepFunction sine = new FreqSweepSine(startFreq, endFreq, dur, phaseOffset.Phase, SamplingRate, BitDepth, channelNo);
        sine.Generate();
        PadChannels(dur + start);
        
        PhaseOffsets[channelNo].Add(new PhaseOffset(sine.endPhase, (int)((start + dur) * SamplingRate)));

        var offset = (int)((double)SamplingRate * start * BitDepth / 8);
        Array.Copy(sine.Waveform, 0, ByteStreams[channelNo], offset, sine.Waveform.Length);
    }
    
    
    public void LinearTrans(int channelNo, double startTime, double endTime, double startVal, double endVal)
    {
        Transition linear = new Linear(ByteStreams[channelNo], startTime, endTime, startVal, endVal, BitDepth, SamplingRate);
        linear.Generator();
        // Array.Copy(linear.resultData, 0, ByteStreams[channelNo], 0, linear.resultData.Length);
        ByteStreams[channelNo] = linear.resultData;
    }
    
    public void SigmoidTrans(int channelNo, double startTime, double endTime, double startVal, double endVal)
    {
        Transition sigmoid = new Sigmoid(ByteStreams[channelNo], startTime, endTime, startVal, endVal, BitDepth, SamplingRate);
        sigmoid.Generator();
        // Array.Copy(linear.resultData, 0, ByteStreams[channelNo], 0, linear.resultData.Length);
        ByteStreams[channelNo] = sigmoid.resultData;
    }

    public void CleanWaveforms()
    {
        for (var i = 0; i < ByteStreams.Count; i++)
        {
            // Only single phase offsets have to cleaned up
            var singlePhaseOffsets = PhaseOffsets[i].GroupBy(p => p.Timestamp).Where(g => g.Count() == 1).ToList();
            foreach (var phaseOffset in singlePhaseOffsets)
            {
                var index = phaseOffset.Key - 1;
                if
               
                // If opposing signs +1 + (-1) = 0, -2 or 2 otherwise
                bool signChange = Math.Sign(rawSamples[i][index + 1]) + Math.Sign(rawSamples[i][index]) == 0;
                while (signChange )
                {
                    //ByteStreams[i][index] = (byte)zeroLevel;
                    
                    index--;
                    signChange = Math.Sign(rawSamples[i][index + 1]) + Math.Sign(rawSamples[i][index]) == 0;
                }
            }
        }
    }

}