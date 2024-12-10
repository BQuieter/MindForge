using MindForge;
using MindForgeClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows.Threading;
using NAudio.Wave;
using Microsoft.AspNetCore.SignalR.Client;

namespace MindForgeClient
{
    internal class CallHelper
    {
        internal static int ChatId;
        public static bool isMuted = false;
        internal static bool InCall = false;
        internal static ObservableCollection<ProfileInformation> Participants;

        private readonly HubConnection _hubConnection;
        private WaveIn waveIn;
        private BufferedWaveProvider bufferedWaveProvider;
        private CancellationTokenSource _cancellationTokenSource;

        internal async static void LeaveCall()
        {
            if (InCall)
            {
                var response = await HttpClientSingleton.httpClient.GetAsync(App.HttpsStr + $"/call/leave/{ChatId}");
            }
        }


    }
}
