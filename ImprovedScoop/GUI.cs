using CG.Game;
using CG.Ship.Modules;
using Gameplay.Utilities;
using HarmonyLib;
using System;
using System.Reflection;
using VoidManager.CustomGUI;
using static UnityEngine.GUILayout;

namespace ImprovedScoop
{
    internal class GUI : ModSettingsMenu
    {
        private readonly FieldInfo PullVelocityField = AccessTools.Field(typeof(CarryableAttractor), "_pullVelocity");
        private readonly FieldInfo CatchRadiusField = AccessTools.Field(typeof(CarryableAttractor), "_catchRadius");

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
            if (Button("Apply") && float.TryParse(MaxRangeMultiplierString, out float maxRangeMultiplier)) {
                ScoopConfig.MaxRangeMultiplier.Value = maxRangeMultiplier;
                IterateAttractors((attractor) => attractor.MaxRange.SetBaseValue(ScoopConfig.maxRangeBase * maxRangeMultiplier));
            }
            if (Button("Reset"))
            {
                ScoopConfig.MaxRangeMultiplier.Value = (float)ScoopConfig.MaxRangeMultiplier.DefaultValue;
                MaxRangeMultiplierString = $"{ScoopConfig.MaxRangeMultiplier.Value}";
                IterateAttractors((attractor) => attractor.MaxRange.SetBaseValue(ScoopConfig.maxRangeBase * ScoopConfig.MaxRangeMultiplier.Value));
            }
            EndHorizontal();


            //Pull Velocity Multiplier
            BeginHorizontal();
            Label("Pull velocity multiplier:");
            PullVelocityMultiplierString = TextField(PullVelocityMultiplierString, MinWidth(80));
            FlexibleSpace();
            if (Button("Apply") && float.TryParse(PullVelocityMultiplierString, out float pullVelocityMultiplier))
            {
                ScoopConfig.PullVelocityMultiplier.Value = pullVelocityMultiplier;
                IterateAttractors((attractor) => ((ModifiableFloat)PullVelocityField.GetValue(attractor)).SetBaseValue(ScoopConfig.pullVelocityBase * CarryableAttractorPatch.TierModifier(attractor) * pullVelocityMultiplier));
            }
            if (Button("Reset"))
            {
                ScoopConfig.PullVelocityMultiplier.Value = (float)ScoopConfig.PullVelocityMultiplier.DefaultValue;
                PullVelocityMultiplierString = $"{ScoopConfig.PullVelocityMultiplier.Value}";
                IterateAttractors((attractor) => ((ModifiableFloat)PullVelocityField.GetValue(attractor)).SetBaseValue(ScoopConfig.pullVelocityBase * CarryableAttractorPatch.TierModifier(attractor) * ScoopConfig.PullVelocityMultiplier.Value));
            }
            EndHorizontal();


            //Catch Radius Multiplier
            BeginHorizontal();
            Label("Catch radius multiplier:");
            CatchRadiusMultiplierString = TextField(CatchRadiusMultiplierString, MinWidth(80));
            FlexibleSpace();
            if (Button("Apply") && float.TryParse(CatchRadiusMultiplierString, out float catchRadiusMultiplier)) {
                ScoopConfig.CatchRadiusMultiplier.Value = catchRadiusMultiplier;
                IterateAttractors((attractor) => CatchRadiusField.SetValue(attractor, ScoopConfig.catchRadiusBase * catchRadiusMultiplier));
            }
            if (Button("Reset"))
            {
                ScoopConfig.CatchRadiusMultiplier.Value = (float)ScoopConfig.CatchRadiusMultiplier.DefaultValue;
                CatchRadiusMultiplierString = $"{ScoopConfig.CatchRadiusMultiplier.Value}";
                IterateAttractors((attractor) => CatchRadiusField.SetValue(attractor, ScoopConfig.catchRadiusBase * ScoopConfig.CatchRadiusMultiplier.Value));
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
