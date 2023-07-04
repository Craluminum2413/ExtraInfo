using System;
using System.Collections.Generic;
using System.Linq;
using Cairo;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;
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
            ItemstackTextComponent itemStackComponent = new(capi, fuel.ResolvedItemstack, 40, 0, EnumFloat.Inline, (cs) => openDetailPageFor(GuiHandbookItemStackPage.PageCodeForStack(cs)));
            richText.Add(itemStackComponent);

            richText.Add(new RichTextComponent(capi, ColorText(Lang.Get("{0} hours", fuel.BurnTimeHours.ToString())) + "\n", CairoFont.WhiteSmallText())
            {
                VerticalAlign = EnumVerticalAlign.Middle
            });
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
                var location = entityType.Code.Clone().WithPathPrefix("creature-");
                var item = capi.World.GetItem(location);
                if (item == null)
                {
                    capi.Logger.Error("ItemCreature: No such entity - {0}", location);
                    continue;
                }

                var stack = new ItemStack(item);
                stacks.Add(stack);
            }
        }

        if (stacks?.Count == 0) return;

        list.AddMarginAndTitle(capi, marginTop: 7, titletext: Lang.Get("Eaten by wild animals."));

        List<RichTextComponentBase> richText = new();

        foreach (var stack in stacks)
        {
            ItemstackTextComponent itemStackComponent = new(capi, stack, 40, 0, EnumFloat.Inline, (cs) => openDetailPageFor(GuiHandbookItemStackPage.PageCodeForStack(cs)));
            richText.Add(itemStackComponent);

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
            ItemstackTextComponent itemStackComponent = new(capi, stack, 40, 0, EnumFloat.Inline, (cs) => openDetailPageFor(GuiHandbookItemStackPage.PageCodeForStack(cs)));
            richText.Add(itemStackComponent);

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

            var location = entityType.Code.Clone().WithPathPrefix("creature-");
            var item = capi.World.GetItem(location);
            if (item == null) continue;
            var stack = new ItemStack(item);

            ItemstackTextComponent itemStackComponent = new(capi, stack, 40, 0, EnumFloat.Inline, (cs) => openDetailPageFor(GuiHandbookItemStackPage.PageCodeForStack(cs)));
            richTextSell.Add(itemStackComponent);

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

            var location = entityType.Code.Clone().WithPathPrefix("creature-");
            var item = capi.World.GetItem(location);
            if (item == null) continue;
            var stack = new ItemStack(item);

            ItemstackTextComponent itemStackComponent = new(capi, stack, 40, 0, EnumFloat.Inline, (cs) => openDetailPageFor(GuiHandbookItemStackPage.PageCodeForStack(cs)));
            richTextBuy.Add(itemStackComponent);

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

    private static TradeProperties GetTradeProps(EntityProperties props) => props.Attributes["tradeProps"].AsObject<TradeProperties>(null);

    private static string GetMinMax(NatFloat natFloat)
    {
        int min = (int)Math.Max(1, Math.Round(natFloat.avg - natFloat.var));
        int max = (int)Math.Max(1, Math.Round(natFloat.avg + natFloat.var));

        return min == max ? $"{min}" : string.Format("{0} - {1}", min, max);
    }

    private static void AddTraderInfo(this List<RichTextComponentBase> richText, ICoreClientAPI capi, TradeItem val, ActionConsumable<string> openDetailPageFor, ItemStack gear)
    {
        ItemstackTextComponent itemStackComponent = new(capi, val.ResolvedItemstack, 40, 0, EnumFloat.Inline, (cs) => openDetailPageFor(GuiHandbookItemStackPage.PageCodeForStack(cs)))
        {
            ShowStacksize = true
        };
        richText.Add(itemStackComponent);
        richText.Add(new RichTextComponent(capi, "\t" + GetMinMax(val.Stock), CairoFont.WhiteSmallText())
        {
            VerticalAlign = EnumVerticalAlign.Middle
        });
        richText.AddEqualSign(capi);
        richText.AddGearStack(capi, openDetailPageFor, gear);
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

    private static void AddGearStack(this List<RichTextComponentBase> richText, ICoreClientAPI capi, ActionConsumable<string> openDetailPageFor, ItemStack gear)
    {
        richText.Add(new ItemstackTextComponent(capi, gear, 40, 0, EnumFloat.Inline, (cs) => openDetailPageFor(GuiHandbookItemStackPage.PageCodeForStack(cs))));
    }

    private static List<JsonItemStackBuildStage> GetFuelStacks(this BlockPitkiln blockPitKiln, ICoreClientAPI capi)
    {
        var fuelStacks = blockPitKiln?.Attributes?["buildMats"]?["fuel"]?.AsObject<JsonItemStackBuildStage[]>();

        List<JsonItemStackBuildStage> stacks = new();
        stacks.AddRange(fuelStacks.Where(stack => stack?.BurnTimeHours != null));

        foreach (var stack in stacks)
        {
            stack.Resolve(capi.World, "");
        }

        return stacks;
    }
}
