using System;
using System.Collections.Generic;
using System.Threading;
using Avalonia.LogicalTree;
using Avalonia.Rendering;
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

    [ObservableProperty] public bool noSweep = true;
    [ObservableProperty] public bool sweep1 = false;
    [ObservableProperty] public bool sweep2 = false;
    [ObservableProperty] public bool sweep3 = false;
    [ObservableProperty] public bool sweep4 = false;

    [ObservableProperty] public bool phase1 = true;
    [ObservableProperty] public bool phase2 = false;
    [ObservableProperty] public bool phase3 = false;
    [ObservableProperty] public bool phase4 = false;
    [ObservableProperty] public bool phase5 = false;
    [ObservableProperty] public double leftPhaseOffset;
    [ObservableProperty] public double rightPhaseOffset;


    [ObservableProperty] public bool refreshing = true;
    [ObservableProperty] public bool alarming = false;


    private readonly double resourcePulseLength = 0.5; 
    private readonly int resourcePulseCount = 3;
    private readonly double resourcePulseDelay = 0.2; 
    private readonly int resourceStartingFrequency = 75;
    private readonly int resourceEndingFrequency = 100;
    
    private readonly double declutterPulseLength = 0.2; 
    private readonly int declutterPulseCount = 5;
    private readonly double declutterPulseDelay = 0.1; 
    private readonly int declutterStartingFrequency = 125;
    private readonly int declutterEndingFrequency = 150;


    private Thread userModeThread; 
    
    
    
    [ObservableProperty] public bool resourceMode = false;
    [ObservableProperty] public bool declutterMode = false;
    [ObservableProperty] public bool breathingMode = false;

    // Hook into this one for breathing up and down presses
    [ObservableProperty] public int breathingVariable = 0;

    private bool PlayingMutex = false;
    
    

    private AudioFileReader audioFile;
    private WasapiOut audioOut;

    private ByteStream? byteStream;

    // private IDictionary<string, ByteStream> FunctionDictionary;
    private ByteStream streamer;

    private double refreshingDur;
    private double alarmingDur;
    private int sweepRange = 25;


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
        
        
        userModeThread = new Thread(userModePlay);
        userModeThread.Start();
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

        for (int i = 0; i < pulseCount; i++)
        {
            if (alarming)
            {
                waveFormGen.Square(channel1, (alarmingDur + pulseDelay) * i, alarmingDur, frequency, frequency);
                waveFormGen.Square(channel2, (alarmingDur + pulseDelay) * i, alarmingDur, frequency, frequency);
            }
            else if (refreshing)
            {
                waveFormGen.Sine(channel1, (refreshingDur + pulseDelay) * i, refreshingDur, frequency, frequency);
                waveFormGen.Sine(channel2, (refreshingDur + pulseDelay) * i, refreshingDur, frequency, frequency);

                waveFormGen.Linear(channel1, (refreshingDur + pulseDelay) * i, refreshingDur / 2, 0, 1);
                waveFormGen.Linear(channel1, (refreshingDur + pulseDelay) * i + refreshingDur / 2, refreshingDur / 2, 1,
                    0);

                waveFormGen.Linear(channel2, (refreshingDur + pulseDelay) * i, refreshingDur / 2, 0, 1);
                waveFormGen.Linear(channel2, (refreshingDur + pulseDelay) * i + refreshingDur / 2, refreshingDur / 2, 1,
                    0);
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
        for (int i = 0; i < pulseCount; i++)
        {
            if (alarming)
            {
                waveFormGen.Square(channel1, (alarmingDur + pulseDelay) * i, alarmingDur, frequency, frequency);
                waveFormGen.Square(channel2, (alarmingDur + pulseDelay) * i, alarmingDur, frequency, frequency);
            }
            else if (refreshing)
            {
                waveFormGen.Sine(channel1, (refreshingDur + pulseDelay) * i, refreshingDur, frequency, frequency);
                waveFormGen.Sine(channel2, (refreshingDur + pulseDelay) * i, refreshingDur, frequency, frequency);

                waveFormGen.Linear(channel1, (refreshingDur + pulseDelay) * i, refreshingDur / 2, 0, 1);
                waveFormGen.Linear(channel1, (refreshingDur + pulseDelay) * i + refreshingDur / 2, refreshingDur / 2, 1,
                    0);

                waveFormGen.Linear(channel2, (refreshingDur + pulseDelay) * i, refreshingDur / 2, 0, 1);
                waveFormGen.Linear(channel2, (refreshingDur + pulseDelay) * i + refreshingDur / 2, refreshingDur / 2, 1,
                    0);
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
        for (int i = 0; i < pulseCount; i++)
        {
            if (alarming)
            {
                waveFormGen.Square(channel1, (alarmingDur + pulseDelay) * i, alarmingDur, frequency, frequency);
                waveFormGen.Square(channel2, (alarmingDur + pulseDelay) * i, alarmingDur, frequency, frequency);
            }
            else if (refreshing)
            {
                waveFormGen.Sine(channel1, (refreshingDur + pulseDelay) * i, refreshingDur, frequency, frequency);
                waveFormGen.Sine(channel2, (refreshingDur + pulseDelay) * i, refreshingDur, frequency, frequency);

                waveFormGen.Linear(channel1, (refreshingDur + pulseDelay) * i, refreshingDur / 2, 0, 1);
                waveFormGen.Linear(channel1, (refreshingDur + pulseDelay) * i + refreshingDur / 2, refreshingDur / 2, 1,
                    0);

                waveFormGen.Linear(channel2, (refreshingDur + pulseDelay) * i, refreshingDur / 2, 0, 1);
                waveFormGen.Linear(channel2, (refreshingDur + pulseDelay) * i + refreshingDur / 2, refreshingDur / 2, 1,
                    0);
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
        
        if (noSweep) sweepRange = 0;
        else sweepRange = 25;

        for (int i = 0; i < pulseCount; i++)
        {
            if (sweep1)
            {
                if (alarming)
                {
                    waveFormGen.Square(channel1, (alarmingDur + pulseDelay) * i, alarmingDur, frequency - sweepRange,
                        frequency);
                    waveFormGen.Square(channel2, (alarmingDur + pulseDelay) * i, alarmingDur, frequency - sweepRange,
                        frequency);
                }
                else if (refreshing)
                {
                    waveFormGen.Sine(channel1, (refreshingDur + pulseDelay) * i, refreshingDur, frequency - sweepRange,
                        frequency);
                    waveFormGen.Sine(channel2, (refreshingDur + pulseDelay) * i, refreshingDur, frequency - sweepRange,
                        frequency);


                    waveFormGen.Linear(channel1, (refreshingDur + pulseDelay) * i, refreshingDur / 2, 0, 1);
                    waveFormGen.Linear(channel1, (refreshingDur + pulseDelay) * i + refreshingDur / 2,
                        refreshingDur / 2, 1, 0);

                    waveFormGen.Linear(channel2, (refreshingDur + pulseDelay) * i, refreshingDur / 2, 0, 1);
                    waveFormGen.Linear(channel2, (refreshingDur + pulseDelay) * i + refreshingDur / 2,
                        refreshingDur / 2, 1, 0);
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
                    waveFormGen.Square(channel1, (alarmingDur + pulseDelay) * i, alarmingDur, frequency + sweepRange,
                        frequency);
                    waveFormGen.Square(channel2, (alarmingDur + pulseDelay) * i, alarmingDur, frequency + sweepRange,
                        frequency);
                }
                else if (refreshing)
                {
                    waveFormGen.Sine(channel1, (refreshingDur + pulseDelay) * i, refreshingDur, frequency + sweepRange,
                        frequency);
                    waveFormGen.Sine(channel2, (refreshingDur + pulseDelay) * i, refreshingDur, frequency + sweepRange,
                        frequency);

                    waveFormGen.Linear(channel1, (refreshingDur + pulseDelay) * i, refreshingDur / 2, 0, 1);
                    waveFormGen.Linear(channel1, (refreshingDur + pulseDelay) * i + refreshingDur / 2,
                        refreshingDur / 2, 1, 0);

                    waveFormGen.Linear(channel2, (refreshingDur + pulseDelay) * i, refreshingDur / 2, 0, 1);
                    waveFormGen.Linear(channel2, (refreshingDur + pulseDelay) * i + refreshingDur / 2,
                        refreshingDur / 2, 1, 0);
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
                    waveFormGen.Square(channel1, (alarmingDur + pulseDelay) * i, alarmingDur, frequency,
                        frequency - sweepRange);
                    waveFormGen.Square(channel2, (alarmingDur + pulseDelay) * i, alarmingDur, frequency,
                        frequency - sweepRange);
                }
                else if (refreshing)
                {
                    waveFormGen.Sine(channel1, (refreshingDur + pulseDelay) * i, refreshingDur, frequency,
                        frequency - sweepRange);
                    waveFormGen.Sine(channel2, (refreshingDur + pulseDelay) * i, refreshingDur, frequency,
                        frequency - sweepRange);

                    waveFormGen.Linear(channel1, (refreshingDur + pulseDelay) * i, refreshingDur / 2, 0, 1);
                    waveFormGen.Linear(channel1, (refreshingDur + pulseDelay) * i + refreshingDur / 2,
                        refreshingDur / 2, 1, 0);

                    waveFormGen.Linear(channel2, (refreshingDur + pulseDelay) * i, refreshingDur / 2, 0, 1);
                    waveFormGen.Linear(channel2, (refreshingDur + pulseDelay) * i + refreshingDur / 2,
                        refreshingDur / 2, 1, 0);
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
                    waveFormGen.Square(channel1, (alarmingDur + pulseDelay) * i, alarmingDur, frequency,
                        frequency + sweepRange);
                    waveFormGen.Square(channel2, (alarmingDur + pulseDelay) * i, alarmingDur, frequency,
                        frequency + sweepRange);
                }
                else if (refreshing)
                {
                    waveFormGen.Sine(channel1, (refreshingDur + pulseDelay) * i, refreshingDur, frequency,
                        frequency + sweepRange);
                    waveFormGen.Sine(channel2, (refreshingDur + pulseDelay) * i, refreshingDur, frequency,
                        frequency + sweepRange);

                    waveFormGen.Linear(channel1, (refreshingDur + pulseDelay) * i, refreshingDur / 2, 0, 1);
                    waveFormGen.Linear(channel1, (refreshingDur + pulseDelay) * i + refreshingDur / 2,
                        refreshingDur / 2, 1, 0);

                    waveFormGen.Linear(channel2, (refreshingDur + pulseDelay) * i, refreshingDur / 2, 0, 1);
                    waveFormGen.Linear(channel2, (refreshingDur + pulseDelay) * i + refreshingDur / 2,
                        refreshingDur / 2, 1, 0);
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

        if (noSweep) sweepRange = 0;
        else sweepRange = 25;

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

        if (alarming)
        {
            possibleOffsets[0] = 0;
            possibleOffsets[1] = (alarmingDur + pulseDelay) / 4;
            possibleOffsets[2] = (alarmingDur + pulseDelay) / 2;
        }

        if (refreshing)
        {
            possibleOffsets[0] = 0;
            possibleOffsets[1] = (refreshingDur + pulseDelay) / 4;
            possibleOffsets[2] = (refreshingDur + pulseDelay) / 2;
        }
        else
        {
            Console.WriteLine("Dumb");
        }

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
            for (int i = 0; i < pulseCount; i++)
            {
                if (alarming)
                {
                    rightPhaseOffsets[i] = (alarmingDur + pulseDelay) / 8;
                    leftPhaseOffsets[i] = 0;
                }

                if (refreshing)
                {
                    rightPhaseOffsets[i] = (refreshingDur + pulseDelay) / 8;
                    leftPhaseOffsets[i] = 0;
                }
                else
                {
                    Console.WriteLine("Dumb");
                }
            }
        }
        else if (phase3)
        {
            for (int i = 0; i < pulseCount; i++)
            {
                if (alarming)
                {
                    rightPhaseOffsets[i] = (alarmingDur + pulseDelay) / 4;
                    leftPhaseOffsets[i] = 0;
                }

                if (refreshing)
                {
                    rightPhaseOffsets[i] = (refreshingDur + pulseDelay) / 4;
                    leftPhaseOffsets[i] = 0;
                }
                else
                {
                    Console.WriteLine("Dumb");
                }
            }
        }
        else if (phase4)
        {
            for (int i = 0; i < pulseCount; i++)
            {
                if (alarming)
                {
                    rightPhaseOffsets[i] = (alarmingDur + pulseDelay) / 2;
                    leftPhaseOffsets[i] = 0;
                }

                if (refreshing)
                {
                    rightPhaseOffsets[i] = (refreshingDur + pulseDelay) / 2;
                    leftPhaseOffsets[i] = 0;
                }
                else
                {
                    Console.WriteLine("Dumb");
                }
            }
        }
        else
        {
            Random rnd = new Random();
            for (int i = 0; i < pulseCount; i++)
            {
                rightPhaseOffsets[i] = possibleOffsets[rigthRandSequence[i % 10]];
                leftPhaseOffsets[i] = possibleOffsets[leftRandSequence[i % 10]];
            }
        }

        Reset();
        var waveFormGen = new WaveformGen(44100, 16, 2);
        for (int i = 0; i < pulseCount; i++)
        {
            if (sweep1)
            {
                if (alarming)
                {
                    waveFormGen.Square(channel1, (alarmingDur + pulseDelay) * i + rightPhaseOffsets[i], alarmingDur,
                        frequency - sweepRange, frequency);
                    waveFormGen.Square(channel2, (alarmingDur + pulseDelay) * i + leftPhaseOffsets[i], alarmingDur,
                        frequency - sweepRange, frequency);
                }
                else if (refreshing)
                {
                    waveFormGen.Sine(channel1, (refreshingDur + pulseDelay) * i + rightPhaseOffsets[i], refreshingDur,
                        frequency - sweepRange, frequency);
                    waveFormGen.Sine(channel2, (refreshingDur + pulseDelay) * i + leftPhaseOffsets[i], refreshingDur,
                        frequency - sweepRange, frequency);

                    waveFormGen.Linear(channel1, (refreshingDur + pulseDelay) * i + rightPhaseOffsets[i],
                        refreshingDur / 2, 0, 1);
                    waveFormGen.Linear(channel1,
                        (refreshingDur + pulseDelay) * i + rightPhaseOffsets[i] + refreshingDur / 2, refreshingDur / 2,
                        1, 0);

                    waveFormGen.Linear(channel2, (refreshingDur + pulseDelay) * i + leftPhaseOffsets[i],
                        refreshingDur / 2, 0, 1);
                    waveFormGen.Linear(channel2,
                        (refreshingDur + pulseDelay) * i + leftPhaseOffsets[i] + refreshingDur / 2, refreshingDur / 2,
                        1, 0);
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
                    waveFormGen.Square(channel1, (alarmingDur + pulseDelay) * i + rightPhaseOffsets[i], alarmingDur,
                        frequency + sweepRange, frequency);
                    waveFormGen.Square(channel2, (alarmingDur + pulseDelay) * i + leftPhaseOffsets[i], alarmingDur,
                        frequency + sweepRange, frequency);
                }
                else if (refreshing)
                {
                    waveFormGen.Sine(channel1, (refreshingDur + pulseDelay) * i + rightPhaseOffsets[i], refreshingDur,
                        frequency + sweepRange, frequency);
                    waveFormGen.Sine(channel2, (refreshingDur + pulseDelay) * i + leftPhaseOffsets[i], refreshingDur,
                        frequency + sweepRange, frequency);

                    waveFormGen.Linear(channel1, (refreshingDur + pulseDelay) * i + rightPhaseOffsets[i],
                        refreshingDur / 2, 0, 1);
                    waveFormGen.Linear(channel1,
                        (refreshingDur + pulseDelay) * i + rightPhaseOffsets[i] + refreshingDur / 2, refreshingDur / 2,
                        1, 0);

                    waveFormGen.Linear(channel2, (refreshingDur + pulseDelay) * i + leftPhaseOffsets[i],
                        refreshingDur / 2, 0, 1);
                    waveFormGen.Linear(channel2,
                        (refreshingDur + pulseDelay) * i + leftPhaseOffsets[i] + refreshingDur / 2, refreshingDur / 2,
                        1, 0);
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
                    waveFormGen.Square(channel1, (alarmingDur + pulseDelay) * i + rightPhaseOffsets[i], alarmingDur,
                        frequency, frequency - sweepRange);
                    waveFormGen.Square(channel2, (alarmingDur + pulseDelay) * i + leftPhaseOffsets[i], alarmingDur,
                        frequency, frequency - sweepRange);
                }
                else if (refreshing)
                {
                    waveFormGen.Sine(channel1, (refreshingDur + pulseDelay) * i + rightPhaseOffsets[i], refreshingDur,
                        frequency, frequency - sweepRange);
                    waveFormGen.Sine(channel2, (refreshingDur + pulseDelay) * i + leftPhaseOffsets[i], refreshingDur,
                        frequency, frequency - sweepRange);

                    waveFormGen.Linear(channel1, (refreshingDur + pulseDelay) * i + rightPhaseOffsets[i],
                        refreshingDur / 2, 0, 1);
                    waveFormGen.Linear(channel1,
                        (refreshingDur + pulseDelay) * i + rightPhaseOffsets[i] + refreshingDur / 2, refreshingDur / 2,
                        1, 0);

                    waveFormGen.Linear(channel2, (refreshingDur + pulseDelay) * i + leftPhaseOffsets[i],
                        refreshingDur / 2, 0, 1);
                    waveFormGen.Linear(channel2,
                        (refreshingDur + pulseDelay) * i + leftPhaseOffsets[i] + refreshingDur / 2, refreshingDur / 2,
                        1, 0);
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
                    waveFormGen.Square(channel1, (alarmingDur + pulseDelay) * i + rightPhaseOffsets[i], alarmingDur,
                        frequency, frequency + sweepRange);
                    waveFormGen.Square(channel2, (alarmingDur + pulseDelay) * i + leftPhaseOffsets[i], alarmingDur,
                        frequency, frequency + sweepRange);
                }
                else if (refreshing)
                {
                    waveFormGen.Sine(channel1, (refreshingDur + pulseDelay) * i + rightPhaseOffsets[i], refreshingDur,
                        frequency, frequency + sweepRange);
                    waveFormGen.Sine(channel2, (refreshingDur + pulseDelay) * i + leftPhaseOffsets[i], refreshingDur,
                        frequency, frequency + sweepRange);

                    waveFormGen.Linear(channel1, (refreshingDur + pulseDelay) * i + rightPhaseOffsets[i],
                        refreshingDur / 2, 0, 1);
                    waveFormGen.Linear(channel1,
                        (refreshingDur + pulseDelay) * i + rightPhaseOffsets[i] + refreshingDur / 2, refreshingDur / 2,
                        1, 0);

                    waveFormGen.Linear(channel2, (refreshingDur + pulseDelay) * i + leftPhaseOffsets[i],
                        refreshingDur / 2, 0, 1);
                    waveFormGen.Linear(channel2,
                        (refreshingDur + pulseDelay) * i + leftPhaseOffsets[i] + refreshingDur / 2, refreshingDur / 2,
                        1, 0);
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



    public void userModePlay()
    {
        for (;;)
        {
            while (PlayingMutex);
            if (breathingMode)
            {
                Breathing(); 
            }
            else if (resourceMode)
            {
                Resource();        
            }
            else if (declutterMode)
            {
                Declutter();
            }
            Thread.Sleep(1000);
        }
    }

    [RelayCommand]
    public void ResourceButtonPressed()
    {
        breathingMode = false;
        resourceMode = !resourceMode;
        declutterMode = false;
    }
    
    [RelayCommand]
    public void BreathingButtonPressed()
    {
        breathingMode = !breathingMode;
        resourceMode = false;
        declutterMode = false;
    }
    
    [RelayCommand]
    public void DeclutterButtonPressed()
    {
        breathingMode = false;
        resourceMode = false;
        declutterMode = !declutterMode;
    }

    [RelayCommand]
    public void BreathingUpButtonPressed()
    {
        if (breathingVariable < 3)
        {
            breathingVariable++;
        }
    }
    
    [RelayCommand]
    public void BreathingDownButtonPressed()
    {
        if (breathingVariable > 0)
        {
            breathingVariable--;
        }
    }


    public double delayFunc(double dur, double i)
    {
        double t_0 = 0.5 * dur;
        double t_1 = dur;

        double k = -(1 / (t_1 - t_0)) * Math.Log(1 / 0.90 - 1);

        var delayFuncValue = (1 / (1 + Math.Exp(-k * (i - t_0)))) * (1 - (1 / (1 + Math.Exp(-k * (i - t_0)))));
        //var delayFuncValue = (1 / (1 + Math.Exp(-k * (i - t_0))));

        double offset = 0.5; // Slowest tick in ms
        double A = 1.4;

        return -1 * A * delayFuncValue + offset;
    }

    public void Breathing()
    {
        PlayingMutex = true;
        Reset();
        
        var cycles = 7;
        var ticks = 15;

        
        
        // DEBUGGING
        List<double> delayTemp = new List<double>();
        for (int i = 0; i < ticks; i++)
        {
            delayTemp.Add(delayFunc(ticks, i));
        }
        
        

        int accum = 0; 
        
        for (int i = 0; i < ticks; i++)
        {
            var waveFormGen = new WaveformGen(44100, 16, 2);
            waveFormGen.Sine(channel1, 0, 0.01 * cycles, 100, 100);
            waveFormGen.Sine(channel2, 0, 0.01 * cycles, 100, 100);

            int latency = (int)(1000 * delayFunc(ticks, i));
            accum += latency;
            
            waveFormGen.Encode();
            streamer = new ByteStream(waveFormGen.ByteStreams, waveFormGen.waveFormat, 80);
            streamer.Play();
            Thread.Sleep(latency);
            Reset();
        }
        Console.WriteLine("Length: " + accum + "\n");
        accum = 0;

        double burstPerMinute = 40 + 10 * breathingVariable;
        for (int i = 0; i < 7; i++)
        {   
            var waveFormGen = new WaveformGen(44100, 16, 2);
            waveFormGen.Sine(channel1, 0, 0.01 * cycles, 100, 100);
            waveFormGen.Sine(channel2, 0, 0.01 * cycles, 100, 100);

            int delay = (int)(1000 * (1 / (burstPerMinute/60)));
            accum += delay;
            
            waveFormGen.Encode();
            streamer = new ByteStream(waveFormGen.ByteStreams, waveFormGen.waveFormat, 80);
            streamer.Play();
            Thread.Sleep(delay);
            Reset();
        }
        Console.WriteLine("Length: " + accum + "\n");
        PlayingMutex = false;
    }

    public void Resource()
    {
        PlayingMutex = true;
        Reset();
                
        int accum = 0; 
        
        //double burstPerMinute = 40 + 10 * breathingVariable;
        double burstPerMinute = 30;
        for (int i = 0; i < 1; i++)
        {   
            var waveFormGen = new WaveformGen(44100, 16, 2);

            for (int j = 0; j < resourcePulseCount; j++)
            {
                double burstLength = (resourcePulseLength + resourcePulseDelay) * (1 + 1.0 / 4.0);
                
                /*
                 * if j = 0 then 0.175
                 * if j = 1 then 0.875
                 * diff is 0.7 which is 0.5 + 0.2
                 *
                 *
                 * 
                 */
                
                
                
                
                
                waveFormGen.Sine(channel1, (resourcePulseLength + resourcePulseDelay) * (1.0 / 4.0), 
                    resourcePulseLength, resourceStartingFrequency, resourceEndingFrequency);
                waveFormGen.Sine(channel2, (resourcePulseLength + resourcePulseDelay) * (0.0 / 4.0), 
                    resourcePulseLength, resourceStartingFrequency, resourceEndingFrequency);
                
                waveFormGen.Linear(channel1, (resourcePulseLength + resourcePulseDelay) * (1.0 / 4.0),
                    resourcePulseLength / 2, 0, 1);
                waveFormGen.Linear(channel1, (resourcePulseLength + resourcePulseDelay) * (1.0 / 4.0) + resourcePulseLength / 2, 
                    resourcePulseLength / 2, 1, 0);
                
                waveFormGen.Linear(channel2, (resourcePulseLength + resourcePulseDelay) * (0.0 / 4.0),
                    resourcePulseLength / 2, 0, 1);
                waveFormGen.Linear(channel2, (resourcePulseLength + resourcePulseDelay) * (0.0 / 4.0) + resourcePulseLength / 2, 
                    resourcePulseLength / 2, 1, 0);
                
                waveFormGen.Encode();
                streamer = new ByteStream(waveFormGen.ByteStreams, waveFormGen.waveFormat, (int)(burstLength * 1000));
                streamer.Play();
                Thread.Sleep((int)((resourcePulseLength + resourcePulseDelay) * 1000));
                Reset();
            }
            
            //int delay = (int)(1000 * (1 / (burstPerMinute/60)));
            int delay = (int)(0.9 * 1000); // 25 bpm, 1/(25/60) - (0.5+0.2)*3 = 0.3
            accum += delay;
                    
            Thread.Sleep(delay);
        }
        Console.WriteLine("Length: " + accum + "\n");
        PlayingMutex = false;
    }

    public void Declutter()
    {
        PlayingMutex = true;
        Reset();
                
        int accum = 0; 
        
        //double burstPerMinute = 40 + 10 * breathingVariable;
        double burstPerMinute = 30;
        for (int i = 0; i < 1; i++)
        {   
            var waveFormGen = new WaveformGen(44100, 16, 2);

            for (int j = 0; j < declutterPulseCount; j++)
            {
                waveFormGen.Square(channel1, (declutterPulseLength + declutterPulseDelay) * (j + 1.0 / 2.0), 
                    declutterPulseLength, declutterStartingFrequency, declutterEndingFrequency);
                waveFormGen.Square(channel2, (declutterPulseLength + declutterPulseDelay) * (j + 0.0 / 2.0), 
                    declutterPulseLength, declutterStartingFrequency, declutterEndingFrequency);
            }
            
            int delay = (int)(1000 * (1 / (burstPerMinute/60)));
            accum += delay;
                    
            waveFormGen.Encode();
            streamer = new ByteStream(waveFormGen.ByteStreams, waveFormGen.waveFormat, delay - 10);
            streamer.Play();
            Thread.Sleep(delay);
            Reset();
        }
        Console.WriteLine("Length: " + accum + "\n");
        PlayingMutex = false;
    }

    private void Reset()
    {
        if (streamer != null) streamer.Dispose();
    }
}