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

    [ObservableProperty] public bool phase1 = true;
    [ObservableProperty] public bool phase2 = false;
    [ObservableProperty] public bool phase3 = false;
    [ObservableProperty] public bool phase4 = false;
    [ObservableProperty] public bool phase5 = false;
    [ObservableProperty] public double leftPhaseOffset;
    [ObservableProperty] public double rightPhaseOffset;


    [ObservableProperty] public bool refreshing = true;
    [ObservableProperty] public bool alarming = false;

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
        alarmingDur = 0.2; // Results in full periods (original: 0.2)
        refreshingDur = 0.5; // Results in full periods (original: 0.5)
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

        var waveFormGen = new WaveformGen(44100, 16, 2);

        frequency = 100;

        for (int i=0; i<pulseCount; i++)
        {
            if (alarming)
            {
                waveFormGen.Square(channel1, (alarmingDur + pulseDelay)*i, alarmingDur, frequency, frequency);
                waveFormGen.Square(channel2, (alarmingDur + pulseDelay)*i, alarmingDur, frequency, frequency);
            }
            else if (refreshing)
            {
                waveFormGen.Sine(channel1, (refreshingDur + pulseDelay)*i, refreshingDur, frequency, frequency);
                waveFormGen.Sine(channel2, (refreshingDur + pulseDelay)*i, refreshingDur, frequency, frequency);
                
                waveFormGen.Linear(channel1, (refreshingDur + pulseDelay)*i, refreshingDur/2, 0, 1);
                waveFormGen.Linear(channel1, (refreshingDur + pulseDelay)*i + refreshingDur/2, refreshingDur/2, 1, 0);
                
                waveFormGen.Linear(channel2, (refreshingDur + pulseDelay)*i, refreshingDur/2, 0, 1);
                waveFormGen.Linear(channel2, (refreshingDur + pulseDelay)*i + refreshingDur/2, refreshingDur/2, 1, 0);
            }
            else
            {
                Console.WriteLine("Dumbass...");
            }
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
            if (alarming)
            {
                waveFormGen.Square(channel1, (alarmingDur + pulseDelay)*i, alarmingDur, frequency, frequency);
                waveFormGen.Square(channel2, (alarmingDur + pulseDelay)*i, alarmingDur, frequency, frequency);
            }
            else if (refreshing)
            {
                waveFormGen.Sine(channel1, (refreshingDur + pulseDelay)*i, refreshingDur, frequency, frequency);
                waveFormGen.Sine(channel2, (refreshingDur + pulseDelay)*i, refreshingDur, frequency, frequency);
                
                waveFormGen.Linear(channel1, (refreshingDur + pulseDelay)*i, refreshingDur/2, 0, 1);
                waveFormGen.Linear(channel1, (refreshingDur + pulseDelay)*i + refreshingDur/2, refreshingDur/2, 1, 0);
                
                waveFormGen.Linear(channel2, (refreshingDur + pulseDelay)*i, refreshingDur/2, 0, 1);
                waveFormGen.Linear(channel2, (refreshingDur + pulseDelay)*i + refreshingDur/2, refreshingDur/2, 1, 0);
            }
            else
            {
                Console.WriteLine("Dumbass...");
            }
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
            if (alarming)
            {
                waveFormGen.Square(channel1, (alarmingDur + pulseDelay)*i, alarmingDur, frequency, frequency);
                waveFormGen.Square(channel2, (alarmingDur + pulseDelay)*i, alarmingDur, frequency, frequency);
            }
            else if (refreshing)
            {
                waveFormGen.Sine(channel1, (refreshingDur + pulseDelay)*i, refreshingDur, frequency, frequency);
                waveFormGen.Sine(channel2, (refreshingDur + pulseDelay)*i, refreshingDur, frequency, frequency);
                
                waveFormGen.Linear(channel1, (refreshingDur + pulseDelay)*i, refreshingDur/2, 0, 1);
                waveFormGen.Linear(channel1, (refreshingDur + pulseDelay)*i + refreshingDur/2, refreshingDur/2, 1, 0);
                
                waveFormGen.Linear(channel2, (refreshingDur + pulseDelay)*i, refreshingDur/2, 0, 1);
                waveFormGen.Linear(channel2, (refreshingDur + pulseDelay)*i + refreshingDur/2, refreshingDur/2, 1, 0);
            }
            else
            {
                Console.WriteLine("Dumbass...");
            }
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
            if (alarming)
            {
                waveFormGen.Square(channel1, (alarmingDur + pulseDelay)*i, alarmingDur, frequency-25, frequency);
                waveFormGen.Square(channel2, (alarmingDur + pulseDelay)*i, alarmingDur, frequency-25, frequency);
            }
            else if (refreshing)
            {
                waveFormGen.Sine(channel1, (refreshingDur + pulseDelay)*i, refreshingDur, frequency-25, frequency);
                waveFormGen.Sine(channel2, (refreshingDur + pulseDelay)*i, refreshingDur, frequency-25, frequency);
                
                
                waveFormGen.Linear(channel1, (refreshingDur + pulseDelay)*i, refreshingDur/2, 0, 1);
                waveFormGen.Linear(channel1, (refreshingDur + pulseDelay)*i + refreshingDur/2, refreshingDur/2, 1, 0);
                
                waveFormGen.Linear(channel2, (refreshingDur + pulseDelay)*i, refreshingDur/2, 0, 1);
                waveFormGen.Linear(channel2, (refreshingDur + pulseDelay)*i + refreshingDur/2, refreshingDur/2, 1, 0);
            }
            else
            {
                Console.WriteLine("Dumbass...");
            }
            }
            else if (sweep2)
            {
            if (alarming)
            {
                waveFormGen.Square(channel1, (alarmingDur + pulseDelay)*i, alarmingDur, frequency+25, frequency);
                waveFormGen.Square(channel2, (alarmingDur + pulseDelay)*i, alarmingDur, frequency+25, frequency);
            }
            else if (refreshing)
            {
                waveFormGen.Sine(channel1, (refreshingDur + pulseDelay)*i, refreshingDur, frequency+25, frequency);
                waveFormGen.Sine(channel2, (refreshingDur + pulseDelay)*i, refreshingDur, frequency+25, frequency);
                
                waveFormGen.Linear(channel1, (refreshingDur + pulseDelay)*i, refreshingDur/2, 0, 1);
                waveFormGen.Linear(channel1, (refreshingDur + pulseDelay)*i + refreshingDur/2, refreshingDur/2, 1, 0);
                
                waveFormGen.Linear(channel2, (refreshingDur + pulseDelay)*i, refreshingDur/2, 0, 1);
                waveFormGen.Linear(channel2, (refreshingDur + pulseDelay)*i + refreshingDur/2, refreshingDur/2, 1, 0);
            }
            else
            {
                Console.WriteLine("Dumbass...");
            }
            }
            else if (sweep3)
            {
            if (alarming)
            {
                waveFormGen.Square(channel1, (alarmingDur + pulseDelay)*i, alarmingDur, frequency, frequency-25);
                waveFormGen.Square(channel2, (alarmingDur + pulseDelay)*i, alarmingDur, frequency, frequency-25);
            }
            else if (refreshing)
            {
                waveFormGen.Sine(channel1, (refreshingDur + pulseDelay)*i, refreshingDur, frequency, frequency-25);
                waveFormGen.Sine(channel2, (refreshingDur + pulseDelay)*i, refreshingDur, frequency, frequency-25);
                
                waveFormGen.Linear(channel1, (refreshingDur + pulseDelay)*i, refreshingDur/2, 0, 1);
                waveFormGen.Linear(channel1, (refreshingDur + pulseDelay)*i + refreshingDur/2, refreshingDur/2, 1, 0);
                
                waveFormGen.Linear(channel2, (refreshingDur + pulseDelay)*i, refreshingDur/2, 0, 1);
                waveFormGen.Linear(channel2, (refreshingDur + pulseDelay)*i + refreshingDur/2, refreshingDur/2, 1, 0);
            }
            else
            {
                Console.WriteLine("Dumbass...");
            }
            }
            else
            {
            if (alarming)
            {
                waveFormGen.Square(channel1, (alarmingDur + pulseDelay)*i, alarmingDur, frequency, frequency+25);
                waveFormGen.Square(channel2, (alarmingDur + pulseDelay)*i, alarmingDur, frequency, frequency+25);
            }
            else if (refreshing)
            {
                waveFormGen.Sine(channel1, (refreshingDur + pulseDelay)*i, refreshingDur, frequency, frequency+25);
                waveFormGen.Sine(channel2, (refreshingDur + pulseDelay)*i, refreshingDur, frequency, frequency+25);
                
                waveFormGen.Linear(channel1, (refreshingDur + pulseDelay)*i, refreshingDur/2, 0, 1);
                waveFormGen.Linear(channel1, (refreshingDur + pulseDelay)*i + refreshingDur/2, refreshingDur/2, 1, 0);
                
                waveFormGen.Linear(channel2, (refreshingDur + pulseDelay)*i, refreshingDur/2, 0, 1);
                waveFormGen.Linear(channel2, (refreshingDur + pulseDelay)*i + refreshingDur/2, refreshingDur/2, 1, 0);
            }
            else
            {
                Console.WriteLine("Dumbass...");
            }
            }
        }

        waveFormGen.Encode();
        streamer = new ByteStream(waveFormGen.ByteStreams, waveFormGen.waveFormat); 
        streamer.Play();
    }

    [RelayCommand]
    public void FindPhase()
    {
        double[] possibleOffsets = new double[3];
        double[] rightPhaseOffsets = new double[pulseCount];
        double[] leftPhaseOffsets = new double[pulseCount];
        
        // Random sequence
        /*
         * 0. 0
         * 1. 2
         * 2. 1
         * 3. 0
         * 4. 2
         * 5. 1
         * 6. 1
         * 7. 1
         * 8. 0
         * 9. 0
        */
        int[] rigthRandSequence = new int[] { 0, 2, 1, 0, 2, 1, 1, 1, 0, 0 };
        int[] leftRandSequence = new int[] { 2, 1, 2, 1, 0, 2, 0, 0, 2, 1 };

        possibleOffsets[0] = 0;
        possibleOffsets[1] = (alarmingDur+pulseDelay)/4;
        possibleOffsets[2] = (alarmingDur+pulseDelay)/2;

        if (phase1)
        {
            for (int i = 0; i < pulseCount; i++)
            {
                rightPhaseOffsets[i] = 0;
                leftPhaseOffsets[i] = 0;
            }
        }
        else if (phase2)
        {
            for (int i=0; i<pulseCount; i++)
            {
                rightPhaseOffsets[i] = (alarmingDur+pulseDelay)/8;
                leftPhaseOffsets[i] = 0;
            }
        }
        else if (phase3)
        {
            for (int i=0; i<pulseCount; i++)
            {
                rightPhaseOffsets[i] = 0;
                leftPhaseOffsets[i] = (alarmingDur+pulseDelay)/4;
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
                rightPhaseOffsets[i] = possibleOffsets[rigthRandSequence[i % 10]];
                leftPhaseOffsets[i] = possibleOffsets[leftRandSequence[i % 10]];
            }
        }

        Reset();
        var waveFormGen = new WaveformGen(44100, 16, 2);
        for (int i=0; i<pulseCount; i++)
        {
            if (sweep1)
            {
            if (alarming)
            {
                waveFormGen.Square(channel1, (alarmingDur + pulseDelay) * i + rightPhaseOffsets[i], alarmingDur, frequency-25, frequency);
                waveFormGen.Square(channel2, (alarmingDur + pulseDelay) * i + leftPhaseOffsets[i], alarmingDur, frequency-25, frequency);
            }
            else if (refreshing)
            {
                waveFormGen.Sine(channel1, (refreshingDur + pulseDelay) * i + rightPhaseOffsets[i], refreshingDur, frequency-25, frequency);
                waveFormGen.Sine(channel2, (refreshingDur + pulseDelay) * i + leftPhaseOffsets[i], refreshingDur, frequency-25, frequency);
                
                waveFormGen.Linear(channel1, (refreshingDur + pulseDelay) * i + rightPhaseOffsets[i], refreshingDur/2, 0, 1);
                waveFormGen.Linear(channel1, (refreshingDur + pulseDelay) * i + rightPhaseOffsets[i] + refreshingDur/2, refreshingDur/2, 1, 0);
                
                waveFormGen.Linear(channel2, (refreshingDur + pulseDelay) * i + leftPhaseOffsets[i], refreshingDur/2, 0, 1);
                waveFormGen.Linear(channel2, (refreshingDur + pulseDelay) * i + leftPhaseOffsets[i] + refreshingDur/2, refreshingDur/2, 1, 0);
            }
            else
            {
                Console.WriteLine("Dumbass...");
            }
            }
            else if (sweep2)
            {
            if (alarming)
            {
                waveFormGen.Square(channel1, (alarmingDur + pulseDelay) * i + rightPhaseOffsets[i], alarmingDur, frequency+25, frequency);
                waveFormGen.Square(channel2, (alarmingDur + pulseDelay) * i + leftPhaseOffsets[i], alarmingDur, frequency+25, frequency);
            }
            else if (refreshing)
            {
                waveFormGen.Sine(channel1, (refreshingDur + pulseDelay) * i + rightPhaseOffsets[i], refreshingDur, frequency+25, frequency);
                waveFormGen.Sine(channel2, (refreshingDur + pulseDelay) * i + leftPhaseOffsets[i], refreshingDur, frequency+25, frequency);
                
                waveFormGen.Linear(channel1, (refreshingDur + pulseDelay) * i + rightPhaseOffsets[i], refreshingDur/2, 0, 1);
                waveFormGen.Linear(channel1, (refreshingDur + pulseDelay) * i + rightPhaseOffsets[i] + refreshingDur/2, refreshingDur/2, 1, 0);
                
                waveFormGen.Linear(channel2, (refreshingDur + pulseDelay) * i + leftPhaseOffsets[i], refreshingDur/2, 0, 1);
                waveFormGen.Linear(channel2, (refreshingDur + pulseDelay) * i + leftPhaseOffsets[i] + refreshingDur/2, refreshingDur/2, 1, 0);
            }
            else
            {
                Console.WriteLine("Dumbass...");
            }
            }
            else if (sweep3)
            {
            if (alarming)
            {
                waveFormGen.Square(channel1, (alarmingDur + pulseDelay) * i + rightPhaseOffsets[i], alarmingDur, frequency, frequency-25);
                waveFormGen.Square(channel2, (alarmingDur + pulseDelay) * i + leftPhaseOffsets[i], alarmingDur, frequency, frequency-25);
            }
            else if (refreshing)
            {
                waveFormGen.Sine(channel1, (refreshingDur + pulseDelay) * i + rightPhaseOffsets[i], refreshingDur, frequency, frequency-25);
                waveFormGen.Sine(channel2, (refreshingDur + pulseDelay) * i + leftPhaseOffsets[i], refreshingDur, frequency, frequency-25);
                
                waveFormGen.Linear(channel1, (refreshingDur + pulseDelay) * i + rightPhaseOffsets[i], refreshingDur/2, 0, 1);
                waveFormGen.Linear(channel1, (refreshingDur + pulseDelay) * i + rightPhaseOffsets[i] + refreshingDur/2, refreshingDur/2, 1, 0);
                
                waveFormGen.Linear(channel2, (refreshingDur + pulseDelay) * i + leftPhaseOffsets[i], refreshingDur/2, 0, 1);
                waveFormGen.Linear(channel2, (refreshingDur + pulseDelay) * i + leftPhaseOffsets[i] + refreshingDur/2, refreshingDur/2, 1, 0);
            }
            else
            {
                Console.WriteLine("Dumbass...");
            }
            }
            else
            {
            if (alarming)
            {
                waveFormGen.Square(channel1, (alarmingDur + pulseDelay) * i + rightPhaseOffsets[i], alarmingDur, frequency, frequency+25);
                waveFormGen.Square(channel2, (alarmingDur + pulseDelay) * i + leftPhaseOffsets[i], alarmingDur, frequency, frequency+25);
            }
            else if (refreshing)
            {
                waveFormGen.Sine(channel1, (refreshingDur + pulseDelay) * i + rightPhaseOffsets[i], refreshingDur, frequency, frequency+25);
                waveFormGen.Sine(channel2, (refreshingDur + pulseDelay) * i + leftPhaseOffsets[i], refreshingDur, frequency, frequency+25);
                
                waveFormGen.Linear(channel1, (refreshingDur + pulseDelay) * i + rightPhaseOffsets[i], refreshingDur/2, 0, 1);
                waveFormGen.Linear(channel1, (refreshingDur + pulseDelay) * i + rightPhaseOffsets[i] + refreshingDur/2, refreshingDur/2, 1, 0);
                
                waveFormGen.Linear(channel2, (refreshingDur + pulseDelay) * i + leftPhaseOffsets[i], refreshingDur/2, 0, 1);
                waveFormGen.Linear(channel2, (refreshingDur + pulseDelay) * i + leftPhaseOffsets[i] + refreshingDur/2, refreshingDur/2, 1, 0);
            }
            else
            {
                Console.WriteLine("Dumbass...");
            }
            }
        }

        waveFormGen.Encode();
        streamer = new ByteStream(waveFormGen.ByteStreams, waveFormGen.waveFormat); 
        streamer.Play();
    }
    
    private void Reset()
    {
        if (streamer != null) streamer.Dispose();
    }
}
