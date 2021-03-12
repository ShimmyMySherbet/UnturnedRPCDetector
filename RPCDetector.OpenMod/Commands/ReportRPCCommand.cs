using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Users;
using ShimmyMySherbet.RPCDetector.Models;

namespace ShimmyMySherbet.RPCDetector.Commands
{
    [CommandDescription("Generates a report of manual RPC Calls"), Command("ReportRPC")]
    public class ReportRPCCommand : Command
    {
        public ReportRPCCommand(IServiceProvider provider) : base(provider)
        {
        }

        protected override async Task OnExecuteAsync()
        {
            await PrintAsync("Generating report...");
            await UniTask.SwitchToThreadPool();
            string report;
            using (MemoryStream stream = new MemoryStream())
            {
                RPCReportGenerator gen = new RPCReportGenerator(stream);
                gen.WriteReport(RPCDetectorCore.Logger);
                report = Encoding.UTF8.GetString(stream.ToArray());
            }

            var paste = PasteAPI.Upload(report);

            string url = $"https://paste.ee/d/{paste.id}";

            await UniTask.SwitchToMainThread();

            await PrintAsync($"RPC Report URL: {url}");

            if (Context.Actor is UnturnedUser uu)
            {
                if (uu.Player != null)
                {
                    uu.Player?.Player?.sendBrowserRequest("Open RPC Report", url);
                }
            }
        }
    }
}