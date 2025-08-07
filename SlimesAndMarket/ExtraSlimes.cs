using SRML.SR;

namespace SlimesAndMarket;

public static class ExtraSlimes
{
    internal static readonly List<(Identifiable.Id, float, float, ProgressDirector.ProgressType[])> SellableItems = [];

    private static EconomyDirector.ValueMap GetValueMap(Identifiable.Id id)
    {
        // Everything that can be sold
        var func = new Predicate<EconomyDirector.ValueMap>(Predicate);
        return Array.Find(SceneContext.Instance.EconomyDirector.baseValueMap, func) ?? Main.ValuesToPatch.Find(func);

        bool Predicate(EconomyDirector.ValueMap x) => x.accept.id == id;
    }

    private static MarketUI.PlortEntry GetPlortEntry(Identifiable.Id id) => Main.PlortsToPatch.Find(x => x.id == id);

    public static void RegisterSlime
    (
        Identifiable.Id slimeId, Identifiable.Id plortId = 0,
        float multiplier = 4f, float basePrice = 0f, float slimeSaturation = 0f,
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
        => SetSellable(itemId, price, saturation, true, progress);

    internal static void SetSellable(Identifiable.Id itemId, float price, float saturation, bool registerToDrone, ProgressDirector.ProgressType[] progress)
    {
        SellableItems.Add((itemId, price, saturation, progress ?? Main.AlreadyUnlocked)); // Mark the item for delayed registry so that plorts appear at the top first

        if (registerToDrone)
            DroneRegistry.RegisterBasicTarget(itemId); // And make it so that the drones can grab time
    }
}