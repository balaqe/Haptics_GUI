using System;

namespace Haptics_GUI.Models.Functions;

public abstract class Function
{
    public double freq;
    public double dur;
    public int channel;
    // public int start;
    public int samplingRate;
    public int bitDepth;
    public byte[] waveform;
    public abstract double Func(double inFreq, int inSamplingRate, int i);

    public Function(double inFreq, double inDur, int inChannel, int inSamplingRate, int inBitDepth)
    {
        freq = inFreq;
        dur = inDur;
        channel = inChannel;
        samplingRate = inSamplingRate;
        bitDepth = inBitDepth;
        // Generator();
    }

    public void Generator()
    {
        int noOfSamples = (int)((double)samplingRate * dur);
        int byteLength = noOfSamples * bitDepth / 8;
        
        waveform = new byte[byteLength];

        for (int i = 0; i < noOfSamples; i++)
        {
            double sampleValue = Func(freq, samplingRate, i);

            byte[] valueBytes;
            switch (bitDepth)
            {
                case 8:
                    valueBytes = new byte[] { (byte)(sampleValue * 127 + 128) }; // 8-bit audio is unsigned
                    break;
                
                case 16:
                    valueBytes = BitConverter.GetBytes((short)(sampleValue * short.MaxValue));
                    break;
                
                default:
                    valueBytes = BitConverter.GetBytes((uint)(sampleValue * uint.MaxValue));
                    break;
            }
            
            int byteIndex = i * (bitDepth / 8);
            for (int j = 0; j < bitDepth/8; j += 1)
            {
                waveform[byteIndex + j] = valueBytes[j];
            }
        }
    }
}