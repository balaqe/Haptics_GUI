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
        
        FunctionDictionary = new Dictionary<string, ByteStream>();
    }
    
    
    // Survey 1
    [RelayCommand]
    public void SineStepPlay()
    {
        if (FunctionDictionary.ContainsKey("SineStep"))
        {
            FunctionDictionary["SineStep"].Dispose();
            FunctionDictionary.Remove("SineStep");  // Remove the key after disposing
            SineStepHelper(); 
           
        }
        else
        {
            SineStepHelper(); 
        }
    }

    private void SineStepHelper()
    {
        var waveFormGen = new WaveformGen();
        waveFormGen.Sine(100.0, 0.5, 5, 0);
        waveFormGen.Sine(100.0, 0.5, 1, 0);
        
        FunctionDictionary.Add("SineStep", new ByteStream(waveFormGen.ByteStreams, waveFormGen.waveFormat)); 
        FunctionDictionary["SineStep"].Play();
        // FunctionDictionary["SineStep"].Dispose();
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
        if (FunctionDictionary.ContainsKey("SineLinearFast"))
        {
            FunctionDictionary["SineLinearFast"].Dispose();
            FunctionDictionary.Remove("SineLinearFast");  // Remove the key after disposing
            SineLinearFastHelper();
        }
        else
        {
            SineLinearFastHelper();
        }
    }
    
    private void SineLinearFastHelper()
    {
        var waveFormGen = new WaveformGen();
        waveFormGen.Sine(100.0, 0.5, 0, 0);
        waveFormGen.LinearTrans(0, 0, 0.1, 0, 1); // int channelNo, double startTime, double endTime, double startVal, double endVal
        waveFormGen.LinearTrans(0, 0.4, 0.5, 1, 0);
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
        if (FunctionDictionary.ContainsKey("SineLinearSlow"))
        {
            FunctionDictionary["SineLinearSlow"].Dispose();
            FunctionDictionary.Remove("SineLinearSlow");  // Remove the key after disposing
            SineLinearSlowHelper();
        }
        else
        {
            SineLinearSlowHelper();
        }
    }

    private void SineLinearSlowHelper()
    {
        var waveFormGen = new WaveformGen();
        waveFormGen.Sine(100.0, 0.5, 0, 0);
        waveFormGen.LinearTrans(0, 0, 0.25, 0, 1); // int channelNo, double startTime, double endTime, double startVal, double endVal
        waveFormGen.LinearTrans(0, 0.25, 0.5, 1, 0);
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
        if (FunctionDictionary.ContainsKey("SineSmoothFast"))
        {
            FunctionDictionary["SineSmoothFast"].Dispose();
            SineSmoothFastHelper();
        }
        else
        {
            SineSmoothFastHelper();
        }
    }

    private void SineSmoothFastHelper()
    {
        var waveFormGen = new WaveformGen();
        waveFormGen.Sine(200.0, 5.0, 0, 0);
        waveFormGen.SigmoidTrans(0, 0, 2.5, 0, 1); // int channelNo, double startTime, double endTime, double startVal, double endVal
        waveFormGen.SigmoidTrans(0, 2.5, 5, 1, 0);
        // waveFormGen.SigmoidTrans(0, 5, 7.5, 0, 1); // int channelNo, double startTime, double endTime, double startVal, double endVal
        // waveFormGen.SigmoidTrans(0, 7.5, 10, 1, 0); 
        FunctionDictionary.Add("SineSmoothFast", new ByteStream(waveFormGen.ByteStreams, waveFormGen.waveFormat)); 
        FunctionDictionary["SineSmoothFast"].Play();
    }
    
    [RelayCommand]
    public void SineSmoothFastStop()
    {
    }
    
    
    [RelayCommand]
    public void SineSmoothSlowPlay()
    {
    }
    
    [RelayCommand]
    public void SineSmoothSlowStop()
    {
    }
    
    
    
    
    
    [RelayCommand]
    public void SquareStepPlay()
    {
        if (FunctionDictionary.ContainsKey("SquareStep"))
        {
            FunctionDictionary["SquareStep"].Dispose();
            FunctionDictionary.Remove("SquareStep");  // Remove the key after disposing
            SquareStepHelper();   
        }
        else
        {
            SquareStepHelper();
        }
    }

    private void SquareStepHelper()
    {
        var waveFormGen = new WaveformGen();
        waveFormGen.Square(200.0, 0.5, 0, 0);
        
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
    }
    
    [RelayCommand]
    public void SquareLinearFastStop()
    {
    }
    
    
    [RelayCommand]
    public void SquareLinearSlowPlay()
    {
    }
    
    [RelayCommand]
    public void SquareLinearSlowStop()
    {
    }
    
    
    
    [RelayCommand]
    public void SquareSmoothFastPlay()
    {
    }
    
    [RelayCommand]
    public void SquareSmoothFastStop()
    {
    }
    
    
    [RelayCommand]
    public void SquareSmoothSlowPlay()
    {
    }
    
    [RelayCommand]
    public void SquareSmoothSlowStop()
    {
    }
}
