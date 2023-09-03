using HarmonyLib;
using Vintagestory.GameContent;
using System.Text;

namespace ExtraInfo;

public partial class HarmonyPatches
{
    [HarmonyPatch(typeof(BlockEntityGroundStorage), nameof(BlockEntityGroundStorage.GetBlockInfo))]
    public static class GroundStorageInfoPatch
    {
        public static void Postfix(BlockEntityGroundStorage __instance, StringBuilder dsc)
        {
            dsc.GetGroundStorageInfo(__instance);
        }
    }
}