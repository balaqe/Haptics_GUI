using System;

namespace Haptics_GUI.Models.FreqSweepFunctions;

public abstract class FreqSweepFunction
{
    protected double startFreq;
    protected double endFreq;
    
    private readonly double dur;
    //protected double start;
    public double startPhase;
    public double endPhase; // Continuously updated by func
    
    protected int samplingRate;
    private int bitDepth;
    protected int channel;
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
        channel = inChannel;
    }

    public abstract double Func(int numberOfSamples, int i);
    
    public void Generate()
    {
        var noOfSamples = (int)((double)samplingRate * dur);
        var byteLength = noOfSamples * bitDepth / 8;
        
        Waveform = new byte[byteLength];
        
        for (int i = 0; i < noOfSamples; i++)
        {
            double sampleValue = Func(noOfSamples, i);

            byte[] valueBytes;
            switch (bitDepth)
            {
                case 8:
                    valueBytes = [(byte)(sampleValue * 127 + 128)]; // 8-bit audio is unsigned
                    break;
                
                case 16:
                    valueBytes = BitConverter.GetBytes((short)(sampleValue * short.MaxValue));
                    break;
                
                default:
                    valueBytes = BitConverter.GetBytes((uint)(sampleValue * uint.MaxValue));
                    break;
            }
            
            var byteIndex = i * (bitDepth / 8);
            for (var j = 0; j < bitDepth/8; j += 1)
            {
                Waveform[byteIndex + j] = valueBytes[j];
            }
        }
    }
}