using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;
using static ExtraInfo.TextExtensions;

namespace ExtraInfo;

public class CollectibleBehaviorTreeGrowthDescription : CollectibleBehavior
{
    public CollectibleBehaviorTreeGrowthDescription(CollectibleObject collObj) : base(collObj)
    {
    }

    public override void GetHeldItemInfo(ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
    {
        base.GetHeldItemInfo(inSlot, dsc, world, withDebugInfo);
        AppendInfo(inSlot, dsc, world);
    }

    private static void AppendInfo(ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world)
    {
        NatFloat sproutDays = null;
        NatFloat matureDays = null;

        if (inSlot.Itemstack.Collectible is ItemTreeSeed item)
        {
            var block = world.GetBlock(AssetLocation.Create("sapling-" + item.Variant["type"] + "-free", item.Code.Domain));
            if (block == null) return;

            sproutDays = block.Attributes["sproutDays"].AsObject<NatFloat>();
            matureDays = block.Attributes["matureDays"].AsObject<NatFloat>();
        }
        else if (inSlot.Itemstack.Collectible is BlockPlant block && !string.IsNullOrEmpty(block.EntityClass))
        {
            sproutDays = block.Attributes["sproutDays"].AsObject<NatFloat>();
            matureDays = block.Attributes["matureDays"].AsObject<NatFloat>();
        }

        if (sproutDays == null && matureDays == null) return;

        dsc.AppendLine(ColorText(Lang.Get("Will sprout in about {0} days", GetMinMax(sproutDays))));
        dsc.AppendLine(ColorText(Lang.Get("Will mature in about {0} days", GetMinMax(matureDays))));
    }
}