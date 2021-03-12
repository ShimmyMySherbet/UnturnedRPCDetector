using System;
using System.Reflection;
using SDG.Unturned;

namespace ShimmyMySherbet.RPCDetector.Models
{
    public struct RPCCaller
    {
        public string Method;
        public MethodBase Caller;
        public Assembly Assembly;
        public SteamChannel Channel;
        public bool IsInternal => Caller.GetType().Assembly == typeof(Provider).Assembly;
        public Type DeclaringType => Caller.DeclaringType;

        public RPCCaller(string method, SteamChannel channel, MethodBase caller)
        {
            Method = method;
            Caller = caller;
            Channel = channel;
            Assembly = caller.DeclaringType.Assembly;
        }

        public override bool Equals(object obj)
        {
            if (obj is RPCCaller caller)
            {
                return string.Equals(caller.Method, Method, StringComparison.InvariantCultureIgnoreCase)
                    //&& caller.Channel == Channel
                    && caller.Caller == Caller;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}