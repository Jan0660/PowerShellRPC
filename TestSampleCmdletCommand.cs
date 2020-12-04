using System;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Timers;
using DiscordRPC;
using DiscordRPC.Logging;
using static PowerShellRPC.Statics;
using Discord.Webhook;

namespace PowerShellRPC
{
    public static class Statics
    {
        #region based

        public static DiscordRpcClient client;

        /// <summary>
        /// The pipe to connect too.
        /// </summary>
        public static int discordPipe = -1;

        //Called when your application first starts.
        //For example, just before your main loop, on OnEnable for unity.
        /// <summary>
        /// The level of logging to use.
        /// </summary>
        public static DiscordRPC.Logging.LogLevel logLevel = DiscordRPC.Logging.LogLevel.Trace;

        #endregion
    }

    [Cmdlet("Start", "DRPC")]
    [OutputType(typeof(string))]
    public class TestSampleCmdletCommand : PSCmdlet
    {
        // This method gets called once for each cmdlet in the pipeline when the pipeline starts executing
        protected override void BeginProcessing()
        {
            WriteVerbose("Begin!");
        }

        // This method will be called for each input received from the pipeline to this cmdlet; if no input is received, this method is not called
        protected override void ProcessRecord()
        {
            #region init client

            client = new DiscordRpcClient("784135633950081095",
                pipe: discordPipe, //The pipe number we can locate discord on. If -1, then we will scan.
                //logger: new DiscordRPC.Logging.ConsoleLogger(logLevel, true),          //The loger to get information back from the client.
                autoEvents: true, //Should the events be automatically called?
                client: new DiscordRPC.IO.ManagedNamedPipeClient() //The pipe client to use. Required in mono to be changed.);
            );
            client.RegisterUriScheme();
            //Set the logger
            //client.Logger = new ConsoleLogger() { Level = LogLevel.Warning };

            //Subscribe to events
            client.OnReady += (sender, e) => { WriteVerbose($"Received Ready from user {e.User.Username}"); };

            client.OnPresenceUpdate += (sender, e) => { WriteVerbose($"Received Update! {e.Presence}"); };

            // UpdatePresence();
            client.SetSubscription(EventType.JoinRequest);
            //Connect to the RPC
            client.Initialize();
            client.ClearPresence();

            #endregion

            WriteObject(@"Connected!");
        }

        // This method will be called once at the end of pipeline execution; if no input is received, this method is not called
        protected override void EndProcessing()
        {
            WriteVerbose("End!");
        }
    }

    [Cmdlet("Update", "DRPC")]
    public class UpdateCommand : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            DiscordWebhookClient wclient = new DiscordWebhookClient(
                784428283592441857, "ITPmxTLC87fX0BTrbHv698cnG8RjvmRGL2JNSmJx87D5Jm31gW9__YJ3yDgHgHfFSJ71");
            //wclient.SendMessageAsync("me when piss");
            Console.WriteLine("update");
            string curdir = this.SessionState.Path.CurrentLocation.Path;
            string dir =
                curdir.Replace(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    "~");
            client.SetPresence(new RichPresence()
            {
                State = dir.Length < 3 ? curdir : dir,
                Details = GetDetails(),
                Assets = new Assets()
                {
                    LargeImageKey = "pwsh7",
                    LargeImageText = $"{this.Host.Name} {this.Host.Version}"
                },
                Timestamps = new Timestamps()
                {
                    Start = DateTime.Now
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
}