using System;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Haptics_GUI.Models;

public class SineGen
{
    public static void PlaySine()
    {
        var first = new SignalGenerator()
        {
            Gain = 1,
            Frequency = 20,
            SweepLengthSecs = 2,
            FrequencyEnd = 100,
            Type = SignalGeneratorType.Sin
        }.Take(TimeSpan.FromMilliseconds(200));
        var playlist1 = first.FollowedBy(TimeSpan.FromMilliseconds(800), first);

        var waveProvider = new MultiplexingSampleProvider(new[] { playlist1 }, 8);

        for (int i = 0; i < 8; i++)
        {
            waveProvider.ConnectInputToOutput(0, i);
        }
        /*
        */

        var audio = new WasapiOut(AudioClientShareMode.Exclusive, false, 100);
        try
        {
            //audio.Init(first);
            audio.Init(waveProvider);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Console.WriteLine($"Playback error: {e.Message}");
        }

        try
        {
            audio.Play();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Console.WriteLine($"Playback error: {e.Message}");
        }
        
        // Wait for playback to complete
        while (audio.PlaybackState == PlaybackState.Playing)
        {
            System.Threading.Thread.Sleep(500);
        }

        audio.Dispose();
    }
}