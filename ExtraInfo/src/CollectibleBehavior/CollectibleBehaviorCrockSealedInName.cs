namespace ExtraInfo;

public class CollectibleBehaviorCrockSealedInName : CollectibleBehavior
{
    public CollectibleBehaviorCrockSealedInName(CollectibleObject collObj) : base(collObj) { }

    public override void GetHeldItemName(StringBuilder sb, ItemStack itemStack)
    {
        if (Core.Config == null || !Core.Config.ShowSealedCrockName)
        {
            return;
        }

        StringBuilder dsc = new();
        if (itemStack.Attributes.GetBool(Text.SealedAttr))
        {
            dsc.Append(Text.SealedText);
            dsc.Append(' ');
        }
        sb.Insert(0, dsc);
    }
}