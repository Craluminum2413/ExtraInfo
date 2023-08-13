using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;

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

        ITreeAttribute attr = itemStack.Attributes.GetTreeAttribute("temperature");

        float temperature = attr.GetFloat("temperature", 20f);
        if (temperature > 20f)
        {
            dsc.Append('[');
            dsc.Append("<font color=\"#EEEE90\">");
            dsc.AppendFormat(Lang.Get("{0}°C"), (int)temperature);
            dsc.Append("</font>");
            dsc.Append("] ");
        }

        sb.Insert(0, dsc);
    }
}