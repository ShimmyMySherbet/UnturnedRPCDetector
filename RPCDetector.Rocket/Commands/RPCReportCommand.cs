using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Rocket.API;
using Rocket.Core.Utils;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using ShimmyMySherbet.RPCDetector.Models;

namespace ShimmyMySherbet.RPCDetector.Commands
{
    public class RPCReportCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "ReportRPC";

        public string Help => "Generates an RPC Report";

        public string Syntax => Name;

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "RPCDetector.GenerateReport" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedChat.Say(caller, "Generating RPC report...");

            Task<string> task = new Task<string>(CreateReport);
            task.ContinueWith((x) =>
            {
                TaskDispatcher.QueueOnMainThread(() =>
                {
                    if (x.IsFaulted || x.IsCanceled)
                    {
                        UnturnedChat.Say(caller, "Failed to generate report.");
                        return;
                    }

                    UnturnedChat.Say(caller, $"RPC Report URL: {x.Result}");

                    if (caller is UnturnedPlayer up)
                    {
                        up.Player.sendBrowserRequest("Open RPC Report", x.Result);
                    }
                });
            });
        }

        public string CreateReport()
        {
            string report;
            using (MemoryStream stream = new MemoryStream())
            {
                RPCReportGenerator gen = new RPCReportGenerator(stream);
                gen.WriteReport(RPCDetectorCore.Logger);
                stream.Position = 0;
                using (StreamReader reader = new StreamReader(stream))
                    report = reader.ReadToEnd();
            }

            var paste = PasteAPI.Upload(report);
            return $"https://paste.ee/d/{paste.id}";
        }
    }
}