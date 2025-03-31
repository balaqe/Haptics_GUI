using System;
using System.Collections.Generic;
using System.IO;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Haptics_GUI.Models;

public class ByteStream
{
    private MultiplexingWaveProvider WaveProvider { get; }
    private WasapiOut WasapiOut { get; }
    
    private WaveFormat WaveFormat { get; init; }

    public ByteStream(List<byte[]?> SampleArrays, WaveFormat? waveFormat)
    {
        // If samples are not present, nothing can be played
        ArgumentNullException.ThrowIfNull(SampleArrays);
        foreach (var samples in SampleArrays)
        {
            ArgumentNullException.ThrowIfNull(samples);
        }

        if (waveFormat.Channels != SampleArrays.Count)
        {
            throw new ArgumentException($"Number of channels do not match: " +
                                        $"SampleArrays.Count = {SampleArrays.Count}" +
                                        $" != " +
                                        $"waveFormat.Channels = {waveFormat.Channels}");
        }

        // If WaveFormat is not passed in default will be 44.1kHz and mono
        WaveFormat = waveFormat != null ? waveFormat : new WaveFormat(44100, 1);

        List<RawSourceWaveStream> waves = [];
        foreach (var samples in SampleArrays)
        {
            waves.Add(new RawSourceWaveStream(new MemoryStream(samples), 
                new WaveFormat(WaveFormat.SampleRate, 1)));
        }
        WaveProvider = new MultiplexingWaveProvider(waves, WaveFormat.Channels);
        WasapiOut = new WasapiOut(AudioClientShareMode.Exclusive, true, 200);
        WasapiOut.Init(WaveProvider);
    }

    public void Play()
    {
        WasapiOut.Play();
    }

    public void Pause()
    {
        WasapiOut.Pause();
    }

    public void Stop()
    {
        WasapiOut.Stop();
    }

    public void Dispose()
    {
        WasapiOut.Dispose();
    }
}