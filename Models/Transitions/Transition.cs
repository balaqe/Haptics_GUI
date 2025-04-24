using System;

namespace Haptics_GUI.Models.Transitions;

public abstract class Transition
{
    public byte[] resultData;
    private byte[] data;
    public double startTime;
    public double endTime;
    public double startVal;
    public double endVal;
    public double dur;
    public int bitDepth;
    public int samplesPerSecond;
    public int startSample;
    public int endSample;

    public abstract double Func(int i);

    public Transition(byte[] inData, double inStartTime, double inEndTime, double inStartVal, double inEndVal, int inBitDepth, int inSampRate)
    {
        data = new byte[inData.Length];
        data = inData;
        startTime = inStartTime;
        endTime = inEndTime;
        startVal = inStartVal;
        endVal = inEndVal;
        bitDepth = inBitDepth;
        dur = endTime - startTime;
        samplesPerSecond = inSampRate;
        startSample = (int)(startTime * samplesPerSecond);
        endSample = (int)(endTime * samplesPerSecond);

        // Generator();
    }

    public void Generator()
    {
        resultData = new byte[data.Length];
        Array.Copy(data, resultData, data.Length);

        int bytesPerSample = bitDepth / 8;
        int startSample = (int)(startTime * samplesPerSecond);
        int endSample = (int)(endTime * samplesPerSecond);

        for (int i=startSample; i<endSample; i++)
        {
            int sampleValue;
            byte[] valueBytes;
            int byteOffset = i * bytesPerSample;

            switch (bitDepth)
            {
                case 8:
                    sampleValue = (int)(Func(i) * data[byteOffset]);
                    valueBytes = new byte[] { (byte)sampleValue}; // 8-bit audio is unsigned
                    break;
                
                case 16:
                    sampleValue = BitConverter.ToInt16(data, byteOffset);
                    sampleValue = (int)(Func(i) * sampleValue);
                    valueBytes = BitConverter.GetBytes((short)sampleValue);
                    break;
                
                default: // 32 bit
                    sampleValue = BitConverter.ToInt32(data, byteOffset);
                    sampleValue = (int)(Func(i) * sampleValue);
                    valueBytes = BitConverter.GetBytes((uint)sampleValue);
                    break;
            }

            for (int j = 0; j < bitDepth/8; j += 1)
            {
                resultData[byteOffset + j] = valueBytes[j];
            }

        }
      
    }
}