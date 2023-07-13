using System;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;
using static ExtraInfo.TextExtensions;

namespace ExtraInfo;

public static class InfoExtensions
{
    public static void GetBloomeryInfo(this StringBuilder dsc, BlockEntityBloomery __instance)
    {
        var api = __instance.Api;

        if (__instance.GetField<bool>("burning"))
        {
            double timeLeft = (__instance.GetField<double>("burningUntilTotalDays") - api.World.Calendar.TotalDays) * 24.0;
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
                return sb.ToString();
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
                .Append(Math.Round(hoursLeft.Value, 2))
                .Append(" hours");
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
        be.CheckTransition(0);

        var hoursLeft = be.GetField<double>("transitionHoursLeft");
        dsc.AppendLine(ColorText(Lang.Get("{0} hours", hoursLeft)));
    }

    public static void GetSkepInfo(this StringBuilder dsc, BlockEntityBeehive be)
    {
        if (be == null) return;
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