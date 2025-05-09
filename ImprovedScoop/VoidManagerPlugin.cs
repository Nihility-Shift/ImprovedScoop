﻿using VoidManager;
using VoidManager.MPModChecks;

namespace ImprovedScoop
{
    public class VoidManagerPlugin : VoidPlugin
    {
        public VoidManagerPlugin()
        {
            GUIDManager.GenerateDictionaries();
        }

        public override MultiplayerType MPType => MultiplayerType.Host;

        public override string Author => MyPluginInfo.PLUGIN_AUTHORS;

        public override string Description => MyPluginInfo.PLUGIN_DESCRIPTION;

        public override string ThunderstoreID => MyPluginInfo.PLUGIN_THUNDERSTORE_ID;

        public override SessionChangedReturn OnSessionChange(SessionChangedInput input)
        {
            return new SessionChangedReturn() { SetMod_Session = true};
        }
    }
}
