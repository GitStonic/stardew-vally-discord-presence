using System;
using DiscordRPC;
using DiscordRPC.IO;
using DiscordRPC.RPC;
using System.Threading;
using System.Threading.Tasks;
using DiscordRPC.Logging;
using DiscordRPC.Message;

namespace stardew_richpresence
{
    class RPC
    {
        private string clientID = "514926552174034963";
        private static bool isRunning = false;
        private static int discordPipe = -1;

        public DiscordRpcClient client;
        public RichPresence presence = new RichPresence();

        public void connect()
        {
            using (client = new DiscordRpcClient(clientID, true, discordPipe))
            {
                client.OnReady += onReady;
                client.OnError += onError;
                client.OnClose += onClose;

                client.SetPresence(presence);

                client.Initialize();

                runLoop();
            }
        }

        private void runLoop()
        {
            isRunning = true;

            while (client != null && isRunning)
            {
                if (client != null) client.Invoke();
                Thread.Sleep(2500);
            }

            client.Dispose();
        }

        private void onReady(object sender, ReadyMessage args)
        {
        }

        private void onError(object sender, ErrorMessage args)
        {
            isRunning = false;
        }

        private void onClose(object sender, CloseMessage args)
        {
            isRunning = false;
        }
    }
}
