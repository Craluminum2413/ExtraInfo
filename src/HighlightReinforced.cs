using System;
using System.Collections.Generic;
using System.Threading;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace ExtraInfo;

public class HighlightReinforced : ModSystem
{
    public static Thread OpThread { get; protected set; }
    public static bool Enabled { get; protected set; }
    public static ICoreClientAPI Api { get; protected set; }
    public static ModSystemBlockReinforcement ModSysBlockReinforcement { get; protected set; }
    public static string StringName => Lang.Get("extrainfo:HighlightReinforcedBlocks");

    public override void StartClientSide(ICoreClientAPI api)
    {
        base.StartClientSide(api);
        api.Input.RegisterHotKey(StringName, Lang.Get("extrainfo:Toggle", StringName), GlKeys.T, HotkeyType.HelpAndOverlays, ctrlPressed: true);
        api.Input.SetHotKeyHandler(StringName, x => ToggleRun(x, api));

        Api = api;
        ModSysBlockReinforcement = api.ModLoader.GetModSystem<ModSystemBlockReinforcement>();
    }

    private bool ToggleRun(KeyCombination x, ICoreClientAPI capi)
    {
        if (!Enabled)
        {
            Enabled = true;
            OpThread = new Thread(Run)
            {
                IsBackground = true,
                Name = "ExtraInfo:ReinforcementsOperator"
            };
            OpThread.Start();
        }
        else
        {
            Enabled = false;
        }

        var StringEnabled = Lang.Get("worldconfig-snowAccum-Enabled");
        var StringDisabled = Lang.Get("worldconfig-snowAccum-Disabled");
        capi.TriggerChatMessage(Lang.Get("extrainfo:Toggle." + Enabled, StringName, Enabled ? StringEnabled : StringDisabled));

        return true;
    }

    private void Run()
    {
        while (Enabled)
        {
            Thread.Sleep(100);
            try
            {
                int rad = GetRadius();

                IClientPlayer player = Api.World.Player;
                BlockPos pPos = player.Entity.Pos.AsBlockPos;
                List<BlockPos> posList = new();
                List<int> colorList = new();

                for (int x = pPos.X - rad; x < pPos.X + rad + 1; x++)
                {
                    for (int y = pPos.Y - rad; y < pPos.Y + rad + 1; y++)
                    {
                        for (int z = pPos.Z - rad; z < pPos.Z + rad + 1; z++)
                        {
                            if (IsAir(x, y, z)) continue;

                            var bPos = new BlockPos(x, y, z);

                            if (!IsReinforced(bPos)) continue;

                            posList.Add(bPos);
                            colorList.Add(GetColor());
                        }
                    }
                }
                Api.Event.EnqueueMainThreadTask(new Action(() => Api.World.HighlightBlocks(player, 5229, posList, colorList)), "ExtraInfo:Reinforcements");
            }
            catch (ThreadAbortException)
            {
                Thread.ResetAbort();
                break;
            }
            catch { }
        }
        ClearHighlights();
    }

    private bool IsAir(int x, int y, int z) => Api.World.BlockAccessor.GetBlock(x, y, z).Id == 0;
    private bool IsReinforced(BlockPos pos) => ModSysBlockReinforcement.IsReinforced(pos);

    private static void ClearHighlights()
    {
        Api.Event.EnqueueMainThreadTask(new Action(() => Api.World.HighlightBlocks(Api.World.Player, 5229, new List<BlockPos>())), "ExtraInfo:Reinforcements");
    }

    private int GetRadius() => 10;
    private int GetColor() => ColorUtil.ToRgba(119, 247, 247, 50);
}