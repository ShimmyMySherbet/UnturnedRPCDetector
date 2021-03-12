# UnturnedRPCDetector
A tool to detect manual RPC calls from Unturned Plugins.

# Usage
To use this tool, run the plugin on your server for a while. The plugin will collect what plugins make and use manual RPC calls, and where in the plugin sends them.
Once the plugin has ran for a while, use the command `/ReportRPC` to generate a text document report.

The plugin detects manual RPC calls when they are made; it can't detect a call that wasn't made.
So, get the most out of this plugin, install it on your server, and run the server with proper usage for a while. If you just immediately generate an RPC report, it won't work.

# Installation

This plugin has both RocketMod and OpenMod versions.

**Openmod:** `om install ShimmyMySherbet.RPCDetector`

**Rocketmod:** See [Releases](https://github.com/ShimmyMySherbet/UnturnedRPCDetector/releases/)
