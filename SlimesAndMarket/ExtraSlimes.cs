using SRML.SR;

namespace SlimesAndMarket;

public static class ExtraSlimes
{
    private static EconomyDirector.ValueMap GetValueMap(Identifiable.Id id)
    {
        // Everything that can be sold
        return Array.Find(SceneContext.Instance.EconomyDirector.baseValueMap, Predicate) ?? Main.ValuesToPatch.Find(Predicate);

        bool Predicate(EconomyDirector.ValueMap x) => x.accept.id == id;
    }

    private static MarketUI.PlortEntry GetPlortEntry(Identifiable.Id id) => Main.PlortsToPatch.Find(x => x.id == id);

    public static void RegisterSlime
    (
        Identifiable.Id slimeId, Identifiable.Id plortId,
        float multiplier = 4f,
        float basePrice = 0f, float slimeSaturation = 0f,
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
        SetSellable(slimeId, value * multiplier, saturation, progress ?? GetPlortEntry(plortId)?.toUnlock); // Allow selling the slime
    }

    public static void SetSellable(Identifiable.Id itemId, float price, float saturation, ProgressDirector.ProgressType[] progress = null)
    {
        PlortRegistry.AddEconomyEntry(itemId, price, saturation); // Create a market entry
        PlortRegistry.AddPlortEntry(itemId, progress ?? Main.AlreadyUnlocked); // Allow progress tracking
        DroneRegistry.RegisterBasicTarget(itemId); // And make it so that the drones can grab time
    }
}