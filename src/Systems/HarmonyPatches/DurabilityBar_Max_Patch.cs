namespace ExtraInfo;

public static class DurabilityBar_Max_Patch
{
    public static void Postfix(ref int __result, CollectibleObject __instance, ItemStack itemstack)
    {
        switch (__instance)
        {
            case BlockLiquidContainerBase liquidContainerBase:
                __result = (int)(liquidContainerBase.CapacityLitres * 100);
                break;
            default: break;
        }
    }
}