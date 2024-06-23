using VoidManager.MPModChecks;

namespace ImprovedScoop
{
    public class VoidManagerPlugin : VoidManager.VoidPlugin
    {
        public VoidManagerPlugin()
        {
            VoidManager.Events.Instance.LateUpdate += GravityScoop.CheckAndEject;
        }

        public override MultiplayerType MPType => MultiplayerType.Host;

        public override string Author => "18107, Dragon";

        public override string Description => "Increases Range, Radius, and Pull speed of gravity scoops. Ignores Mission Items, Configurable in-game. \n\nInspired by Maverik's BetterScoop";
    }
}
