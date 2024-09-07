using BepInEx.Configuration;
using System.Collections.Generic;
using System.Text;

namespace ImprovedScoop
{
    internal class ScoopConfig
    {
        internal static readonly List<GUIDUnion> ItemBlacklistDefault = new()
        {
            new("ee69440bbce371e458daeba6eee12a49"), //EnemyLuerer
            new("6c37b5363f7ef7844881a301dca76572"), //OxygenTank
            new("ec0fc0790a706ef4facab39da5d9de04"), //PowerFuse
            new("e1bdce573e8182b4d95aacb841301d7c") //Homunculus
        };
        internal static readonly List<GUIDUnion> ItemEjectlistDefault = new()
        {
            new("ee69440bbce371e458daeba6eee12a49"), //EnemyLuerer
        };
        internal const float maxRangeBase = 200f;
        internal const float pullVelocityBase = 10f;
        internal const float catchRadiusBase = 3f;

        internal static ConfigEntry<string> ItemBlacklist;
        internal static ConfigEntry<string> ItemEjectlist;
        internal static ConfigEntry<float> MaxRangeMultiplier;
        internal static ConfigEntry<float> PullVelocityMultiplier;
        internal static ConfigEntry<float> CatchRadiusMultiplier;

        internal static ConfigEntry<bool> ProcessAlloys;
        internal static ConfigEntry<bool> ProcessBiomass;
        internal static ConfigEntry<bool> ProcessShards;
        internal static ConfigEntry<bool> ProcessSummonShards;
        internal static ConfigEntry<bool> MoveToShelf;

        internal static void BindConfigs()
        {
            ItemBlacklist = BepinPlugin.instance.Config.Bind("GravityScoop", "ItemBlacklist", GUIDsToHex(ItemBlacklistDefault));
            ItemEjectlist = BepinPlugin.instance.Config.Bind("GravityScoop", "ItemEjectlist", GUIDsToHex(ItemEjectlistDefault));
            MaxRangeMultiplier = BepinPlugin.instance.Config.Bind("GravityScoop", "MaxRangeMultiplier", 2f);
            PullVelocityMultiplier = BepinPlugin.instance.Config.Bind("GravityScoop", "PullVelocityMultiplier", 3f);
            CatchRadiusMultiplier = BepinPlugin.instance.Config.Bind("GravityScoop", "CatchRadiusMultiplier", 1.5f);
            ProcessAlloys = BepinPlugin.instance.Config.Bind("GravityScoop", "ProcessAlloys", false);
            ProcessBiomass = BepinPlugin.instance.Config.Bind("GravityScoop", "ProcessBiomass", false);
            ProcessShards = BepinPlugin.instance.Config.Bind("GravityScoop", "ProcessShards", false);
            ProcessSummonShards = BepinPlugin.instance.Config.Bind("GravityScoop", "ProcessSummonShards", false);
            MoveToShelf = BepinPlugin.instance.Config.Bind("GravityScoop", "MoveToShelf", false);
        }

        internal static List<GUIDUnion> HexToGUIDs(string str)
        {
            if (string.IsNullOrEmpty(str)) return new List<GUIDUnion>();

            string[] strs = str.Split(',');
            List<GUIDUnion> guids = new();
            foreach (string s in strs)
            {
                guids.Add(new GUIDUnion(s));
            }
            return guids;
        }

        internal static string GUIDsToHex(List<GUIDUnion> guids)
        {
            if (guids.Count == 0) return "";

            StringBuilder sb = new();
            foreach (GUIDUnion guid in guids)
            {
                sb.Append(guid.AsHex());
                sb.Append(",");
            }
            sb.Remove(sb.Length-1, 1);
            return sb.ToString();
        }
    }
}
