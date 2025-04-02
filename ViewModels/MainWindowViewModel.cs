using System;
using System.Collections.Generic;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Haptics_GUI.Models;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace Haptics_GUI.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] public string filePath;
    [ObservableProperty] public double duration;
    [ObservableProperty] public double fastSmooth;
    [ObservableProperty] public double slowSmooth;
    private AudioFileReader audioFile;
    private WasapiOut audioOut;

    private ByteStream? byteStream;
    
    private IDictionary<string, ByteStream> FunctionDictionary;
    

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
        duration = 0.5;
        fastSmooth = 0.1;
        slowSmooth = 0.25;
        
        FunctionDictionary = new Dictionary<string, ByteStream>();
    }
    
    
    // Survey 1
    [RelayCommand]
    public void SineStepPlay()
    {
        Reset();

        var waveFormGen = new WaveformGen();
        waveFormGen.Sine(100.0, duration, 5, 0);
        waveFormGen.Sine(100.0, duration, 1, 0);
        
        FunctionDictionary.Add("SineStep", new ByteStream(waveFormGen.ByteStreams, waveFormGen.waveFormat)); 
        FunctionDictionary["SineStep"].Play();
    }

    [RelayCommand]
    public void SineStepStop()
    {
        if (FunctionDictionary.ContainsKey("SineStep"))
        {
            FunctionDictionary["SineStep"].Dispose();
        }
    }
    
    
    [RelayCommand]
    public void SineLinearFastPlay()
    {
        Reset();

        var waveFormGen = new WaveformGen();
        waveFormGen.Sine(100.0, duration, 0, 0);
        waveFormGen.LinearTrans(0, 0, fastSmooth, 0, 1); // int channelNo, double startTime, double endTime, double startVal, double endVal
        waveFormGen.LinearTrans(0, (duration-fastSmooth), duration, 1, 0);
        FunctionDictionary.Add("SineLinearFast", new ByteStream(waveFormGen.ByteStreams, waveFormGen.waveFormat)); 
        FunctionDictionary["SineLinearFast"].Play();
    }
    
    [RelayCommand]
    public void SineLinearFastStop()
    {
        if (FunctionDictionary.ContainsKey("SineLinearFast"))
        {
            FunctionDictionary["SineLinearFast"].Dispose();
        }
    }
    
    
    [RelayCommand]
    public void SineLinearSlowPlay()
    {
        Reset();

        var waveFormGen = new WaveformGen();
        waveFormGen.Sine(100.0, duration, 0, 0);
        waveFormGen.LinearTrans(0, 0, slowSmooth, 0, 1); // int channelNo, double startTime, double endTime, double startVal, double endVal
        waveFormGen.LinearTrans(0, (duration-slowSmooth), duration, 1, 0);
        FunctionDictionary.Add("SineLinearSlow", new ByteStream(waveFormGen.ByteStreams, waveFormGen.waveFormat)); 
        FunctionDictionary["SineLinearSlow"].Play();
    }

    [RelayCommand]
    public void SineLinearSlowStop()
    {
        if (FunctionDictionary.ContainsKey("SineLinearSlow"))
        {
            FunctionDictionary["SineLinearSlow"].Dispose();
        }
        
    }
    
    [RelayCommand]
    public void SineSmoothFastPlay()
    {
        Reset();

        var waveFormGen = new WaveformGen();
        waveFormGen.Sine(100.0, duration, 0, 0);
        waveFormGen.SigmoidTrans(0, 0, fastSmooth, 0, 1); // int channelNo, double startTime, double endTime, double startVal, double endVal
        waveFormGen.SigmoidTrans(0, (duration-fastSmooth), duration, 1, 0);
        FunctionDictionary.Add("SineSmoothFast", new ByteStream(waveFormGen.ByteStreams, waveFormGen.waveFormat)); 
        FunctionDictionary["SineSmoothFast"].Play();
    }
    
    [RelayCommand]
    public void SineSmoothFastStop()
    {
        if (FunctionDictionary.ContainsKey("SineSmoothFast"))
        {
            FunctionDictionary["SineSmoothFast"].Dispose();
        }
    }
    
    
    [RelayCommand]
    public void SineSmoothSlowPlay()
    {
        Reset();

        var waveFormGen = new WaveformGen();
        waveFormGen.Sine(100.0, duration, 0, 0);
        waveFormGen.SigmoidTrans(0, 0, slowSmooth, 0, 1); // int channelNo, double startTime, double endTime, double startVal, double endVal
        waveFormGen.SigmoidTrans(0, (duration-slowSmooth), duration, 1, 0);
        FunctionDictionary.Add("SineSmoothSlow", new ByteStream(waveFormGen.ByteStreams, waveFormGen.waveFormat)); 
        FunctionDictionary["SineSmoothSlow"].Play();
    }
    
    [RelayCommand]
    public void SineSmoothSlowStop()
    {
        if (FunctionDictionary.ContainsKey("SineSmoothSlow"))
        {
            FunctionDictionary["SineSmoothSlow"].Dispose();
        }
    }
    
    
    
    
    
    [RelayCommand]
    public void SquareStepPlay()
    {
        Reset();

        var waveFormGen = new WaveformGen();
        waveFormGen.Square(100.0, duration, 0, 0);
        FunctionDictionary.Add("SquareStep", new ByteStream(waveFormGen.ByteStreams, waveFormGen.waveFormat)); 
        FunctionDictionary["SquareStep"].Play();
    }
    
    [RelayCommand]
    public void SquareStepStop()
    {
        if (FunctionDictionary.ContainsKey("SquareStep"))
        {
            FunctionDictionary["SquareStep"].Dispose();
        }
    }
    
    
    [RelayCommand]
    public void SquareLinearFastPlay()
    {
        Reset();

        var waveFormGen = new WaveformGen();
        waveFormGen.Square(100.0, duration, 0, 0);
        waveFormGen.LinearTrans(0, 0, fastSmooth, 0, 1); // int channelNo, double startTime, double endTime, double startVal, double endVal
        waveFormGen.LinearTrans(0, (duration-fastSmooth), duration, 1, 0);
        FunctionDictionary.Add("SquareLinearFast", new ByteStream(waveFormGen.ByteStreams, waveFormGen.waveFormat)); 
        FunctionDictionary["SquareLinearFast"].Play();
    }
    
    [RelayCommand]
    public void SquareLinearFastStop()
    {
        if (FunctionDictionary.ContainsKey("SquareLinearFast"))
        {
            FunctionDictionary["SquareLinearFast"].Dispose();
        }
    }
    
    
    [RelayCommand]
    public void SquareLinearSlowPlay()
    {
        Reset();

        var waveFormGen = new WaveformGen();
        waveFormGen.Square(100.0, duration, 0, 0);
        waveFormGen.LinearTrans(0, 0, slowSmooth, 0, 1); // int channelNo, double startTime, double endTime, double startVal, double endVal
        waveFormGen.LinearTrans(0, (duration-slowSmooth), duration, 1, 0);
        FunctionDictionary.Add("SquareLinearSlow", new ByteStream(waveFormGen.ByteStreams, waveFormGen.waveFormat)); 
        FunctionDictionary["SquareLinearSlow"].Play();
    }
    
    [RelayCommand]
    public void SquareLinearSlowStop()
    {
        if (FunctionDictionary.ContainsKey("SquareLinearSlow"))
        {
            FunctionDictionary["SquareLinearSlow"].Dispose();
        }
    }
    
    
    
    [RelayCommand]
    public void SquareSmoothFastPlay()
    {
        Reset();

        var waveFormGen = new WaveformGen();
        waveFormGen.Square(100.0, duration, 0, 0);
        waveFormGen.SigmoidTrans(0, 0, fastSmooth, 0, 1); // int channelNo, double startTime, double endTime, double startVal, double endVal
        waveFormGen.SigmoidTrans(0, (duration-fastSmooth), duration, 1, 0);
        FunctionDictionary.Add("SquareSmoothFast", new ByteStream(waveFormGen.ByteStreams, waveFormGen.waveFormat)); 
        FunctionDictionary["SquareSmoothFast"].Play();
    }
    
    [RelayCommand]
    public void SquareSmoothFastStop()
    {
        if (FunctionDictionary.ContainsKey("SquareSmoothFast"))
        {
            FunctionDictionary["SquareSmoothFast"].Dispose();
        }
    }
    
    
    [RelayCommand]
    public void SquareSmoothSlowPlay()
    {
        Reset();

        var waveFormGen = new WaveformGen();
        waveFormGen.Square(100.0, duration, 0, 0);
        waveFormGen.SigmoidTrans(0, 0, slowSmooth, 0, 1); // int channelNo, double startTime, double endTime, double startVal, double endVal
        waveFormGen.SigmoidTrans(0, (duration-slowSmooth), duration, 1, 0);
        FunctionDictionary.Add("SquareSmoothSlow", new ByteStream(waveFormGen.ByteStreams, waveFormGen.waveFormat)); 
        FunctionDictionary["SquareSmoothSlow"].Play();
    }
    
    [RelayCommand]
    public void SquareSmoothSlowStop()
    {
        if (FunctionDictionary.ContainsKey("SquareSmoothFlow"))
        {
            FunctionDictionary["SquareSmoothSlow"].Dispose();
        }
    }

    private void Reset()
    {
        foreach (KeyValuePair<string, ByteStream> entry in FunctionDictionary)
        {
            entry.Value.Dispose();
        }

        if (FunctionDictionary.Count > 0) FunctionDictionary.Clear();
    }
}
