using HarmonyLib;
using SRML;
using SRML.SR;

namespace SlimesAndMarket;

internal sealed class Main : ModEntryPoint
{
    public static List<EconomyDirector.ValueMap> ValuesToPatch;
    public static List<MarketUI.PlortEntry> PlortsToPatch;

    public static readonly ProgressDirector.ProgressType[] AlreadyUnlocked = [];

    private static readonly List<(Identifiable.Id SlimeId, Identifiable.Id PlortId, ProgressDirector.ProgressType[] Progress)> VANILLA_SLIMES =
    [
        (Identifiable.Id.PINK_SLIME, Identifiable.Id.PINK_PLORT, AlreadyUnlocked),
        (Identifiable.Id.TABBY_SLIME, Identifiable.Id.TABBY_PLORT, AlreadyUnlocked),
        (Identifiable.Id.ROCK_SLIME, Identifiable.Id.ROCK_PLORT, AlreadyUnlocked),

        (Identifiable.Id.PHOSPHOR_SLIME, Identifiable.Id.PHOSPHOR_PLORT, AlreadyUnlocked),
        (Identifiable.Id.RAD_SLIME, Identifiable.Id.RAD_PLORT, [ProgressDirector.ProgressType.UNLOCK_QUARRY]),
        (Identifiable.Id.CRYSTAL_SLIME, Identifiable.Id.CRYSTAL_PLORT, [ProgressDirector.ProgressType.UNLOCK_QUARRY]),

        (Identifiable.Id.HUNTER_SLIME, Identifiable.Id.HUNTER_PLORT, [ProgressDirector.ProgressType.UNLOCK_MOSS]),
        (Identifiable.Id.HONEY_SLIME, Identifiable.Id.HONEY_PLORT, [ProgressDirector.ProgressType.UNLOCK_MOSS]),

        (Identifiable.Id.QUANTUM_SLIME, Identifiable.Id.QUANTUM_PLORT, [ProgressDirector.ProgressType.UNLOCK_RUINS]),

        (Identifiable.Id.DERVISH_SLIME, Identifiable.Id.DERVISH_PLORT, [ProgressDirector.ProgressType.UNLOCK_DESERT]),
        (Identifiable.Id.TANGLE_SLIME, Identifiable.Id.TANGLE_PLORT, [ProgressDirector.ProgressType.UNLOCK_DESERT]),
        (Identifiable.Id.MOSAIC_SLIME, Identifiable.Id.MOSAIC_PLORT, [ProgressDirector.ProgressType.UNLOCK_DESERT]),
        (Identifiable.Id.FIRE_SLIME, Identifiable.Id.FIRE_PLORT, [ProgressDirector.ProgressType.UNLOCK_DESERT]),

        (Identifiable.Id.BOOM_SLIME, Identifiable.Id.BOOM_PLORT, [ProgressDirector.ProgressType.UNLOCK_QUARRY, ProgressDirector.ProgressType.UNLOCK_MOSS]),
        (Identifiable.Id.PUDDLE_SLIME, Identifiable.Id.PUDDLE_PLORT, [ProgressDirector.ProgressType.UNLOCK_QUARRY, ProgressDirector.ProgressType.UNLOCK_MOSS]),
    ];

    private static readonly List<(Identifiable.Id ID, float Price, float Saturation, ProgressDirector.ProgressType[] Progress)> FOODS =
    [
        (Identifiable.Id.CARROT_VEGGIE, 2f, 5f, AlreadyUnlocked),
        (Identifiable.Id.OCAOCA_VEGGIE, 4f, 5f, [ProgressDirector.ProgressType.UNLOCK_QUARRY]),
        (Identifiable.Id.PARSNIP_VEGGIE, 7f, 5f, [ProgressDirector.ProgressType.UNLOCK_DESERT]),
        (Identifiable.Id.BEET_VEGGIE, 3f, 5f, AlreadyUnlocked),
        (Identifiable.Id.ONION_VEGGIE, 7f, 5f, [ProgressDirector.ProgressType.UNLOCK_QUARRY]),

        (Identifiable.Id.PEAR_FRUIT, 7f, 5f, [ProgressDirector.ProgressType.UNLOCK_DESERT]),
        (Identifiable.Id.POGO_FRUIT, 2f, 5f, AlreadyUnlocked),
        (Identifiable.Id.LEMON_FRUIT, 5f, 5f, [ProgressDirector.ProgressType.UNLOCK_RUINS]),
        (Identifiable.Id.CUBERRY_FRUIT, 3f, 5f, AlreadyUnlocked),
        (Identifiable.Id.MANGO_FRUIT, 4f, 5f, [ProgressDirector.ProgressType.UNLOCK_MOSS]),
    ];

    public override void PreLoad()
    {
        HarmonyInstance.PatchAll();

        var type = typeof(PlortRegistry);
        ValuesToPatch = AccessTools.Field(type, "valueMapsToPatch").GetValue(null) as List<EconomyDirector.ValueMap>;
        PlortsToPatch = AccessTools.Field(type, "plortsToPatch").GetValue(null) as List<MarketUI.PlortEntry>;
    }

    public override void Load()
    {
        VANILLA_SLIMES.ForEach(x => ExtraSlimes.RegisterSlime(x.SlimeId, x.PlortId, progress: x.Progress));
        FOODS.ForEach(x => ExtraSlimes.SetSellable(x.ID, x.Price, x.Saturation, x.Progress));

        var growableExists = SRModLoader.IsModPresent("kookagingergrow");
        ExtraSlimes.SetSellable(Identifiable.Id.KOOKADOBA_FRUIT, growableExists ? 30f : 75f, 5f, !growableExists, [ProgressDirector.ProgressType.UNLOCK_OGDEN_MISSIONS]);
        ExtraSlimes.SetSellable(Identifiable.Id.GINGER_VEGGIE, growableExists ? 75f : 500f, 5f, !growableExists, [ProgressDirector.ProgressType.UNLOCK_DESERT]);

        // Only loading the special slime sales if relevant mods are enabled, because there would be no other way the player would be able to sell them otherwise

        if (SRModLoader.IsModPresent("komikspl_quicksilver_rancher"))
            ExtraSlimes.RegisterSlime(Identifiable.Id.QUICKSILVER_SLIME, Identifiable.Id.QUICKSILVER_PLORT, 5f, 200f, 100f, [ProgressDirector.ProgressType.UNLOCK_MOCHI_MISSIONS]);

        if (SRModLoader.IsModPresent("puresaberslime"))
            ExtraSlimes.RegisterSlime(Identifiable.Id.SABER_SLIME, Identifiable.Id.SABER_PLORT, 50f, progress: [ProgressDirector.ProgressType.UNLOCK_OGDEN_MISSIONS]);

        if (SRModLoader.IsModPresent("more_vaccing"))
        {
            ExtraSlimes.RegisterSlime(Identifiable.Id.GOLD_SLIME, Identifiable.Id.GOLD_PLORT, 10f);
            ExtraSlimes.RegisterSlime(Identifiable.Id.LUCKY_SLIME, Enum.TryParse<Identifiable.Id>("LUCKY_PLORT", out var plort) ? plort : 0, 25f, 250f, 125f);
        }

        if (SRModLoader.IsModPresent("glitchrancher"))
            ExtraSlimes.RegisterSlime(Identifiable.Id.GLITCH_SLIME, Enum.TryParse<Identifiable.Id>("GLITCH_PLORT", out var plort) ? plort : 0, 10f, 60f, 10f, [ProgressDirector.ProgressType.ENTER_ZONE_SLIMULATION]);
    }

    public override void PostLoad()
    {
        // Delaying the addition of slimes and other non-plort items as sellable things so that plorts are always at the top
        foreach (var (id, price, saturation, progress) in ExtraSlimes.SellableItems)
        {
            PlortRegistry.AddEconomyEntry(id, price, saturation); // Create a market entry
            PlortRegistry.AddPlortEntry(id, progress); // Allow progress tracking
        }
    }
}