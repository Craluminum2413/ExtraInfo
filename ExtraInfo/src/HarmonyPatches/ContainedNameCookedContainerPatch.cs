namespace ExtraInfo;

[HarmonyPatchCategory("Other")]
[HarmonyPatch(typeof(BlockCookedContainerBase), nameof(BlockCookedContainerBase.GetContainedInfo))]
public static class ContainedNameCookedContainerPatch
{
    [HarmonyPostfix]
    public static void Postfix(ref string __result, ItemSlot inSlot)
    {
        __result = __result.GetCrockSealedInName(inSlot);
    }
}