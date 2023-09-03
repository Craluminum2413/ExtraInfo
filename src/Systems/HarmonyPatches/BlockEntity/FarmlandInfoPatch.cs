using HarmonyLib;
using Vintagestory.GameContent;
using System.Text;

namespace ExtraInfo;

public partial class HarmonyPatches
{
    [HarmonyPatch(typeof(BlockEntityFarmland), nameof(BlockEntityFarmland.GetBlockInfo))]
    public static class FarmlandInfoPatch
    {
        public static void Postfix(BlockEntityFarmland __instance, StringBuilder dsc)
        {
            dsc.GetFarmlandDropSoilChanceInfo(__instance);
            dsc.GetFarmlandInfo(__instance);
        }
    }
}