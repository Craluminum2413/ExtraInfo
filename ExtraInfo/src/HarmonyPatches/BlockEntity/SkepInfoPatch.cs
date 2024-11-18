namespace ExtraInfo;

[HarmonyPatch(typeof(BlockEntityBeehive), nameof(BlockEntityBeehive.GetBlockInfo))]
public static class SkepInfoPatch
{
    public static void Postfix(BlockEntityBeehive __instance, StringBuilder dsc)
    {
        dsc.GetSkepInfo(__instance);
    }
}