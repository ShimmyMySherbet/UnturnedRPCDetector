using Rocket.API;

namespace ShimmyMySherbet.RPCDetector.RocketMod
{
    public class RPCConfig : IRocketPluginConfiguration
    {
        public bool PrintManualRPCCalls = true;
        public bool BlockmanualRPCCalls = false;

        public void LoadDefaults()
        {
        }
    }
}