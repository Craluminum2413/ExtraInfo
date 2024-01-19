namespace ExtraInfo;

public partial class HarmonyPatches
{
    [HarmonyPatch(typeof(CollectibleBehaviorHandbookTextAndExtraInfo), nameof(CollectibleBehaviorHandbookTextAndExtraInfo.GetHandbookInfo))]
    public static class GetHandbookInfoPatch
    {
        public static void Postfix(ref RichTextComponentBase[] __result, ItemSlot inSlot, ICoreClientAPI capi, ActionConsumable<string> openDetailPageFor)
        {
            List<RichTextComponentBase> list = __result.ToList();

            list.AddEntityHealthAndDamageInfo(inSlot, capi);
            list.AddEntityDropsInfo(inSlot, capi, openDetailPageFor);
            list.AddEntityDropsInfoForDrop(inSlot, capi, openDetailPageFor);
            list.AddPanningDropsInfo(inSlot, capi, openDetailPageFor);
            // list.AddTroughInfo(inSlot, capi, openDetailPageFor);
            list.AddPitKilnInfo(inSlot, capi, openDetailPageFor);
            list.AddTraderPropsInfo(inSlot, capi, openDetailPageFor);
            list.AddTradersInfo(inSlot, capi, openDetailPageFor);
            list.AddEntityDietInfoForBlock(inSlot, capi, openDetailPageFor);
            list.AddEntityDietInfo(inSlot, capi, openDetailPageFor);

            __result = list.ToArray();
        }
    }
}