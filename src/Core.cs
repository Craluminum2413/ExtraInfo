using Vintagestory.API.Client;
using Vintagestory.API.Common;

[assembly: ModInfo("Extra Info")]

namespace ExtraInfo;

public class Core : ModSystem
{
    public override void StartClientSide(ICoreClientAPI api)
    {
        base.StartClientSide(api);
        api.World.Logger.Event("started 'Extra Info' mod");
    }
}
