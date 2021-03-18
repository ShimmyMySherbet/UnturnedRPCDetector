using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Rocket.API;
using Rocket.Core.Logging;
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

        public string Syntax => $"ReportRPC (txt/paste)";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "RPCDetector.GenerateReport" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedChat.Say(caller, "Generating RPC report...");

            int mode = 0;
            if (command.Length >= 1)
            {
                switch(command[0].ToLower())
                {
                    case "txt":
                        mode = 0;
                        break;
                    case "paste":
                        mode = 1;
                        break;
                    default:
                        UnturnedChat.Say(caller, "Unknown log mode.");
                        break;
                }
            }

            Task<ReportGen> task = new Task<ReportGen>(() => CreateReport(mode));
            task.ContinueWith((Task<ReportGen> x) =>
            {
                TaskDispatcher.QueueOnMainThread(() =>
                {
                    if (x.IsFaulted || x.IsCanceled)
                    {
                        UnturnedChat.Say(caller, "Failed to generate report.");
                        return;
                    }

                    UnturnedChat.Say(caller, x.Result.Message);
                    if (caller is UnturnedPlayer up && x.Result.ReportURL != null)
                    {
                        up.Player.sendBrowserRequest("Open RPC Report", x.Result.ReportURL);
                    }
                });
            });
            task.Start();
        }

        public ReportGen CreateReport(int tool = 0)
        {
            string report;
            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    RPCReportGenerator gen = new RPCReportGenerator(stream);
                    gen.WriteReport(RPCDetectorCore.Logger);
                    stream.Position = 0;
                    using (StreamReader reader = new StreamReader(stream))
                        report = reader.ReadToEnd();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogError($"Failed to write report: {ex.Message}");
                throw;
            }

            switch (tool)
            {
                case 0:
                    return Source_PasteEE(report);

                case 1:
                    return Source_Log(report);
            }
            return new ReportGen(null, "Failed to upload report.");
        }

        public ReportGen Source_PasteEE(string report)
        {
            PasteResponse paste;
            try
            {
                paste = PasteAPI.Upload(report);
            }
            catch (System.Exception ex)
            {
                Logger.LogError($"Failed to upload report: {ex.Message}");
                throw;
            }
            var url = $"https://paste.ee/d/{paste.id}";
            return new ReportGen(url, $"RPC Report URL: {url}");
        }

        public ReportGen Source_Log(string report)
        {
            string name = $"RPCReport_{DateTime.Now.Ticks}.log";
            string fileName = Path.Combine(Rocket.Core.Environment.LogsDirectory, name);

            File.WriteAllText(fileName, report);

            return new ReportGen(null, $"Report written to {name} in logs folder.");
        }

        public struct ReportGen
        {
            public ReportGen(string url, string msg)
            {
                ReportURL = url;
                Message = msg;
            }

            public string ReportURL;
            public string Message;
        }
    }
}