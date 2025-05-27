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

    public void SpawnChannels(int channels)
    {
        while (Channels.Count <= channels)
        {
            Channels.Add(new Bakery(Format));
        }
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
        SpawnChannels(channel);
        Channels[channel].GenerateSine(startTime, duration, startFreq, endFreq);
        PadChannels();
    }
    
    public void Square(int channel, double startTime, double duration, double startFreq, double endFreq)
    {
        SpawnChannels(channel);
        Channels[channel].GenerateSquare(startTime, duration, startFreq, endFreq);
        PadChannels();
    }

    public void Sigmoid(int channel, double startTime, double duration, double startVal, double endVal)
    {
        SpawnChannels(channel);
        Channels[channel].GenerateSigmoidTransition(startTime, duration, startVal, endVal);
    }
    
    public void Linear(int channel, double startTime, double duration, double startVal, double endVal)
    {
        SpawnChannels(channel);
        Channels[channel].GenerateLinearTransition(startTime, duration, startVal, endVal);
    }

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
