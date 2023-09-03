using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Client;
using Vintagestory.GameContent;
using Vintagestory.API.Common.Entities;

namespace ExtraInfo;

public partial class HarmonyPatches
{
    [HarmonyPatch(typeof(ModSystemHandbook), methodName: "OnHelpHotkey")]
    public static class OpenHandbookForEntityPatch
    {
        public static void Postfix(ref bool __result, ModSystemHandbook __instance)
        {
            GuiDialogHandbook dialog = __instance.GetField<GuiDialogHandbook>("dialog");
            ICoreClientAPI capi = __instance.GetField<ICoreClientAPI>("capi");

            if (capi.World.Player.Entity.Controls.ShiftKey && capi.World.Player.CurrentEntitySelection != null)
            {
                Entity entity = capi.World.Player.CurrentEntitySelection.Entity;

                ItemStack stack = capi.World.GetEntityType(entity.Code).GetCreatureStack(capi);
                if (stack == null)
                {
                    __result = true;
                    return;
                }

                if (!dialog.OpenDetailPageFor(GuiHandbookItemStackPage.PageCodeForStack(stack)))
                {
                    dialog.OpenDetailPageFor(GuiHandbookItemStackPage.PageCodeForStack(new ItemStack(stack.Collectible)));
                }
            }
            __result = true;
        }
    }
}