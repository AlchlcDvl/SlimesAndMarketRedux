A mod for Slime Rancher 1 that allows you to sell slimes, just like their plorts! This is a continuation of the original [Slimes And Market](https://www.nexusmods.com/slimerancher/mods/118) mod.

Just to make some things clear:
1. You can sell all types of vanilla slimes normally (except for gold, lucky, saber and quicksilver slimes; all of which require specific mods, see later in the list)
2. You cannot sell tarr or largo slimes (I might add them though if there's enough demand for it)
3. Slime sale profits are on average four times as much as their plort sales (aka 4 x Plort Price)
4. Gold and lucky slimes can only be sold if you have the [More Vaccables](https://www.nexusmods.com/slimerancher/mods/4) mod are are sold for 10 times their plort price, lucky slimes are sold for 5 x 250 newbucks
5. Quicksilver slimes can only be sold if you have the [Quicksilver Rancher](https://www.nexusmods.com/slimerancher/mods/130) mod are are sold for 5 times their plort price
6. This mod supports selling pure saber slimes using the [Pure Saber Slimes](https://www.nexusmods.com/slimerancher/mods/75) mod, which are sold for 50 times their plort price

FOR DEVELOPERS:
To allow﻿ selling your own modded slimes you must do three things.

First, in your modinfo.json file, add the following line:


```json
"load_after": ["slimesandmarket"]
```

This way, your mod loads after this one (needed as this mod is hooking into SRML's PlortRegistry and you trying to add your custom entry prior to the hooking would break the chain).

Once done, anywhere in your code, add the following method:

```cs
﻿public static void SoftRegisterSlimeMarket(Identifiable.Id slimeId, Identifiable.Id plortId, float multiplier = 4f, float basePrice = 0f, float slimeSaturation = 0f)
{
    ﻿try
    ﻿{
        ﻿﻿SlimesAndMarket.ExtraSlimes.RegisterSlime(slimeId, plortId, multiplier, basePrice, slimeSaturation);
    ﻿} catch {}
}
```

This is to avoid making your mod dependent on this one.

After that, in your main class' Load method, after setting up all of your slimes use the following method for each slime (add the mod's dll to the list of dependencies to allow the mod to compile):

```cs
if (SRModLoader.IsModPresent("slimesandmarket"))
{
    ﻿YourClass.SoftRegisterSlimeMarket(YourSlimeId, YourSlimesPlortId, Multiplier, NonPlortBasePrice, NonPlortSaturation); // Repeat this for every slime of yours
}
```

Where `YourSlimeId` is the `Identifiable.Id` of your custom slime, `YourSlimesPlortId` is the `Idenitifiable.Id` of your custom slime's plort (or enter 0 if you want to use a different base price), `Multiplier` is the factor of the base price, `NonPlortBasePrice` is the other base price of your custom slime if it doesn't produce a plort or if you want its base price to not depend on its resulting plort and `NonPlortSaturation` is the market saturation of your slime if wasn't dependent on the same value as its plort.

The above code block ensures that it runs only if the mod is there in your mods folder.
