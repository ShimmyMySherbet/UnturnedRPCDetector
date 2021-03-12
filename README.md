# UnturnedRPCDetector
A tool to detect manual RPC calls from Unturned Plugins.

# Usage
To use this tool, run the plugin on your server for a while. The plugin will collect what plugins make and use manual RPC calls, and where in the plugin sends them.
Once the plugin has ran for a while, use the command `/ReportRPC` to generate a text document report.

The plugin detects manual RPC calls when they are made; it can't detect a call that wasn't made.
So, get the most out of this plugin, install it on your server, and run the server with proper usage for a while. If you just immediately generate an RPC report, it won't work.

# Update Emulation
This plugin also has a feature where it can block all manual RPC calls while leaving all calls from Unturned itself in-tact, to emulate how plugins will act after the update. To enable this feature, set `BlockManualRPCCalls` to true in the config.
**Note:** This can and will cause plugins that make these manual RPC calls act funky, though it's a good test to see how your server will act after the update.

# Installation

This plugin has both RocketMod and OpenMod versions.

**Openmod:** `om install ShimmyMySherbet.RPCDetector`

**Rocketmod:** See [Releases](https://github.com/ShimmyMySherbet/UnturnedRPCDetector/releases/)
