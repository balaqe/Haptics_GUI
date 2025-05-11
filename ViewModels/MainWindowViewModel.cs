using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Haptics_GUI.Models;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace Haptics_GUI.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] public string filePath;
    [ObservableProperty] public int channel1;
    [ObservableProperty] public int channel2;

    [ObservableProperty] public int pulseCount;
    [ObservableProperty] public bool count1;
    [ObservableProperty] public bool count2;
    [ObservableProperty] public bool count3;
    [ObservableProperty] public bool customCount;

    [ObservableProperty] public int frequency;
    [ObservableProperty] public bool freq1;
    [ObservableProperty] public bool freq2;
    [ObservableProperty] public bool freq3;
    [ObservableProperty] public bool customFreq;

    [ObservableProperty] public double pulseDelay;
    [ObservableProperty] public bool pulseDelay1;
    [ObservableProperty] public bool pulseDelay2;
    [ObservableProperty] public bool pulseDelay3;

    [ObservableProperty] public bool sweep1;
    [ObservableProperty] public bool sweep2;
    [ObservableProperty] public bool sweep3;
    [ObservableProperty] public bool sweep4;

    [ObservableProperty] public bool phase1;
    [ObservableProperty] public bool phase2;
    [ObservableProperty] public bool phase3;
    [ObservableProperty] public bool phase4;
    [ObservableProperty] public bool phase5;
    [ObservableProperty] public double leftPhaseOffset;
    [ObservableProperty] public double rightPhaseOffset;


    private AudioFileReader audioFile;
    private WasapiOut audioOut;

    private ByteStream? byteStream;
    
    // private IDictionary<string, ByteStream> FunctionDictionary;
    private ByteStream streamer;

    private double refreshingDur;
    private double alarmingDur;
    

    [RelayCommand]
    public void PlayGenFile()
    {
        byteStream.Play();
    }

    [RelayCommand]
    public void StopGenFile()
    {
        byteStream.Stop();
    }


    [RelayCommand]
    public void PlayFile()
    {
        audioFile = new AudioFileReader(filePath);
        
        audioOut = new WasapiOut(AudioClientShareMode.Exclusive, false, 100);
        
        audioOut.Init(audioFile);
        audioOut.Play();
    }

    [RelayCommand]
    public void StopFile()
    {
        audioOut.Stop();
        audioOut.Dispose();
    }
    public MainWindowViewModel()
    {
        filePath = string.Empty;
        frequency = 100;
        alarmingDur = 0.32; // Results in full periods (original: 0.2)
        refreshingDur = 0.4; // Results in full periods (original: 0.5)
        count1 = true;
        count2 = false;
        count3 = false;
        channel1 = 0;
        channel2 = 1;
        pulseDelay = 0.2;
        // duration = 0.2;
        // fastDuration = 0.2;
        // slowDuration = 0.5;
        // fastSmooth = 0.1;
        // slowSmooth = 0.25;
        // squareAmp = 0.81;
        
        // FunctionDictionary = new Dictionary<string, ByteStream>();


    }
    
    
    // Survey 1
    [RelayCommand]
    public void SineStepPlay()
    {
        // streamer.Dispose();
        // FunctionDictionary.Add("SweepingSineStep", new ByteStream(waveFormGen.ByteStreams, waveFormGen.WaveFormat)); 
        // FunctionDictionary["SweepingSineStep"].Play();
    }

    [RelayCommand]
    public void FindAlarmCount()
    {
        if (!customFreq)
        {
            if (count1) // 1
            {
                pulseCount = 1;
            }
            else if (count2) // 3
            {
                pulseCount = 3;
            }
            else // 5
            {
                pulseCount = 5;
            }
        }

        // Console.WriteLine("Pulse count: " + pulseCount);

        Reset();

        //var foo = new WaveformGen(44100, 16, 2);
        var waveFormGen = new WaveformGen(44100, 16, 2);

        frequency = 100;

        for (int i=0; i<pulseCount; i++)
        {
            /*
            foo.Sine(0, 0, 1, 100, 100);
            foo.Linear(0, 0, 1, 1, 0);
            foo.Sine(1, 1, 1, 200, 300);
            foo.Linear(1, 1, 1, 0.5, 1);
            //foo.Encode();
            */
            waveFormGen.Sine(channel1, (alarmingDur + pulseDelay)*i, alarmingDur, frequency, frequency);
            waveFormGen.Sine(channel2, (alarmingDur + pulseDelay)*i, alarmingDur, frequency, frequency);
        }
        waveFormGen.Encode();

        streamer = new ByteStream(waveFormGen.ByteStreams, waveFormGen.waveFormat); 
        streamer.Play();
    }

    [RelayCommand]
    public void FindFreq()
    {
        if (!customFreq)
        {
            if (freq1) // 100 Hz
            {
                frequency = 100;
            }
            else if (freq2) // 125 Hz
            {
                frequency = 125;
            }
            else // freq3 // 75 Hz
            {
                frequency = 75;
            }
        }

        // Console.WriteLine("Freq: " + frequency);

        Reset();
        
        var waveFormGen = new WaveformGen(44100, 16, 2);
        for (int i=0; i<pulseCount; i++)
        {
            waveFormGen.Sine(channel1, (alarmingDur + pulseDelay)*i, alarmingDur, frequency, frequency);
            waveFormGen.Sine(channel2, (alarmingDur + pulseDelay)*i, alarmingDur, frequency, frequency);
        }

        waveFormGen.Encode();
        streamer = new ByteStream(waveFormGen.ByteStreams, waveFormGen.waveFormat); 
        streamer.Play();

        /*
        var waveFormGen = new FreqSweepWaveformGen(44100, 16);
        for (int i=0; i<pulseCount; i++)
        {
            waveFormGen.Sine(frequency, frequency, alarmingDur, channel1, (alarmingDur + pulseDelay)*i);
            waveFormGen.Sine(frequency, frequency, alarmingDur, channel2, (alarmingDur + pulseDelay)*i);
        }
        streamer = new ByteStream(waveFormGen.ByteStreams, waveFormGen.WaveFormat); 
        streamer.Play();
        */
    }

    [RelayCommand]
    public void FindPulseDelay()
    {
        if (pulseDelay1)
        {
            pulseDelay = 0.2;
        }
        else if (pulseDelay2)
        {
            pulseDelay = 0.3;
        }
        else
        {
            pulseDelay = 0.1;
        }

        // Console.WriteLine("Freq: " + frequency);

        Reset();
        
        
        var waveFormGen = new WaveformGen(44100, 16, 2);
        for (int i=0; i<pulseCount; i++)
        {
            waveFormGen.Sine(channel1, (alarmingDur + pulseDelay)*i, alarmingDur, frequency, frequency);
            waveFormGen.Sine(channel2, (alarmingDur + pulseDelay)*i, alarmingDur, frequency, frequency);
        }

        waveFormGen.Encode();
        streamer = new ByteStream(waveFormGen.ByteStreams, waveFormGen.waveFormat); 
        streamer.Play();

        /*
        var waveFormGen = new FreqSweepWaveformGen(44100, 16);
        for (int i=0; i<pulseCount; i++)
        {
            waveFormGen.Sine(frequency, frequency, alarmingDur, channel1, (alarmingDur + pulseDelay)*i);
            waveFormGen.Sine(frequency, frequency, alarmingDur, channel2, (alarmingDur + pulseDelay)*i);
        }
        streamer = new ByteStream(waveFormGen.ByteStreams, waveFormGen.WaveFormat); 
        streamer.Play();
        */
    }
    
    [RelayCommand]
    public void FindSweep()
    {

        Reset();
        var waveFormGen = new WaveformGen(44100, 16, 2);
        for (int i=0; i<pulseCount; i++)
        {
            if (sweep1)
            {
                waveFormGen.Sine(channel1, (alarmingDur + pulseDelay)*i, alarmingDur, frequency-25, frequency);
                waveFormGen.Sine(channel2, (alarmingDur + pulseDelay)*i, alarmingDur, frequency-25, frequency);
            }
            else if (sweep2)
            {
                waveFormGen.Sine(channel1, (alarmingDur + pulseDelay)*i, alarmingDur, frequency+25, frequency);
                waveFormGen.Sine(channel2, (alarmingDur + pulseDelay)*i, alarmingDur, frequency+25, frequency);
            }
            else if (sweep3)
            {
                waveFormGen.Sine(channel1, (alarmingDur + pulseDelay)*i, alarmingDur, frequency, frequency-25);
                //waveFormGen.Sine(channel1, (alarmingDur + pulseDelay)*i + alarmingDur/2, alarmingDur/2, frequency, frequency-25);
                // waveFormGen.Sine(frequency, frequency-25, alarmingDur/2, channel1, (alarmingDur + pulseDelay)*i);
                // waveFormGen.Sine(frequency, frequency, alarmingDur/2, channel1, (alarmingDur + pulseDelay)*i + alarmingDur/2);

                waveFormGen.Sine(channel2, (alarmingDur + pulseDelay)*i, alarmingDur, frequency, frequency-25);
                //waveFormGen.Sine(channel2, (alarmingDur + pulseDelay)*i + alarmingDur/2, alarmingDur/2, frequency, frequency-25);
                // waveFormGen.Sine(frequency, frequency-25, alarmingDur/2, channel2, (alarmingDur + pulseDelay)*i);
                // waveFormGen.Sine(frequency, frequency, alarmingDur/2, channel2, (alarmingDur + pulseDelay)*i + alarmingDur/2);
            }
            else
            {
                waveFormGen.Sine(channel1, (alarmingDur + pulseDelay)*i, alarmingDur, frequency, frequency+25);
                //waveFormGen.Sine(channel1, (alarmingDur + pulseDelay)*i + alarmingDur/2, alarmingDur/2, frequency, frequency+25);
                // waveFormGen.Sine(frequency, frequency+25, alarmingDur/2, channel1, (alarmingDur + pulseDelay)*i);
                // waveFormGen.Sine(frequency, frequency, alarmingDur/2, channel1, (alarmingDur + pulseDelay)*i + alarmingDur/2);

                waveFormGen.Sine(channel2, (alarmingDur + pulseDelay)*i, alarmingDur, frequency, frequency+25);
                //waveFormGen.Sine(channel2, (alarmingDur + pulseDelay)*i + alarmingDur/2, alarmingDur/2, frequency, frequency+25);
                // waveFormGen.Sine(frequency, frequency+25, alarmingDur/2, channel2, (alarmingDur + pulseDelay)*i);
                // waveFormGen.Sine(frequency, frequency, alarmingDur/2, channel2, (alarmingDur + pulseDelay)*i + alarmingDur/2);
            }
        }

        waveFormGen.Encode();
        streamer = new ByteStream(waveFormGen.ByteStreams, waveFormGen.waveFormat); 
        streamer.Play();

        /*
        var waveFormGen = new FreqSweepWaveformGen(44100, 16);
        for (int i=0; i<pulseCount; i++)
        {
            if (sweep1)
            {
                waveFormGen.Sine(frequency-25, frequency, alarmingDur, channel1, (alarmingDur + pulseDelay)*i);
                // waveFormGen.Sine(frequency, frequency, alarmingDur/2, channel1, (alarmingDur + pulseDelay)*i + alarmingDur/2);

                waveFormGen.Sine(frequency-25, frequency, alarmingDur, channel2, (alarmingDur + pulseDelay)*i);
                // waveFormGen.Sine(frequency, frequency, alarmingDur/2, channel2, (alarmingDur + pulseDelay)*i + alarmingDur/2);
            }
            else if (sweep2)
            {
                waveFormGen.Sine(frequency+25, frequency, alarmingDur, channel1, (alarmingDur + pulseDelay)*i);
                // waveFormGen.Sine(frequency, frequency, alarmingDur/2, channel1, (alarmingDur + pulseDelay)*i + alarmingDur/2);

                waveFormGen.Sine(frequency+25, frequency, alarmingDur, channel2, (alarmingDur + pulseDelay)*i);
                // waveFormGen.Sine(frequency, frequency, alarmingDur/2, channel2, (alarmingDur + pulseDelay)*i + alarmingDur/2);
            }
            else if (sweep3)
            {
                // waveFormGen.Sine(frequency, frequency-25, alarmingDur/2, channel1, (alarmingDur + pulseDelay)*i);
                // waveFormGen.Sine(frequency, frequency, alarmingDur/2, channel1, (alarmingDur + pulseDelay)*i + alarmingDur/2);

                // waveFormGen.Sine(frequency, frequency-25, alarmingDur/2, channel2, (alarmingDur + pulseDelay)*i);
                // waveFormGen.Sine(frequency, frequency, alarmingDur/2, channel2, (alarmingDur + pulseDelay)*i + alarmingDur/2);
            }
            else
            {
                // waveFormGen.Sine(frequency, frequency+25, alarmingDur/2, channel1, (alarmingDur + pulseDelay)*i);
                // waveFormGen.Sine(frequency, frequency, alarmingDur/2, channel1, (alarmingDur + pulseDelay)*i + alarmingDur/2);

                // waveFormGen.Sine(frequency, frequency+25, alarmingDur/2, channel2, (alarmingDur + pulseDelay)*i);
                // waveFormGen.Sine(frequency, frequency, alarmingDur/2, channel2, (alarmingDur + pulseDelay)*i + alarmingDur/2);
            }
        }
        streamer = new ByteStream(waveFormGen.ByteStreams, waveFormGen.WaveFormat); 
        streamer.Play();
        */
    }

    [RelayCommand]
    public void FindPhase()
    {
        double[] possibleOffsets = new double[3];
        double[] rightPhaseOffsets = new double[pulseCount];
        double[] leftPhaseOffsets = new double[pulseCount];

        possibleOffsets[0] = 0;
        possibleOffsets[1] = (alarmingDur+pulseDelay)/4;
        possibleOffsets[2] = (alarmingDur+pulseDelay)/2;

        // We don't need a case for phase 1 as it is the default sync case
        if (phase2)
        {
            for (int i=0; i<pulseCount; i++)
            {
                rightPhaseOffsets[i] = (alarmingDur+pulseDelay)/4;
                leftPhaseOffsets[i] = 0;
            }
        }
        else if (phase3)
        {
            for (int i=0; i<pulseCount; i++)
            {
                leftPhaseOffsets[i] = (alarmingDur+pulseDelay)/4;
                rightPhaseOffsets[i] = 0;
            }
        }
        else if (phase4)
        {
            for (int i=0; i<pulseCount; i++)
            {
                rightPhaseOffsets[i] = (alarmingDur+pulseDelay)/2;
                leftPhaseOffsets[i] = 0;
            }
        }
        else
        {
            Random rnd = new Random();

            for (int i=0; i<pulseCount; i++)
            {
                rightPhaseOffsets[i] = possibleOffsets[rnd.Next(0, 2)];
                leftPhaseOffsets[i] = possibleOffsets[rnd.Next(0, 2)];
            }
        }

        Reset();
        var waveFormGen = new WaveformGen(44100, 16, 2);
        for (int i=0; i<pulseCount; i++)
        {
            if (sweep1)
            {
                waveFormGen.Sine(channel1, (alarmingDur + pulseDelay + rightPhaseOffsets[i])*i, alarmingDur, frequency-25, frequency);
                waveFormGen.Sine(channel2, (alarmingDur + pulseDelay + leftPhaseOffsets[i])*i, alarmingDur, frequency-25, frequency);
            }
            else if (sweep2)
            {
                waveFormGen.Sine(channel1, (alarmingDur + pulseDelay + rightPhaseOffsets[i])*i, alarmingDur, frequency+25, frequency);
                waveFormGen.Sine(channel2, (alarmingDur + pulseDelay + leftPhaseOffsets[i])*i, alarmingDur, frequency+25, frequency);
            }
            else if (sweep3)
            {
                waveFormGen.Sine(channel1, (alarmingDur + pulseDelay + rightPhaseOffsets[i])*i, alarmingDur, frequency, frequency-25);

                waveFormGen.Sine(channel2, (alarmingDur + pulseDelay + leftPhaseOffsets[i])*i, alarmingDur, frequency, frequency-25);
            }
            else
            {
                waveFormGen.Sine(channel1, (alarmingDur + pulseDelay + rightPhaseOffsets[i])*i, alarmingDur, frequency, frequency+25);

                waveFormGen.Sine(channel2, (alarmingDur + pulseDelay + leftPhaseOffsets[i])*i, alarmingDur, frequency, frequency+25);
            }
        }

        waveFormGen.Encode();
        streamer = new ByteStream(waveFormGen.ByteStreams, waveFormGen.waveFormat); 
        streamer.Play();

        /*
        var waveFormGen = new FreqSweepWaveformGen(44100, 16);
        for (int i=0; i<pulseCount; i++)
        {
            if (sweep1)
            {
                waveFormGen.Sine(frequency-25, frequency, alarmingDur, channel1, (alarmingDur + pulseDelay + rightPhaseOffsets[i])*i);
                // waveFormGen.Sine(frequency, frequency, alarmingDur/2, channel1, (alarmingDur + pulseDelay)*i + alarmingDur/2);

                waveFormGen.Sine(frequency-25, frequency, alarmingDur, channel2, (alarmingDur + pulseDelay + leftPhaseOffsets[i])*i);
                // waveFormGen.Sine(frequency, frequency, alarmingDur/2, channel2, (alarmingDur + pulseDelay)*i + alarmingDur/2);
            }
            else if (sweep2)
            {
                waveFormGen.Sine(frequency+25, frequency, alarmingDur, channel1, (alarmingDur + pulseDelay + rightPhaseOffsets[i])*i);
                // waveFormGen.Sine(frequency, frequency, alarmingDur/2, channel1, (alarmingDur + pulseDelay)*i + alarmingDur/2);

                waveFormGen.Sine(frequency+25, frequency, alarmingDur, channel2, (alarmingDur + pulseDelay + leftPhaseOffsets[i])*i);
                // waveFormGen.Sine(frequency, frequency, alarmingDur/2, channel2, (alarmingDur + pulseDelay)*i + alarmingDur/2);
            }
            else if (sweep3)
            {
                // waveFormGen.Sine(frequency, frequency, alarmingDur/2, channel1, (alarmingDur + pulseDelay)*i);
                // waveFormGen.Sine(frequency, frequency-25, alarmingDur/2, channel1, (alarmingDur + pulseDelay)*i + alarmingDur/2);

                // waveFormGen.Sine(frequency, frequency-25, alarmingDur/2, channel2, (alarmingDur + pulseDelay)*i);
                // waveFormGen.Sine(frequency, frequency, alarmingDur/2, channel2, (alarmingDur + pulseDelay)*i + alarmingDur/2);
            }
            else
            {
                // waveFormGen.Sine(frequency, frequency+25, alarmingDur/2, channel1, (alarmingDur + pulseDelay)*i);
                // waveFormGen.Sine(frequency, frequency, alarmingDur/2, channel1, (alarmingDur + pulseDelay)*i + alarmingDur/2);

                // waveFormGen.Sine(frequency, frequency+25, alarmingDur/2, channel2, (alarmingDur + pulseDelay)*i);
                // waveFormGen.Sine(frequency, frequency, alarmingDur/2, channel2, (alarmingDur + pulseDelay)*i + alarmingDur/2);
            }
        }
        streamer = new ByteStream(waveFormGen.ByteStreams, waveFormGen.WaveFormat); 
        streamer.Play();
        */
    }
    
    private void Reset()
    {
        if (streamer != null) streamer.Dispose();
    }
}
