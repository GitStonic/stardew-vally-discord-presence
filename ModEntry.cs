using System;
using System.Threading;
using System.Timers;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System.Text;

namespace stardew_richpresence
{
    public class ModEntry : Mod
    {
        // PreInitialize RPC Client

        RPC discordRpc;

        public override void Entry(IModHelper helper)
        {
            // RPC Definition
            Thread thread = new Thread(() => {
                discordRpc = new RPC();
                discordRpc.connect();
            });

            thread.Start();

            // Initalization 
            Helper.Events.GameLoop.GameLaunched += GameLaunched;

            // Menu Changed
            Helper.Events.Display.MenuChanged += MenuChanged;

            // In Game
            Helper.Events.GameLoop.DayStarted += AfterStart;
            Helper.Events.GameLoop.TimeChanged += UpdateRpcData;
        }

        private void MenuChanged(object sender, MenuChangedEventArgs e)
        {
            Monitor.Log("Menu Changed");
        }

        private void AfterStart(object sender, DayStartedEventArgs e)
        {
            Monitor.Log("In game");
            
            discordRpc.client.SetPresence(discordRpc.presence);
        }

        private void UpdateRpcData(object sender, TimeChangedEventArgs e)
        {
            Monitor.Log("Update Time Data Event");

            discordRpc.presence.Details = $"{SDate.Now().DayOfWeek}, The {SDate.Now().Day}";
            discordRpc.presence.State = $"Year : {SDate.Now().Year} | Season : {SDate.Now().Season} | Time : {e.NewTime}";

            discordRpc.presence.Assets = new DiscordRPC.Assets()
            {
                LargeImageKey = "icon",
                LargeImageText = "In Game"
            };

            discordRpc.client.SetPresence(discordRpc.presence);
        }

        private void GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            Monitor.Log("Game Launched Event");

            discordRpc.presence.State = "Selecting";
            discordRpc.presence.Details = "In Main Menu";
            discordRpc.presence.Assets = new DiscordRPC.Assets()
            {
                LargeImageKey = "icon",
                LargeImageText = "In Menu"
            };
        }
    }
}
