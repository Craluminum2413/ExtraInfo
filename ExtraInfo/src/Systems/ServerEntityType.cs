namespace ExtraInfo;

public class ServerEntityType : ModSystem
{
    public static Dictionary<AssetLocation, float> HealthList { get; set; } = new();
    public static Dictionary<AssetLocation, float> DamageList { get; set; } = new();
    public static Dictionary<AssetLocation, int> DamageTierList { get; set; } = new();

    public class EntityType
    {
        public float MaxHealth { get; set; }
        public float Damage { get; set; }
        public int DamageTier { get; set; }
    }

    public override void AssetsFinalize(ICoreAPI api)
    {
        foreach (EntityProperties type in api.World.EntityTypes)
        {
            if (type?.Server == null) continue;
            if (type?.Server?.BehaviorsAsJsonObj == null) continue;
            foreach (JsonObject behavior in type.Server.BehaviorsAsJsonObj)
            {
                if (behavior.ToString().Contains("taskai"))
                {
                    if (DamageList.ContainsKey(type.Code)) continue;
                    if (DamageTierList.ContainsKey(type.Code)) continue;

                    DamageList.Add(type.Code, (float)(Array.Find(behavior?.Token?["aitasks"]?.ToObject<EntityType[]>(), x => x?.Damage != 0)?.Damage ?? 0));
                    DamageTierList.Add(type.Code, (int)(Array.Find(behavior?.Token?["aitasks"]?.ToObject<EntityType[]>(), x => x?.DamageTier != 0)?.DamageTier ?? 0));
                }
                if (behavior.ToString().Contains("health"))
                {
                    if (HealthList.ContainsKey(type.Code)) continue;

                    HealthList.Add(type.Code, behavior.AsObject<EntityType>().MaxHealth);
                }
            }
        }
    }
}
