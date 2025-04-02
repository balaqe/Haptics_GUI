using System;
using System.Collections;
using System.Collections.Generic;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using Haptics_GUI.Models.Functions;
using Haptics_GUI.Models.Transitions;

namespace Haptics_GUI.Models;

public class WaveformGen
{
    public int SamplingRate;
    public byte BitDepth;
    public List<byte[]> ByteStreams;
    public WaveFormat waveFormat;
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
        sine.Generator();
        padChannels(dur);

        int offset = (int)((double)SamplingRate * start * BitDepth / 8);
        Array.Copy(sine.waveform, 0, ByteStreams[channelNo], offset, sine.waveform.Length);
    }

    public void Square (double freq, double dur, int channelNo, int start)
    {
        spawnChannel(channelNo, dur, start);
        Function square = new Square(freq, dur, channelNo, SamplingRate, BitDepth);
        square.Generator();
        padChannels(dur);

        int offset = (int)((double)SamplingRate * start * BitDepth / 8);
        Array.Copy(square.waveform, 0, ByteStreams[channelNo], offset, square.waveform.Length);
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

    private void spawnChannel (int channelNo, double dur, int start)
    {
        int noOfSamples = (int)((double)SamplingRate * (dur + start));
        int  byteLength = noOfSamples * BitDepth / 8;
        
        while (ByteStreams.Count <= channelNo) // Check if channel exists
        {
            ByteStreams.Add(new byte[byteLength]); // Create channels if needed
            waveFormat = new WaveFormat(SamplingRate, BitDepth, ByteStreams.Count);
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
