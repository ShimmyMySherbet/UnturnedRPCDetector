using System.Collections.Generic;
using System.IO;
using System.Text;
using Rocket.API;
using Rocket.Core.Logging;
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

        /*
         * No point running this async, since it locks the RPC logger,
         * which in turn will lock all RPC calls
         */

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedChat.Say(caller, "Generating report...");
            string report;
            using (MemoryStream stream = new MemoryStream())
            {
                RPCReportGenerator gen = new RPCReportGenerator(stream);
                gen.WriteReport(RPCDetectorCore.Logger);
                report = Encoding.UTF8.GetString(stream.ToArray());
            }

            var paste = PasteAPI.Upload(report);

            string url = $"https://paste.ee/d/{paste.id}";

            Logger.Log($"RPC Report URL: {url}");
            if (caller is UnturnedPlayer pl)
            {
                pl.Player.sendBrowserRequest("RPC Detector Report", url);
            }
        }
    }
}