using Microsoft.AspNetCore.SignalR.Client;
using NAudio.Wave;
using System.IO;
using System.Windows.Threading;

namespace MindForgeClient
{
    internal class AudioPlayer
    {
        public readonly HubConnection hubConnection;
        private IWavePlayer waveOut;
        private WaveFormat waveFormat;
        public AudioPlayer(HubConnection hubConnection)
        {
            this.hubConnection = hubConnection;
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
            {

                hubConnection.On<byte[], WaveFormat>("GetAudio", (audioData, format) =>
                {
                    PlayAudio(audioData, format);
                });
            }));
        }
        public async Task StartPlayingAsync()
        {
            try
            {
                await Task.Delay(-1);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}"); 
                waveOut?.Stop();
                waveOut?.Dispose();
            }
        }
        private void PlayAudio(byte[] audioData, WaveFormat waveFormat)
        {
            if (waveOut != null)
            {
                waveOut.Stop();
                waveOut.Dispose();
            }

            waveFormat = waveFormat;
            var memoryStream = new MemoryStream(audioData);
            var rawSourceWaveStream = new RawSourceWaveStream(memoryStream, waveFormat);

            waveOut = new WaveOutEvent();
            waveOut.Init(rawSourceWaveStream);
            waveOut.Play();
        }
        public void StopPlaying()
        {
            waveOut?.Stop();
            waveOut?.Dispose();
        }
    }
}
