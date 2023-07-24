using System.Reflection;
using HarmonyLib;
using Vintagestory.API.Common;
using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.GameContent;
using System.Linq;
using System.Text;
using Vintagestory.API.MathTools;

namespace ExtraInfo;

public class HarmonyPatches : ModSystem
{
    public const string HarmonyID = "craluminum2413.extrainfo";

    public override bool AllowRuntimeReload => true;

    public override void StartClientSide(ICoreClientAPI api)
    {
        base.StartClientSide(api);
        new Harmony(HarmonyID).PatchAll(Assembly.GetExecutingAssembly());
    }

    public override void Dispose()
    {
        new Harmony(HarmonyID).UnpatchAll();
        base.Dispose();
    }

    [HarmonyPatch(typeof(CollectibleBehaviorHandbookTextAndExtraInfo), nameof(CollectibleBehaviorHandbookTextAndExtraInfo.GetHandbookInfo))]
    public static class GetHandbookInfoPatch
    {
        public static void Postfix(ref RichTextComponentBase[] __result, ItemSlot inSlot, ICoreClientAPI capi, ActionConsumable<string> openDetailPageFor)
        {
            List<RichTextComponentBase> list = __result.ToList();

            list.AddEntityDropsInfo(inSlot, capi, openDetailPageFor);
            list.AddEntityDropsInfoForDrop(inSlot, capi, openDetailPageFor);
            list.AddPanningDropsInfo(inSlot, capi, openDetailPageFor);
            list.AddTroughInfo(inSlot, capi, openDetailPageFor);
            list.AddPitKilnInfo(inSlot, capi, openDetailPageFor);
            list.AddTraderPropsInfo(inSlot, capi, openDetailPageFor);
            list.AddTradersInfo(inSlot, capi, openDetailPageFor);
            list.AddEntityDietInfoForBlock(inSlot, capi, openDetailPageFor);
            list.AddEntityDietInfo(inSlot, capi, openDetailPageFor);

            __result = list.ToArray();
        }
    }

    [HarmonyPatch(typeof(CollectibleObject), nameof(CollectibleObject.GetHeldItemInfo))]
    public static class HeldItemInfoPatch
    {
        public static void Postfix(ItemSlot inSlot, StringBuilder dsc)
        {
            if (inSlot.Itemstack.Collectible is BlockBomb blockBomb) dsc.GetBombInfo(blockBomb, null);
        }
    }

    [HarmonyPatch(typeof(BlockEntity), nameof(BlockEntity.GetBlockInfo))]
    public static class BlockEntityInfoPatch
    {
        public static void Postfix(BlockEntity __instance, StringBuilder dsc)
        {
            dsc.GetBombInfo(null, __instance as BlockEntityBomb);
            dsc.GetTransientInfo(__instance as BlockEntityTransient);
            // dsc.GetMechanicalBlockInfo(__instance);
        }
    }

    [HarmonyPatch(typeof(BlockEntityPitKiln), nameof(BlockEntityPitKiln.GetBlockInfo))]
    public static class PitKilnInfoPatch
    {
        public static void Postfix(BlockEntityPitKiln __instance, StringBuilder dsc)
        {
            dsc.GetPitKilnInfo(__instance);
        }
    }

    [HarmonyPatch(typeof(Block), nameof(Block.GetPlacedBlockInfo))]
    public static class PlacedBlockInfoPatch
    {
        public static void Postfix(ref string __result, IWorldAccessor world, BlockPos pos)
        {
            __result = __result.GetCokeInfo(world, pos);
            __result = __result.GetSteelInfo(world, pos);
            __result = __result.GetCharcoalPitInfo(world, pos);
        }
    }

    [HarmonyPatch(typeof(BlockEntityBloomery), nameof(BlockEntityBloomery.GetBlockInfo))]
    public static class BloomeryInfoPatch
    {
        public static void Postfix(BlockEntityBloomery __instance, StringBuilder dsc)
        {
            dsc.GetBloomeryInfo(__instance);
        }
    }

    [HarmonyPatch(typeof(BlockEntityOpenableContainer), nameof(BlockEntityOpenableContainer.GetBlockInfo))]
    public static class ContainerInfoPatch
    {
        public static void Postfix(BlockEntityOpenableContainer __instance, StringBuilder dsc)
        {
            dsc.GetQuernInfo(__instance);
        }
    }

    [HarmonyPatch(typeof(BlockEntityBeehive), nameof(BlockEntityBeehive.GetBlockInfo))]
    public static class SkepInfoPatch
    {
        public static void Postfix(BlockEntityBeehive __instance, StringBuilder dsc)
        {
            dsc.GetSkepInfo(__instance);
        }
    }

    [HarmonyPatch(typeof(BlockEntityStaticTranslocator), nameof(BlockEntityStaticTranslocator.GetBlockInfo))]
    public static class TranslocatorInfoPatch
    {
        public static void Postfix(BlockEntityStaticTranslocator __instance, StringBuilder dsc)
        {
            dsc.GetTranslocatorInfo(__instance);
        }
    }
}