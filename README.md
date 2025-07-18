# Slimes And Market Redux
A mod for Slime Rancher 1 that allows you to sell slimes, just like their plorts! This is a continuation of the original [Slimes And Market](https://www.nexusmods.com/slimerancher/mods/118) mod.

## Info
Just to make some things clear:
1. You can sell all types of vanilla slimes normally (except for glitch, gold, lucky, saber and quicksilver slimes; all of which require specific mods, see later in the list)
2. You cannot sell tarr or largo slimes (I might add them though if there's enough demand for it)
3. Slime sale profits are on average four times as much as their plort sales (aka 4 x Plort Price)
4. Gold and lucky slimes can only be sold if you have the [More Vaccables](https://www.nexusmods.com/slimerancher/mods/4) mod are are sold for 10 times their plort price, lucky slimes are sold for 5 x 250 newbucks
5. Quicksilver slimes can only be sold if you have the [Quicksilver Rancher](https://www.nexusmods.com/slimerancher/mods/130) mod are are sold for 5 times their plort price
6. This mod supports selling pure saber slimes using the [Pure Saber Slimes](https://www.nexusmods.com/slimerancher/mods/75) mod, which are sold for 50 times their plort price
7. Glitch slimes can be sold using the [Glitch Rancher](https://www.nexusmods.com/slimerancher/mods/86) mod, which are sold for 10 times their plort price
8. You can sell veggies and fruits

## For Developers

### Compatibility & Selling
To allow selling anything from your mod, you need to do two things.

First, add the mod's dll file to your list of dependencies (be it a `Libs` folder or VS configurations).

This way you can use the mod's code in your code and allow your mod to compile properly.

Then, in your modinfo.json file, add the following line:

```json
"load_after": ["slimesandmarket"]
```

Or, if the json property is already there, add `"slimesandmarket"` as an element to that array.

This way, your mod loads after this one (needed as this mod is hooking into SRML's PlortRegistry and you trying to add your custom entry prior to the hooking would break the chain).

For subsequent sections, using `SRModLoader.IsModPresent("slimesandmarket")` ensures that it runs only if the mod is there in your mods folder.

### Selling Modded Slimes
To allow selling your own modded slimes you must do two things.

Anywhere in your code, add the following code:

```cs
public static void SoftRegisterSlimeMarket(Identifiable.Id slimeId, Identifiable.Id plortId, float multiplier = 4f, float basePrice = 0f, float slimeSaturation = 0f, ProgressDirector.ProgressType[] progress = null)
{
    try
    {
        SlimesAndMarket.ExtraSlimes.RegisterSlime(slimeId, plortId, multiplier, basePrice, slimeSaturation, progress);
    } catch {}
}
```

This is to avoid making your mod dependent on this one.

After that, after setting up all of your slimes use the following method for each slime (add the mod's dll to the list of dependencies to allow the mod to compile):

```cs
if (SRModLoader.IsModPresent("slimesandmarket"))
{
    YourClass.SoftRegisterSlimeMarket(YourSlimeId, YourSlimesPlortId, Multiplier, NonPlortBasePrice, NonPlortSaturation, ProgressToUnlock); // Repeat this for every slime of yours
}
```

Where:
`YourSlimeId` is the `Identifiable.Id` of your custom slime
`YourSlimesPlortId` is the `Identifiable.Id` of your custom slime's plort (or enter 0 if you want to use a different base price)
`Multiplier` is the factor of the base price
`NonPlortBasePrice` is the other base price of your custom slime if it doesn't produce a plort or if you want its base price to not depend on its resulting plort
`NonPlortSaturation` is the market saturation of your slime if wasn't dependent on the same value as its plort.
`ProgressToUnlock` is an array of `ProgressDirector.ProgressType` values to signal what progress the player should have reached for the market sale be unlocked (leave null to either always be unlocked or be unlocked the same time as their plort)

### Selling Modded Items

Similar to how you would sell modded slimes, anywhere in your code add the following code:

```cs
public static void SoftRegisterItemMarket(Identifiable.Id itemId, float value, float saturation, ProgressDirector.ProgressType[] progress = null)
{
    try
    {
        SlimesAndMarket.ExtraSlimes.SetSellable(itemId, value, saturation, progress);
    } catch {}
}
```

After that, after setting up all of your items use the following method for each item:

```cs
if (SRModLoader.IsModPresent("slimesandmarket"))
{
    YourClass.SoftRegisterSlimeMarket(YourItemId, ItemPrice, ItemSaturation, ProgressToUnlock); // Repeat this for every item of yours
}
```

Where:
`YourItemId` is the `Identifiable.Id` of your custom slime
`ItemPrice` is the base market price for your item
`ItemSaturation` is the market saturation of your item
`ProgressToUnlock` is an array of `ProgressDirector.ProgressType` values to signal what progress the player should have reached for the market sale be unlocked