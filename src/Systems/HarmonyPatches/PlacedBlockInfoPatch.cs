using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace ExtraInfo;

public partial class HarmonyPatches
{
    [HarmonyPatch(typeof(Block), nameof(Block.GetPlacedBlockInfo))]
    public static class PlacedBlockInfoPatch
    {
        public static void Postfix(ref string __result, IWorldAccessor world, BlockPos pos)
        {
            __result = __result.GetCokeInfo(world, pos);
            __result = __result.GetSteelInfo(world, pos);
            __result = __result.GetCharcoalPitInfo(world, pos);
            __result = __result.GetBlockBreakingTimeInfo(world, pos);
        }
    }
}