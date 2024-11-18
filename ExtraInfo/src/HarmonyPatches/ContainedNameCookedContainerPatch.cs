namespace ExtraInfo;

[HarmonyPatch(typeof(BlockCookedContainerBase), nameof(BlockCookedContainerBase.GetContainedInfo))]
public static class ContainedNameCookedContainerPatch
{
    public static void Postfix(ref string __result, ItemSlot inSlot)
    {
        __result = __result.GetCrockSealedInName(inSlot);
    }
}