using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Haptics_GUI.Models;
using System.Runtime.InteropServices;


namespace Haptics_GUI.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{

    [DllImport("Lib/soundPlayer.dll", CallingConvention=System.Runtime.InteropServices.CallingConvention.StdCall)]
    public static extern int play(float[] waveform, int len, int numChannels);

    [ObservableProperty] public string filePath;
    [ObservableProperty] public double duration;
    [ObservableProperty] public double fastDuration;
    [ObservableProperty] public double slowDuration;
    [ObservableProperty] public double fastSmooth;
    [ObservableProperty] public double slowSmooth;
    [ObservableProperty] public double squareAmp;
    [ObservableProperty] public double frequency;
    [ObservableProperty] public int channel1;
    [ObservableProperty] public int channel2;

    public MainWindowViewModel()
    {
        filePath = string.Empty;
        duration = 0.2;
        fastDuration = 0.2;
        slowDuration = 0.5;
        fastSmooth = 0.1;
        slowSmooth = 0.25;
        squareAmp = 0.81;
        frequency = 100;
        channel1 = 5;
        channel2 = 1;
 
    }
    
    
    // Survey 1
    [RelayCommand]
    public void SineStepPlay()
    {
        Reset();

        var waveFormGen = new WaveformGen();
        waveFormGen.Sine(frequency, duration, channel1, 0);
        waveFormGen.Sine(frequency, duration, channel2, 0);

        PlayWave(waveFormGen);
    }

    [RelayCommand]
    public void SineStepStop()
    {
        // if (FunctionDictionary.ContainsKey("SineStep"))
        // {
        //     FunctionDictionary["SineStep"].Dispose();
        // }
    }
    
    
    [RelayCommand]
    public void SineLinearFastPlay()
    {
        Reset();

        var waveFormGen = new WaveformGen();
        waveFormGen.Sine(frequency, fastDuration, channel1, 0);
        waveFormGen.LinearTrans(channel1, 0, fastSmooth, 0, 1); // int channelNo, double startTime, double endTime, double startVal, double endVal
        waveFormGen.LinearTrans(channel1, (fastDuration-fastSmooth), fastDuration, 1, 0);
        waveFormGen.Sine(frequency, fastDuration, channel2, 0);
        waveFormGen.LinearTrans(channel2, 0, fastSmooth, 0, 1); // int channelNo, double startTime, double endTime, double startVal, double endVal
        waveFormGen.LinearTrans(channel2, (fastDuration-fastSmooth), fastDuration, 1, 0);

        PlayWave(waveFormGen);

        AutoExport(waveFormGen, channel1, "SineLinearFast.csv");

        // FunctionDictionary.Add("SineLinearFast", new ByteStream(waveFormGen.ByteStreams, waveFormGen.waveFormat)); 
        // FunctionDictionary["SineLinearFast"].Play();

        // string projectDir = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..");
        // string exportDir = Path.Combine(projectDir, "exported_audio");
        // string filePath = Path.Combine(exportDir, "SineLinearFast.wav");

        byte[] exportStream = GenAudio(100, (decimal)fastDuration, waveFormGen.ByteStreams[channel1]);
        // WriteFile(exportStream, "@Signal1.wav");

    }
    
    [RelayCommand]
    public void SineLinearFastStop()
    {
        // if (FunctionDictionary.ContainsKey("SineLinearFast"))
        // {
        //     FunctionDictionary["SineLinearFast"].Dispose();
        // }
    }
    
    
    [RelayCommand]
    public void SineLinearSlowPlay()
    {
        Reset();

        var waveFormGen = new WaveformGen();
        waveFormGen.Sine(frequency, slowDuration, channel1, 0);
        waveFormGen.LinearTrans(channel1, 0, slowSmooth, 0, 1); // int channelNo, double startTime, double endTime, double startVal, double endVal
        waveFormGen.LinearTrans(channel1, (slowDuration-slowSmooth), slowDuration, 1, 0);
        waveFormGen.Sine(frequency, slowDuration, channel2, 0);
        waveFormGen.LinearTrans(channel2, 0, slowSmooth, 0, 1); // int channelNo, double startTime, double endTime, double startVal, double endVal
        waveFormGen.LinearTrans(channel2, (slowDuration-slowSmooth), slowDuration, 1, 0);

        PlayWave(waveFormGen);

        AutoExport(waveFormGen, channel1, "SineLinearSlow.csv");

        // FunctionDictionary.Add("SineLinearSlow", new ByteStream(waveFormGen.ByteStreams, waveFormGen.waveFormat)); 
        // FunctionDictionary["SineLinearSlow"].Play();

        byte[] exportStream = GenAudio(100, (decimal)slowDuration, waveFormGen.ByteStreams[channel1]);
        // WriteFile(exportStream, "@Signal2.wav");
    }

    [RelayCommand]
    public void SineLinearSlowStop()
    {
        // if (FunctionDictionary.ContainsKey("SineLinearSlow"))
        // {
        //     FunctionDictionary["SineLinearSlow"].Dispose();
        // }
        
    }
    
    [RelayCommand]
    public void SineSmoothFastPlay()
    {
        Reset();

        var waveFormGen = new WaveformGen();
        waveFormGen.Sine(frequency, fastDuration, channel1, 0);
        waveFormGen.SigmoidTrans(channel1, 0, fastSmooth, 0, 1); // int channelNo, double startTime, double endTime, double startVal, double endVal
        waveFormGen.SigmoidTrans(channel1, (fastDuration-fastSmooth), fastDuration, 1, 0);
        waveFormGen.Sine(frequency, fastDuration, channel2, 0);
        waveFormGen.SigmoidTrans(channel2, 0, fastSmooth, 0, 1); // int channelNo, double startTime, double endTime, double startVal, double endVal
        waveFormGen.SigmoidTrans(channel2, (fastDuration-fastSmooth), fastDuration, 1, 0);

        PlayWave(waveFormGen);

        // FunctionDictionary.Add("SineSmoothFast", new ByteStream(waveFormGen.ByteStreams, waveFormGen.waveFormat)); 
        // FunctionDictionary["SineSmoothFast"].Play();
    }
    
    [RelayCommand]
    public void SineSmoothFastStop()
    {
        // if (FunctionDictionary.ContainsKey("SineSmoothFast"))
        // {
        //     FunctionDictionary["SineSmoothFast"].Dispose();
        // }
    }
    
    
    [RelayCommand]
    public void SineSmoothSlowPlay()
    {
        Reset();

        var waveFormGen = new WaveformGen();
        // var waveFormGen = new WaveformGen(44100, 32);
        waveFormGen.Sine(frequency, slowDuration, channel1, 0);
        waveFormGen.SigmoidTrans(channel1, 0, slowSmooth, 0, 1); // int channelNo, double startTime, double endTime, double startVal, double endVal
        waveFormGen.SigmoidTrans(channel1, (slowDuration-slowSmooth), slowDuration, 1, 0);
        waveFormGen.Sine(frequency, slowDuration, channel2, 0);
        waveFormGen.SigmoidTrans(channel2, 0, slowSmooth, 0, 1); // int channelNo, double startTime, double endTime, double startVal, double endVal
        waveFormGen.SigmoidTrans(channel2, (slowDuration-slowSmooth), slowDuration, 1, 0);

        PlayWave(waveFormGen);
    }
    
    [RelayCommand]
    public void SineSmoothSlowStop()
    {
        // if (FunctionDictionary.ContainsKey("SineSmoothSlow"))
        // {
        //     FunctionDictionary["SineSmoothSlow"].Dispose();
        // }
    }
    
    
    
    
    
    [RelayCommand]
    public void SquareStepPlay()
    {
        Reset();

        var waveFormGen = new WaveformGen();
        waveFormGen.Square(frequency, duration, channel1, 0);
        waveFormGen.Square(frequency, duration, channel2, 0);
        waveFormGen.LinearTrans(channel1, 0, duration, squareAmp, squareAmp);
        waveFormGen.LinearTrans(channel2, 0, duration, squareAmp, squareAmp);

        PlayWave(waveFormGen);
    }
    
    [RelayCommand]
    public void SquareStepStop()
    {
        // if (FunctionDictionary.ContainsKey("SquareStep"))
        // {
        //     FunctionDictionary["SquareStep"].Dispose();
        // }
    }
    
    
    [RelayCommand]
    public void SquareLinearFastPlay()
    {
        Reset();

        var waveFormGen = new WaveformGen();
        waveFormGen.Square(frequency, fastDuration, channel1, 0);
        waveFormGen.LinearTrans(channel1, 0, fastSmooth, 0, 1); // int channelNo, double startTime, double endTime, double startVal, double endVal
        waveFormGen.LinearTrans(channel1, (fastDuration-fastSmooth), fastDuration, 1, 0);
        waveFormGen.Square(frequency, fastDuration, channel2, 0);
        waveFormGen.LinearTrans(channel2, 0, fastSmooth, 0, 1); // int channelNo, double startTime, double endTime, double startVal, double endVal
        waveFormGen.LinearTrans(channel2, (fastDuration-fastSmooth), fastDuration, 1, 0);
        waveFormGen.LinearTrans(channel1, 0, duration, squareAmp, squareAmp);
        waveFormGen.LinearTrans(channel2, 0, duration, squareAmp, squareAmp);

        PlayWave(waveFormGen);
    }
    
    [RelayCommand]
    public void SquareLinearFastStop()
    {
        // if (FunctionDictionary.ContainsKey("SquareLinearFast"))
        // {
        //     FunctionDictionary["SquareLinearFast"].Dispose();
        // }
    }
    
    
    [RelayCommand]
    public void SquareLinearSlowPlay()
    {
        Reset();

        var waveFormGen = new WaveformGen();
        waveFormGen.Square(frequency, slowDuration, channel1, 0);
        waveFormGen.LinearTrans(channel1, 0, slowSmooth, 0, 1); // int channelNo, double startTime, double endTime, double startVal, double endVal
        waveFormGen.LinearTrans(channel1, (slowDuration-slowSmooth), slowDuration, 1, 0);
        waveFormGen.Square(frequency, slowDuration, channel2, 0);
        waveFormGen.LinearTrans(channel2, 0, slowSmooth, 0, 1); // int channelNo, double startTime, double endTime, double startVal, double endVal
        waveFormGen.LinearTrans(channel2, (slowDuration-slowSmooth), slowDuration, 1, 0);
        waveFormGen.LinearTrans(channel1, 0, duration, squareAmp, squareAmp);
        waveFormGen.LinearTrans(channel2, 0, duration, squareAmp, squareAmp);

        PlayWave(waveFormGen);
    }
    
    [RelayCommand]
    public void SquareLinearSlowStop()
    {
        // if (FunctionDictionary.ContainsKey("SquareLinearSlow"))
        // {
        //     FunctionDictionary["SquareLinearSlow"].Dispose();
        // }
    }
    
    
    
    [RelayCommand]
    public void SquareSmoothFastPlay()
    {
        Reset();

        var waveFormGen = new WaveformGen();
        waveFormGen.Square(frequency, fastDuration, channel1, 0);
        waveFormGen.SigmoidTrans(channel1, 0, fastSmooth, 0, 1); // int channelNo, double startTime, double endTime, double startVal, double endVal
        waveFormGen.SigmoidTrans(channel1, (fastDuration-fastSmooth), fastDuration, 1, 0);
        waveFormGen.Square(frequency, fastDuration, channel2, 0);
        waveFormGen.SigmoidTrans(channel2, 0, fastSmooth, 0, 1); // int channelNo, double startTime, double endTime, double startVal, double endVal
        waveFormGen.SigmoidTrans(channel2, (fastDuration-fastSmooth), fastDuration, 1, 0);
        waveFormGen.LinearTrans(channel1, 0, duration, squareAmp, squareAmp);
        waveFormGen.LinearTrans(channel2, 0, duration, squareAmp, squareAmp);

        PlayWave(waveFormGen);
    }
    
    [RelayCommand]
    public void SquareSmoothFastStop()
    {
        // if (FunctionDictionary.ContainsKey("SquareSmoothFast"))
        // {
        //     FunctionDictionary["SquareSmoothFast"].Dispose();
        // }
    }
    
    
    [RelayCommand]
    public void SquareSmoothSlowPlay()
    {
        Reset();

        var waveFormGen = new WaveformGen();
        waveFormGen.Square(frequency, slowDuration, channel1, 0);
        waveFormGen.SigmoidTrans(channel1, 0, slowSmooth, 0, 1); // int channelNo, double startTime, double endTime, double startVal, double endVal
        waveFormGen.SigmoidTrans(channel1, (slowDuration-slowSmooth), slowDuration, 1, 0);
        waveFormGen.Square(frequency, slowDuration, channel2, 0);
        waveFormGen.SigmoidTrans(channel2, 0, slowSmooth, 0, 1); // int channelNo, double startTime, double endTime, double startVal, double endVal
        waveFormGen.SigmoidTrans(channel2, (slowDuration-slowSmooth), slowDuration, 1, 0);
        waveFormGen.LinearTrans(channel1, 0, duration, squareAmp, squareAmp);
        waveFormGen.LinearTrans(channel2, 0, duration, squareAmp, squareAmp);

        PlayWave(waveFormGen);
    }
    
    [RelayCommand]
    public void SquareSmoothSlowStop()
    {
        // if (FunctionDictionary.ContainsKey("SquareSmoothFlow"))
        // {
        //     FunctionDictionary["SquareSmoothSlow"].Dispose();
        // }
    }

    private void Reset()
    {
        // foreach (KeyValuePair<string, ByteStream> entry in FunctionDictionary)
        // {
        //     entry.Value.Dispose();
        // }

        // if (FunctionDictionary.Count > 0) FunctionDictionary.Clear();
    }

    private void AutoExport(WaveformGen waveFormGen, int channel, string fileName)
    {
        short[] waveform = new short[waveFormGen.ByteStreams[channel].Length / 2];
        for (int i = 0; i < waveform.Length; i++)
        {
            // Adjust the byte ordering if needed (endianness)
            waveform[i] = BitConverter.ToInt16(waveFormGen.ByteStreams[channel], i * 2);
        }

        // string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        // string folderPath = AppContext.BaseDirectory;
        // string projectDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", ".."));
        string projectDir = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..");
        string filePath = Path.Combine(projectDir, "debug_plot", fileName);

        using (StreamWriter file = new StreamWriter(filePath))
        {
            for (int i=0; i<waveform.Length; i++)
            {
                short item = waveform[i];
                file.Write(i.ToString() + "," + item.ToString() + "\n");
            }
        }
    }

    private static byte[] GenAudio(double freq, decimal duration, byte[] data) {
        int sampleRate = 44100; // Hz
        // decimal duration = 1; // sec
        int numberOfSamples = (int)((decimal)sampleRate * duration);
        int fileSize = numberOfSamples * 2 + 44; // 16-bit samples, add header size

        byte[] header = GenHeader(fileSize, sampleRate, numberOfSamples);
        // byte[] data = GenData(duration, freq, sampleRate);

        // Combine header and data
        byte[] retBuff = new byte[fileSize];
        Buffer.BlockCopy(header, 0, retBuff, 0, 44);
        Buffer.BlockCopy(data, 0, retBuff, 44, data.Length);


        return retBuff;
    }

    private static byte[] GenHeader (int fileSize, int sampleRate, int numberOfSamples) {

        byte[] header = new byte[44];
        
        // RIFF
        byte[] Riff = Encoding.ASCII.GetBytes("RIFF");
        Buffer.BlockCopy(Riff, 0, header, 0, 4);

        // File size (total file size minus 8 bytes for RIFF and size)
        byte[] fileSizeBytes = BitConverter.GetBytes(fileSize - 8);
        Buffer.BlockCopy(fileSizeBytes, 0, header, 4, 4);

        // WAVE
        byte[] Wave = Encoding.ASCII.GetBytes("WAVE");
        Buffer.BlockCopy(Wave, 0, header, 8, 4);

        // fmt + null
        byte[] Fmt = Encoding.ASCII.GetBytes("fmt ");
        Buffer.BlockCopy(Fmt, 0, header, 12, 4);
        
        // Length of format data
        header[16] = 16;
        
        // Type of format (1 is PCM) - 2 byte integer
        header[20] = 1;

        // Number of channels
        header[22] = 1;
        
        // Sample rate (44100 in little-endian)
        byte[] sampleRateBytes = BitConverter.GetBytes(sampleRate);
        Buffer.BlockCopy(sampleRateBytes, 0, header, 24, 4);

        // Byte rate ((Sample Rate * BitsPerSample * Channels) / 8)
        int byteRate = sampleRate * 2 * 1; // 16-bit, mono
        byte[] byteRateBytes = BitConverter.GetBytes(byteRate);
        Buffer.BlockCopy(byteRateBytes, 0, header, 28, 4);

        // Block align ((BitsPerSample * Channels) / 8)
        header[32] = 2;

        // Bits per sample
        header[34] = 16;

        // Data chunk header
        byte[] DataString = Encoding.ASCII.GetBytes("data");
        Buffer.BlockCopy(DataString, 0, header, 36, 4);

        // Data size
        byte[] dataSizeBytes = BitConverter.GetBytes(numberOfSamples * 2);
        Buffer.BlockCopy(dataSizeBytes, 0, header, 40, 4);

        return header;
    }

    private static byte[] GenData (decimal duration, double freq, int sampleRate) {
        int numberOfSamples = sampleRate * (int)duration;
        byte[] data = new byte[numberOfSamples * 2];

        for (int i = 0; i < numberOfSamples; i++) {
            double omega = 2 * Math.PI * freq / sampleRate;
            short value = (short)(Math.Sin(omega * i) * short.MaxValue); // Using sin for a pure tone
            byte[] valueBytes = BitConverter.GetBytes(value);
            data[i * 2] = valueBytes[0];
            data[i * 2 + 1] = valueBytes[1];
        }

        return data;
    }

    private static void WriteFile(byte[] buff, string filePath) {
        using (FileStream fsNew = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            fsNew.Write(buff, 0, buff.Length);
        } 
    }

    private void PlayWave(WaveformGen waveFormGen)
    {
        int bitDepthDivider = (int)waveFormGen.BitDepth / 8;
        int len = waveFormGen.ByteStreams[0].Length;
        int sampleCount = len / bitDepthDivider;
        int waveCount = waveFormGen.ByteStreams.Count;
        float[] transferBuff = new float[sampleCount*waveCount];

        int normalMax;
        switch (waveFormGen.BitDepth)
        {
            case 8:
                normalMax = Byte.MaxValue/2;
                break;
            case 16:
                normalMax = Int16.MaxValue;
                break;
            default: // 32 bit
                normalMax = Int32.MaxValue;
                break;
        }

        int writtenSamples = 0;
        for (int i=0; i<waveCount; i++)
        {
            for (int j=0; j<sampleCount; j++)
            {
                switch (waveFormGen.BitDepth)
                {
                    case 8:
                        var sample8 = BitConverter.ToChar(waveFormGen.ByteStreams[i], j * bitDepthDivider);
                        transferBuff[writtenSamples] = sample8 / (float)normalMax;
                        break;
                    case 16:
                        var sample16 = BitConverter.ToInt16(waveFormGen.ByteStreams[i], j * bitDepthDivider);
                        transferBuff[writtenSamples] = sample16 / (float)normalMax;

                        break;
                    default: // 32 bit
                        var sample32 = BitConverter.ToInt32(waveFormGen.ByteStreams[i], j * bitDepthDivider);
                        // transferBuff[j][i] = sample32 / (float)normalMax;
                        transferBuff[writtenSamples] = sample32 / (float)normalMax;
                        break;
                }

                writtenSamples++;
            }
        }

        play(transferBuff, sampleCount, waveCount);
    }
}
