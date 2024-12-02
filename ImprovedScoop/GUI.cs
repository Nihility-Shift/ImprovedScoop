using CG.Game;
using CG.Ship.Modules;
using System;
using VoidManager.CustomGUI;
using static UnityEngine.GUILayout;

namespace ImprovedScoop
{
    internal class GUI : ModSettingsMenu
    {
        private string ItemBlacklistString;
        private string ItemEjectlistString;
        private string MaxRangeMultiplierString = $"{ScoopConfig.MaxRangeMultiplier.Value}";
        private string PullVelocityMultiplierString = $"{ScoopConfig.PullVelocityMultiplier.Value}";
        private string CatchRadiusMultiplierString = $"{ScoopConfig.CatchRadiusMultiplier.Value}";

        public override string Name() => MyPluginInfo.PLUGIN_NAME;

        public override void OnOpen()
        {
            ItemBlacklistString = GUIDManager.GetDisplayNameList(CarryableAttractorPatch.dotNotAttract);
            ItemEjectlistString = GUIDManager.GetDisplayNameList(GravityScoopEject.eject);
        }

        public override void Draw()
        {

            //Item Blacklist
            BeginHorizontal();
            Label("Ignore these items:");
            ItemBlacklistString = TextField(ItemBlacklistString, MinWidth(80));
            FlexibleSpace();
            if (Button("Apply"))
            {
                CarryableAttractorPatch.dotNotAttract = GUIDManager.GetGUIDs(ItemBlacklistString);
                ItemBlacklistString = GUIDManager.GetDisplayNameList(CarryableAttractorPatch.dotNotAttract);
                ScoopConfig.ItemBlacklist.Value = ScoopConfig.GUIDsToHex(GUIDManager.GetGUIDs(ItemBlacklistString));
            }
            if (Button("Reset"))
            {
                CarryableAttractorPatch.dotNotAttract = ScoopConfig.ItemBlacklistDefault;
                ScoopConfig.ItemBlacklist.Value = ScoopConfig.GUIDsToHex(CarryableAttractorPatch.dotNotAttract);
                ItemBlacklistString = GUIDManager.GetDisplayNameList(CarryableAttractorPatch.dotNotAttract);
            }
            EndHorizontal();


            //Item Ejectlist
            BeginHorizontal();
            Label("Eject these items:");
            ItemEjectlistString = TextField(ItemEjectlistString, MinWidth(80));
            FlexibleSpace();
            if (Button("Apply"))
            {
                GravityScoopEject.eject = GUIDManager.GetGUIDs(ItemEjectlistString);
                ItemEjectlistString = GUIDManager.GetDisplayNameList(GravityScoopEject.eject);
                ScoopConfig.ItemEjectlist.Value = ScoopConfig.GUIDsToHex(GUIDManager.GetGUIDs(ItemEjectlistString));
            }
            if (Button("Reset"))
            {
                GravityScoopEject.eject = ScoopConfig.ItemEjectlistDefault;
                ScoopConfig.ItemEjectlist.Value = ScoopConfig.GUIDsToHex(GravityScoopEject.eject);
                ItemEjectlistString = GUIDManager.GetDisplayNameList(GravityScoopEject.eject);
            }
            EndHorizontal();


            //Max Range Multiplier
            BeginHorizontal();
            Label("Max range multiplier:");
            MaxRangeMultiplierString = TextField(MaxRangeMultiplierString, MinWidth(80));
            FlexibleSpace();
            if (Button("Apply") && float.TryParse(MaxRangeMultiplierString, out float maxRangeMultiplier)) 
            {
                IterateAttractors((attractor) => attractor.MaxRange.SetBaseValue(attractor.MaxRange.BaseValue / ScoopConfig.MaxRangeMultiplier.Value));
                ScoopConfig.MaxRangeMultiplier.Value = maxRangeMultiplier;
                IterateAttractors((attractor) => attractor.MaxRange.SetBaseValue(attractor.MaxRange.BaseValue * maxRangeMultiplier));
            }
            if (Button("Reset"))
            {
                IterateAttractors((attractor) => attractor.MaxRange.SetBaseValue(attractor.MaxRange.BaseValue / ScoopConfig.MaxRangeMultiplier.Value));
                ScoopConfig.MaxRangeMultiplier.Value = (float)ScoopConfig.MaxRangeMultiplier.DefaultValue;
                MaxRangeMultiplierString = $"{ScoopConfig.MaxRangeMultiplier.Value}";
                IterateAttractors((attractor) => attractor.MaxRange.SetBaseValue(attractor.MaxRange.BaseValue * ScoopConfig.MaxRangeMultiplier.Value));
            }
            EndHorizontal();


            //Pull Velocity Multiplier
            BeginHorizontal();
            Label("Pull velocity multiplier:");
            PullVelocityMultiplierString = TextField(PullVelocityMultiplierString, MinWidth(80));
            FlexibleSpace();
            if (Button("Apply") && float.TryParse(PullVelocityMultiplierString, out float pullVelocityMultiplier))
            {
                IterateAttractors((attractor) => attractor._pullVelocity.SetBaseValue(attractor._pullVelocity.BaseValue / ScoopConfig.PullVelocityMultiplier.Value));
                ScoopConfig.PullVelocityMultiplier.Value = pullVelocityMultiplier;
                IterateAttractors((attractor) => attractor._pullVelocity.SetBaseValue(attractor._pullVelocity.BaseValue * pullVelocityMultiplier));
            }
            if (Button("Reset"))
            {
                IterateAttractors((attractor) => attractor._pullVelocity.SetBaseValue(attractor._pullVelocity.BaseValue / ScoopConfig.PullVelocityMultiplier.Value));
                ScoopConfig.PullVelocityMultiplier.Value = (float)ScoopConfig.PullVelocityMultiplier.DefaultValue;
                PullVelocityMultiplierString = $"{ScoopConfig.PullVelocityMultiplier.Value}";
                IterateAttractors((attractor) => attractor._pullVelocity.SetBaseValue(attractor._pullVelocity.BaseValue * ScoopConfig.PullVelocityMultiplier.Value));
            }
            EndHorizontal();


            //Catch Radius Multiplier
            BeginHorizontal();
            Label("Catch radius multiplier:");
            CatchRadiusMultiplierString = TextField(CatchRadiusMultiplierString, MinWidth(80));
            FlexibleSpace();
            if (Button("Apply") && float.TryParse(CatchRadiusMultiplierString, out float catchRadiusMultiplier))
            {
                IterateAttractors((attractor) => attractor._catchRadius /= ScoopConfig.CatchRadiusMultiplier.Value);
                ScoopConfig.CatchRadiusMultiplier.Value = catchRadiusMultiplier;
                IterateAttractors((attractor) => attractor._catchRadius *= catchRadiusMultiplier);
            }
            if (Button("Reset"))
            {
                IterateAttractors((attractor) => attractor._catchRadius /= ScoopConfig.CatchRadiusMultiplier.Value);
                ScoopConfig.CatchRadiusMultiplier.Value = (float)ScoopConfig.CatchRadiusMultiplier.DefaultValue;
                CatchRadiusMultiplierString = $"{ScoopConfig.CatchRadiusMultiplier.Value}";
                IterateAttractors((attractor) => attractor._catchRadius *= ScoopConfig.CatchRadiusMultiplier.Value);
            }
            EndHorizontal();
        }

        private void IterateAttractors(Action<CarryableAttractor> action)
        {
            CarryableAttractor[] attractors = ClientGame.Current?.PlayerShip?.GetComponentsInChildren<CarryableAttractor>();
            if (attractors == null) return;
            foreach (CarryableAttractor attractor in attractors)
            {
                action(attractor);
            }
        }
    }
}
