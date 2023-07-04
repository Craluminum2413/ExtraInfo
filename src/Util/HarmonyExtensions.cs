using HarmonyLib;

namespace ExtraInfo;

public static class HarmonyExtensions
{
    public static T GetField<T>(this object instance, string fieldname)
    {
        return (T)AccessTools.Field(instance.GetType(), fieldname).GetValue(instance);
    }

    public static void SetField(this object instance, string fieldname, object setVal)
    {
        AccessTools.Field(instance.GetType(), fieldname).SetValue(instance, setVal);
    }
}
