namespace ExtraInfo;

[HarmonyPatch(typeof(BlockEntityOpenableContainer), nameof(BlockEntityOpenableContainer.GetBlockInfo))]
public static class ContainerInfoPatch
{
    public static void Postfix(BlockEntityOpenableContainer __instance, StringBuilder dsc)
    {
        dsc.GetQuernInfo(__instance);
    }
}