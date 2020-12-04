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
        public static DiscordRpcClient client;
    }

    [Cmdlet("Start", "DRPC")]
    public class StartCommand : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            #region init client

            client = new DiscordRpcClient("784135633950081095",
                autoEvents: true, //Should the events be automatically called?
                client: new DiscordRPC.IO.ManagedNamedPipeClient() //The pipe client to use. Required in mono to be changed.);
            );
            client.RegisterUriScheme();
            
            //Subscribe to events
            client.OnReady += (sender, e) => { WriteVerbose($"Received Ready from user {e.User.Username}"); };

            client.OnPresenceUpdate += (sender, e) => { WriteVerbose($"Received Update! {e.Presence}"); };

            client.Initialize();
            client.ClearPresence();

            #endregion

            WriteObject(@"Connected!");
        }
    }

    [Cmdlet("Update", "DRPC")]
    public class UpdateCommand : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            string curdir = this.SessionState.Path.CurrentLocation.Path;
            string dir =
                curdir.Replace(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    "~");
            client.SetPresence(new RichPresence()
            {
                State = dir.Length < 3 ? curdir : dir,
                Details = GetDetails(),
                Assets = new ()
                {
                    LargeImageKey = "pwsh7",
                    LargeImageText = $"{this.Host.Name} {this.Host.Version}"
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
            client.ClearPresence();
            client.Dispose();
        }
    }
}