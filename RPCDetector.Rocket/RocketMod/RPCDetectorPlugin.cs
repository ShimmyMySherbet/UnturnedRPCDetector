using HarmonyLib;
using Rocket.API;
using Rocket.Core.Plugins;
using ShimmyMySherbet.RPCDetector.RocketMod;

namespace ShimmyMySherbet.RPCDetector
{
    public class RPCDetector : RocketPlugin<RPCConfig>
    {
        public Harmony HarmonyInstance;

        public override void LoadPlugin()
        {
            base.LoadPlugin();
            HarmonyInstance = new Harmony("RPCDetector");
            RPCDetectorCore.Init(HarmonyInstance, this);
        }

        public override void UnloadPlugin(PluginState state = PluginState.Unloaded)
        {
            base.UnloadPlugin(state);
            RPCDetectorCore.Unload(HarmonyInstance);
        }
    }
}