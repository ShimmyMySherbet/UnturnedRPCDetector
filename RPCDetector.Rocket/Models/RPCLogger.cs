using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using SDG.Unturned;
using ShimmyMySherbet.RPCDetector.Models;

namespace ShimmyMySherbet.RPCDetector
{
    public class RPCLogger
    {
        public Dictionary<Assembly, AssemblyRPCLog> Logs = new Dictionary<Assembly, AssemblyRPCLog>();

        public delegate void CallReceivedArgs(RPCCaller caller);
        public event CallReceivedArgs CallReceived;

        public DateTime Created = DateTime.Now;

        public void TryRegisterCaller(string methodName, MethodBase caller, SteamChannel instance)
        {
            RPCCaller rpcCaller = new RPCCaller(methodName, instance, caller);
            CallReceived?.Invoke(rpcCaller);

            lock (Logs)
            {
                if (!Logs.ContainsKey(caller.DeclaringType.Assembly))
                {
                    Logs.Add(caller.DeclaringType.Assembly, new AssemblyRPCLog() { Assembly = caller.DeclaringType.Assembly });
                }
                Logs[caller.DeclaringType.Assembly].TryRegisterCaller(rpcCaller);
            }
        }
    }
}