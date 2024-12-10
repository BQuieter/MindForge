using Microsoft.AspNetCore.SignalR.Client;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindForgeClient
{
    internal class AudioStreamer
    {
        private readonly HubConnection hubConnection;
        private int chatId;
        private WaveIn waveIn;
        private BufferedWaveProvider bufferedWaveProvider;
        private CancellationTokenSource cancellationTokenSource;
        public AudioStreamer(HubConnection hubConnection)
        {
            this.hubConnection = hubConnection;
        }

        public async Task StartStreamingAsync(int chatId)
        {
            this.chatId = chatId;
            cancellationTokenSource = new CancellationTokenSource();

            // Находим девайс по умолчанию
            var devices = WaveIn.DeviceCount;
            if (devices == 0)
            {
                Console.WriteLine("No microphone devices found.");
                return;
            }

            waveIn = new WaveIn();
            waveIn.DeviceNumber = 0; 
            waveIn.WaveFormat = new WaveFormat(44100, 2); 

            bufferedWaveProvider = new BufferedWaveProvider(waveIn.WaveFormat);
            bufferedWaveProvider.DiscardOnBufferOverflow = true;
            waveIn.DataAvailable += OnDataAvailable;

            try
            {
                waveIn.StartRecording();

                // Ждём окончания потока
                await Task.Delay(-1, cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Audio streaming stopped.");
                waveIn.StopRecording();
                waveIn.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                waveIn.StopRecording();
                waveIn.Dispose();
            }
        }

        private async void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            try
            {
                byte[] buffer = e.Buffer.ToArray();
                WaveFormat waveFormat = waveIn.WaveFormat;
                await hubConnection.SendAsync("Send", buffer, chatId, waveFormat);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending audio: {ex.Message}");
            }
        }


        public void StopStreaming()
        {
            cancellationTokenSource.Cancel();
        }
    }
}

