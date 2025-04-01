using System;
using System.Collections;
using System.Collections.Generic;
using Haptics_GUI.Models.Functions;

namespace Haptics_GUI.Models;

public class WaveformGen
{
    public int SamplingRate;
    public byte BitDepth;
    public List<byte[]> ByteStreams;
    private double longestTrack;

    public WaveformGen() // Default constructor
    {
        SamplingRate = 44100;
        BitDepth = 16;
        longestTrack = 0;
        ByteStreams = new List<byte[]>();
    }

    public WaveformGen(int samplingRate, int bitDepth) // Custom constructor
    {
        SamplingRate = samplingRate;
        BitDepth = (byte)bitDepth;
        longestTrack = 0;
        ByteStreams = new List<byte[]>();
    }

    public void Sine (double freq, double dur, int channelNo, int start)
    {
        spawnChannel(channelNo, dur, start);
        Function sine = new Sine(freq, dur, channelNo, SamplingRate, BitDepth);
        padChannels(dur);

        int offset = (int)((double)SamplingRate * start * BitDepth / 8);
        Array.Copy(sine.waveform, 0, ByteStreams[channelNo], offset, sine.waveform.Length);
    }

    private void spawnChannel (int channelNo, double dur, int start)
    {
        int noOfSamples = (int)((double)SamplingRate * (dur + start));
        int  byteLength = noOfSamples * BitDepth / 8;
        
        while (ByteStreams.Count <= channelNo) // Check if channel exists
        {
            ByteStreams.Add(new byte[byteLength]); // Create channels if needed
        }

        if (ByteStreams[channelNo].Length < byteLength) {
            var temp = ByteStreams[channelNo];
            Array.Resize(ref temp, byteLength);
            ByteStreams[channelNo] = temp;
        }
    }

    private void padChannels (double dur)
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
}
