namespace ExtraInfo;

public partial class HarmonyPatches
{
    [HarmonyPatch(typeof(BlockEntityBeehive), nameof(BlockEntityBeehive.GetBlockInfo))]
    public static class SkepInfoPatch
    {
        public static void Postfix(BlockEntityBeehive __instance, StringBuilder dsc)
        {
            dsc.GetSkepInfo(__instance);
        }
    }
}