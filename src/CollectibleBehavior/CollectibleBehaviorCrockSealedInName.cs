using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

namespace ExtraInfo;

public class CollectibleBehaviorCrockSealedInName : CollectibleBehavior
{
    public CollectibleBehaviorCrockSealedInName(CollectibleObject collObj) : base(collObj)
    {
    }

    public override void GetHeldItemName(StringBuilder sb, ItemStack itemStack)
    {
        base.GetHeldItemName(sb, itemStack);

        StringBuilder dsc = new();

        if (itemStack.Attributes.GetBool("sealed"))
        {
            dsc.Append('[');
            dsc.Append("<font color=\"#90EE90\">");
            dsc.Append(Lang.Get("Sealed."));
            dsc.Append("</font>");
            dsc.Append("] ");
        }

        sb.Insert(0, dsc);
    }
}