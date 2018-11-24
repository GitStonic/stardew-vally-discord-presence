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

        // Current Game Data
        private int day;
        private int year;
        private string time;
        private string season;
        private DayOfWeek dayOfWeek;

        private static RichPresence presence = new RichPresence()
        {
            Details = "Selecting...",
            State = "Main Menu",
            Assets = new Assets()
            {
                LargeImageKey = "default",
                LargeImageText = "In Main Menu"
            },
            Timestamps = Timestamps.Now
        };

        public override void Entry(IModHelper helper)
        {
            Monitor.Log("Game has launched!");
            
            helper.Events.GameLoop.DayStarted += AfterStart;

            // RPC Client
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
       
        private void AfterStart(object sender, EventArgs e)
        {
            Monitor.Log("Save Event!");

            Monitor.Log("Updating game data");

            presence.State = $"Day : {SDate.Now().DayOfWeek}, The {SDate.Now().Day}";
            presence.Details = $"Season : {SDate.Now().Season} | Year : {SDate.Now().Year}";
            presence.Assets = new Assets
            {
                LargeImageKey = "default",
                LargeImageText = "In Game"
            };

            client.SetPresence(presence);
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

        public void onReady(object sender, ReadyMessage args)
        {
            Monitor.Log($"Connected to RPC Client, Version : {args.Version}");
        }

        public void onClose(object sender, CloseMessage args)
        {
            Monitor.Log($"Disconnected from the RPC Client, Error : {args.Reason}");
        }

        public void onError(object sender, ErrorMessage args)
        {
            Monitor.Log($"Error occurd within discord. ({args.Message}) ({args.Code})");
        }
    }
}
