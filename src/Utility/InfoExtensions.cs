using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.Client.NoObf;
using Vintagestory.GameContent;
using static ExtraInfo.TextExtensions;

namespace ExtraInfo;

public static class InfoExtensions
{
    public static void GetWorkableTempInfoForAnvil(this StringBuilder dsc, BlockEntityAnvil __instance)
    {
        if (__instance.WorkItemStack == null || __instance.SelectedRecipe == null)
        {
            return;
        }

        ItemStack stack = __instance.WorkItemStack;

        float meltingpoint = stack.Collectible.GetMeltingPoint(__instance.Api.World, null, new DummySlot(stack));

        float workableTemp = (stack.Collectible.Attributes?["workableTemperature"].Exists) switch
        {
            true => stack.Collectible.Attributes["workableTemperature"].AsFloat(meltingpoint / 2),
            _ => meltingpoint / 2,
        };

        _ = workableTemp switch
        {
            0 => dsc.AppendLine(ColorText(Lang.Get("extrainfo:AlwaysWorkable"))),
            _ => dsc.AppendLine(ColorText(Lang.Get("extrainfo:WorkableTemperature", (int)workableTemp)))
        };
    }

    public static void GetWorkableTempInfoForItem(this StringBuilder dsc, ItemSlot inSlot, IWorldAccessor world)
    {
        ItemStack stack = inSlot.Itemstack;
        if (stack.Collectible is not IAnvilWorkable) return;

        float temperature = stack.Collectible.GetTemperature(world, stack);
        float meltingpoint = stack.Collectible.GetMeltingPoint(world, null, inSlot);

        float workableTemp = (stack.Collectible.Attributes?["workableTemperature"].Exists) switch
        {
            true => stack.Collectible.Attributes["workableTemperature"].AsFloat(meltingpoint / 2),
            _ => meltingpoint / 2,
        };

        _ = workableTemp switch
        {
            0 => dsc.AppendLine(ColorText(Lang.Get("extrainfo:AlwaysWorkable"))),
            _ => dsc.AppendLine(ColorText(Lang.Get("extrainfo:WorkableTemperature", (int)workableTemp)))
        };

        if (inSlot is DummySlot || inSlot is ItemSlotCreative) return; // do not show in handbook and creative inventory

        if (temperature < workableTemp)
        {
            dsc.AppendLine(ColorText(Lang.Get("Too cold to work")));
        }
    }

    public static string GetBlockBreakingTimeInfo(this string __result, IWorldAccessor world, BlockPos pos)
    {
        StringBuilder sb = new(__result);

        var damagedBlocks = (world as ClientMain).GetField<Dictionary<BlockPos, BlockDamage>>("damagedBlocks");
        if (damagedBlocks?.Count == 0) return __result;

        var currentBlockDamage = damagedBlocks.FirstOrDefault(x => x.Key == pos).Value;
        if (currentBlockDamage == null) return __result;

        var block = world.BlockAccessor.GetBlock(pos);
        var initialResistance = block.Resistance;
        var remainingResistance = currentBlockDamage.RemainingResistance;

        var percent = ((initialResistance - remainingResistance) / initialResistance * 100).ToString("F0");
        // var seconds = Lang.Get("{0} seconds", remainingResistance.ToString("F02"));

        sb.AppendLine().Append(ColorText(Lang.Get("extrainfo:RemainingResistance", percent + "%")));
        // sb.AppendLine().Append(ColorText(Lang.Get("extrainfo:RemainingResistance", string.Format("{0}% | {1}", percent, seconds))));

        return sb.ToString().TrimEnd();
    }

    public static void GetGroundStorageInfo(this StringBuilder dsc, BlockEntityGroundStorage __instance)
    {
        if (__instance?.StorageProps?.Layout != EnumGroundStorageLayout.Stacking) return;
        if (__instance?.Inventory?.Count == 0) return;

        var api = __instance.Api;
        var centerPos = __instance.Pos;

        var totalAmount = __instance.GetTotalAmount();
        var totalAmountSame = totalAmount;

        var invalid = false;
        for (int y = centerPos.Y - 1; !invalid; y--)
        {
            var pos = new BlockPos(centerPos.X, y, centerPos.Z);
            if (api.World.IsGroundStorage(pos, out var beGroundStorage))
            {
                totalAmount += beGroundStorage.GetTotalAmount();

                if (__instance.HasSameContent(beGroundStorage))
                {
                    totalAmountSame += beGroundStorage.GetTotalAmount();
                }
            }
            else
            {
                invalid = true;
                break;
            }
        }

        invalid = false;
        for (int y = centerPos.Y + 1; !invalid; y++)
        {
            var pos = new BlockPos(centerPos.X, y, centerPos.Z);
            if (api.World.IsGroundStorage(pos, out var beGroundStorage))
            {
                totalAmount += beGroundStorage.GetTotalAmount();

                if (__instance.HasSameContent(beGroundStorage))
                {
                    totalAmountSame += beGroundStorage.GetTotalAmount();
                }
            }
            else
            {
                invalid = true;
                break;
            }
        }

        dsc.AppendLine();
        dsc.Append(ColorText(Lang.Get("tabname-general")));
        dsc.Append(": ");
        dsc.Append(totalAmount).AppendLine();

        dsc.Append(ColorText(Lang.Get("extrainfo:Current")));
        dsc.Append(": ");
        dsc.Append(totalAmountSame).AppendLine();
    }

    public static void GetFarmlandInfo(this StringBuilder dsc, BlockEntityFarmland __instance)
    {
        var timeLeft = __instance.TotalHoursForNextStage - __instance.Api.World.Calendar.TotalHours;

        var block = GetCrop();

        if (block != null && (GetCropStage(block) < block.CropProps.GrowthStages))
        {
            dsc.AppendLine(ColorText(Lang.Get("{0} hours", timeLeft)));
        }

        Block GetCrop()
        {
            var block = __instance.Api.World.BlockAccessor.GetBlock(__instance.UpPos);
            if (block == null || block.CropProps == null)
            {
                return null;
            }
            return block;
        }

        static int GetCropStage(Block block)
        {
            int.TryParse(block.LastCodePart(), out var stage);
            return stage;
        }
    }

    public static void GetBloomeryInfo(this StringBuilder dsc, BlockEntityBloomery __instance)
    {
        var api = __instance.Api;

        if (__instance.GetField<bool>("burning"))
        {
            double timeLeft = (__instance.GetField<double>("burningUntilTotalDays") - api.World.Calendar.TotalDays) * api.World.Calendar.HoursPerDay;
            dsc.AppendLine(ColorText(Lang.Get("{0} hours", Math.Round(timeLeft, 2))));
        }
    }

    public static void GetQuernInfo(this StringBuilder dsc, BlockEntityOpenableContainer blockEntity)
    {
        if (blockEntity is not BlockEntityQuern quern) return;

        if (quern.CanGrind() && quern.GrindSpeed > 0)
        {
            double percent = quern.inputGrindTime / quern.maxGrindingTime();
            int mss = quern.InputSlot?.Itemstack?.StackSize + (quern.OutputSlot?.Itemstack?.StackSize ?? 0) ?? 1;
            double stackSize = (double)(quern.InputSlot?.Itemstack?.StackSize ?? 0) / mss;
            stackSize = 1.0 - stackSize;
            stackSize += percent / mss;
            stackSize *= 100;
            percent *= 100;

            dsc.Append(ColorText(Lang.Get("tabname-general")));
            dsc.Append(' ');
            dsc.AppendFormat("{0:#}%", Math.Round(stackSize, 2)).AppendLine();

            dsc.Append(ColorText(Lang.Get("extrainfo:One")));
            dsc.Append(' ');
            dsc.AppendFormat("{0:#}%", Math.Round(percent, 2)).AppendLine();
        }
    }

    public static string GetCokeInfo(this string __result, IWorldAccessor world, BlockPos pos)
    {
        if (world.BlockAccessor.GetBlock(pos) is not BlockCokeOvenDoor) return __result;

        StringBuilder sb = new(__result);

        BlockPos[] neighborPositions = new[] { pos.NorthCopy(), pos.EastCopy(), pos.SouthCopy(), pos.WestCopy() };
        double? hoursLeft = null;

        foreach (var neighborPos in neighborPositions)
        {
            var neighborBE = world.BlockAccessor.GetBlockEntity(neighborPos) as BlockEntityCoalPile;

            if (neighborBE?.IsBurning == false)
            {
                return __result;
            }

            double? neighborHoursLeft = neighborBE?.GetHoursLeft(neighborBE.GetField<double>("burnStartTotalHours"));

            if (neighborHoursLeft.HasValue && (!hoursLeft.HasValue || neighborHoursLeft.Value < hoursLeft.Value))
            {
                hoursLeft = neighborHoursLeft.Value;
            }
        }

        if (hoursLeft.HasValue)
        {
            sb.AppendLine()
                .Append(ColorText(Lang.Get("item-coke")))
                .Append(": ")
                .Append(ColorText(Lang.Get("{0} hours", Math.Round(hoursLeft.Value, 2))));
        }

        return sb.ToString().TrimEnd();
    }

    public static string GetSteelInfo(this string __result, IWorldAccessor world, BlockPos pos)
    {
        if (world.BlockAccessor.GetBlock(pos) is not BlockDoor blockDoor) return __result;
        if (blockDoor.FirstCodePart() != "irondoor") return __result;

        StringBuilder sb = new(__result);

        var _pos = blockDoor.IsUpperHalf() switch
        {
            true => pos.DownCopy(),
            false => pos,
        };

        var neighborPositions = new[]
        {
            _pos.NorthCopy(3),
            _pos.EastCopy(3),
            _pos.SouthCopy(3),
            _pos.WestCopy(3)
        };

        foreach (var blockPos in neighborPositions)
        {
            if (world.BlockAccessor.GetBlockEntity(blockPos) is not BlockEntityStoneCoffin be) continue;

            var progress = be.GetField<double>("progress");
            if (progress <= 0.0) continue;

            sb.AppendLine(ColorText(Lang.Get("Carburization: {0}% complete", (int)(progress * 100.0))));
        }

        return sb.ToString().TrimEnd();
    }

    public static string GetCharcoalPitInfo(this string __result, IWorldAccessor world, BlockPos pos)
    {
        var blockEntity = world.BlockAccessor.GetBlockEntity(pos.DownCopy());
        if (blockEntity is not BlockEntityCharcoalPit be) return __result;

        StringBuilder sb = new(__result);

        switch (be.GetField<int>("state"))
        {
            case > 0:
                {
                    double timeLeft = be.GetField<double>("finishedAfterTotalHours") - world.Calendar.TotalHours;
                    sb.Append(ColorText(Lang.Get("block-charcoalpit")));
                    sb.Append(": ");
                    sb.AppendLine(ColorText(Lang.Get("{0} hours", Math.Round(timeLeft, 2))));
                    return sb.ToString().TrimEnd();
                }

            default:
                {
                    double timeLeft = be.GetField<double>("startingAfterTotalHours") - world.Calendar.TotalHours;
                    sb.Append(ColorText(Lang.Get("block-charcoalpit")));
                    sb.Append(": ");
                    sb.Append(ColorText(Lang.Get("Warming up...")));
                    sb.Append(' ');
                    sb.AppendLine(ColorText(Lang.Get("{0} seconds", Math.Round(timeLeft, 2) * 100)));
                    return sb.ToString().TrimEnd();
                }
        }
    }

    public static void GetPitKilnInfo(this StringBuilder dsc, BlockEntityPitKiln __instance)
    {
        var api = __instance.Api;

        if (__instance.Lit)
        {
            double timeLeft = __instance.BurningUntilTotalHours - api.World.Calendar.TotalHours;
            dsc.AppendLine(ColorText(Lang.Get("{0} hours", Math.Round(timeLeft, 2))));
        }
    }

    public static void GetBombInfo(this StringBuilder sb, Block block, BlockEntityBomb be)
    {
        if (be != null)
        {
            sb.AppendLine(ColorText(Lang.Get("extrainfo:bomb-BlastRadius", be.BlastRadius)));
            sb.AppendLine(ColorText(Lang.Get("extrainfo:bomb-InjureRadius", be.InjureRadius)));
            sb.AppendLine(ColorText(Lang.Get("extrainfo:bomb-BlastType", be.BlastType switch
            {
                EnumBlastType.OreBlast => Lang.Get("blockmaterial-Ore"),
                EnumBlastType.RockBlast => Lang.Get("blockmaterial-Stone"),
                EnumBlastType.EntityBlast => Lang.Get("tabname-creatures"),
                _ => Lang.Get("foodcategory-unknown")
            })));

            sb.AppendLine(ColorText(Lang.Get("extrainfo:bomb-FuseTime", Lang.Get("{0} seconds", be.FuseTimeSeconds))));
        }
        else if (block != null)
        {
            if (block.Attributes == null) return;

            var blastRadius = (float)block.Attributes["blastRadius"].AsInt();
            var injureRadius = (float)block.Attributes["injureRadius"].AsInt();
            var blastType = (EnumBlastType)block.Attributes["blastType"].AsInt();

            sb.AppendLine(ColorText(Lang.Get("extrainfo:bomb-BlastRadius", blastRadius)));
            sb.AppendLine(ColorText(Lang.Get("extrainfo:bomb-InjureRadius", injureRadius)));
            sb.AppendLine(ColorText(Lang.Get("extrainfo:bomb-BlastType", blastType switch
            {
                EnumBlastType.OreBlast => Lang.Get("blockmaterial-Ore"),
                EnumBlastType.RockBlast => Lang.Get("blockmaterial-Stone"),
                EnumBlastType.EntityBlast => Lang.Get("tabname-creatures"),
                _ => Lang.Get("foodcategory-unknown")
            })));
        }
    }

    public static void GetTransientInfo(this StringBuilder dsc, BlockEntityTransient be)
    {
        if (be == null) return;

        var props = be.GetField<TransientProperties>("props");
        if (props is null) return;

        be.CheckTransition(0);

        var hoursLeft = be.GetField<double>("transitionHoursLeft");
        dsc.AppendLine(ColorText(Lang.Get("{0} hours", hoursLeft)));
    }

    public static void GetSkepInfo(this StringBuilder dsc, BlockEntityBeehive be)
    {
        if (be == null) return;
        if (be.Block is not BlockSkep) return;
        var hoursLeft = be.GetField<double>("harvestableAtTotalHours") - be.Api.World.Calendar.TotalHours;
        dsc.AppendLine(ColorText(Lang.Get("{0} hours", hoursLeft)));
    }

    public static void GetTranslocatorInfo(this StringBuilder dsc, BlockEntityStaticTranslocator be)
    {
        if (be == null) return;
        if (be.tpLocation == null) return;

        BlockPos pos = be.Api.World.DefaultSpawnPosition.AsBlockPos;
        BlockPos targetpos = be.tpLocation.Copy().Sub(pos.X, 0, pos.Z);
        if (be.tpLocationIsOffset)
        {
            targetpos.Add(be.Pos.X, pos.Y, pos.Z);
        }
        dsc.AppendLine(ColorText(Lang.Get("Teleports to {0}", targetpos)));
    }

    // /// <summary>
    // /// Mechanical block info (speed, total torque, available torque, resistance)
    // /// </summary>
    // private static void GetMechanicalBlockInfo(this StringBuilder sb, BlockEntity blockEntity)
    // {
    //     var network = blockEntity.GetBehavior<BEBehaviorMPBase>().Network;
    //     if (network == null) return;

    //     sb.AppendLine(ColorText(Lang.Get("extrainfo:mechanics-Speed", network.Speed)));
    //     sb.AppendLine(ColorText(Lang.Get("extrainfo:mechanics-TotalAvailableTorque", network.TotalAvailableTorque)));
    //     sb.AppendLine(ColorText(Lang.Get("extrainfo:mechanics-NetworkTorque", network.NetworkTorque)));
    //     sb.AppendLine(ColorText(Lang.Get("extrainfo:mechanics-NetworkResistance", network.NetworkResistance)));
    // }
}