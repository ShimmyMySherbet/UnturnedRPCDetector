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
            var config = Configuration.Instance;
            RPCDetectorCore.Init(HarmonyInstance, config.PrintManualRPCCalls, config.BlockRPCCalls);
        }

        public override void UnloadPlugin(PluginState state = PluginState.Unloaded)
        {
            base.UnloadPlugin(state);
            HarmonyInstance.UnpatchAll("RPCDetector");
        }
    }
}