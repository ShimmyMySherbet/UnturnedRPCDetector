using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using SDG.Unturned;
using ShimmyMySherbet.RPCDetector.Models;
using Steamworks;
using UnityEngine;
using RLog = Rocket.Core.Logging.Logger;

namespace ShimmyMySherbet.RPCDetector
{
#pragma warning disable CS0612 // ESteamPacket.KICKED ignore

    public static class RPCDetectorCore
    {
        private static Assembly Unturned = typeof(Provider).Assembly;

        public static RPCLogger Logger;

        public const bool PrintUnturnedCalls = false;

        public static RPCDetector Plugin;

        public static bool PrintCalls => Plugin != null ? Plugin.Configuration.Instance.PrintManualRPCCalls : false;
        public static bool BlockCalls => Plugin != null ? Plugin.Configuration.Instance.BlockmanualRPCCalls : false;

        /// <summary>
        /// Initializes the RPCDetectorCore.
        /// </summary>
        public static void Init(Harmony harmony, RPCDetector instance)
        {
            Logger = new RPCLogger();
            Plugin = instance;
            Dictionary<MethodInfo, MethodInfo> patchMappings = new Dictionary<MethodInfo, MethodInfo>();

            foreach (MethodInfo patchMethod in typeof(RPCDetectorCore).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).Where(x => Attribute.IsDefined(x, typeof(MPatch))))
            {
                var patchParameters = patchMethod.GetParameters().ToList();

                patchParameters.RemoveAll(x => x.Name.StartsWith("__"));

                foreach (MethodInfo BaseMethod in typeof(SteamChannel).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
                {
                    var baseParameters = BaseMethod.GetParameters();
                    if (patchMethod.Name.Equals(BaseMethod.Name, StringComparison.InvariantCultureIgnoreCase) && patchParameters.Count == baseParameters.Length)
                    {
                        bool equal = true;
                        for (int i = 0; i < baseParameters.Length; i++)
                        {
                            if (patchParameters[i].ParameterType != baseParameters[i].ParameterType)
                            {
                                equal = false;
                            }
                        }

                        if (equal && !patchMappings.ContainsKey(patchMethod))
                        {
                            patchMappings.Add(patchMethod, BaseMethod);
                        }
                    }
                }
            }

            foreach (var patchMapping in patchMappings)
            {
                harmony.CreateProcessor(patchMapping.Value)
                    .AddPrefix(patchMapping.Key)
                    .Patch();
            }

            Logger.CallReceived += Logger_CallReceived;
            RLog.Log($"Created {patchMappings.Count} patches.");
        }

        public static void Unload(Harmony harmony)
        {
            harmony.UnpatchAll(harmony.Id);
            if (Logger != null)
            {
                Logger.CallReceived -= Logger_CallReceived;
            }
            Logger = null;
            Plugin = null;
        }

        /// <summary>
        /// Gets the last Non-RPCPatches and SteamChannel caller
        /// </summary>
        /// <returns>Caller</returns>
        private static MethodBase GetCaller()
        {
            StackTrace trace = new StackTrace();
            for (int i = 0; i < trace.FrameCount; i++)
            {
                var frame = trace.GetFrame(i);
                bool isCaller = frame.GetMethod().DeclaringType != typeof(RPCDetectorCore)
                             && frame.GetMethod().DeclaringType != typeof(SteamChannel);
                if (isCaller)
                {
                    return frame.GetMethod();
                }
            }
            return null;
        }

        private static bool IsInternal(MethodBase caller) => caller.DeclaringType.Assembly == Unturned;

        private static void Logger_CallReceived(RPCCaller caller)
        {
            if (PrintCalls)
            {
                PrintCaller(caller.Method, caller.Caller);
            }
        }

        /// <summary>
        /// Pretty-Prints the call to the console
        /// </summary>
        private static void PrintCaller(string name, MethodBase caller)
        {
            bool isInternal = IsInternal(caller);
            if (isInternal && !PrintUnturnedCalls) return;

            var prevColor = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("[RPC Call] ");

            if (isInternal)
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write("[Unturned] ");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"[{caller.DeclaringType.Assembly.GetName().Name}] ");
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"{caller.DeclaringType.Name}.{caller.Name}");

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("-> ");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(name + "\n");

            Console.ForegroundColor = prevColor;
        }

        #region "Patches"

        [MPatch]
        public static bool send(SteamChannel __instance, string name, CSteamID steamID, ESteamPacket type, params object[] arguments)
        {
            var caller = GetCaller();
            if (caller != null && !IsInternal(caller))
            {
                lock (Logger)
                    Logger?.TryRegisterCaller(name, caller, __instance);
                return !BlockCalls;
            }
            return true;
        }

        [MPatch]
        public static bool sendAside(SteamChannel __instance, string name, CSteamID steamID, ESteamPacket type, params object[] arguments)
        {
            var caller = GetCaller();
            if (caller != null && !IsInternal(caller))
            {
                lock (Logger)
                    Logger?.TryRegisterCaller(name, caller, __instance);
                return !BlockCalls;
            }
            return true;
        }

        [MPatch]
        public static bool send(SteamChannel __instance, ESteamCall mode, byte bound, ESteamPacket type, int size, byte[] packet)
        {
            if (type == ESteamPacket.KICKED) return !BlockCalls; // ID collision bug
            var caller = GetCaller();
            if (caller != null && !IsInternal(caller))
            {
                lock (Logger)
                    Logger?.TryRegisterCaller(type.ToString(), caller, __instance);
                return !BlockCalls;
            }
            return true;
        }

        [MPatch]
        public static bool send(SteamChannel __instance, string name, ESteamCall mode, byte bound, ESteamPacket type, params object[] arguments)
        {
            var caller = GetCaller();
            if (caller != null && !IsInternal(caller))
            {
                lock (Logger)
                    Logger?.TryRegisterCaller(name, caller, __instance);
                return !BlockCalls;
            }
            return true;
        }

        [MPatch]
        public static bool send(SteamChannel __instance, ESteamCall mode, byte x, byte y, byte area, ESteamPacket type, int size, byte[] packet)
        {
            if (type == ESteamPacket.KICKED) return !BlockCalls; // ID collision bug
            var caller = GetCaller();
            if (caller != null && !IsInternal(caller))
            {
                lock (Logger)
                    Logger?.TryRegisterCaller(type.ToString(), caller, __instance);
                return !BlockCalls;
            }
            return true;
        }

        [MPatch]
        public static bool send(SteamChannel __instance, string name, ESteamCall mode, byte x, byte y, byte area, ESteamPacket type, params object[] arguments)
        {
            var caller = GetCaller();
            if (caller != null && !IsInternal(caller))
            {
                lock (Logger)
                    Logger?.TryRegisterCaller(name, caller, __instance);
                return !BlockCalls;
            }
            return true;
        }

        [MPatch]
        public static bool send(SteamChannel __instance, ESteamCall mode, ESteamPacket type, int size, byte[] packet)
        {
            if (type == ESteamPacket.KICKED) return !BlockCalls; // ID collision bug
            var caller = GetCaller();
            if (caller != null && !IsInternal(caller))
            {
                lock (Logger)
                    Logger?.TryRegisterCaller(type.ToString(), caller, __instance);
                return !BlockCalls;
            }
            return true;
        }

        [MPatch]
        public static bool send(SteamChannel __instance, string name, ESteamCall mode, ESteamPacket type, params object[] arguments)
        {
            var caller = GetCaller();
            if (caller != null && !IsInternal(caller))
            {
                lock (Logger)
                    Logger?.TryRegisterCaller(name, caller, __instance);
                return !BlockCalls;
            }
            return true;
        }

        [MPatch]
        public static bool send(SteamChannel __instance, ESteamCall mode, Vector3 point, float radius, ESteamPacket type, int size, byte[] packet)
        {
            if (type == ESteamPacket.KICKED) return !BlockCalls; // ID collision bug
            var caller = GetCaller();
            if (caller != null && !IsInternal(caller))
            {
                lock (Logger)
                    Logger?.TryRegisterCaller(type.ToString(), caller, __instance);
                return !BlockCalls;
            }
            return true;
        }

        [MPatch]
        public static bool send(SteamChannel __instance, string name, ESteamCall mode, Vector3 point, float radius, ESteamPacket type, params object[] arguments)
        {
            var caller = GetCaller();
            if (caller != null && !IsInternal(caller))
            {
                lock (Logger)
                    Logger?.TryRegisterCaller(name, caller, __instance);
                return !BlockCalls;
            }
            return true;
        }

        #endregion "Patches"
    }
}