using System;
using System.IO;
using System.Linq;
using System.Management.Automation;
using DiscordRPC;
using static PowerShellRPC.Statics;

namespace PowerShellRPC
{
    public static class Statics
    {
        public static DiscordRpcClient Client;
        public static string LargeImageKey = "pwsh7";
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public static string LargeImageText = null;
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public static string SmallImageKey = null;
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public static string SmallImageText = null;
        public static string ApplicationId = "784135633950081095";

        /// <summary>
        /// Shortens a string to the specified <paramref name="length"></paramref>, ending the string with "...".
        /// </summary>
        public static string Shorten(this string str, int length)
        {
            if (str.Length >= length)
            {
                return str.Remove(length - 3) + "...";
            }

            return str;
        }
    }

    [Cmdlet("Start", "DRPC")]
    public class StartCommand : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            #region init client

            Client = new DiscordRpcClient(Statics.ApplicationId,
                autoEvents: true, //Should the events be automatically called?
                client: new DiscordRPC.IO.ManagedNamedPipeClient() //The pipe client to use. Required in mono to be changed.);
            );
            Client.RegisterUriScheme();
            
            //Subscribe to events
            Client.OnReady += (sender, e) => { WriteVerbose($"Received Ready from user {e.User.Username}"); };

            Client.OnPresenceUpdate += (sender, e) => { WriteVerbose($"Received Update! {e.Presence}"); };

            Client.Initialize();
            Client.ClearPresence();

            #endregion

            WriteObject(@"Connected!");
        }
    }

    [Cmdlet("Update", "DRPC")]
    public class UpdateCommand : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            string curdir = this.SessionState.Path.CurrentLocation.Path.Replace('\\', '/');
            string dir =
                curdir.Replace(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile).Replace('\\', '/'),
                    "~");
            Client.SetPresence(new RichPresence()
            {
                State = dir.Length < 3 ? curdir.Shorten(128) : dir.Shorten(128),
                Details = GetDetails(),
                Assets = new ()
                {
                    LargeImageKey = Statics.LargeImageKey,
                    LargeImageText = LargeImageText ?? $"{this.Host.Name} {this.Host.Version}",
                    SmallImageKey = SmallImageKey,
                    SmallImageText = SmallImageText
                }
            });
        }

        static int i = -1;

        private string GetDetails()
        {
            i++;
            if (i == 2) i = 0;
            return i switch
            {
                0 => $"{this.SessionState.Scripts.Count} Scripts",
                1 => $"{this.JobRepository.Jobs.Count} Jobs",
                _ => "THE DEV OF THIS IS A CRINGE"
            };
        }
    }

    [Cmdlet("Stop", "DRPC")]
    public class StopCommand : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            Client.ClearPresence();
            Client.Dispose();
        }
    }
}