namespace ExtraInfo;

[HarmonyPatchCategory("RemoveTradeHandbookInfo")]
[HarmonyPatch(typeof(TradeHandbookInfo), "AddTraderHandbookInfo")]
public static class RemoveVanillaTradeHandbookInfoPatch
{
    [HarmonyPrefix]
    public static bool Prefix() => false;
}