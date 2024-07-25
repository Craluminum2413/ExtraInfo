namespace ExtraInfo;

public static class DurabilityBar_ShouldDisplay_Patch
{
    public static void Postfix(ref bool __result, CollectibleObject __instance, ItemStack itemstack)
    {
        switch (__instance)
        {
            case BlockLiquidContainerBase liquidContainerBase:
                __result = liquidContainerBase.AllowHeldLiquidTransfer;
                break;
            default: break;
        }
    }
}