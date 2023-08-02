using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

namespace ExtraInfo;

public static class Constants
{
    public static string StringEnabled => Lang.Get("worldconfig-snowAccum-Enabled");
    public static string StringDisabled => Lang.Get("worldconfig-snowAccum-Disabled");

    public static string ToggleName(string name) => Lang.Get("extrainfo:Toggle", name);
    public static string StringToggle(bool state, string name) => Lang.Get("extrainfo:Toggle." + state, name, state ? StringEnabled : StringDisabled);

    /// <summary>
    /// Colors with 50% transparency
    /// </summary>
    public static class ColorsRGBA
    {
        public static int Cyan => ColorUtil.ColorFromRgba(new Vec4f(0f, 1f, 1f, 0.5f)); // #77f7f7
        public static int Red => ColorUtil.ColorFromRgba(new Vec4f(1f, 0.4f, 0.4f, 0.5f)); // #ff6666
        public static int Yellow => ColorUtil.ColorFromRgba(new Vec4f(1f, 1f, 0.4f, 0.5f)); // #ffff66
    }
}