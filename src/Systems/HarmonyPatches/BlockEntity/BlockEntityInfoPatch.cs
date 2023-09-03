using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.GameContent;
using System.Text;

namespace ExtraInfo;

public partial class HarmonyPatches
{
    [HarmonyPatch(typeof(BlockEntity), nameof(BlockEntity.GetBlockInfo))]
    public static class BlockEntityInfoPatch
    {
        public static void Postfix(BlockEntity __instance, StringBuilder dsc)
        {
            dsc.GetBombInfo(null, __instance as BlockEntityBomb);
            dsc.GetTranslocatorInfo(__instance as BlockEntityStaticTranslocator);
            // dsc.GetMechanicalBlockInfo(__instance);
        }
    }
}