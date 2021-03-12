using System;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OpenMod.API.Plugins;
using OpenMod.Unturned.Plugins;

[assembly: PluginMetadata("RPCDetector", DisplayName = "RPCDetector")]

namespace ShimmyMySherbet.RPCDetector
{
    public class RPCDetectorPlugin : OpenModUnturnedPlugin
    {
        private readonly IConfiguration m_Configuration;
        private readonly IStringLocalizer m_StringLocalizer;
        public readonly ILogger<RPCDetectorPlugin> m_Logger;

        public bool PrintRPCCalls => m_Configuration.GetValue<bool>("PrintRPCCalls");
        public bool BlockRPCCalls => m_Configuration.GetValue<bool>("BlockManualRPCCalls");

        public RPCDetectorPlugin(
            IConfiguration configuration,
            IStringLocalizer stringLocalizer,
            ILogger<RPCDetectorPlugin> logger,
            IServiceProvider serviceProvider) : base(serviceProvider)
        {
            m_Configuration = configuration;
            m_StringLocalizer = stringLocalizer;
            m_Logger = logger;
        }

        protected override UniTask OnLoadAsync()
        {
            RPCDetectorCore.Init(Harmony, this);
            return UniTask.CompletedTask;
        }

        protected override UniTask OnUnloadAsync()
        {
            RPCDetectorCore.Unload(Harmony);
            return UniTask.CompletedTask;
        }
    }
}