using Vintagestory.API.Config;

namespace ExtraInfo;

public static class TextExtensions
{
    public static string ColorText(string text) => Lang.Get("extrainfo:Color.Cyan", text);
}