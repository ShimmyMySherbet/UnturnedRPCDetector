using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HarmonyLib;
using SDG.Unturned;

namespace ShimmyMySherbet.RPCDetector.Models
{
    public class RPCReportGenerator
    {
        public Stream Stream;

        public RPCReportGenerator(Stream destination)
        {
            Stream = destination;
        }

        public void WriteReport(RPCLogger loggerClient)
        {
            using (StreamWriter writer = new StreamWriter(Stream, Encoding.UTF8, 1028, true))
            {
                writer.WriteLine("RPC Caller Report");
                writer.WriteLine("Auto-generated using RPCDetector by ShimmyMySherbet#5694");
                writer.WriteLine();

                writer.WriteLine($"Server Name: {Provider.serverName}");
                writer.WriteLine($"Server ID: {Provider.serverID}");
                writer.WriteLine($"Server Version: {Provider.APP_VERSION}");
                writer.WriteLine();
                lock (loggerClient)
                {
                    writer.WriteLine($"Number of plugins using manual RPC Calls: {loggerClient.Logs.Count}.");
                    writer.WriteLine($"Total number of manual RPC callers: {loggerClient.Logs.Sum(x => x.Value.Callers.Count())}.");
                    writer.WriteLine($"Profiling time: {Math.Floor(DateTime.Now.Subtract(loggerClient.Created).TotalMinutes)} min.");
                }

                writer.WriteLine();
                writer.WriteLine("Note: RPCDetector can only detect RPC calls as they are made. This report can be inconclusive if the plugins did not send any manual RPC messages during the profiling time.");
                writer.WriteLine();
                writer.WriteLine();

                writer.WriteLine($"<-------Dump RPC Callers------->");
                writer.WriteLine();
                lock (loggerClient)
                {
                    foreach (var log in loggerClient.Logs.Values)
                    {
                        writer.WriteLine($"<-- {log.Assembly.FullName} -->");
                        writer.WriteLine($"Assembly Name: {log.Assembly.GetName().Name}");
                        writer.WriteLine($"Code Base: {log.Assembly.CodeBase}");
                        writer.WriteLine();

                        Dictionary<Type, List<RPCCaller>> typeCallers = new Dictionary<Type, List<RPCCaller>>();
                        foreach (RPCCaller caller in log.Callers)
                        {
                            if (!typeCallers.ContainsKey(caller.DeclaringType)) typeCallers.Add(caller.DeclaringType, new List<RPCCaller>());
                            typeCallers[caller.DeclaringType].Add(caller);
                        }
                        writer.WriteLine($"Manual RPC Callers: {log.Callers.Count}");
                        writer.WriteLine($"Types with Manual RPC Calls: {typeCallers.Count}");
                        writer.WriteLine("<Callers>");
                        writer.WriteLine();
                        foreach (var typeCaller in typeCallers)
                        {
                            writer.WriteLine($"<Type: {typeCaller.Key.FullName}>");
                            writer.WriteLine();
                            foreach (var caller in typeCaller.Value)
                            {
                                writer.WriteLine($"{caller.Caller.FullDescription()}");
                                writer.WriteLine($"Remote Method: {caller.Method}");
                                writer.WriteLine($"Channel Tag: {caller.Channel.tag}");
                                writer.WriteLine();
                            }
                            writer.WriteLine();
                        }
                        writer.WriteLine();
                    }
                }
            }
        }
    }
}