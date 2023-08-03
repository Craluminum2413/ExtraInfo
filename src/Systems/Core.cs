using Vintagestory.API.Common;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

[assembly: ModInfo(name: "Extra Info", Side = "Universal", RequiredOnClient = false, RequiredOnServer = false)]

namespace ExtraInfo;

public class Core : ModSystem
{
    public override void Start(ICoreAPI api)
    {
        base.Start(api);
        api.RegisterCollectibleBehaviorClass("ExtraInfo:TreeGrowthDescription", typeof(CollectibleBehaviorTreeGrowthDescription));
        api.World.Logger.Event("started 'Extra Info' mod");
    }

    public override void AssetsFinalize(ICoreAPI api)
    {
        if (api.Side != EnumAppSide.Client) return;

        foreach (var obj in api.World.Collectibles)
        {
            if (obj is not (ItemTreeSeed or BlockPlant)) continue;

            obj.CollectibleBehaviors = obj.CollectibleBehaviors.Append(new CollectibleBehaviorTreeGrowthDescription(obj));
        }
    }
}
