using SRML.SR;
using UnityEngine;

namespace SlimesAndMarket;

/// <summary>
/// Original registry class.
/// </summary>
[Obsolete("Moved to MarketRegistry due to the former name ExtraSlimes now being a misnomer.")]
public static class ExtraSlimes
{
    /// <summary>
    /// Registers a slime to the plort market.
    /// </summary>
    /// <param name="slimeId">The id of the slime being registered.</param>
    /// <param name="plortId">The id of the slime's plort, if it exists. Defaults to <c>Identifiable.Id.NONE</c>.</param>
    /// <param name="multiplier">The multiplier factor that decides the slime's market price. Defaults to <c>2.5f</c>.</param>
    /// <param name="basePrice">The base price of the slime. Defaults to <c>0f</c>.</param>
    /// <param name="slimeSaturation">The market saturation of the slime. Defaults to <c>0f</c>.</param>
    /// <param name="progress">How much progress must the player have reached to achieve. Defaults to <c>null</c> indicating already unlocked.</param>
    /// <exception cref="Exception">Thrown if market values couldn't be set properly.</exception>
    public static void RegisterSlime
    (
        Identifiable.Id slimeId, Identifiable.Id plortId = 0,
        float multiplier = 1f, float basePrice = 0f, float slimeSaturation = 0f,
        ProgressDirector.ProgressType[] progress = null
    ) => MarketRegistry.RegisterSlime(slimeId, plortId, multiplier, basePrice, slimeSaturation, progress);

    /// <summary>
    /// Registers a generic identifiable item as sellable to the plort market.
    /// </summary>
    /// <param name="itemId">The id of the item being registered.</param>
    /// <param name="price">The base price of the slime.</param>
    /// <param name="saturation">The market saturation of the slime. Defaults to <c>0f</c>.</param>
    /// <param name="progress">How much progress must the player have reached to unlock the sale on the market UI. Defaults to <c>null</c> indicating already unlocked.</param>
    public static void SetSellable(Identifiable.Id itemId, float price, float saturation = 0f, ProgressDirector.ProgressType[] progress = null)
        => MarketRegistry.SetSellable(itemId, price, saturation, progress);
}

/// <summary>
/// Registry class that handles registering market entries.
/// </summary>
public static class MarketRegistry
{
    internal static readonly List<(Identifiable.Id, float, float, ProgressDirector.ProgressType[])> SellableItems = [];
    internal static readonly List<(Identifiable.Id, float, ProgressDirector.ProgressType[])> SellableTarrs = [];
    internal static readonly HashSet<string> FoodSuffixes = ["_VEGGIE", "_HEN", "_FRUIT"];

    private static readonly HashSet<Identifiable.Id> AlreadyRegistered = new(Identifiable.idComparer);

    private static (float, float) GetValueMap(Identifiable.Id id, float price = 0f, float saturation = 0f)
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
    /// <param name="slimeId">The id of the slime being registered.</param>
    /// <param name="plortId">The id of the slime's plort, if it exists. Defaults to <c>Identifiable.Id.NONE</c> to allow searching for the plort.</param>
    /// <param name="multiplier">The multiplier factor that decides the slime's market price. Defaults to <c>1f</c>.</param>
    /// <param name="basePrice">The base price of the slime. Defaults to <c>0f</c> to enable price calculation.</param>
    /// <param name="slimeSaturation">The market saturation of the slime. Defaults to <c>0f</c> to enable saturation calculation.</param>
    /// <param name="progress">How much progress must the player have reached to achieve. Defaults to <c>null</c> indicating already unlocked.</param>
    /// <exception cref="Exception">Thrown if market values couldn't be set properly.</exception>
    public static void RegisterSlime
    (
        Identifiable.Id slimeId, Identifiable.Id plortId = 0,
        float multiplier = 1f, float basePrice = 0f, float slimeSaturation = 0f,
        ProgressDirector.ProgressType[] progress = null
    )
    {
        if (!Config.REGISTER_SLIMES)
            return;

        if (!AlreadyRegistered.Add(slimeId))
        {
            Main.Console.LogError($"{slimeId} is already registered!");
            return;
        }

        var (value, saturation) = slimeId switch
        {
            _ when basePrice > 0f => (basePrice, slimeSaturation),
            _ when plortId != 0 => GetValueMap(plortId, basePrice, slimeSaturation),
            _ => throw new Exception($"Could not find/set up sellable plort data for {plortId} for slime {slimeId}")
        };
        SetSellable(slimeId, value * multiplier, true, saturation, progress ?? GetPlortEntry(plortId)?.toUnlock); // Allow selling the slime
    }

    /// <summary>
    /// Registers a food item as sellable to the plort market.
    /// </summary>
    /// <param name="itemId">The id of the item being registered.</param>
    /// <param name="price">The base price of the item.</param>
    /// <param name="saturation">The market saturation of the item. Defaults to <c>0f</c> to enable saturation calculation.</param>
    /// <param name="progress">How much progress must the player have reached to unlock the sale on the market UI. Defaults to <c>null</c> indicating already unlocked.</param>
    public static void RegisterFood(Identifiable.Id itemId, float price, float saturation = 0f, ProgressDirector.ProgressType[] progress = null)
    {
        if (Config.REGISTER_FOODS)
            SetSellable(itemId, price, false, saturation, progress);
    }

    /// <summary>
    /// Registers a slime science item as sellable to the plort market.
    /// </summary>
    /// <param name="itemId">The id of the item being registered.</param>
    /// <param name="price">The base price of the item.</param>
    /// <param name="saturation">The market saturation of the item. Defaults to <c>0f</c> to enable saturation calculation.</param>
    /// <param name="progress">How much progress must the player have reached to unlock the sale on the market UI. Defaults to <c>null</c> indicating already unlocked.</param>
    public static void RegisterItem(Identifiable.Id itemId, float price, float saturation = 0f, ProgressDirector.ProgressType[] progress = null)
    {
        if (Config.REGISTER_ITEMS)
            SetSellable(itemId, price, false, saturation, progress);
    }

    /// <summary>
    /// Registers a generic slime science item as sellable to the plort market.
    /// </summary>
    /// <param name="itemId">The id of the item being registered.</param>
    /// <param name="price">The base price of the item.</param>
    /// <param name="saturation">The market saturation of the item. Defaults to <c>0f</c> to enable saturation calculation.</param>
    /// <param name="progress">How much progress must the player have reached to unlock the sale on the market UI. Defaults to <c>null</c> indicating already unlocked.</param>
    [Obsolete("Prefer using the other RegisterX methods instead, as this one is not influenced by configs.")]
    public static void SetSellable(Identifiable.Id itemId, float price, float saturation = 0f, ProgressDirector.ProgressType[] progress = null)
        => SetSellable(itemId, price, false, saturation, progress);

    internal static void SetSellable(Identifiable.Id itemId, float price, bool skipCheck = false, float saturation = 0f, ProgressDirector.ProgressType[] progress = null)
    {
        if (Config.DO_NOT_REGISTER_ANYTHING)
            return;

        if (!skipCheck && !AlreadyRegistered.Add(itemId))
        {
            Main.Console.LogError($"{itemId} is already registered!");
            return;
        }

        // Mark the item for delayed registry so that plorts appear at the top first
        SellableItems.Add((itemId, price, saturation > 0f ? saturation : CalculateSaturation(price), progress));
    }

    /// <summary>
    /// Calculates the market saturation of an entry based on its price.
    /// </summary>
    /// <param name="price">The price of the entry.</param>
    /// <returns>The closest approximation to the vanilla market saturation chart.</returns>
    public static float CalculateSaturation(float price) => price == 0f ? 1f : Mathf.Round(82f / Mathf.Pow(price, 0.36f));

    /// <summary>
    /// Registers a tarr-like slime to the plort market.
    /// </summary>
    /// <param name="tarrId">The id of the tarr.</param>
    /// <param name="multiplier">The multiplier factor that decides the tarr's market price. Defaults to <c>1f</c>.</param>
    /// <param name="progress">How much progress must the player have reached to unlock the sale on the market UI. Defaults to <c>null</c> indicating already unlocked.</param>
    public static void RegisterTarr(Identifiable.Id tarrId, float multiplier = 1f, ProgressDirector.ProgressType[] progress = null)
    {
        if (!Config.REGISTER_TARRS)
            return;

        if (!AlreadyRegistered.Contains(tarrId))
        {
            Main.Console.LogError($"{tarrId} is already registered!");
            return;
        }

        SellableTarrs.Add((tarrId, multiplier, progress));
        DroneRegistry.RegisterBasicTarget(tarrId);
    }

    /// <summary>
    /// Registers a largo to the slime market.
    /// </summary>
    /// <param name="largoId">The id of the largo.</param>
    /// <param name="price">The base price of the largo. Defaults to <c>0f</c> to enable price calculation.</param>
    /// <param name="saturation">The market saturation of the largo. Defaults to <c>0f</c> to enable saturation calculation.</param>
    /// <param name="progress">How much progress must the player have reached to unlock the sale on the market UI. Defaults to <c>null</c> indicating already unlocked.</param>
    public static void RegisterLargo(Identifiable.Id largoId, float price = 0f, float saturation = 0f, ProgressDirector.ProgressType[] progress = null)
    {
        // Ensure the id can be registered first

        if (!Config.REGISTER_LARGOS)
            return;

        if (!AlreadyRegistered.Add(largoId))
        {
            Main.Console.LogError($"{largoId} is already registered!");
            return;
        }

        // Split and assign name parts
        var parts = largoId.ToString().Split('_');

        var name1 = parts[0];
        var name2 = parts[1];

        // If no price is given, attempt to calculate one
        if (price == 0f)
        {
            var (price1, result1) = FindValues(name1);
            var (price2, result2) = FindValues(name2);

            float multiplier;

            if (result1 == result2)
                multiplier = FindMultiplierForDouble(result1);
            else
            {
                if (result1 == FindResult.Plort)
                    price1 *= 2.5f;
                else if (result1 == FindResult.Neither)
                    price2 *= 2f;

                if (result2 == FindResult.Plort)
                    price2 *= 2.5f;
                else if (result2 == FindResult.Neither)
                    price1 *= 2f;

                multiplier = 0.9f;
            }

            price = (price1 + price2) * multiplier;
        }

        SetSellable(largoId, price, true, saturation, progress ?? CalculateProgress(name1, name2)); // Allow selling the largo
    }

    private static (float, FindResult) FindValues(string name)
    {
        var (foundPrice, _) = GetValueMap(Parse<Identifiable.Id>(name + "_SLIME"));

        if (foundPrice > 0f)
            return (foundPrice, FindResult.Slime);

        (foundPrice, _) = GetValueMap(Parse<Identifiable.Id>(name + "_PLORT"));

        if (foundPrice > 0f)
            return (foundPrice, FindResult.Plort);

        return (0f, FindResult.Neither);
    }

    private static T Parse<T>(string name) where T : struct, Enum => (T)Enum.Parse(typeof(T), name);

    private static float FindMultiplierForDouble(FindResult result) => result switch
    {
        FindResult.Slime => 0.9f,
        FindResult.Plort => 2.25f,
        _ => throw new("Unable to find a value to base off of!"),
    };

    private static ProgressDirector.ProgressType[] CalculateProgress(string name1, string name2)
    {
        var plort1 = GetPlortEntry(Parse<Identifiable.Id>(name1 + "_PLORT"))?.toUnlock ?? [];
        var plort2 = GetPlortEntry(Parse<Identifiable.Id>(name2 + "_PLORT"))?.toUnlock ?? [];
        return [.. plort1, .. plort2];
    }

    private enum FindResult
    {
        Slime,
        Plort,
        Neither
    }

    /// <summary>
    /// Adds a suffix for foods to avoid food items from automatically registered.
    /// </summary>
    /// <param name="suffix">The suffix to register.</param>
    public static void RegisterFoodSuffix(string suffix) => FoodSuffixes.Add(suffix);
}