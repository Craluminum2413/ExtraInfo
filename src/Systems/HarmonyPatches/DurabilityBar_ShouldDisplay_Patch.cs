namespace ExtraInfo;

public static class DurabilityBar_ShouldDisplay_Patch
{
    public static void Postfix(ref bool __result, CollectibleObject __instance, ItemStack itemstack, ICoreAPI ___api)
    {
        if (!___api.Side.IsClient()) return;
        switch (__instance)
        {
            case BlockLiquidContainerBase liquidContainerBase:
                __result = liquidContainerBase.AllowHeldLiquidTransfer;
                break;
            default: break;
        }
    }
}