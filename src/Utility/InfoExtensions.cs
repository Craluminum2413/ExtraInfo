namespace ExtraInfo;

public static class InfoExtensions
{
    public static void GetWorkableTempInfoForAnvil(this StringBuilder dsc, BlockEntityAnvil blockEntity)
    {
        if (blockEntity == null) return;

        if (blockEntity.WorkItemStack == null || blockEntity.SelectedRecipe == null)
        {
            return;
        }

        ItemStack stack = blockEntity.WorkItemStack;

        float meltingpoint = stack.Collectible.GetMeltingPoint(blockEntity.Api.World, null, new DummySlot(stack));

        float workableTemp = (stack.Collectible.Attributes?[Constants.Text.WorkableTemperatureAttr].Exists) switch
        {
            true => stack.Collectible.Attributes[Constants.Text.WorkableTemperatureAttr].AsFloat(meltingpoint / 2),
            _ => meltingpoint / 2,
        };

        _ = workableTemp switch
        {
            0 => dsc.AppendLine(ColorText(Constants.Text.AlwaysWorkable)),
            _ => dsc.AppendLine(ColorText(Constants.Text.WorkableTemperature(workableTemp)))
        };
    }

    public static void GetWorkableTempInfoForItem(this StringBuilder dsc, ItemSlot inSlot, IWorldAccessor world)
    {
        CollectibleObject obj = inSlot.Itemstack.Collectible;
        if (obj is not IAnvilWorkable) return;

        float temperature = obj.GetTemperature(world, inSlot.Itemstack);
        float meltingpoint = obj.GetMeltingPoint(world, null, inSlot);

        float workableTemp = (obj.Attributes?[Constants.Text.WorkableTemperatureAttr].Exists) switch
        {
            true => obj.Attributes[Constants.Text.WorkableTemperatureAttr].AsFloat(meltingpoint / 2),
            _ => meltingpoint / 2,
        };

        _ = workableTemp switch
        {
            0 => dsc.AppendLine(ColorText(Constants.Text.AlwaysWorkable)),
            _ => dsc.AppendLine(ColorText(Constants.Text.WorkableTemperature(workableTemp)))
        };

        if (inSlot is DummySlot || inSlot is ItemSlotCreative) return; // do not show in handbook and creative inventory

        if (temperature < workableTemp)
        {
            dsc.AppendLine(ColorText(Constants.Text.TooColdToWork));
        }
    }

    public static string GetBlockBreakingTimeInfo(this string __result, IWorldAccessor world, BlockPos pos)
    {
        StringBuilder sb = new(__result);

        Dictionary<BlockPos, BlockDamage> damagedBlocks = (world as ClientMain).GetField<Dictionary<BlockPos, BlockDamage>>("damagedBlocks");
        if (damagedBlocks?.Count == 0) return __result;

        BlockDamage currentBlockDamage = damagedBlocks.FirstOrDefault(x => x.Key == pos).Value;
        if (currentBlockDamage == null) return __result;

        Block block = world.BlockAccessor.GetBlock(pos);
        float totalValue = block.Resistance;
        float remainingValue = currentBlockDamage.RemainingResistance;

        float remainingPercentage = remainingValue / totalValue * 100;

        sb.AppendLine().Append(ColorText(Constants.Text.RemainingResistance(remainingPercentage.ToString("F0"))));

        return sb.ToString().TrimEnd();
    }

    public static void GetGroundStorageInfo(this StringBuilder dsc, BlockEntityGroundStorage blockEntity)
    {
        if (blockEntity == null) return;
        if (blockEntity?.StorageProps?.Layout != EnumGroundStorageLayout.Stacking) return;
        if (blockEntity?.Inventory?.Count == 0) return;

        ICoreAPI api = blockEntity.Api;
        BlockPos centerPos = blockEntity.Pos;

        int totalAmount = blockEntity.GetTotalAmount();
        int totalAmountSame = totalAmount;

        bool invalid = false;
        for (int y = centerPos.Y - 1; !invalid; y--)
        {
            BlockPos pos = new(centerPos.X, y, centerPos.Z);
            if (api.World.IsGroundStorage(pos, out BlockEntityGroundStorage blockEntityGroundStorage))
            {
                totalAmount += blockEntityGroundStorage.GetTotalAmount();

                if (blockEntity.HasSameContent(blockEntityGroundStorage))
                {
                    totalAmountSame += blockEntityGroundStorage.GetTotalAmount();
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
            BlockPos pos = new(centerPos.X, y, centerPos.Z);
            if (api.World.IsGroundStorage(pos, out BlockEntityGroundStorage blockEntityGroundStorage))
            {
                totalAmount += blockEntityGroundStorage.GetTotalAmount();

                if (blockEntity.HasSameContent(blockEntityGroundStorage))
                {
                    totalAmountSame += blockEntityGroundStorage.GetTotalAmount();
                }
            }
            else
            {
                invalid = true;
                break;
            }
        }

        dsc.AppendLine();
        dsc.Append(ColorText(Constants.Text.Everything));
        dsc.Append(": ");
        dsc.Append(totalAmount).AppendLine();

        dsc.Append(ColorText(Constants.Text.Current));
        dsc.Append(": ");
        dsc.Append(totalAmountSame).AppendLine();
    }

    public static string GetCrockSealedInName(this string __result, ItemSlot inSlot)
    {
        StringBuilder oldSb = new(__result);

        if (inSlot?.Itemstack?.Attributes?.GetBool(Constants.Text.SealedAttr) == true)
        {
            StringBuilder newSb = new();

            newSb.Append(Constants.Text.SealedText);
            newSb.Append(' ');

            oldSb.Insert(0, newSb);
        }

        return oldSb.ToString();
    }

    public static void GetFarmlandDropSoilChanceInfo(this StringBuilder dsc, BlockEntityFarmland blockEntity)
    {
        if (blockEntity == null) return;

        bool isModEnabled = blockEntity.Api.ModLoader.IsModEnabled(Constants.Modid.FarmlandDropsSoil);
        if (!isModEnabled)
        {
            return;
        }

        Mod mod = blockEntity.Api.ModLoader.GetMod(Constants.Modid.FarmlandDropsSoil);

        float nutrients = blockEntity.Nutrients.Zip(blockEntity.OriginalFertility, (current, original) => current / original).Min();

        dsc.AppendLine(ColorText(string.Format(Constants.Text.FormatPercent, mod.Info.Name, (int)(nutrients * 100))));
    }

    public static void GetFarmlandInfo(this StringBuilder dsc, BlockEntityFarmland blockEntity)
    {
        if (blockEntity == null) return;

        double hours = blockEntity.TotalHoursForNextStage - blockEntity.Api.World.Calendar.TotalHours;

        Block block = GetCrop();

        if (block != null && (GetCropStage(block) < block.CropProps.GrowthStages))
        {
            dsc.AppendLine(ColorText(Constants.Text.HoursAndMinutes(hours)));
        }

        Block GetCrop()
        {
            Block block = blockEntity.Api.World.BlockAccessor.GetBlock(blockEntity.UpPos);
            if (block == null || block.CropProps == null)
            {
                return null;
            }
            return block;
        }

        static int GetCropStage(Block block)
        {
            int.TryParse(block.LastCodePart(), out int stage);
            return stage;
        }
    }

    public static void GetBloomeryInfo(this StringBuilder dsc, BlockEntityBloomery blockEntity)
    {
        if (blockEntity == null) return;
        ICoreAPI api = blockEntity.Api;

        if (blockEntity.GetField<bool>("burning"))
        {
            double timeLeft = (blockEntity.GetField<double>("burningUntilTotalDays") - api.World.Calendar.TotalDays) * api.World.Calendar.HoursPerDay;
            double _timeLeft = Math.Round(timeLeft, 2);
            dsc.AppendLine(ColorText(Constants.Text.Hours(_timeLeft)));
        }
    }

    public static void GetQuernInfo(this StringBuilder dsc, BlockEntityOpenableContainer blockEntity)
    {
        if (blockEntity == null) return;
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

            dsc.Append(ColorText(Constants.Text.Everything));
            dsc.Append(' ');
            dsc.AppendFormat("{0:#}%", Math.Round(stackSize, 2)).AppendLine();

            dsc.Append(ColorText(Constants.Text.One));
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

        foreach (BlockPos neighborPos in neighborPositions)
        {
            BlockEntityCoalPile neighborBE = world.BlockAccessor.GetBlockEntity(neighborPos) as BlockEntityCoalPile;

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
            double _hoursLeft = Math.Round(hoursLeft.Value, 2);
            sb.AppendLine()
                .Append(ColorText(Constants.Text.Coke))
                .Append(": ")
                .Append(ColorText(Constants.Text.Hours(_hoursLeft)));
        }

        return sb.ToString().TrimEnd();
    }

    public static string GetSteelInfo(this string __result, IWorldAccessor world, BlockPos pos)
    {
        if (world.BlockAccessor.GetBlock(pos) is not BlockDoor blockDoor) return __result;
        if (blockDoor.FirstCodePart() != "irondoor") return __result;

        StringBuilder sb = new(__result);

        BlockPos _pos = blockDoor.IsUpperHalf() switch
        {
            true => pos.DownCopy(),
            false => pos,
        };

        BlockPos[] neighborPositions = new BlockPos[]
        {
            _pos.NorthCopy(3),
            _pos.EastCopy(3),
            _pos.SouthCopy(3),
            _pos.WestCopy(3)
        };

        foreach (BlockPos blockPos in neighborPositions)
        {
            if (world.BlockAccessor.GetBlockEntity(blockPos) is not BlockEntityStoneCoffin be) continue;

            double progress = be.GetField<double>("progress");
            if (progress <= 0.0) continue;

            int percent = (int)(progress * 100.0);
            sb.AppendLine(Constants.Text.CarburizationComplete(percent));
        }

        return sb.ToString().TrimEnd();
    }

    public static string GetCharcoalPitInfo(this string __result, IWorldAccessor world, BlockPos pos)
    {
        if (world.BlockAccessor.GetBlockEntity(pos.DownCopy()) is not BlockEntityCharcoalPit blockEntity) return __result;

        StringBuilder sb = new(__result);

        switch (blockEntity.GetField<int>("state"))
        {
            case > 0:
                {
                    double timeLeft = blockEntity.GetField<double>("finishedAfterTotalHours") - world.Calendar.TotalHours;
                    double hours = Math.Round(timeLeft, 2);
                    sb.Append(ColorText(Constants.Text.CharcoalPit));
                    sb.Append(": ");
                    sb.AppendLine(ColorText(Constants.Text.Hours(hours)));
                    return sb.ToString().TrimEnd();
                }

            default:
                {
                    double timeLeft = blockEntity.GetField<double>("startingAfterTotalHours") - world.Calendar.TotalHours;
                    double seconds = Math.Round(timeLeft, 2) * 100;
                    sb.Append(ColorText(Constants.Text.CharcoalPit));
                    sb.Append(": ");
                    sb.Append(ColorText(Constants.Text.WarmingUp));
                    sb.Append(' ');
                    sb.AppendLine(ColorText(Constants.Text.Seconds(seconds)));
                    return sb.ToString().TrimEnd();
                }
        }
    }

    public static void GetPitKilnInfo(this StringBuilder dsc, BlockEntityPitKiln blockEntity)
    {
        if (blockEntity == null) return;
        ICoreAPI api = blockEntity.Api;

        if (blockEntity.Lit)
        {
            double hours = blockEntity.BurningUntilTotalHours - api.World.Calendar.TotalHours;
            dsc.AppendLine(ColorText(Constants.Text.HoursAndMinutes(hours)));
        }
    }

    public static void GetBombInfo(this StringBuilder sb, Block block, BlockEntityBomb blockEntity)
    {
        if (blockEntity != null)
        {
            sb.AppendLine(ColorText(Constants.Text.BlastRadius(blockEntity.BlastRadius)));
            sb.AppendLine(ColorText(Constants.Text.InjureRadius(blockEntity.InjureRadius)));
            sb.AppendLine(ColorText(Constants.Text.BlastType(blockEntity.BlastType)));
            sb.AppendLine(ColorText(Constants.Text.FuseTimeSeconds(blockEntity.FuseTimeSeconds)));
        }
        else if (block != null)
        {
            if (block.Attributes == null) return;
            sb.AppendLine(ColorText(Constants.Text.BlastRadius(block.Attributes[Constants.Text.BlastRadiusAttr].AsInt())));
            sb.AppendLine(ColorText(Constants.Text.InjureRadius(block.Attributes[Constants.Text.InjureRadiusAttr].AsInt())));
            sb.AppendLine(ColorText(Constants.Text.BlastType(block.Attributes[Constants.Text.BlastTypeAttr].AsObject<EnumBlastType>())));
        }
    }

    public static void GetTransientInfo(this StringBuilder dsc, BlockEntityTransient blockEntity)
    {
        if (blockEntity == null) return;
        TransientProperties props = blockEntity.GetField<TransientProperties>("props");
        if (props == null) return;

        blockEntity.CheckTransition(0);

        double hoursLeft = blockEntity.GetField<double>("transitionHoursLeft");
        dsc.AppendLine(ColorText(Constants.Text.Hours(hoursLeft)));
    }

    public static void GetSkepInfo(this StringBuilder dsc, BlockEntityBeehive blockEntity)
    {
        if (blockEntity == null) return;
        if (blockEntity.Block is not BlockSkep) return;
        double hoursLeft = blockEntity.GetField<double>("harvestableAtTotalHours") - blockEntity.Api.World.Calendar.TotalHours;
        dsc.AppendLine(ColorText(Constants.Text.Hours(hoursLeft)));
    }

    public static void GetTranslocatorInfo(this StringBuilder dsc, BlockEntityStaticTranslocator blockEntity)
    {
        if (blockEntity == null) return;
        if (blockEntity.tpLocation == null) return;

        BlockPos pos = blockEntity.Api.World.DefaultSpawnPosition.AsBlockPos;
        BlockPos targetpos = blockEntity.tpLocation.Copy().Sub(pos.X, 0, pos.Z);
        if (blockEntity.tpLocationIsOffset)
        {
            targetpos.Add(blockEntity.Pos.X, pos.Y, pos.Z);
        }
        dsc.AppendLine(ColorText(Constants.Text.TeleportsTo(targetpos)));
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