namespace ExtraInfo;

public static class ItemstackExtensions
{
    public static ItemStack GetCreatureStack(this EntityProperties entityType, ICoreClientAPI capi)
    {
        AssetLocation location = entityType.Code.Clone().WithPathPrefix("creature-");
        Item item = capi.World.GetItem(location);
        return item == null ? null : new ItemStack(item);
    }
}