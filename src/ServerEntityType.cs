using System;
using System.Collections.Generic;
using Vintagestory.API.Common;

namespace ExtraInfo;

public class ServerEntityType : ModSystem
{
    public override bool AllowRuntimeReload => true;

    public static Dictionary<AssetLocation, BlockDropItemStack[]> HarvestableDrops { get; set; } = new();
    public static Dictionary<AssetLocation, float> HealthList { get; set; } = new();
    public static Dictionary<AssetLocation, float> DamageList { get; set; } = new();
    public static Dictionary<AssetLocation, int> DamageTierList { get; set; } = new();

    public BlockDropItemStack[] Drops { get; set; }
    public float MaxHealth { get; set; }
    public float Damage { get; set; }
    public int DamageTier { get; set; }

    public override void AssetsFinalize(ICoreAPI api)
    {
        foreach (var type in api.World.EntityTypes)
        {
            if (type?.Server == null) continue;
            if (type?.Server?.BehaviorsAsJsonObj == null) continue;
            foreach (var behavior in type.Server.BehaviorsAsJsonObj)
            {
                if (behavior.ToString().Contains("harvestable"))
                {
                    if (HarvestableDrops.ContainsKey(type.Code)) continue;

                    HarvestableDrops.Add(type.Code, behavior.AsObject<ServerEntityType>().Drops);
                }
                if (behavior.ToString().Contains("taskai"))
                {
                    if (DamageList.ContainsKey(type.Code)) continue;
                    if (DamageTierList.ContainsKey(type.Code)) continue;

                    DamageList.Add(type.Code, (float)(Array.Find(behavior?.Token?["aitasks"]?.ToObject<ServerEntityType[]>(), x => x?.Damage != 0)?.Damage ?? 0));
                    DamageTierList.Add(type.Code, (int)(Array.Find(behavior?.Token?["aitasks"]?.ToObject<ServerEntityType[]>(), x => x?.DamageTier != 0)?.DamageTier ?? 0));
                }
                if (behavior.ToString().Contains("health"))
                {
                    if (HealthList.ContainsKey(type.Code)) continue;

                    HealthList.Add(type.Code, behavior.AsObject<ServerEntityType>().MaxHealth);
                }
            }
        }
    }
}
