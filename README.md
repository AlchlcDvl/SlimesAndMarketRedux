# Slimes And Market Redux
A mod for Slime Rancher 1 that allows you to sell slimes, just like their plorts! This is a continuation of the original [Slimes And Market](https://www.nexusmods.com/slimerancher/mods/118) mod.

## Info
Just to make some things clear:
1. You can sell most types of vanilla slimes normally (exceptions are glitch, gold, lucky, saber and quicksilver slimes; all of which require specific mods, see later in the list).
2. You can sell tarr slimes (prices are averaged from the current economy) and largo slimes (calculated based on their components).
3. Slime sale profits are **2.5x** their plort sales (e.g., Pink Slime Price = 2.5 * Pink Plort Price).
4. **Gold and Lucky Slimes:** Can only be sold if you have the [More Vaccables](https://www.nexusmods.com/slimerancher/mods/4) mod.
    * **Gold Slimes:** Sold for **10x** their plort price.
    * **Lucky Slimes:** Sold for **5x** their plort price (approx. 1250 Newbucks if plorts are disabled).
5. **Quicksilver Slimes:** Can only be sold if you have the [Quicksilver Rancher](https://www.nexusmods.com/slimerancher/mods/130) mod. Sold for **5x** their plort price.
6. **Saber Slimes:** Supports selling pure saber slimes using the [Pure Saber Slimes](https://www.nexusmods.com/slimerancher/mods/75) mod. Sold for **50x** their plort price.
7. **Glitch Slimes:** Can be sold using the [Glitch Rancher](https://www.nexusmods.com/slimerancher/mods/86) mod. Sold for **10x** their plort price.
8. You can also sell veggies, fruits, slime science items, and other item groups if enabled in the config.

## For Developers

### Compatibility & Selling
To allow selling items from your mod via the Plort Market, you must add `SlimesAndMarket.dll` as a reference in your project dependencies.

**Note:** You should check `SRModLoader.IsModPresent("slimesandmarket")` before running any registration code to prevent crashes if the user does not have this mod installed.

### Selling Modded Slimes
To allow selling your own modded slimes, add a wrapper method to your code to safely access the registry:

```cs
public static void RegisterSlimeForMarket(Identifiable.Id slimeId, Identifiable.Id plortId = Identifiable.Id.NONE, float multiplier = 1f, float basePrice = 0f, float slimeSaturation = 0f, ProgressDirector.ProgressType[] progress = null)
{
    SlimesAndMarket.MarketRegistry.RegisterSlime(slimeId, plortId, multiplier, basePrice, slimeSaturation, progress);
}
```
Then, in your `Load()` method use the above like this:
```cs
if (SRModLoader.IsModPresent("slimesandmarket"))
{
    // Example: Registering a custom slime
    RegisterSlimeForMarket(
        MyMod.MySlimeId,
        MyMod.MyPlortId,
        2.5f, // Multiplier
        0f,   // Base Price (0 to use Plort value)
        0f,   // Saturation (0 to calculate automatically)
        null  // Progress (null to inherit from Plort)
    );
}
```
**Parameters:**
- **slimeId:** The Identifiable.Id of your custom slime.
- **plortId:** The Identifiable.Id of your slime's plort.
- **multiplier:** The multiplier applied to the base value (e.g., 2.5f).
- **basePrice:** Explicit price. Set to 0f to calculate price based on the plortId value.
- **slimeSaturation:** Explicit saturation. Set to 0f to calculate automatically.
- **progress:** Unlock conditions. Leave null to unlock automatically when the plort is unlocked.

### Selling Modded Items (Foods/Resources)
To register items other than slimes (like custom foods or resources), use RegisterFood or RegisterItem.

Wrapper Example:
```cs
public static void RegisterFoodForMarket(Identifiable.Id itemId, float price, float saturation, ProgressDirector.ProgressType[] progress = null)
{
    SlimesAndMarket.MarketRegistry.RegisterFood(itemId, price, saturation, progress);
}
```

Usage:
```cs
if (SRModLoader.IsModPresent("slimesandmarket"))
{
    RegisterFoodForMarket(MyMod.MyCustomVeggieId, 15f, 10f);
}
```

**Parameters:**
- **itemId:** The Identifiable.Id of your custom item.
- **price:** Explicit price.
- **saturation:** Explicit saturation. Set to 0f to calculate automatically.
- **progress:** Unlock conditions. Leave null to unlock automatically when the game starts.