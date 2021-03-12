# UnturnedRPCDetector
A tool to detect manual RPC calls from Unturned Plugins.

To use this tool, run the plugin on your server for a while. The plugin will collect what plugins make and use manual RPC calls, and where in the plugin sends them.

Once the plugin has ran for a while, use the command /ReportRPC to generate a text document report.

The plugin detects manual RPC calls when they are made, so it can't detect a call that wasn't made.
