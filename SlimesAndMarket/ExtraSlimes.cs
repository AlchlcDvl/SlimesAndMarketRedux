using SRML.SR;
using UnityEngine;

namespace SlimesAndMarket;

/// <summary>
/// Original registry class.
/// </summary>
[Obsolete("Moved to MarketRegistry due to the former name being a misnomer.")]
public static class ExtraSlimes
{
    /// <summary>
    /// Registers a slime to the plort market.
    /// </summary>
    /// <param name="slimeId">The id of the slime being registered..</param>
    /// <param name="plortId">The id of the slime's plort, if it exists. Defaults to <c>Identifiable.Id.NONE</c>.</param>
    /// <param name="multiplier">The multiplier factor that decides the slime's market price. Defaults to <c>2.5f</c>.</param>
    /// <param name="basePrice">The base price of the slime. Defaults to <c>0f</c>.</param>
    /// <param name="slimeSaturation">The market saturation of the slime. Defaults to <c>0f</c>.</param>
    /// <param name="progress">How much progress must the player have reached to achieve. Defaults to <c>null</c> indicating already unlocked.</param>
    /// <exception cref="Exception">Thrown if market values couldn't be set properly.</exception>
    public static void RegisterSlime
    (
        Identifiable.Id slimeId, Identifiable.Id plortId = 0,
        float multiplier = 2.5f, float basePrice = 0f, float slimeSaturation = 0f,
        ProgressDirector.ProgressType[] progress = null
    ) => MarketRegistry.RegisterSlime(slimeId, plortId, multiplier, basePrice, slimeSaturation, progress);

    /// <summary>
    /// Registers a generic identifiable item as sellable to the plort market.
    /// </summary>
    /// <param name="itemId">The id of the item being registered.</param>
    /// <param name="price">The base price of the slime.</param>
    /// <param name="saturation">The market saturation of the slime. Defaults to <c>0f</c></param>
    /// <param name="progress">How much progress must the player have reached to unlock the sale on the market UI. Defaults to <c>null</c> indicating already unlocked.</param>
    public static void SetSellable(Identifiable.Id itemId, float price, float saturation = 0f, ProgressDirector.ProgressType[] progress = null)
        => MarketRegistry.SetSellable(itemId, price, true, saturation, progress);
}

/// <summary>
/// Registry class that handles registering market entries.
/// </summary>
public static class MarketRegistry
{
    internal static readonly List<(Identifiable.Id, float, float, ProgressDirector.ProgressType[])> SellableItems = [];

    private static (float, float) GetValueMap(Identifiable.Id id, float price, float saturation)
    {
        // Everything that can be sold
        var func = new Predicate<EconomyDirector.ValueMap>(Predicate);
        var value = Array.Find(SceneContext.Instance.EconomyDirector.baseValueMap, func) ?? PlortRegistry.valueMapsToPatch.Find(func);

        return ((value?.value ?? 0) > 0f ? value.value : price, (value?.fullSaturation ?? 0) > 0f ? value.fullSaturation : saturation);

        bool Predicate(EconomyDirector.ValueMap x) => x.accept.id == id;
    }

    private static MarketUI.PlortEntry GetPlortEntry(Identifiable.Id id) => PlortRegistry.plortsToPatch.Find(x => x.id == id);

    /// <summary>
    /// Registers a slime to the plort market.
    /// </summary>
    /// <param name="slimeId">The id of the slime being registered..</param>
    /// <param name="plortId">The id of the slime's plort, if it exists. Defaults to <c>Identifiable.Id.NONE</c>.</param>
    /// <param name="multiplier">The multiplier factor that decides the slime's market price. Defaults to <c>2.5f</c>.</param>
    /// <param name="basePrice">The base price of the slime. Defaults to <c>0f</c>.</param>
    /// <param name="slimeSaturation">The market saturation of the slime. Defaults to <c>0f</c>.</param>
    /// <param name="progress">How much progress must the player have reached to achieve. Defaults to <c>null</c> indicating already unlocked.</param>
    /// <exception cref="Exception">Thrown if market values couldn't be set properly.</exception>
    public static void RegisterSlime
    (
        Identifiable.Id slimeId, Identifiable.Id plortId = 0,
        float multiplier = 2.5f, float basePrice = 0f, float slimeSaturation = 0f,
        ProgressDirector.ProgressType[] progress = null
    )
    {
        var (value, saturation) = slimeId switch
        {
            _ when basePrice > 0f => (basePrice, slimeSaturation),
            _ when plortId != 0 => GetValueMap(plortId, basePrice, slimeSaturation),
            _ => throw new Exception($"Could not find/set up sellable plort data for {plortId} for slime {slimeId}")
        };
        SetSellable(slimeId, value * multiplier, saturation, progress ?? GetPlortEntry(plortId)?.toUnlock); // Allow selling the slime
    }

    /// <summary>
    /// Registers a generic identifiable item as sellable to the plort market.
    /// </summary>
    /// <param name="itemId">The id of the item being registered.</param>
    /// <param name="price">The base price of the slime.</param>
    /// <param name="saturation">The market saturation of the slime. Defaults to <c>0f</c></param>
    /// <param name="progress">How much progress must the player have reached to unlock the sale on the market UI. Defaults to <c>null</c> indicating already unlocked.</param>
    public static void SetSellable(Identifiable.Id itemId, float price, float saturation = 0f, ProgressDirector.ProgressType[] progress = null)
        => SetSellable(itemId, price, true, saturation, progress);

    internal static void SetSellable(Identifiable.Id itemId, float price, bool registerToDrone, float saturation = 0f, ProgressDirector.ProgressType[] progress = null)
    {
        // Mark the item for delayed registry so that plorts appear at the top first
        SellableItems.Add((itemId, price, saturation > 0f ? saturation : CalculateSaturation(price), progress ?? Main.AlreadyUnlocked));

        if (registerToDrone)
            DroneRegistry.RegisterBasicTarget(itemId); // And make it so that the drones can grab them
    }

    /// <summary>
    /// Calculates the market saturation of an entry based on its price.
    /// </summary>
    /// <param name="price">The price of the entry.</param>
    /// <returns>The closest approximation to the vanilla market saturation chart.</returns>
    public static float CalculateSaturation(float price) => price == 0f ? 1f : Mathf.Round(82f / Mathf.Pow(price, 0.36f));
}