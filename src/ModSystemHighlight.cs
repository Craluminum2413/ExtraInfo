using System;
using System.Collections.Generic;
using System.Threading;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

namespace ExtraInfo;

public class ModSystemHighlight : ModSystem, IHighlightThread
{
    public bool Enabled { get; set; }
    public Thread OpThread { get; set; }

    public virtual string Name { get; }
    public virtual string ThreadName { get; }
    public virtual string OperatorName => ThreadName + "Operator";
    public virtual int Radius { get; }

    public string StringEnabled => Lang.Get("worldconfig-snowAccum-Enabled");
    public string StringDisabled => Lang.Get("worldconfig-snowAccum-Disabled");
    public string StringToggle => Lang.Get("extrainfo:Toggle." + Enabled, Name, Enabled ? StringEnabled : StringDisabled);

    public bool ToggleRun(ICoreClientAPI capi)
    {
        if (!Enabled)
        {
            Enabled = true;
            OpThread = new Thread(() => Run(capi))
            {
                IsBackground = true,
                Name = OperatorName
            };
            OpThread.Start();
        }
        else
        {
            Enabled = false;
        }

        capi.TriggerChatMessage(StringToggle);

        return true;
    }

    protected void Run(ICoreClientAPI capi)
    {
        while (Enabled)
        {
            Thread.Sleep(100);
            try
            {
                OnRunning(capi);
            }
            catch (ThreadAbortException)
            {
                Thread.ResetAbort();
                break;
            }
            catch { }
        }
        ClearHighlights(capi);
    }

    public virtual void OnRunning(ICoreClientAPI capi)
    {
    }

    private void ClearHighlights(ICoreClientAPI capi)
    {
        capi.Event.EnqueueMainThreadTask(new Action(() => capi.World.HighlightBlocks(capi.World.Player, 5229, new List<BlockPos>())), ThreadName);
    }
}