namespace ExtraInfo;

public partial class HarmonyPatches
{
    [HarmonyPatch(typeof(BlockEntity), nameof(BlockEntity.GetBlockInfo))]
    public static class BlockEntityInfoPatch
    {
        public static void Postfix(BlockEntity __instance, StringBuilder dsc)
        {
            dsc.GetBombInfo(null, __instance as BlockEntityBomb);
            dsc.GetTransientInfo(__instance as BlockEntityTransient);
            // dsc.GetMechanicalBlockInfo(__instance);
        }
    }
}