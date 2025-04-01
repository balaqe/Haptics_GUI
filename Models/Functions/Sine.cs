using System;

namespace Haptics_GUI.Models.Functions;

public class Sine : Function
{
    public Sine(double inFreq, double inDur, uint inChannel, uint inStart, uint inSamplingRate, uint inBitDepth)
    {
        freq = inFreq;
        dur = inDur;
        channel = inChannel;
        start = inStart;
        samplingRate = inSamplingRate;
        bitDepth = inBitDepth;
        Generator();
    }

    public override void Generator()
    {
        uint noOfSamples = (uint)((double)samplingRate * (dur + start));
        uint samplesToStart = (uint)((double)samplingRate * start);
        uint byteLength = noOfSamples * bitDepth / 8;
        // uint bytesToStart = samplesToStart * bitDepth / 8;
        
        waveform = new byte[byteLength];

        for (uint i = samplesToStart; i < noOfSamples; i++)
        {
            double omega = 2*Math.PI*freq/(double)samplingRate;
            double sampleValue = Math.Sin(omega * (i - samplesToStart));

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
            
            uint byteIndex = i * (bitDepth / 8);
            for (int j = 0; j < bitDepth; j += 8)
            {
                waveform[byteIndex + j] = valueBytes[j];
            }
        }
        
    }
}