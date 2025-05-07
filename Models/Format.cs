using System.Runtime.CompilerServices;
using Avalonia.Data;

namespace Haptics_GUI.Models;

public class Format
{
    public Format(int channels)
    {
        SamplingRate = 44100;
        BitDepth = 16;
        Channels = channels;
    }
    
    public Format(int sampleRate, int bitDepth, int channels)
    {
        SamplingRate = sampleRate;
        BitDepth = bitDepth;
        Channels = channels;
    }

    public int SamplingRate { get; init; }
    public int BitDepth { get; init; }
    public int Channels { get; set; }
}