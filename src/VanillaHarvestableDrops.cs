using System.Collections.Generic;
using Vintagestory.API.Common;

namespace ExtraInfo;

public class VanillaHarvestableDrops : ModSystem
{
    public static Dictionary<AssetLocation, BlockDropItemStack[]> HarvestableBehaviors { get; set; } = new();

    public BlockDropItemStack[] Drops { get; set; }

    public override void AssetsFinalize(ICoreAPI api)
    {
        foreach (var type in api.World.EntityTypes)
        {
            if (type?.Server == null) continue;
            if (type?.Server?.BehaviorsAsJsonObj == null) continue;
            foreach (var behavior in type.Server.BehaviorsAsJsonObj)
            {
                if (behavior.ToString().Contains("harvestable"))
                {
                    HarvestableBehaviors.Add(type.Code, behavior.AsObject<VanillaHarvestableDrops>().Drops);
                }
            }
        }
    }
}
