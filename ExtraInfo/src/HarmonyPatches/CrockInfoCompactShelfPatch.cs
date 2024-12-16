namespace ExtraInfo;

[HarmonyPatchCategory("Other")]
[HarmonyPatch(typeof(BlockEntityShelf), nameof(BlockEntityShelf.CrockInfoCompact))]
public static class CrockInfoCompactShelfPatch
{
    [HarmonyPostfix]
    public static void Postfix(ref string __result, ItemSlot inSlot)
    {
        __result = __result.GetCrockSealedInName(inSlot);
    }
}