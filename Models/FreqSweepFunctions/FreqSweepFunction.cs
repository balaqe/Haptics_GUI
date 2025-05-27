using System;

namespace Haptics_GUI.Models.FreqSweepFunctions;

public abstract class FreqSweepFunction
{
    /*
    protected double startFreq;
    protected double endFreq;
    
    private readonly double dur;
    public double startPhase;
    public double endPhase; // Continuously updated by func
    
    protected int samplingRate;
    private int bitDepth;
    protected int sampleCount;

    public double[] rawSamples;
    public byte[] Waveform;

    
    protected FreqSweepFunction(double inStartFreq, double inEndFreq, double inDur, double inStartPhase,
        int inSamplingRate, int inBitDepth, int inChannel)
    {
        startFreq = inStartFreq;
        endFreq = inEndFreq;
        dur = inDur;
        //start = inStart;
        startPhase = inStartPhase;
        endPhase = 0;
        samplingRate = inSamplingRate;
        bitDepth = inBitDepth;
        sampleCount = (int)((double)samplingRate * dur);
        
        rawSamples = new double[sampleCount];
    }

    public abstract double Func(int i);

    public void Encode()
    {
        var byteLength = sampleCount * bitDepth / 8;

        Waveform = new byte[byteLength];
        
        for (int i = 0; i < sampleCount; i++)
        {
            byte[] valueBytes;
            switch (bitDepth)
            {
                case 8:
                    valueBytes = [(byte)(rawSamples[i] * 127 + 128)]; // 8-bit audio is unsigned
                    break;
                
                case 16:
                    valueBytes = BitConverter.GetBytes((short)(rawSamples[i] * short.MaxValue));
                    break;
                
                default:
                    valueBytes = BitConverter.GetBytes((uint)(rawSamples[i] * uint.MaxValue));
                    break;
            }
            
            var byteIndex = i * (bitDepth / 8);
            for (var j = 0; j < bitDepth/8; j += 1)
            {
                Waveform[byteIndex + j] = valueBytes[j];
            }
        }
    }
    
    public void Generate()
    {
        for (int i = 0; i < sampleCount; i++)
        {
            rawSamples[i] = Func(i);
        }
        
        Encode();
    }
    */
}