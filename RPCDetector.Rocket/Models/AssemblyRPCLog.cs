using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ShimmyMySherbet.RPCDetector.Models
{
    public class AssemblyRPCLog
    {
        public Assembly Assembly;
        public List<RPCCaller> Callers = new List<RPCCaller>();

        public void TryRegisterCaller(RPCCaller caller)
        {
            lock(Callers)
            {
                if (!Callers.Contains(caller))
                {
                    Callers.Add(caller);
                }
            }
        }
    }
}
