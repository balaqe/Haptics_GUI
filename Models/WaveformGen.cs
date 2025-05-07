using System;
using System.Collections.Generic;
using System.Linq;
using NAudio.Wave;
using Haptics_GUI.Models.Functions;
using Haptics_GUI.Models.Transitions;

namespace Haptics_GUI.Models;

public class WaveformGen
{
    public Format Format;
    public WaveFormat waveFormat;
    
    public List<Bakery> Channels;
    public List<byte[]> ByteStreams;

    public WaveformGen(int inSamplingRate, int inBitDepth, int inChannels)
    {
        Format = new Format(inSamplingRate, inBitDepth, inChannels);
        waveFormat = new WaveFormat(inSamplingRate, inBitDepth, inChannels);
        Channels = new List<Bakery>();
        ByteStreams = new List<byte[]>();
    }
    
    public WaveformGen(int inChannels)
    {
        Format = new Format(inChannels);
        waveFormat = new WaveFormat(Format.SamplingRate, Format.BitDepth, inChannels);
        Channels = new List<Bakery>();
        ByteStreams = new List<byte[]>();
    }

    public void PadChannels()
    {
        if (Channels == null || Channels.Count == 0) return;
        
        var longestChannelLength = Channels.Max(b => b.BakedSamples.Length);
        foreach (var channel in Channels)
        {
            channel.PadSamples(longestChannelLength);
        }
    }

    // Channels start at 0
    public void Sine(int channel, double startTime, double duration, double startFreq, double endFreq)
    {
        if (Channels.Count <= channel)
        {
            Channels.Add(new Bakery(Format));
        }
        Channels[channel].GenerateSine(startTime, duration, startFreq, endFreq);
        PadChannels();
    }
    
    public void Square(int channel, double startTime, double duration, double startFreq, double endFreq)
    {
        if (Channels.Count <= channel)
        {
            Channels.Add(new Bakery(Format));
        }
        Channels[channel].GenerateSquare(startTime, duration, startFreq, endFreq);
        PadChannels();
    }

    public void Sigmoid(int channel, double startTime, double duration, double startVal, double endVal)
    {
        if (Channels.Count <= channel)
        {
            Channels.Add(new Bakery(Format));
        }
        Channels[channel].GenerateSigmoidTransition(startTime, duration, startVal, endVal);
    }



    /*
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
    */

    /*
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
    */
    
    
    
    public void Encode()
    {
        ByteStreams.Clear();
        PadChannels();
        var longestChannelLength = Channels.Max(b => b.BakedSamples.Length);
        foreach (var channel in Channels)
        {
            channel.Bake();
            
            var byteLength = longestChannelLength * (Format.BitDepth / 8);
            ByteStreams.Add(new byte[byteLength]);
            
            for (int i = 0; i < longestChannelLength; i++)
            {
                byte[] valueBytes;
                switch (Format.BitDepth)
                {
                    case 8:
                        valueBytes = [(byte)(channel.BakedSamples[i] * 127 + 128)]; // 8-bit audio is unsigned
                        break;
                
                    case 16:
                        valueBytes = BitConverter.GetBytes((short)(channel.BakedSamples[i] * short.MaxValue));
                        break;
                
                    default:
                        valueBytes = BitConverter.GetBytes((uint)(channel.BakedSamples[i] * uint.MaxValue));
                        break;
                }
            
                var byteIndex = i * (Format.BitDepth / 8);
                for (var j = 0; j < Format.BitDepth/8; j += 1)
                {
                    ByteStreams.Last()[byteIndex + j] = valueBytes[j];
                }
            }
        }
    }
}
