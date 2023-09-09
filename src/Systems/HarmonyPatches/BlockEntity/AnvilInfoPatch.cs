namespace ExtraInfo;

public partial class HarmonyPatches
{
    [HarmonyPatch(typeof(BlockEntityAnvil), nameof(BlockEntityAnvil.GetBlockInfo))]
    public static class AnvilInfoPatch
    {
        public static void Postfix(BlockEntityAnvil __instance, StringBuilder dsc)
        {
            dsc.GetWorkableTempInfoForAnvil(__instance);
        }
    }
}