namespace ExtraInfo;

[HarmonyPatch(typeof(BlockEntityAnvil), nameof(BlockEntityAnvil.GetBlockInfo))]
public static class AnvilInfoPatch
{
    public static void Postfix(BlockEntityAnvil __instance, StringBuilder dsc)
    {
        dsc.GetWorkableTempInfoForAnvil(__instance);
    }
}