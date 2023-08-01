using System;
using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace ExtraInfo;

public class HighlightMycelium : ModSystemHighlight
{
    public override string Name => Lang.Get("extrainfo:HighlightMycelium");
    public override string ThreadName => "ExtraInfo:Mycelium";
    public override int Radius => 64;

    // public int FirstHighlightColor => ColorUtil.ColorFromRgba(new Vec4f(1f, 0.4f, 0.4f, 0.5f)); // #ff6666
    public int FirstHighlightColor => SecondHighlightColor;
    public int SecondHighlightColor => ColorUtil.ColorFromRgba(new Vec4f(1f, 1f, 0.4f, 0.5f)); // #ffff66

    public override void StartClientSide(ICoreClientAPI api)
    {
        base.StartClientSide(api);
        api.Input.RegisterHotKey(Name, Lang.Get("extrainfo:Toggle", Name), GlKeys.M, HotkeyType.HelpAndOverlays, shiftPressed: true);
        api.Input.SetHotKeyHandler(Name, _ => ToggleRun(api));
    }

    public override void OnRunning(ICoreClientAPI capi)
    {
        List<BlockPos> positions = new();
        List<int> colors = new();
        var playerPos = capi.World.Player.Entity.Pos.AsBlockPos;

        capi.World.BlockAccessor.WalkBlocks(playerPos.AddCopy(-Radius, -Radius, -Radius), playerPos.AddCopy(Radius, Radius, Radius), (_, x, y, z) =>
        {
            var bPos = new BlockPos(x, y, z);

            var beMycelium = GetMycelium(bPos, capi);
            if (beMycelium != null)
            {
                positions.Add(bPos);
                colors.Add(FirstHighlightColor);

                var range = beMycelium.GetField<int>("growRange");

                capi.World.BlockAccessor.WalkBlocks(bPos.AddCopy(-range, -range, -range), bPos.AddCopy(range, range, range), (_, x, y, z) =>
                {
                    var bPos = new BlockPos(x, y, z);

                    if (positions.Contains(bPos)) return;

                    positions.Add(bPos);
                    colors.Add(SecondHighlightColor);
                });
            }
        });

        capi.Event.EnqueueMainThreadTask(new Action(() => capi.World.HighlightBlocks(capi.World.Player, 5229, positions, colors)), ThreadName);
    }

    private BlockEntityMycelium GetMycelium(BlockPos pos, ICoreAPI api) => api.World.BlockAccessor.GetBlockEntity(pos) as BlockEntityMycelium;
}