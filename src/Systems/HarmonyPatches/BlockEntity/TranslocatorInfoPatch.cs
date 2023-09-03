using HarmonyLib;
using Vintagestory.GameContent;
using System.Text;

namespace ExtraInfo;

public partial class HarmonyPatches
{
    [HarmonyPatch(typeof(BlockEntityStaticTranslocator), nameof(BlockEntityStaticTranslocator.GetBlockInfo))]
    public static class TranslocatorInfoPatch
    {
        public static void Postfix(BlockEntityStaticTranslocator __instance, StringBuilder dsc)
        {
            dsc.GetTranslocatorInfo(__instance);
        }
    }
}