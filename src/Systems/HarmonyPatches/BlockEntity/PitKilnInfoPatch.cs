namespace ExtraInfo;

public partial class HarmonyPatches
{
    [HarmonyPatch(typeof(BlockEntityPitKiln), nameof(BlockEntityPitKiln.GetBlockInfo))]
    public static class PitKilnInfoPatch
    {
        public static void Postfix(BlockEntityPitKiln __instance, StringBuilder dsc)
        {
            dsc.GetPitKilnInfo(__instance);
        }
    }
}