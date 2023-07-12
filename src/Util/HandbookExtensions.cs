using System;
using System.Collections.Generic;
using System.Linq;
using Cairo;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;
using Vintagestory.GameContent;
using Vintagestory.ServerMods.NoObf;
using static ExtraInfo.TextExtensions;

namespace ExtraInfo;

public static class HandbookExtensions
{
    public static void AddPitKilnInfo(this List<RichTextComponentBase> list, ItemSlot inSlot, ICoreClientAPI capi, ActionConsumable<string> openDetailPageFor)
    {
        if (inSlot.Itemstack.Collectible is not BlockPitkiln blockPitKiln) return;

        var fuelStacks = blockPitKiln.GetFuelStacks(capi);

        list.AddMarginAndTitle(capi, marginTop: 7, titletext: Lang.Get("extrainfo:Fuel"));

        List<RichTextComponentBase> richText = new();

        foreach (var fuel in fuelStacks)
        {
            richText.AddStack(capi, openDetailPageFor, fuel.ResolvedItemstack);
            richText.Add(new RichTextComponent(capi, Lang.Get("{0} hours", fuel.BurnTimeHours.ToString()) + "\n", CairoFont.WhiteSmallText())
            {
                VerticalAlign = EnumVerticalAlign.Middle
            });
        }

        list.AddRange(richText);
    }

    public static void AddPanningDropsInfo(this List<RichTextComponentBase> list, ItemSlot inSlot, ICoreClientAPI capi, ActionConsumable<string> openDetailPageFor)
    {
        if (inSlot.Itemstack.Collectible is not BlockPan blockPan) return;

        var panningDrops = GetPanningDrops(capi, blockPan);

        list.AddMarginAndTitle(capi, marginTop: 7, titletext: Lang.Get("extrainfo:PanningDrops"));

        List<RichTextComponentBase> richText = new();

        foreach (var keyVal in panningDrops)
        {
            var stackLists = keyVal.Key.GroupBy(stack => stack.Collectible.FirstCodePart()).Select(group => group.ToArray()).ToList();
            foreach (var stacks in stackLists)
            {
                richText.AddStacks(capi, openDetailPageFor, stacks);
            }

            richText.AddEqualSign(capi);
            richText.Add(new ClearFloatTextComponent(capi, unScaleMarginTop: 7));

            int count = 3;
            foreach (var drop in panningDrops[keyVal.Key])
            {
                if (!drop.Resolve(capi.World, "")) continue;
                richText.AddStack(capi, openDetailPageFor, drop.ResolvedItemstack);

                var extraMul = drop.DropModbyStat is null ? 1f : capi.World.Player.Entity.Stats.GetBlended(drop.DropModbyStat);

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

        list.AddMarginAndTitle(capi, marginTop: 7, titletext: Lang.Get("extrainfo:AvailableFood"));

        List<RichTextComponentBase> richText = new();

        foreach (var config in blockTrough.contentConfigs)
        {
            if (config.Foodfor?.Length == 0) continue;

            if (config.Content.Code.ToShortString().Contains("-*"))
            {
                var stacks = GetWildcardTroughStacks(capi, config);
                richText.AddStacks(capi, openDetailPageFor, stacks.ToArray());
            }
            else
            {
                richText.AddStack(capi, openDetailPageFor, config.Content.ResolvedItemstack);
            }

            richText.AddEqualSign(capi);

            foreach (var entityType in capi.World.EntityTypes)
            {
                if (RegistryObjectType.WildCardMatches(entityType.Code.ToString(), config.Foodfor.ToList().ConvertAll(x => x.ToString()), out _))
                {
                    var stack = GetCreatureStack(capi, entityType);
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

        var stacks = new List<ItemStack>();
        foreach (var entityType in capi.World.EntityTypes)
        {
            var blockDiet = entityType?.Attributes?["blockDiet"].AsObject<string[]>();
            if (blockDiet == null) continue;

            if (block.WildCardMatch(blockDiet)
                || (block is BlockBerryBush && blockDiet.Contains("Berry"))
                || (block is BlockBeehive && blockDiet.Contains("Honey")))
            {
                var stack = GetCreatureStack(capi, entityType);
                if (stack == null) continue;
                stacks.Add(stack);
            }
        }

        if (stacks?.Count == 0) return;

        list.AddMarginAndTitle(capi, marginTop: 7, titletext: Lang.Get("Eaten by wild animals."));

        List<RichTextComponentBase> richText = new();

        foreach (var stack in stacks)
        {
            richText.AddStack(capi, openDetailPageFor, stack);
            richText.Add(new RichTextComponent(capi, "\t", CairoFont.WhiteSmallText())
            {
                VerticalAlign = EnumVerticalAlign.Middle
            });
        }

        list.AddRange(richText);
    }

    public static void AddEntityDietInfo(this List<RichTextComponentBase> list, ItemSlot inSlot, ICoreClientAPI capi, ActionConsumable<string> openDetailPageFor)
    {
        if (inSlot.Itemstack.Collectible is not ItemCreature itemCreature) return;

        var entity = capi.World.GetEntityType(new AssetLocation(itemCreature.Code.Domain, itemCreature.CodeEndWithoutParts(1)));

        var blockDiet = entity?.Attributes?["blockDiet"].AsObject<string[]>();
        if (blockDiet == null) return;

        var stacks = new List<ItemStack>();
        foreach (var block in capi.World.Blocks)
        {
            if (block.WildCardMatch(blockDiet)
                || (block is BlockBerryBush && blockDiet.Contains("Berry"))
                || (block is BlockBeehive && blockDiet.Contains("Honey")))
            {
                var stack = new ItemStack(block);
                stacks.Add(stack);
            }
        }

        list.AddMarginAndTitle(capi, marginTop: 7, titletext: Lang.Get("extrainfo:Food"));

        List<RichTextComponentBase> richText = new();

        foreach (var stack in stacks)
        {
            richText.AddStack(capi, openDetailPageFor, stack);

            richText.Add(new RichTextComponent(capi, "\t", CairoFont.WhiteSmallText())
            {
                VerticalAlign = EnumVerticalAlign.Middle
            });
        }

        list.AddRange(richText);
    }

    public static void AddTraderPropsInfo(this List<RichTextComponentBase> list, ItemSlot inSlot, ICoreClientAPI capi, ActionConsumable<string> openDetailPageFor)
    {
        if (inSlot.Itemstack.Collectible is not ItemCreature itemCreature) return;

        var entityProperties = capi.World.GetEntityType(new AssetLocation(itemCreature.Code.Domain, itemCreature.CodeEndWithoutParts(1)));
        if (entityProperties.Class != "EntityTrader") return;

        var tradeProps = GetTradeProps(entityProperties);
        if (tradeProps == null) return;

        var gear = new ItemStack(capi.World.GetItem(new AssetLocation("gear-rusty")));
        var buyingStacks = tradeProps.Buying.List.Where(tradeItem => tradeItem.Resolve(capi.World, "")).ToList();
        var sellingStacks = tradeProps.Selling.List.Where(tradeItem2 => tradeItem2.Resolve(capi.World, "")).ToList();

        list.AddMarginAndTitle(capi, marginTop: 7, titletext: Lang.Get("You can Sell"));
        List<RichTextComponentBase> richTextSell = new();
        foreach (var item in buyingStacks)
        {
            richTextSell.AddTraderInfo(capi, item, openDetailPageFor, gear);
        }
        list.AddRange(richTextSell);

        list.AddMarginAndTitle(capi, marginTop: 7, titletext: Lang.Get("You can Buy"));
        List<RichTextComponentBase> richTextBuy = new();
        foreach (var item in sellingStacks)
        {
            richTextBuy.AddTraderInfo(capi, item, openDetailPageFor, gear);
        }
        list.AddRange(richTextBuy);
    }

    public static void AddTradersInfo(this List<RichTextComponentBase> list, ItemSlot inSlot, ICoreClientAPI capi, ActionConsumable<string> openDetailPageFor)
    {
        var collObj = inSlot.Itemstack.Collectible;
        // var itemstack = inSlot.Itemstack; // x.Attributes == itemstack.Attributes

        List<RichTextComponentBase> richTextSell = new();
        TradeItem[] buyingList = new TradeItem[] { };
        foreach (var entityType in capi.World.EntityTypes.Where(x => x.Class == "EntityTrader" && GetTradeProps(x)?.Buying.List.Any(x => x.Code == collObj.Code) == true))
        {
            buyingList = GetTradeProps(entityType)?.Buying.List;
            if (buyingList == null) break;

            var stack = GetCreatureStack(capi, entityType);
            if (stack == null) continue;

            richTextSell.AddStack(capi, openDetailPageFor, stack);
            richTextSell.Add(new RichTextComponent(capi, "\t", CairoFont.WhiteSmallText())
            {
                VerticalAlign = EnumVerticalAlign.Middle
            });
        }
        if (buyingList?.Length != 0)
        {
            list.AddMarginAndTitle(capi, marginTop: 7, titletext: Lang.Get("Purchased by"));
            list.AddRange(richTextSell);
        }

        List<RichTextComponentBase> richTextBuy = new();
        TradeItem[] sellingList = new TradeItem[] { };
        foreach (var entityType in capi.World.EntityTypes.Where(x => x.Class == "EntityTrader" && GetTradeProps(x)?.Selling.List.Any(x => x.Code == collObj.Code) == true))
        {
            sellingList = GetTradeProps(entityType)?.Selling.List;
            if (sellingList == null) break;

            var stack = GetCreatureStack(capi, entityType);
            if (stack == null) continue;

            richTextBuy.AddStack(capi, openDetailPageFor, stack);
            richTextBuy.Add(new RichTextComponent(capi, "\t", CairoFont.WhiteSmallText())
            {
                VerticalAlign = EnumVerticalAlign.Middle
            });
        }
        if (sellingList?.Length != 0)
        {
            list.AddMarginAndTitle(capi, marginTop: 7, titletext: Lang.Get("Sold by"));
            list.AddRange(richTextBuy);
        }
    }

    private static ItemStack GetCreatureStack(ICoreClientAPI capi, EntityProperties entityType)
    {
        var location = entityType.Code.Clone().WithPathPrefix("creature-");
        var item = capi.World.GetItem(location);
        return item == null ? null : new ItemStack(item);
    }

    private static TradeProperties GetTradeProps(EntityProperties props) => props.Attributes["tradeProps"].AsObject<TradeProperties>(null);

    private static string GetMinMax(NatFloat natFloat)
    {
        int min = (int)Math.Max(1, Math.Round(natFloat.avg - natFloat.var));
        int max = (int)Math.Max(1, Math.Round(natFloat.avg + natFloat.var));

        return min == max ? $"{min}" : string.Format("{0} - {1}", min, max);
    }

    private static string GetMinMaxPercent(PanningDrop drop, float extraMul)
    {
        var min = (drop.Chance.avg - drop.Chance.var) * extraMul * 100;
        var max = (drop.Chance.avg + drop.Chance.var) * extraMul * 100;
        return min == max ? $"{min} %" : string.Format("{0} - {1} %", min, max);
    }

    private static void AddTraderInfo(this List<RichTextComponentBase> richText, ICoreClientAPI capi, TradeItem val, ActionConsumable<string> openDetailPageFor, ItemStack gear)
    {
        richText.AddStack(capi, openDetailPageFor, val.ResolvedItemstack, showStacksize: true);
        richText.Add(new RichTextComponent(capi, "\t" + GetMinMax(val.Stock), CairoFont.WhiteSmallText())
        {
            VerticalAlign = EnumVerticalAlign.Middle
        });
        richText.AddEqualSign(capi);
        richText.AddStack(capi, openDetailPageFor, gear);
        richText.Add(new RichTextComponent(capi, GetMinMax(val.Price) + "\n", CairoFont.WhiteSmallText())
        {
            VerticalAlign = EnumVerticalAlign.Middle
        });
    }

    private static void AddMarginAndTitle(this List<RichTextComponentBase> list, ICoreClientAPI capi, float marginTop, string titletext)
    {
        list.Add(new ClearFloatTextComponent(capi, marginTop));
        list.Add(new RichTextComponent(capi, titletext + "\n", CairoFont.WhiteSmallText().WithWeight(FontWeight.Bold)));
    }

    private static void AddEqualSign(this List<RichTextComponentBase> richText, ICoreClientAPI capi)
    {
        richText.Add(new RichTextComponent(capi, " = ", CairoFont.WhiteMediumText())
        {
            VerticalAlign = EnumVerticalAlign.Middle
        });
    }

    private static void AddStack(this List<RichTextComponentBase> richText, ICoreClientAPI capi, ActionConsumable<string> openDetailPageFor, ItemStack stack, bool showStacksize = false)
    {
        richText.Add(new ItemstackTextComponent(capi, stack, 40, 0, EnumFloat.Inline, (cs) => openDetailPageFor(GuiHandbookItemStackPage.PageCodeForStack(cs)))
        {
            ShowStacksize = showStacksize
        });
    }

    private static void AddStacks(this List<RichTextComponentBase> richText, ICoreClientAPI capi, ActionConsumable<string> openDetailPageFor, ItemStack[] stacks, bool showStacksize = false)
    {
        richText.Add(new SlideshowItemstackTextComponent(capi, stacks, 40, EnumFloat.Inline, (cs) => openDetailPageFor(GuiHandbookItemStackPage.PageCodeForStack(cs)))
        {
            ShowStackSize = showStacksize
        });
    }

    private static List<JsonItemStackBuildStage> GetFuelStacks(this BlockPitkiln blockPitKiln, ICoreClientAPI capi)
    {
        return ObjectCacheUtil.GetOrCreate(capi, blockPitKiln.Code.ToString(), delegate
        {
            var fuelStacks = blockPitKiln?.Attributes?["buildMats"]?["fuel"]?.AsObject<JsonItemStackBuildStage[]>();

            List<JsonItemStackBuildStage> stacks = new();
            stacks.AddRange(fuelStacks.Where(stack => stack?.BurnTimeHours != null));

            foreach (var stack in stacks)
            {
                stack.Resolve(capi.World, "");
            }

            return stacks;
        });
    }

    private static Dictionary<ItemStack[], PanningDrop[]> GetPanningDrops(ICoreClientAPI capi, BlockPan blockPan)
    {
        return ObjectCacheUtil.GetOrCreate(capi, blockPan.Code.ToString(), delegate
        {
            var dropsBySourceMat = blockPan.GetField<Dictionary<string, PanningDrop[]>>("dropsBySourceMat");

            Dictionary<ItemStack[], PanningDrop[]> panningDrops = new();

            foreach (var key in dropsBySourceMat.Keys)
            {
                List<ItemStack> blockStacks = new();
                foreach (var block in capi.World.Blocks.Where(x => x.WildCardMatch(key)))
                {
                    string rocktype = block?.Variant["rock"];

                    foreach (var drop in dropsBySourceMat[key])
                    {
                        if (drop.Code.Path.Contains("{rocktype}"))
                        {
                            drop.Code.Path = drop.Code.Path.Replace("{rocktype}", rocktype);
                        }
                    }

                    var stack = new ItemStack(block);
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
        return ObjectCacheUtil.GetOrCreate(capi, config.Code, delegate
        {
            var stacks = new List<ItemStack>();
            foreach (var obj in capi.World.Collectibles.Where(x => x.WildCardMatch(config.Content.Code)))
            {
                var stack = new ItemStack(obj);
                if (!stack.ResolveBlockOrItem(capi.World)) continue;
                stacks.Add(stack);
            }
            return stacks;
        });
    }
}
