using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace ExtraInfo;

public static class GroundStorageExtensions
{
    public static bool IsGroundStorage(this IWorldAccessor world, BlockPos pos, out BlockEntityGroundStorage beGroundStorage)
    {
        return (beGroundStorage = world.BlockAccessor.GetBlockEntity(pos) as BlockEntityGroundStorage) != null;
    }

    public static int GetTotalAmount(this BlockEntityGroundStorage beGroundStorage)
    {
        return beGroundStorage?.Inventory?[0]?.StackSize ?? 0;
    }

    public static ItemStack GetContainedStack(this BlockEntityGroundStorage beGroundStorage)
    {
        return beGroundStorage?.Inventory?[0]?.Itemstack;
    }

    public static bool HasSameContent<T>(this T beGroundStorage, T beGroundStorageOther) where T : BlockEntityGroundStorage
    {
        var thisStack = GetContainedStack(beGroundStorage);
        var otherStack = GetContainedStack(beGroundStorageOther);

        if (thisStack == null || otherStack == null) return false;

        var obj = thisStack.Collectible;
        return obj.Equals(thisStack, otherStack, "temperature");
    }
}