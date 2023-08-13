using Vintagestory.API.Common;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

[assembly: ModInfo(name: "Extra Info", modID: "extrainfo", Side = "Universal", RequiredOnClient = false, RequiredOnServer = false)]

namespace ExtraInfo;

public class Core : ModSystem
{
    public override void Start(ICoreAPI api)
    {
        base.Start(api);
        api.RegisterCollectibleBehaviorClass("ExtraInfo:TreeGrowthDescription", typeof(CollectibleBehaviorTreeGrowthDescription));
        api.RegisterCollectibleBehaviorClass("ExtraInfo:CrockSealedInName", typeof(CollectibleBehaviorCrockSealedInName));
        api.RegisterCollectibleBehaviorClass("ExtraInfo:TemperatureInName", typeof(CollectibleBehaviorTemperatureInName));
        api.World.Logger.Event("started '{0}' mod", Mod.Info.Name);
    }

    public override void AssetsFinalize(ICoreAPI api)
    {
        if (api.Side != EnumAppSide.Client)
        {
            return;
        }

        foreach (CollectibleObject obj in api.World.Collectibles)
        {
            if (obj is ItemTreeSeed or BlockPlant)
            {
                obj.CollectibleBehaviors = obj.CollectibleBehaviors.Append(new CollectibleBehaviorTreeGrowthDescription(obj));
            }

            if (obj is BlockCrock)
            {
                obj.CollectibleBehaviors = obj.CollectibleBehaviors.Append(new CollectibleBehaviorCrockSealedInName(obj));
            }

            obj.CollectibleBehaviors = obj.CollectibleBehaviors.Append(new CollectibleBehaviorTemperatureInName(obj));
        }
    }
}
