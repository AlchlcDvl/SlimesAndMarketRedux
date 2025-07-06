using SRML.SR;

namespace SlimesAndMarket;

public static class ExtraSlimes
{
    private static EconomyDirector.ValueMap GetValueMap(Identifiable.Id id)
    {
        // Everything that can be sold

        foreach (var valueMap in SRSingleton<SceneContext>.Instance.EconomyDirector.baseValueMap)
        {
            if (valueMap.accept.id == id)
                return valueMap;
        }

        foreach (var valueMap in Main.ValuesToPatch)
        {
            if (valueMap.accept.id == id)
                return valueMap;
        }

        return null;
    }

    private static MarketUI.PlortEntry GetPlortEntry(Identifiable.Id id)
    {
        foreach (var entry in Main.PlortsToPatch)
        {
            if (entry.id == id)
                return entry;
        }

        return null;
    }

    public static void RegisterSlime
    (
        Identifiable.Id slimeId,
        Identifiable.Id plortId,
        float multiplier = 4f,
        float basePrice = 0f,
        float slimeSaturation = 0f,
        ProgressDirector.ProgressType[] progress = null
    )
    {
        var valueMap = GetValueMap(plortId);
        var (value, saturation) = slimeId switch
        {
            _ when valueMap != null => (valueMap.value, valueMap.fullSaturation),
            _ when basePrice != 0f && slimeSaturation != 0f => (basePrice, slimeSaturation),
            _ => throw new Exception($"Could not find/set up sellable plort data for {plortId} for slime {slimeId}")
        };
        PlortRegistry.AddEconomyEntry(slimeId, value * multiplier, saturation); // Create a market entry
        PlortRegistry.AddPlortEntry(slimeId, progress ?? GetPlortEntry(plortId)?.toUnlock ?? []); // Allow progress tracking
        DroneRegistry.RegisterBasicTarget(slimeId); // And make so that the drones can grab slimes
    }
}