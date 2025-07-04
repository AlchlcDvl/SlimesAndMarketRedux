using SRML.SR;

namespace SlimesAndMarket;

public static class ExtraSlimes
{
    public static readonly List<(Identifiable.Id, Identifiable.Id)> VANILLA_SLIMES =
    [
        (Identifiable.Id.PINK_SLIME, Identifiable.Id.PINK_PLORT),
        (Identifiable.Id.TABBY_SLIME, Identifiable.Id.TABBY_PLORT),
        (Identifiable.Id.ROCK_SLIME, Identifiable.Id.ROCK_PLORT),
        (Identifiable.Id.PHOSPHOR_SLIME, Identifiable.Id.PHOSPHOR_PLORT),
        (Identifiable.Id.RAD_SLIME, Identifiable.Id.RAD_PLORT),
        (Identifiable.Id.BOOM_SLIME, Identifiable.Id.BOOM_PLORT),
        (Identifiable.Id.CRYSTAL_SLIME, Identifiable.Id.CRYSTAL_PLORT),
        (Identifiable.Id.HUNTER_SLIME, Identifiable.Id.HUNTER_PLORT),
        (Identifiable.Id.HONEY_SLIME, Identifiable.Id.HONEY_PLORT),
        (Identifiable.Id.QUANTUM_SLIME, Identifiable.Id.QUANTUM_PLORT),
        (Identifiable.Id.DERVISH_SLIME, Identifiable.Id.DERVISH_PLORT),
        (Identifiable.Id.TANGLE_SLIME, Identifiable.Id.TANGLE_PLORT),
        (Identifiable.Id.MOSAIC_SLIME, Identifiable.Id.MOSAIC_PLORT),
        (Identifiable.Id.PUDDLE_SLIME, Identifiable.Id.PUDDLE_PLORT),
        (Identifiable.Id.FIRE_SLIME, Identifiable.Id.FIRE_PLORT),
    ];

    public static EconomyDirector.ValueMap GetValueMap(Identifiable.Id id)
    {
        // Everything that can be sold
        foreach (var valueMap in SRSingleton<SceneContext>.Instance.EconomyDirector.baseValueMap)
        {
            // Main.Instance.ConsoleInstance.Log("GetValueMap: " + valueMap.accept.id);

            if (id == valueMap.accept.id)
                return valueMap;
        }

        return null;
    }

    public static void RegisterSlime(Identifiable.Id slimeId, Identifiable.Id plortId, float multiplier = 4f, float basePrice = 0f, float slimeSaturation = 0f)
    {
        var valueMap = GetValueMap(plortId);
        var (value, saturation) = slimeId switch
        {
            Identifiable.Id.QUICKSILVER_SLIME when plortId == 0 => (basePrice, slimeSaturation),
            Identifiable.Id.LUCKY_SLIME => (basePrice, slimeSaturation),
            _ when valueMap != null => (valueMap.value, valueMap.fullSaturation),
            _ => throw new Exception($"Could not find sellable plort data for {plortId} for slime {slimeId}")
        };
        PlortRegistry.RegisterPlort(slimeId, value * multiplier, saturation);
        DroneRegistry.RegisterBasicTarget(slimeId); // And make so that the drones can grab slimes
    }
}