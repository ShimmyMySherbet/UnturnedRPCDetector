using HarmonyLib;
using Rocket.API;
using Rocket.Core.Plugins;

namespace ShimmyMySherbet.RPCDetector
{
    public class RPCDetector : RocketPlugin
    {
        public Harmony HarmonyInstance;

        public override void LoadPlugin()
        {
            base.LoadPlugin();
            HarmonyInstance = new Harmony("RPCDetector");
            RPCDetectorCore.Init(HarmonyInstance);
        }

        public override void UnloadPlugin(PluginState state = PluginState.Unloaded)
        {
            base.UnloadPlugin(state);
            HarmonyInstance.UnpatchAll("RPCDetector");
        }
    }
}