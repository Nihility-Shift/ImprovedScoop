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
        internal const float maxRangeBase = 200f;
        internal const float pullVelocityBase = 10f;
        internal const float catchRadiusBase = 3f;

        internal static ConfigEntry<string> ItemBlacklist;
        internal static ConfigEntry<float> MaxRangeMultiplier;
        internal static ConfigEntry<float> PullVelocityMultiplier;
        internal static ConfigEntry<float> CatchRadiusMultiplier;

        internal static void BindConfigs(BepinPlugin plugin)
        {
            ItemBlacklist = plugin.Config.Bind("GravityScoop", "ItemBlacklist", GUIDsToHex(ItemBlacklistDefault));
            MaxRangeMultiplier = plugin.Config.Bind("GravityScoop", "MaxRangeMultiplier", 2f);
            PullVelocityMultiplier = plugin.Config.Bind("GravityScoop", "PullVelocityMultiplier", 3f);
            CatchRadiusMultiplier = plugin.Config.Bind("GravityScoop", "CatchRadiusMultiplier", 1.5f);
        }

        internal static List<GUIDUnion> HexToGUIDs(string str)
        {
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
