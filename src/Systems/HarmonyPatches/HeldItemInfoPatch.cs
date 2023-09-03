using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.GameContent;
using System.Text;

namespace ExtraInfo;

public partial class HarmonyPatches
{
    [HarmonyPatch(typeof(CollectibleObject), nameof(CollectibleObject.GetHeldItemInfo))]
    public static class HeldItemInfoPatch
    {
        public static void Postfix(ItemSlot inSlot, IWorldAccessor world, StringBuilder dsc)
        {
            CollectibleObject obj = inSlot.Itemstack.Collectible;
            dsc.GetBombInfo(obj as BlockBomb, null);
            dsc.GetWorkableTempInfoForItem(inSlot, world);
        }
    }
}