namespace ExtraInfo;

public partial class HarmonyPatches
{
    [HarmonyPatch(typeof(BlockEntityBloomery), nameof(BlockEntityBloomery.GetBlockInfo))]
    public static class BloomeryInfoPatch
    {
        public static void Postfix(BlockEntityBloomery __instance, StringBuilder dsc)
        {
            dsc.GetBloomeryInfo(__instance);
        }
    }
}