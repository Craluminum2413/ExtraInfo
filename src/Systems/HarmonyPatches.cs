using System.Reflection;
using HarmonyLib;
using Vintagestory.API.Common;
using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.GameContent;
using System.Linq;
using System.Text;
using Vintagestory.API.MathTools;
using Vintagestory.API.Common.Entities;

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

            list.AddEntityHealthAndDamageInfo(inSlot, capi);
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

    [HarmonyPatch(typeof(ModSystemHandbook), methodName: "OnHelpHotkey")]
    public static class OpenHandbookForEntityPatch
    {
        public static void Postfix(ref bool __result, ModSystemHandbook __instance)
        {
            var dialog = __instance.GetField<GuiDialogHandbook>("dialog");
            var capi = __instance.GetField<ICoreClientAPI>("capi");

            if (capi.World.Player.Entity.Controls.ShiftKey && capi.World.Player.CurrentEntitySelection != null)
            {
                Entity entity = capi.World.Player.CurrentEntitySelection.Entity;

                var stack = capi.World.GetEntityType(entity.Code).GetCreatureStack(capi);
                if (stack == null)
                {
                    __result = true;
                    return;
                }

                if (!dialog.OpenDetailPageFor(GuiHandbookItemStackPage.PageCodeForStack(stack)))
                {
                    dialog.OpenDetailPageFor(GuiHandbookItemStackPage.PageCodeForStack(new ItemStack(stack.Collectible)));
                }
            }
            __result = true;
        }
    }

    [HarmonyPatch(typeof(BlockEntityGroundStorage), nameof(BlockEntityGroundStorage.GetBlockInfo))]
    public static class GroundStorageInfoPatch
    {
        public static void Postfix(BlockEntityGroundStorage __instance, StringBuilder dsc)
        {
            dsc.GetGroundStorageInfo(__instance);
        }
    }

    [HarmonyPatch(typeof(CollectibleObject), nameof(CollectibleObject.GetHeldItemInfo))]
    public static class HeldItemInfoPatch
    {
        public static void Postfix(ItemSlot inSlot, IWorldAccessor world, StringBuilder dsc)
        {
            var obj = inSlot.Itemstack.Collectible;
            dsc.GetBombInfo(obj as BlockBomb, null);
            dsc.GetWorkableTempInfoForItem(inSlot, world);
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

    [HarmonyPatch(typeof(BlockEntityAnvil), nameof(BlockEntityAnvil.GetBlockInfo))]
    public static class AnvilInfoPatch
    {
        public static void Postfix(BlockEntityAnvil __instance, StringBuilder dsc)
        {
            dsc.GetWorkableTempInfoForAnvil(__instance);
        }
    }

    [HarmonyPatch(typeof(BlockEntityFarmland), nameof(BlockEntityFarmland.GetBlockInfo))]
    public static class GetFarmlandInfoPatch
    {
        public static void Postfix(BlockEntityFarmland __instance, StringBuilder dsc)
        {
            dsc.GetFarmlandInfo(__instance);
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
            __result = __result.GetBlockBreakingTimeInfo(world, pos);
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