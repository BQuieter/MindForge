using Microsoft.AspNetCore.SignalR.Client;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace MindForgeClient
{
    internal class AudioPlayer
    {
        public readonly HubConnection hubConnection;
        private IWavePlayer _waveOut;
        private WaveFormat _waveFormat;
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
                _waveOut?.Stop();
                _waveOut?.Dispose();
            }
        }

        private void PlayAudio(byte[] audioData, WaveFormat waveFormat)
        {
            if (_waveOut != null)
            {
                // Stop existing playback before starting new
                _waveOut.Stop();
                _waveOut.Dispose();
            }

            _waveFormat = waveFormat;
            var memoryStream = new MemoryStream(audioData);
            var rawSourceWaveStream = new RawSourceWaveStream(memoryStream, _waveFormat);

            _waveOut = new WaveOutEvent();
            _waveOut.Init(rawSourceWaveStream);
            _waveOut.Play();
        }


        public void StopPlaying()
        {
            _waveOut?.Stop();
            _waveOut?.Dispose();
        }
    }
}
