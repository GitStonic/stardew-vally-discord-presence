using System;
using System.Threading;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using DiscordRPC;
using DiscordRPC.Message;

namespace SVBDP
{
    class ModEntry : Mod
    {
        private static string clientID = "514926552174034963";
        private static DiscordRpcClient client;
        private static bool isRunning = false;
        private static int discordPipe = -1;

        private static RichPresence presence = new RichPresence()
        {
            Details = "Loading...",
            State = "Main Menu",
            Assets = new Assets()
            {
                LargeImageKey = "default",
                LargeImageText = "In Main Menu",
                SmallImageKey = "default"
            }
        };

        public override void Entry(IModHelper helper)
        {
            Monitor.Log("Game has launched!");
            Thread thread = new Thread(() => ConnectRPC());
            thread.Start();
        }

        private void ConnectRPC()
        {
            using (client = new DiscordRpcClient(clientID, true, discordPipe))
            {
                client.OnReady += onReady;
                client.OnError += onError;
                client.OnClose += onClose;

                client.SetPresence(presence);

                client.Initialize();

                RpcLoop();
            }
        }

        private void onReady(object sender, ReadyMessage args)
        {
            Monitor.Log($"Connected to RPC Client, Version : {args.Version}");
        }

        private void onClose(object sender, CloseMessage args)
        {
            Monitor.Log($"Disconnected from the RPC Client, Error : {args.Reason}");
        }

        private void onError(object sender, ErrorMessage args)
        {
            Monitor.Log($"Error occurd within discord. ({args.Message}) ({args.Code})");
        }

        private void RpcLoop()
        {
            // The main RPC Loop, you know for like updating shit and shit...
            isRunning = true;

            while (client != null && isRunning)
            {
                if (client != null) client.Invoke();
                Thread.Sleep(2500);
            }

            client.Dispose();
        }
    }
}
