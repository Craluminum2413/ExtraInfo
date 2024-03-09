namespace ExtraInfo;

public class CollectibleBehaviorTreeGrowthDescription : CollectibleBehavior
{
    public CollectibleBehaviorTreeGrowthDescription(CollectibleObject collObj) : base(collObj) { }

    public override void GetHeldItemInfo(ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
    {
        AppendInfo(inSlot, dsc, world);
    }

    private static void AppendInfo(ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world)
    {
        NatFloat sproutDays = null;
        NatFloat matureDays = null;

        if (inSlot.Itemstack.Collectible is ItemTreeSeed item)
        {
            Block block = world.GetBlock(AssetLocation.Create("sapling-" + item.Variant["type"] + "-free", item.Code.Domain));
            if (block == null) return;

            sproutDays = block.Attributes[Constants.Text.SproutDaysAttr].AsObject<NatFloat>();
            matureDays = block.Attributes[Constants.Text.MatureDaysAttr].AsObject<NatFloat>();
        }
        else if (inSlot.Itemstack.Collectible is BlockPlant block && !string.IsNullOrEmpty(block.EntityClass))
        {
            sproutDays = block.Attributes[Constants.Text.SproutDaysAttr].AsObject<NatFloat>();
            matureDays = block.Attributes[Constants.Text.MatureDaysAttr].AsObject<NatFloat>();
        }

        if (sproutDays != null)
        {
            dsc.AppendLine(ColorText(Constants.Text.WillSproutIn(GetMinMax(sproutDays))));
        }
        if (matureDays != null)
        {
            dsc.AppendLine(ColorText(Constants.Text.WillMatureIn(GetMinMax(matureDays))));
        }
    }
}