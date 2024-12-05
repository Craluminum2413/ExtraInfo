namespace ExtraInfo;

[HarmonyPatchCategory("OpenHandbookForEntity")]
[HarmonyPatch(typeof(ModSystemSurvivalHandbook), methodName: "OnSurvivalHandbookHotkey")]
public static class OpenHandbookForEntityPatch
{
    [HarmonyPostfix]
    public static void Postfix(ref bool __result, ModSystemSurvivalHandbook __instance)
    {
        GuiDialogHandbook dialog = __instance.GetField<GuiDialogHandbook>("dialog");
        ICoreClientAPI capi = __instance.GetField<ICoreClientAPI>("capi");

        if (capi.World.Player.CurrentEntitySelection != null)
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