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
        var waveFormGen = new WaveformGen();
        waveFormGen.Sine(200.0, 1.0, 0, 0);
        waveFormGen.Sine(200.0, 1.0, 0, 2);
        // waveFormGen.Sine(400.0, 5.0, 2, 4);
        byteStream = new ByteStream(waveFormGen.ByteStreams, new WaveFormat(44100, 16, 1));
    }
}
