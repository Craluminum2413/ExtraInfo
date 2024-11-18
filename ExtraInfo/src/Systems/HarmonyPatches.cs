namespace ExtraInfo;

public class HarmonyPatches : ModSystem
{
    public Harmony HarmonyInstance => new(Mod.Info.ModID);

    public override void StartClientSide(ICoreClientAPI api)
    {
        HarmonyInstance.Patch(original: typeof(Block).GetMethod(nameof(Block.GetPlacedBlockInfo)), postfix: typeof(PlacedBlockInfoPatch).GetMethod(nameof(PlacedBlockInfoPatch.Postfix)));
        HarmonyInstance.Patch(original: typeof(BlockCookedContainerBase).GetMethod(nameof(BlockCookedContainerBase.GetContainedInfo)), postfix: typeof(ContainedNameCookedContainerPatch).GetMethod(nameof(ContainedNameCookedContainerPatch.Postfix)));
        HarmonyInstance.Patch(original: typeof(BlockEntityShelf).GetMethod(nameof(BlockEntityShelf.CrockInfoCompact)), postfix: typeof(CrockInfoCompactShelfPatch).GetMethod(nameof(CrockInfoCompactShelfPatch.Postfix)));
        HarmonyInstance.Patch(original: typeof(CollectibleBehaviorHandbookTextAndExtraInfo).GetMethod(nameof(CollectibleBehaviorHandbookTextAndExtraInfo.GetHandbookInfo)), postfix: typeof(GetHandbookInfoPatch).GetMethod(nameof(GetHandbookInfoPatch.Postfix)));
        HarmonyInstance.Patch(original: typeof(CollectibleObject).GetMethod(nameof(CollectibleObject.GetHeldItemInfo)), postfix: typeof(HeldItemInfoPatch).GetMethod(nameof(HeldItemInfoPatch.Postfix)));
        HarmonyInstance.Patch(original: typeof(ModSystemSurvivalHandbook).GetMethod("OnSurvivalHandbookHotkey", AccessTools.all), postfix: typeof(OpenHandbookForEntityPatch).GetMethod(nameof(OpenHandbookForEntityPatch.Postfix)));

        HarmonyInstance.Patch(original: typeof(BlockEntity).GetMethod(nameof(BlockEntity.GetBlockInfo)), postfix: typeof(BlockEntityInfoPatch).GetMethod(nameof(BlockEntityInfoPatch.Postfix)));
        HarmonyInstance.Patch(original: typeof(BlockEntityAnvil).GetMethod(nameof(BlockEntityAnvil.GetBlockInfo)), postfix: typeof(AnvilInfoPatch).GetMethod(nameof(AnvilInfoPatch.Postfix)));
        HarmonyInstance.Patch(original: typeof(BlockEntityBeehive).GetMethod(nameof(BlockEntityBeehive.GetBlockInfo)), postfix: typeof(SkepInfoPatch).GetMethod(nameof(SkepInfoPatch.Postfix)));
        HarmonyInstance.Patch(original: typeof(BlockEntityBloomery).GetMethod(nameof(BlockEntityBloomery.GetBlockInfo)), postfix: typeof(BloomeryInfoPatch).GetMethod(nameof(BloomeryInfoPatch.Postfix)));
        HarmonyInstance.Patch(original: typeof(BlockEntityFarmland).GetMethod(nameof(BlockEntityFarmland.GetBlockInfo)), postfix: typeof(FarmlandInfoPatch).GetMethod(nameof(FarmlandInfoPatch.Postfix)));
        HarmonyInstance.Patch(original: typeof(BlockEntityGroundStorage).GetMethod(nameof(BlockEntityGroundStorage.GetBlockInfo)), postfix: typeof(GroundStorageInfoPatch).GetMethod(nameof(GroundStorageInfoPatch.Postfix)));
        HarmonyInstance.Patch(original: typeof(BlockEntityOpenableContainer).GetMethod(nameof(BlockEntityOpenableContainer.GetBlockInfo)), postfix: typeof(ContainerInfoPatch).GetMethod(nameof(ContainerInfoPatch.Postfix)));
        HarmonyInstance.Patch(original: typeof(BlockEntityPitKiln).GetMethod(nameof(BlockEntityPitKiln.GetBlockInfo)), postfix: typeof(PitKilnInfoPatch).GetMethod(nameof(PitKilnInfoPatch.Postfix)));
        HarmonyInstance.Patch(original: typeof(BlockEntityStaticTranslocator).GetMethod(nameof(BlockEntityStaticTranslocator.GetBlockInfo)), postfix: typeof(TranslocatorInfoPatch).GetMethod(nameof(TranslocatorInfoPatch.Postfix)));

        HarmonyInstance.Patch(original: typeof(CollectibleObject).GetMethod(nameof(CollectibleObject.GetItemDamageColor)), postfix: typeof(DurabilityBar_Color_Patch).GetMethod(nameof(DurabilityBar_Color_Patch.Postfix)));
        HarmonyInstance.Patch(original: typeof(CollectibleObject).GetMethod(nameof(CollectibleObject.ShouldDisplayItemDamage)), postfix: typeof(DurabilityBar_ShouldDisplay_Patch).GetMethod(nameof(DurabilityBar_ShouldDisplay_Patch.Postfix)));
        HarmonyInstance.Patch(original: typeof(CollectibleObject).GetMethod(nameof(CollectibleObject.GetRemainingDurability)), postfix: typeof(DurabilityBar_Current_Patch).GetMethod(nameof(DurabilityBar_Current_Patch.Postfix)));
        HarmonyInstance.Patch(original: typeof(CollectibleObject).GetMethod(nameof(CollectibleObject.GetMaxDurability)), postfix: typeof(DurabilityBar_Max_Patch).GetMethod(nameof(DurabilityBar_Max_Patch.Postfix)));
    }

    public override void Dispose()
    {
        HarmonyInstance.UnpatchAll(HarmonyInstance.Id);
    }
}