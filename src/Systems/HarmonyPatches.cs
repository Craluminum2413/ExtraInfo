using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Client;
using Vintagestory.GameContent;

namespace ExtraInfo;

public partial class HarmonyPatches : ModSystem
{
    public const string HarmonyID = "craluminum2413.extrainfo";
    public static Harmony HarmonyInstance { get; set; } = new Harmony(HarmonyID);

    public override bool AllowRuntimeReload => true;

    public override void StartClientSide(ICoreClientAPI api)
    {
        base.StartClientSide(api);
        HarmonyInstance.Patch(original: typeof(Block).GetMethod(nameof(Block.GetPlacedBlockInfo)), postfix: typeof(PlacedBlockInfoPatch).GetMethod(nameof(PlacedBlockInfoPatch.Postfix)));
        HarmonyInstance.Patch(original: typeof(BlockCookedContainerBase).GetMethod(nameof(BlockCookedContainerBase.GetContainedInfo)), postfix: typeof(ContainedNameCookedContainerPatch).GetMethod(nameof(ContainedNameCookedContainerPatch.Postfix)));
        HarmonyInstance.Patch(original: typeof(BlockEntityShelf).GetMethod(nameof(BlockEntityShelf.CrockInfoCompact)), postfix: typeof(CrockInfoCompactShelfPatch).GetMethod(nameof(CrockInfoCompactShelfPatch.Postfix)));
        HarmonyInstance.Patch(original: typeof(CollectibleBehaviorHandbookTextAndExtraInfo).GetMethod(nameof(CollectibleBehaviorHandbookTextAndExtraInfo.GetHandbookInfo)), postfix: typeof(GetHandbookInfoPatch).GetMethod(nameof(GetHandbookInfoPatch.Postfix)));
        HarmonyInstance.Patch(original: typeof(CollectibleObject).GetMethod(nameof(CollectibleObject.GetHeldItemInfo)), postfix: typeof(HeldItemInfoPatch).GetMethod(nameof(HeldItemInfoPatch.Postfix)));
        HarmonyInstance.Patch(original: typeof(ModSystemHandbook).GetMethod("OnHelpHotkey", AccessTools.all), postfix: typeof(OpenHandbookForEntityPatch).GetMethod(nameof(OpenHandbookForEntityPatch.Postfix)));

        HarmonyInstance.Patch(original: typeof(BlockEntity).GetMethod(nameof(BlockEntity.GetBlockInfo)), postfix: typeof(BlockEntityInfoPatch).GetMethod(nameof(BlockEntityInfoPatch.Postfix)));
        HarmonyInstance.Patch(original: typeof(BlockEntityAnvil).GetMethod(nameof(BlockEntityAnvil.GetBlockInfo)), postfix: typeof(AnvilInfoPatch).GetMethod(nameof(AnvilInfoPatch.Postfix)));
        HarmonyInstance.Patch(original: typeof(BlockEntityBeehive).GetMethod(nameof(BlockEntityBeehive.GetBlockInfo)), postfix: typeof(SkepInfoPatch).GetMethod(nameof(SkepInfoPatch.Postfix)));
        HarmonyInstance.Patch(original: typeof(BlockEntityBloomery).GetMethod(nameof(BlockEntityBloomery.GetBlockInfo)), postfix: typeof(BloomeryInfoPatch).GetMethod(nameof(BloomeryInfoPatch.Postfix)));
        HarmonyInstance.Patch(original: typeof(BlockEntityFarmland).GetMethod(nameof(BlockEntityFarmland.GetBlockInfo)), postfix: typeof(FarmlandInfoPatch).GetMethod(nameof(FarmlandInfoPatch.Postfix)));
        HarmonyInstance.Patch(original: typeof(BlockEntityGroundStorage).GetMethod(nameof(BlockEntityGroundStorage.GetBlockInfo)), postfix: typeof(GroundStorageInfoPatch).GetMethod(nameof(GroundStorageInfoPatch.Postfix)));
        HarmonyInstance.Patch(original: typeof(BlockEntityOpenableContainer).GetMethod(nameof(BlockEntityOpenableContainer.GetBlockInfo)), postfix: typeof(ContainerInfoPatch).GetMethod(nameof(ContainerInfoPatch.Postfix)));
        HarmonyInstance.Patch(original: typeof(BlockEntityPitKiln).GetMethod(nameof(BlockEntityPitKiln.GetBlockInfo)), postfix: typeof(PitKilnInfoPatch).GetMethod(nameof(PitKilnInfoPatch.Postfix)));
        HarmonyInstance.Patch(original: typeof(BlockEntityStaticTranslocator).GetMethod(nameof(BlockEntityStaticTranslocator.GetBlockInfo)), postfix: typeof(TranslocatorInfoPatch).GetMethod(nameof(TranslocatorInfoPatch.Postfix)));
    }

    public override void Dispose()
    {
        HarmonyInstance.Unpatch(original: typeof(Block).GetMethod(nameof(Block.GetPlacedBlockInfo)), type: HarmonyPatchType.All, HarmonyID);
        HarmonyInstance.Unpatch(original: typeof(BlockCookedContainerBase).GetMethod(nameof(BlockCookedContainerBase.GetContainedInfo)), type: HarmonyPatchType.All, HarmonyID);
        HarmonyInstance.Unpatch(original: typeof(BlockEntityShelf).GetMethod(nameof(BlockEntityShelf.CrockInfoCompact)), type: HarmonyPatchType.All, HarmonyID);
        HarmonyInstance.Unpatch(original: typeof(CollectibleBehaviorHandbookTextAndExtraInfo).GetMethod(nameof(CollectibleBehaviorHandbookTextAndExtraInfo.GetHandbookInfo)), type: HarmonyPatchType.All, HarmonyID);
        HarmonyInstance.Unpatch(original: typeof(CollectibleObject).GetMethod(nameof(CollectibleObject.GetHeldItemInfo)), type: HarmonyPatchType.All, HarmonyID);
        HarmonyInstance.Unpatch(original: typeof(ModSystemHandbook).GetMethod("OnHelpHotkey", AccessTools.all), type: HarmonyPatchType.All, HarmonyID);

        HarmonyInstance.Unpatch(original: typeof(BlockEntity).GetMethod(nameof(BlockEntity.GetBlockInfo)), type: HarmonyPatchType.All, HarmonyID);
        HarmonyInstance.Unpatch(original: typeof(BlockEntityAnvil).GetMethod(nameof(BlockEntityAnvil.GetBlockInfo)), type: HarmonyPatchType.All, HarmonyID);
        HarmonyInstance.Unpatch(original: typeof(BlockEntityBeehive).GetMethod(nameof(BlockEntityBeehive.GetBlockInfo)), type: HarmonyPatchType.All, HarmonyID);
        HarmonyInstance.Unpatch(original: typeof(BlockEntityBloomery).GetMethod(nameof(BlockEntityBloomery.GetBlockInfo)), type: HarmonyPatchType.All, HarmonyID);
        HarmonyInstance.Unpatch(original: typeof(BlockEntityFarmland).GetMethod(nameof(BlockEntityFarmland.GetBlockInfo)), type: HarmonyPatchType.All, HarmonyID);
        HarmonyInstance.Unpatch(original: typeof(BlockEntityGroundStorage).GetMethod(nameof(BlockEntityGroundStorage.GetBlockInfo)), type: HarmonyPatchType.All, HarmonyID);
        HarmonyInstance.Unpatch(original: typeof(BlockEntityOpenableContainer).GetMethod(nameof(BlockEntityOpenableContainer.GetBlockInfo)), type: HarmonyPatchType.All, HarmonyID);
        HarmonyInstance.Unpatch(original: typeof(BlockEntityPitKiln).GetMethod(nameof(BlockEntityPitKiln.GetBlockInfo)), type: HarmonyPatchType.All, HarmonyID);
        HarmonyInstance.Unpatch(original: typeof(BlockEntityStaticTranslocator).GetMethod(nameof(BlockEntityStaticTranslocator.GetBlockInfo)), type: HarmonyPatchType.All, HarmonyID);
        base.Dispose();
    }
}