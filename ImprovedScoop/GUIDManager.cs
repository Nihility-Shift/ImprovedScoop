using CG.Game.Scenarios.Conditions;
using CG.Objects;
using ResourceAssets;
using System.Collections.Generic;
using System.Linq;

namespace ImprovedScoop
{
    internal class GUIDManager
    {
        private static Dictionary<GUIDUnion, string> DisplayNames;

        public static void GenerateDictionaries()
        {
            List<CarryableDef> items = ResourceAssetContainer<CarryableContainer, AbstractCarryableObject, CarryableDef>.Instance?.AssetDescriptions;

            if (items == null || items.Count == 0)
            {
                BepinPlugin.Log.LogInfo("GenerateDictionaries could not load AssetDescriptions");
                return;
            }

            DisplayNames = items.ToDictionary(item => item.AssetGuid, item => item.Asset.DisplayName);
            //GUIDs = items.ToDictionary(item => item.Asset.DisplayName, item => item.Asset.assetGuid);
        }

        public static List<GUIDUnion> GetGUIDs(string displayNameCSV)
        {
            string[] displayNames = displayNameCSV.Split(',');
            List<GUIDUnion> guids = new();


            foreach (string displayName in displayNames)
            {
                //KeyValuePair<GUIDUnion, string> nameMatch = DisplayNames.FirstOrDefault(kvp => kvp.Value.Equals(displayName.Trim(), System.StringComparison.CurrentCultureIgnoreCase));
                //if (string.IsNullOrEmpty(nameMatch.Value)) continue;
                //guids.Add(nameMatch.Key);

                KeyValuePair<GUIDUnion, string> nameMatch = DisplayNames.FirstOrDefault(kvp => kvp.Value.ToLower().Contains(displayName.Trim().ToLower()));
                if (string.IsNullOrEmpty(nameMatch.Value)) continue;
                guids.Add(nameMatch.Key);
            }


            return guids;
        }

        public static string GetDisplayNameList(List<GUIDUnion> guids)
        {
            List<string> str = new();
            foreach (GUIDUnion guid in guids)
            {
                if (DisplayNames.ContainsKey(guid))
                {
                    str.Add(DisplayNames[guid]);
                }
            }
            return string.Join(", ", str);
        }
    }
}
