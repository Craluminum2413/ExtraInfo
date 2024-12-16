namespace ExtraInfo;

public class CollectibleBehaviorTemperatureInName : CollectibleBehavior
{
    public CollectibleBehaviorTemperatureInName(CollectibleObject collObj) : base(collObj) { }

    public override void GetHeldItemName(StringBuilder sb, ItemStack itemStack)
    {
        if (Core.Config == null || !Core.Config.ShowTemperatureInName)
        {
            return;
        }

        StringBuilder dsc = new();
        if (!itemStack.Collectible.HasTemperature(itemStack))
        {
            return;
        }

        ITreeAttribute attr = itemStack.Attributes.GetTreeAttribute(Text.TemperatureAttr);

        float temperature = attr.GetFloat(Text.TemperatureAttr, 20f);
        if (temperature > 20f)
        {
            dsc.Append(Text.TemperatureText(temperature));
            dsc.Append(' ');
        }

        sb.Insert(0, dsc);
    }
}