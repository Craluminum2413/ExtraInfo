namespace ExtraInfo;

public class CollectibleBehaviorTemperatureInName : CollectibleBehavior
{
    public CollectibleBehaviorTemperatureInName(CollectibleObject collObj) : base(collObj)
    {
    }

    public override void GetHeldItemName(StringBuilder sb, ItemStack itemStack)
    {
        base.GetHeldItemName(sb, itemStack);

        StringBuilder dsc = new();

        if (!itemStack.Collectible.HasTemperature(itemStack))
        {
            return;
        }

        ITreeAttribute attr = itemStack.Attributes.GetTreeAttribute(Constants.Text.TemperatureAttr);

        float temperature = attr.GetFloat(Constants.Text.TemperatureAttr, 20f);
        if (temperature > 20f)
        {
            dsc.Append(Constants.Text.TemperatureText(temperature));
            dsc.Append(' ');
        }

        sb.Insert(0, dsc);
    }
}