using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;

namespace ExtraInfo;

public static class ItemstackExtensions
{
    public static ItemStack GetCreatureStack(this EntityProperties entityType, ICoreClientAPI capi)
    {
        var location = entityType.Code.Clone().WithPathPrefix("creature-");
        var item = capi.World.GetItem(location);
        return item == null ? null : new ItemStack(item);
    }
}