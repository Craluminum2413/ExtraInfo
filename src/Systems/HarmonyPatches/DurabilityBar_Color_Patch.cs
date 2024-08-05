namespace ExtraInfo;

public static class DurabilityBar_Color_Patch
{
    public static void Postfix(ref int __result, CollectibleObject __instance, ItemStack itemstack, ICoreAPI ___api)
    {
        if (!___api.Side.IsClient()) return;
        switch (__instance)
        {
            case BlockLiquidContainerBase liquidContainerBase:
                int current = liquidContainerBase.GetRemainingDurability(itemstack);
                int max = liquidContainerBase.GetMaxDurability(itemstack);
                int num = GameMath.Clamp(100 * current / max, 0, 99);
                __result = StyleExtensions.LiquidColorGradient[num];
                break;
            default: break;
        }
    }
}