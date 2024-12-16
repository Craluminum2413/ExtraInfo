namespace ExtraInfo;

[HarmonyPatchCategory("Other")]
[HarmonyPatch(typeof(CollectibleBehaviorHandbookTextAndExtraInfo), "addProcessesIntoInfo")]
public static class AddProcessesIntoInfoPatch
{
    [HarmonyPostfix]
    public static void Postfix(ref bool __result, ItemSlot inSlot, ICoreClientAPI capi, ActionConsumable<string> openDetailPageFor, ItemStack stack, List<RichTextComponentBase> components, float marginTop, float marginBottom, bool haveText)
    {
        components.AddBeehiveKilnInfo(stack, capi, openDetailPageFor);
    }
}