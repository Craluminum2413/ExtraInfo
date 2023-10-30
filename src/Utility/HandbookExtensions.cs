namespace ExtraInfo;

public static class HandbookExtensions
{
    public static void AddPitKilnInfo(this List<RichTextComponentBase> list, ItemSlot inSlot, ICoreClientAPI capi, ActionConsumable<string> openDetailPageFor)
    {
        if (inSlot.Itemstack.Collectible is not BlockPitkiln blockPitKiln) return;

        List<JsonItemStackBuildStage> fuelStacks = blockPitKiln.GetFuelStacks(capi);

        list.AddMarginAndTitle(capi, marginTop: 7, titletext: Constants.Text.Fuel);

        List<RichTextComponentBase> richText = new();

        foreach (JsonItemStackBuildStage fuel in fuelStacks)
        {
            richText.AddStack(capi, openDetailPageFor, fuel.ResolvedItemstack);
            richText.Add(new RichTextComponent(capi, Constants.Text.Hours((float)fuel.BurnTimeHours) + "\n", CairoFont.WhiteSmallText())
            {
                VerticalAlign = EnumVerticalAlign.Middle
            });
        }

        list.AddRange(richText);
    }

    public static void AddPanningDropsInfo(this List<RichTextComponentBase> list, ItemSlot inSlot, ICoreClientAPI capi, ActionConsumable<string> openDetailPageFor)
    {
        if (inSlot.Itemstack.Collectible is not BlockPan blockPan) return;

        Dictionary<ItemStack[], PanningDrop[]> panningDrops = GetPanningDrops(capi, blockPan);

        list.AddMarginAndTitle(capi, marginTop: 7, titletext: Constants.Text.PanningDrops);

        List<RichTextComponentBase> richText = new();

        foreach (KeyValuePair<ItemStack[], PanningDrop[]> keyVal in panningDrops)
        {
            List<ItemStack[]> stackLists = keyVal.Key.GroupBy(stack => stack.Collectible.FirstCodePart()).Select(group => group.ToArray()).ToList();
            foreach (ItemStack[] stacks in stackLists)
            {
                richText.AddStacks(capi, openDetailPageFor, stacks);
            }

            richText.AddEqualSign(capi);
            richText.Add(new ClearFloatTextComponent(capi, unScaleMarginTop: 7));

            int count = 3;
            foreach (PanningDrop drop in panningDrops[keyVal.Key])
            {
                if (!drop.Resolve(capi.World, "")) continue;
                richText.AddStack(capi, openDetailPageFor, drop.ResolvedItemstack);

                float extraMul = drop.DropModbyStat is null ? 1f : capi.World.Player.Entity.Stats.GetBlended(drop.DropModbyStat);

                count--;
                richText.Add(new RichTextComponent(capi, GetMinMaxPercent(drop, extraMul) + (count == 0 ? "\n" : "\t\t\t\t\t"), CairoFont.WhiteSmallText())
                {
                    VerticalAlign = EnumVerticalAlign.Middle
                });

                if (count == 0) count = 3;
            }

            richText.Add(new ClearFloatTextComponent(capi, unScaleMarginTop: 7));
        }

        list.AddRange(richText);
    }

    // TODO: AddTroughInfoForEntity, AddTroughInfoForFood
    public static void AddTroughInfo(this List<RichTextComponentBase> list, ItemSlot inSlot, ICoreClientAPI capi, ActionConsumable<string> openDetailPageFor)
    {
        if (inSlot.Itemstack.Collectible is not BlockTroughBase blockTrough) return;

        list.AddMarginAndTitle(capi, marginTop: 7, titletext: Constants.Text.AvailableFood);

        List<RichTextComponentBase> richText = new();

        foreach (ContentConfig config in blockTrough.contentConfigs)
        {
            if (config.Foodfor?.Length == 0) continue;

            if (config.Content.Code.ToShortString().Contains("-*"))
            {
                List<ItemStack> stacks = GetWildcardTroughStacks(capi, config);
                richText.AddStacks(capi, openDetailPageFor, stacks.ToArray());
            }
            else
            {
                richText.AddStack(capi, openDetailPageFor, config.Content.ResolvedItemstack);
            }

            richText.AddEqualSign(capi);

            foreach (EntityProperties entityType in capi.World.EntityTypes)
            {
                if (RegistryObjectType.WildCardMatches(entityType.Code.ToString(), config.Foodfor.ToList().ConvertAll(x => x.ToString()), out _))
                {
                    ItemStack stack = entityType.GetCreatureStack(capi);
                    if (stack == null) continue;
                    richText.AddStack(capi, openDetailPageFor, stack);
                }
            }

            richText.Add(new ClearFloatTextComponent(capi, unScaleMarginTop: 7));
        }

        list.AddRange(richText);
    }

    public static void AddEntityDietInfoForBlock(this List<RichTextComponentBase> list, ItemSlot inSlot, ICoreClientAPI capi, ActionConsumable<string> openDetailPageFor)
    {
        if (inSlot.Itemstack.Collectible is not Block block) return;

        List<ItemStack> stacks = new();
        foreach (EntityProperties entityType in capi.World.EntityTypes)
        {
            string[] blockDiet = entityType?.Attributes?["blockDiet"].AsObject<string[]>();
            if (blockDiet == null) continue;

            if (block.WildCardMatch(blockDiet)
                || (block is BlockBerryBush && blockDiet.Contains("Berry"))
                || (block is BlockBeehive && blockDiet.Contains("Honey")))
            {
                ItemStack stack = entityType.GetCreatureStack(capi);
                if (stack == null) continue;
                stacks.Add(stack);
            }
        }

        if (stacks?.Count == 0) return;

        list.AddMarginAndTitle(capi, marginTop: 7, titletext: Constants.Text.EatenByWildAnimals);

        List<RichTextComponentBase> richText = new();

        foreach (ItemStack stack in stacks)
        {
            richText.AddStack(capi, openDetailPageFor, stack);
        }

        list.AddRange(richText);
    }

    public static void AddEntityDietInfo(this List<RichTextComponentBase> list, ItemSlot inSlot, ICoreClientAPI capi, ActionConsumable<string> openDetailPageFor)
    {
        if (inSlot.Itemstack.Collectible is not ItemCreature itemCreature) return;

        EntityProperties entity = capi.World.GetEntityType(new AssetLocation(itemCreature.Code.Domain, itemCreature.CodeEndWithoutParts(1)));

        string[] blockDiet = entity?.Attributes?["blockDiet"].AsObject<string[]>();
        if (blockDiet == null) return;

        List<ItemStack> stacks = new();
        foreach (Block block in capi.World.Blocks)
        {
            if (block.WildCardMatch(blockDiet)
                || (block is BlockBerryBush && blockDiet.Contains("Berry"))
                || (block is BlockBeehive && blockDiet.Contains("Honey")))
            {
                ItemStack stack = new(block);
                stacks.Add(stack);
            }
        }

        list.AddMarginAndTitle(capi, marginTop: 7, titletext: Constants.Text.Food);

        List<RichTextComponentBase> richText = new();

        foreach (ItemStack stack in stacks)
        {
            richText.AddStack(capi, openDetailPageFor, stack);
        }

        list.AddRange(richText);
    }

    public static void AddEntityHealthAndDamageInfo(this List<RichTextComponentBase> list, ItemSlot inSlot, ICoreClientAPI capi)
    {
        if (inSlot.Itemstack.Collectible is not ItemCreature itemCreature) return;

        EntityProperties entityType = capi.World.GetEntityType(new AssetLocation(itemCreature.Code.Domain, itemCreature.CodeEndWithoutParts(1)));

        float health = ServerEntityType.HealthList.FirstOrDefault(x => x.Key == entityType.Code).Value;
        float damage = ServerEntityType.DamageList.FirstOrDefault(x => x.Key == entityType.Code).Value;
        int damageTier = ServerEntityType.DamageTierList.FirstOrDefault(x => x.Key == entityType.Code).Value;

        StringBuilder sb = new();

        if (health != 0) sb.AppendLine(Constants.Text.Health(health));
        if (damage != 0) sb.AppendLine(Constants.Text.Damage(damage));
        if (damage != 0) sb.AppendLine(Constants.Text.DamageTier(damageTier));

        if (sb.Length == 0) return;

        string text = sb.ToString();

        list.Add(new RichTextComponent(capi, text, CairoFont.WhiteSmallText())
        {
            VerticalAlign = EnumVerticalAlign.Middle
        });
    }

    public static void AddEntityDropsInfo(this List<RichTextComponentBase> list, ItemSlot inSlot, ICoreClientAPI capi, ActionConsumable<string> openDetailPageFor)
    {
        if (inSlot.Itemstack.Collectible is not ItemCreature itemCreature) return;

        EntityProperties entityType = capi.World.GetEntityType(new AssetLocation(itemCreature.Code.Domain, itemCreature.CodeEndWithoutParts(1)));

        List<BlockDropItemStack> harvestStacks = GetHarvestableDrops(capi, entityType);
        if (harvestStacks?.Count != 0)
        {
            list.AddMarginAndTitle(capi, marginTop: 7, titletext: Constants.Text.HarvestableDropsText);

            List<RichTextComponentBase> richTextHarvest = new();

            foreach (BlockDropItemStack stack in harvestStacks)
            {
                richTextHarvest.AddStack(capi, openDetailPageFor, stack.ResolvedItemstack);
                richTextHarvest.Add(new RichTextComponent(capi, GetMinMax(stack.Quantity) + "\n", CairoFont.WhiteSmallText())
                {
                    VerticalAlign = EnumVerticalAlign.Middle
                });
            }

            list.AddRange(richTextHarvest);
        }

        if (entityType.Drops?.Length != 0)
        {
            list.AddMarginAndTitle(capi, marginTop: 7, titletext: Constants.Text.OtherDropsText);

            List<RichTextComponentBase> richTextOther = new();

            foreach (BlockDropItemStack stack in entityType.Drops)
            {
                richTextOther.AddStack(capi, openDetailPageFor, stack.ResolvedItemstack);
                richTextOther.Add(new RichTextComponent(capi, GetMinMax(stack.Quantity) + "\n", CairoFont.WhiteSmallText())
                {
                    VerticalAlign = EnumVerticalAlign.Middle
                });
            }

            list.AddRange(richTextOther);
        }
    }

    public static void AddEntityDropsInfoForDrop(this List<RichTextComponentBase> list, ItemSlot inSlot, ICoreClientAPI capi, ActionConsumable<string> openDetailPageFor)
    {
        CollectibleObject collObj = inSlot.Itemstack.Collectible;

        List<RichTextComponentBase> richTextHarvest = new();
        List<RichTextComponentBase> richTextDrop = new();

        foreach (EntityProperties entityType in capi.World.EntityTypes)
        {
            List<BlockDropItemStack> harvestStacks = GetHarvestableDrops(capi, entityType);
            if (harvestStacks?.Count != 0 && harvestStacks.Find(stack => stack?.Code == collObj?.Code) != null)
            {
                ItemStack stack = entityType.GetCreatureStack(capi);
                if (stack == null) continue;

                richTextHarvest.AddStack(capi, openDetailPageFor, stack);
            }

            if (entityType.Drops?.Length != 0 && entityType.Drops.ToList().Find(stack => stack.Code == collObj.Code) != null)
            {
                ItemStack stack = entityType.GetCreatureStack(capi);
                if (stack == null) continue;

                richTextDrop.AddStack(capi, openDetailPageFor, stack);
            }
        }

        if (richTextHarvest?.Count != 0)
        {
            list.AddMarginAndTitle(capi, marginTop: 7, titletext: Constants.Text.HarvestableDropsText);
            list.AddRange(richTextHarvest);
        }

        if (richTextDrop?.Count != 0)
        {
            list.AddMarginAndTitle(capi, marginTop: 7, titletext: Constants.Text.OtherDropsText);
            list.AddRange(richTextDrop);
        }
    }

    public static void AddTraderPropsInfo(this List<RichTextComponentBase> list, ItemSlot inSlot, ICoreClientAPI capi, ActionConsumable<string> openDetailPageFor)
    {
        if (inSlot.Itemstack.Collectible is not ItemCreature itemCreature) return;

        EntityProperties entityProperties = capi.World.GetEntityType(new AssetLocation(itemCreature.Code.Domain, itemCreature.CodeEndWithoutParts(1)));
        if (entityProperties.Class != "EntityTrader") return;

        TradeProperties tradeProps = GetTradeProps(entityProperties);
        if (tradeProps == null) return;

        ItemStack gear = new(capi.World.GetItem(new AssetLocation("gear-rusty")));
        List<TradeItem> buyingStacks = tradeProps.Buying.List.Where(tradeItem => tradeItem.Resolve(capi.World, "")).ToList();
        List<TradeItem> sellingStacks = tradeProps.Selling.List.Where(tradeItem2 => tradeItem2.Resolve(capi.World, "")).ToList();

        list.AddMarginAndTitle(capi, marginTop: 7, titletext: Constants.Text.YouCanSell);
        List<RichTextComponentBase> richTextSell = new();
        foreach (TradeItem item in buyingStacks)
        {
            richTextSell.AddTraderInfo(capi, item, openDetailPageFor, gear);
        }
        list.AddRange(richTextSell);

        list.AddMarginAndTitle(capi, marginTop: 7, titletext: Constants.Text.YouCanBuy);
        List<RichTextComponentBase> richTextBuy = new();
        foreach (TradeItem item in sellingStacks)
        {
            richTextBuy.AddTraderInfo(capi, item, openDetailPageFor, gear);
        }
        list.AddRange(richTextBuy);
    }

    public static void AddTradersInfo(this List<RichTextComponentBase> list, ItemSlot inSlot, ICoreClientAPI capi, ActionConsumable<string> openDetailPageFor)
    {
        CollectibleObject collObj = inSlot.Itemstack.Collectible;
        // var itemstack = inSlot.Itemstack; // x.Attributes == itemstack.Attributes

        List<RichTextComponentBase> richTextSell = new();
        TradeItem[] buyingList = System.Array.Empty<TradeItem>();
        foreach (EntityProperties entityType in capi.World.EntityTypes.Where(x => x.Class == "EntityTrader" && GetTradeProps(x)?.Buying.List.Any(x => x.Code == collObj.Code) == true))
        {
            buyingList = GetTradeProps(entityType)?.Buying.List;
            if (buyingList == null) break;

            ItemStack stack = entityType.GetCreatureStack(capi);
            if (stack == null) continue;

            richTextSell.AddStack(capi, openDetailPageFor, stack);
        }
        if (buyingList?.Length != 0)
        {
            list.AddMarginAndTitle(capi, marginTop: 7, titletext: Constants.Text.PurchasedBy);
            list.AddRange(richTextSell);
        }

        List<RichTextComponentBase> richTextBuy = new();
        TradeItem[] sellingList = System.Array.Empty<TradeItem>();
        foreach (EntityProperties entityType in capi.World.EntityTypes.Where(x => x.Class == "EntityTrader" && GetTradeProps(x)?.Selling.List.Any(x => x.Code == collObj.Code) == true))
        {
            sellingList = GetTradeProps(entityType)?.Selling.List;
            if (sellingList == null) break;

            ItemStack stack = entityType.GetCreatureStack(capi);
            if (stack == null) continue;

            richTextBuy.AddStack(capi, openDetailPageFor, stack);
        }
        if (sellingList?.Length != 0)
        {
            list.AddMarginAndTitle(capi, marginTop: 7, titletext: Constants.Text.SoldBy);
            list.AddRange(richTextBuy);
        }
    }

    private static TradeProperties GetTradeProps(EntityProperties props) => props.Attributes["tradeProps"].AsObject<TradeProperties>(null);

    private static List<JsonItemStackBuildStage> GetFuelStacks(this BlockPitkiln blockPitKiln, ICoreClientAPI capi)
    {
        return ObjectCacheUtil.GetOrCreate(capi, "pitKilnFuelStacks-" + blockPitKiln.Code, delegate
        {
            JsonItemStackBuildStage[] fuelStacks = blockPitKiln?.Attributes?["buildMats"]?["fuel"]?.AsObject<JsonItemStackBuildStage[]>();

            List<JsonItemStackBuildStage> stacks = new();
            stacks.AddRange(fuelStacks.Where(stack => stack?.BurnTimeHours != null));

            foreach (JsonItemStackBuildStage stack in stacks)
            {
                stack.Resolve(capi.World, "");
            }

            return stacks;
        });
    }

    private static Dictionary<ItemStack[], PanningDrop[]> GetPanningDrops(ICoreClientAPI capi, BlockPan blockPan)
    {
        return ObjectCacheUtil.GetOrCreate(capi, "blockPanDrops-" + blockPan.Code, delegate
        {
            Dictionary<string, PanningDrop[]> dropsBySourceMat = blockPan.GetField<Dictionary<string, PanningDrop[]>>("dropsBySourceMat");

            Dictionary<ItemStack[], PanningDrop[]> panningDrops = new();

            foreach (string key in dropsBySourceMat.Keys)
            {
                List<ItemStack> blockStacks = new();
                foreach (Block block in capi.World.Blocks.Where(x => x.WildCardMatch(key)))
                {
                    string rocktype = block?.Variant["rock"];

                    foreach (PanningDrop drop in dropsBySourceMat[key])
                    {
                        if (drop.Code.Path.Contains("{rocktype}"))
                        {
                            drop.Code.Path = drop.Code.Path.Replace("{rocktype}", rocktype);
                        }
                    }

                    ItemStack stack = new(block);
                    if (!stack.ResolveBlockOrItem(capi.World)) continue;
                    if (stack.Collectible.Variant["layer"] != null) continue; // Ignore layer variants
                    blockStacks.Add(stack);
                }
                panningDrops.Add(blockStacks.ToArray(), dropsBySourceMat[key]);
                blockStacks = new();
            }
            return panningDrops;
        });
    }

    private static List<ItemStack> GetWildcardTroughStacks(ICoreClientAPI capi, ContentConfig config)
    {
        return ObjectCacheUtil.GetOrCreate(capi, "troughWildcardStacks-" + config.Code, delegate
        {
            List<ItemStack> stacks = new();
            foreach (CollectibleObject obj in capi.World.Collectibles.Where(x => x.WildCardMatch(config.Content.Code)))
            {
                ItemStack stack = new(obj);
                if (!stack.ResolveBlockOrItem(capi.World)) continue;
                stacks.Add(stack);
            }
            return stacks;
        });
    }

    private static List<BlockDropItemStack> GetHarvestableDrops(ICoreClientAPI capi, EntityProperties entityType)
    {
        return ObjectCacheUtil.GetOrCreate(capi, "harvestableDrops-" + entityType.Code, delegate
        {
            Dictionary<AssetLocation, BlockDropItemStack[]> harvestableBehaviors = ServerEntityType.HarvestableDrops;
            if (harvestableBehaviors?.Count == 0 || !harvestableBehaviors.ContainsKey(entityType.Code))
            {
                return new();
            }

            BlockDropItemStack[] drops = harvestableBehaviors[entityType.Code];
            if (drops == null) return new();

            List<BlockDropItemStack> stacks = new();

            foreach (BlockDropItemStack drop in drops)
            {
                if (drop.Resolve(capi.World, "BehaviorHarvestable ", entityType.Code))
                {
                    stacks.Add(drop);
                }
            }

            return stacks;
        });
    }
}
