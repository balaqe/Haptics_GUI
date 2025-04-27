using System;
using System.Collections.Generic;
using System.Linq;
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

    public class PhaseOffset
    {
        public double Phase;
        public double Timestamp;

        public PhaseOffset()
        {
            Phase = 0.0;
            Timestamp = 0.0;
        }

        public PhaseOffset(double phase, double timestamp)
        {
            Phase = phase;
            Timestamp = timestamp;
        }
    }
    public List<List<PhaseOffset>> PhaseOffsets;
    
    public WaveFormat WaveFormat;
    private double longestTrack;

    public FreqSweepWaveformGen() // Default constructor
    {
        SamplingRate = 44100;
        BitDepth = 16;
        longestTrack = 0;
        ByteStreams = new List<byte[]>();
        PhaseOffsets = new List<List<PhaseOffset>>();
    }

    public FreqSweepWaveformGen(int samplingRate, int bitDepth) // Custom constructor
    {
        SamplingRate = samplingRate;
        BitDepth = (byte)bitDepth;
        longestTrack = 0;
        ByteStreams = new List<byte[]>();
        PhaseOffsets = new List<List<PhaseOffset>>();
    }
    
    private void SpawnChannel (int channelNo, double dur, double start)
    {
        var noOfSamples = (int)((double)SamplingRate * (dur + start));
        var  byteLength = noOfSamples * BitDepth / 8;
        
        while (ByteStreams.Count <= channelNo) // Check if channel exists
        {
            ByteStreams.Add(new byte[byteLength]); // Create channels if needed
            WaveFormat = new WaveFormat(SamplingRate, BitDepth, ByteStreams.Count);
            PhaseOffsets.Add(new List<PhaseOffset>());
        }

        if (ByteStreams[channelNo].Length < byteLength) {
            var temp = ByteStreams[channelNo];
            Array.Resize(ref temp, byteLength);
            ByteStreams[channelNo] = temp;
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
    }
    
    public void Sine (double startFreq, double endFreq, double dur, int channelNo, double start)
    {
        SpawnChannel(channelNo, dur, start);
        
        // Finding phase of last waveform
        var phaseOffsetList = PhaseOffsets[channelNo].Where(p => p.Timestamp == start * SamplingRate).ToList();
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
        
        PhaseOffsets[channelNo].Add(new PhaseOffset(sine.endPhase, (start + dur) * SamplingRate));
        
        /*
         * cleaning waveforms before playing
         */

        var offset = (int)((double)SamplingRate * start * BitDepth / 8);
        Array.Copy(sine.Waveform, 0, ByteStreams[channelNo], offset, sine.Waveform.Length);
    }

}