using Rocket.API;

namespace ShimmyMySherbet.RPCDetector.RocketMod
{
    public class RPCConfig : IRocketPluginConfiguration
    {
        public bool PrintManualRPCCalls = true;
        public bool BlockRPCCalls = false;

        public void LoadDefaults()
        {
        }
    }
}