using System.Text;
using Vintagestory.API.Common;

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

        if (itemStack.Attributes.GetBool(Constants.Text.SealedAttr))
        {
            dsc.Append(Constants.Text.SealedText);
            dsc.Append(' ');
        }

        sb.Insert(0, dsc);
    }
}