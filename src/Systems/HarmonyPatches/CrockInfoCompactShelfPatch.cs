namespace ExtraInfo;

[HarmonyPatch(typeof(BlockEntityShelf), nameof(BlockEntityShelf.CrockInfoCompact))]
public static class CrockInfoCompactShelfPatch
{
    public static void Postfix(ref string __result, ItemSlot inSlot)
    {
        __result = __result.GetCrockSealedInName(inSlot);
    }
}