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
        var waveFormGen = new WaveformGen();
        waveFormGen.Sine(600.0, 5.0, 0, 0);
        waveFormGen.Sine(100.0, 5.0, 1, 0);
        byteStream = new ByteStream(waveFormGen.ByteStreams, new WaveFormat(44100, 16, 2));
        byteStream.Play();
    }

    [RelayCommand]
    public void StopGenFile()
    {
        byteStream?.Stop();
    }

    [RelayCommand]
    public void PauseGenFile()
    {
        byteStream?.Pause();
    }

    [RelayCommand]
    public void DisposeGenFile()
    {
        byteStream?.Dispose();
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
            
            var waveFormGen = new WaveformGen();
            waveFormGen.Sine(200.0, 1.0, 0, 0);
            waveFormGen.Sine(200.0, 1.0, 1, 2);
            
            FunctionDictionary.Add("SineStep", new ByteStream(waveFormGen.ByteStreams, 
                new WaveFormat(44100, 16, 2))); 
            FunctionDictionary["SineStep"].Play();
        }
        else
        {
            
            var waveFormGen = new WaveformGen();
            waveFormGen.Sine(200.0, 1.0, 0, 0);
            waveFormGen.Sine(200.0, 1.0, 1, 2);
            
            FunctionDictionary.Add("SineStep", new ByteStream(waveFormGen.ByteStreams, 
                new WaveFormat(44100, 16, 2))); 
            FunctionDictionary["SineStep"].Play();
            while (FunctionDictionary["SineStep"].WasapiOut.PlaybackState == PlaybackState.Playing)
            {
                System.Threading.Thread.Sleep(100);
            }
            FunctionDictionary["SineStep"].Dispose();
        }
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
    }
    
    [RelayCommand]
    public void SineLinearFastStop()
    {
    }
    
    
    [RelayCommand]
    public void SineLinearSlowPlay()
    {
    }
    
    [RelayCommand]
    public void SineLinearSlowStop()
    {
    }
    
    
    [RelayCommand]
    public void SineSmoothFastPlay()
    {
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
    }
    
    [RelayCommand]
    public void SquareStepStop()
    {
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
