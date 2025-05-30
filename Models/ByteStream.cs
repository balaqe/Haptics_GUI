using System;
using System.Collections.Generic;
using System.IO;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace Haptics_GUI.Models;

public class ByteStream
{
    private MultiplexingWaveProvider WaveProvider { get; }
    public WasapiOut WasapiOut { get; set; }

    private List<RawSourceWaveStream> Waves;
    
    private WaveFormat WaveFormat { get; init; }

    public ByteStream(List<byte[]> sampleArrays, WaveFormat waveFormat)
    {
        // If samples are not present, nothing can be played
        ArgumentNullException.ThrowIfNull(sampleArrays);
        foreach (var samples in sampleArrays)
        {
            ArgumentNullException.ThrowIfNull(samples);
        }

        ArgumentNullException.ThrowIfNull(waveFormat);
        /*
        if (waveFormat.Channels != sampleArrays.Count)
        {
            throw new ArgumentException($"Number of channels do not match: " +
                                        $"sampleArrays.Count = {sampleArrays.Count}" +
                                        $" != " +
                                        $"waveFormat.Channels = {waveFormat.Channels}");
        }
        */

        // If WaveFormat is not passed in default will be 44.1kHz and mono
        WaveFormat = waveFormat;

        Waves = [];
        foreach (var samples in sampleArrays)
        {
            Waves.Add(new RawSourceWaveStream(new MemoryStream(samples), 
                new WaveFormat(WaveFormat.SampleRate, WaveFormat.BitsPerSample, 1)));
        }
        WaveProvider = new MultiplexingWaveProvider(Waves, WaveFormat.Channels);
        WasapiOut = new WasapiOut(AudioClientShareMode.Exclusive, true, 2000);//(int)(1/waveFormat.SampleRate * (double)sampleArrays[0].Length)*1000);
        WasapiOut.Init(WaveProvider);
    }

    public ByteStream(List<byte[]> sampleArrays, WaveFormat waveFormat, int latency)
    {
        if(latency < 0) throw new ArgumentOutOfRangeException(nameof(latency));
        if (waveFormat == null)
        {
            WaveFormat = new WaveFormat();
        }
        else
        {
            WaveFormat = waveFormat;
        }
        
        Waves = [];
        foreach (var samples in sampleArrays)
        {
            Waves.Add(new RawSourceWaveStream(new MemoryStream(samples), 
                new WaveFormat(WaveFormat.SampleRate, WaveFormat.BitsPerSample, 1)));
        }
        WaveProvider = new MultiplexingWaveProvider(Waves, WaveFormat.Channels);

        if (latency > 2000)
        {
            WasapiOut = new WasapiOut(AudioClientShareMode.Exclusive, true, 2000);//(int)(1/waveFormat.SampleRate * (double)sampleArrays[0].Length)*1000);
        }
        else
        {
            WasapiOut = new WasapiOut(AudioClientShareMode.Exclusive, true, latency);
        }

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