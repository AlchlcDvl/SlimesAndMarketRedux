using SRML;
using SRML.SR;
using ProgressType = ProgressDirector.ProgressType;

namespace SlimesAndMarket;

internal sealed class Main : ModEntryPoint
{
    public static readonly ProgressType[] AlreadyUnlocked = [];

    private static readonly List<(Identifiable.Id SlimeId, Identifiable.Id PlortId, ProgressType[] Progress)> VANILLA_SLIMES =
    [
        (Identifiable.Id.PINK_SLIME, Identifiable.Id.PINK_PLORT, AlreadyUnlocked),
        (Identifiable.Id.TABBY_SLIME, Identifiable.Id.TABBY_PLORT, AlreadyUnlocked),
        (Identifiable.Id.ROCK_SLIME, Identifiable.Id.ROCK_PLORT, AlreadyUnlocked),

        (Identifiable.Id.PHOSPHOR_SLIME, Identifiable.Id.PHOSPHOR_PLORT, AlreadyUnlocked),
        (Identifiable.Id.RAD_SLIME, Identifiable.Id.RAD_PLORT, [ProgressType.UNLOCK_QUARRY]),
        (Identifiable.Id.CRYSTAL_SLIME, Identifiable.Id.CRYSTAL_PLORT, [ProgressType.UNLOCK_QUARRY]),

        (Identifiable.Id.HUNTER_SLIME, Identifiable.Id.HUNTER_PLORT, [ProgressType.UNLOCK_MOSS]),
        (Identifiable.Id.HONEY_SLIME, Identifiable.Id.HONEY_PLORT, [ProgressType.UNLOCK_MOSS]),

        (Identifiable.Id.QUANTUM_SLIME, Identifiable.Id.QUANTUM_PLORT, [ProgressType.UNLOCK_RUINS]),

        (Identifiable.Id.DERVISH_SLIME, Identifiable.Id.DERVISH_PLORT, [ProgressType.UNLOCK_DESERT]),
        (Identifiable.Id.TANGLE_SLIME, Identifiable.Id.TANGLE_PLORT, [ProgressType.UNLOCK_DESERT]),
        (Identifiable.Id.MOSAIC_SLIME, Identifiable.Id.MOSAIC_PLORT, [ProgressType.UNLOCK_DESERT]),
        (Identifiable.Id.FIRE_SLIME, Identifiable.Id.FIRE_PLORT, [ProgressType.UNLOCK_DESERT]),

        (Identifiable.Id.BOOM_SLIME, Identifiable.Id.BOOM_PLORT, [ProgressType.UNLOCK_QUARRY, ProgressType.UNLOCK_MOSS]),
        (Identifiable.Id.PUDDLE_SLIME, Identifiable.Id.PUDDLE_PLORT, [ProgressType.UNLOCK_QUARRY, ProgressType.UNLOCK_MOSS]),
    ];

    private static readonly List<(Identifiable.Id ID, float Price, ProgressType[] Progress)> FOODS =
    [
        (Identifiable.Id.CARROT_VEGGIE, 2f, AlreadyUnlocked),
        (Identifiable.Id.OCAOCA_VEGGIE, 4f, [ProgressType.UNLOCK_QUARRY]),
        (Identifiable.Id.PARSNIP_VEGGIE, 7f, [ProgressType.UNLOCK_DESERT]),
        (Identifiable.Id.BEET_VEGGIE, 3f, AlreadyUnlocked),
        (Identifiable.Id.ONION_VEGGIE, 7f, [ProgressType.UNLOCK_QUARRY]),

        (Identifiable.Id.PEAR_FRUIT, 7f, [ProgressType.UNLOCK_DESERT]),
        (Identifiable.Id.POGO_FRUIT, 2f, AlreadyUnlocked),
        (Identifiable.Id.LEMON_FRUIT, 5f, [ProgressType.UNLOCK_RUINS]),
        (Identifiable.Id.CUBERRY_FRUIT, 3f, AlreadyUnlocked),
        (Identifiable.Id.MANGO_FRUIT, 4f, [ProgressType.UNLOCK_MOSS]),
    ];

    public override void PreLoad() => HarmonyInstance.PatchAll(typeof(Main).Assembly);

    public override void Load()
    {
        VANILLA_SLIMES.ForEach(x => MarketRegistry.RegisterSlime(x.SlimeId, x.PlortId, progress: x.Progress));
        FOODS.ForEach(x => MarketRegistry.SetSellable(x.ID, x.Price, 0f, x.Progress));

        var growableExists = SRModLoader.IsModPresent("kookagingergrow");
        MarketRegistry.SetSellable(Identifiable.Id.KOOKADOBA_FRUIT, growableExists ? 30f : 75f, !growableExists, 0f, [ProgressType.UNLOCK_OGDEN_MISSIONS]);
        MarketRegistry.SetSellable(Identifiable.Id.GINGER_VEGGIE, growableExists ? 125f : 500f, !growableExists, growableExists ? 0f : 10000f, [ProgressType.UNLOCK_DESERT]);

        // Only loading the special slime sales if relevant mods are enabled, because there would be no other way the player would be able to sell them otherwise

        if (SRModLoader.IsModPresent("komikspl_quicksilver_rancher"))
            MarketRegistry.RegisterSlime(Identifiable.Id.QUICKSILVER_SLIME, Identifiable.Id.QUICKSILVER_PLORT, 5f, 200f, 0f, [ProgressType.UNLOCK_MOCHI_MISSIONS]);

        if (SRModLoader.IsModPresent("puresaberslime"))
            MarketRegistry.RegisterSlime(Identifiable.Id.SABER_SLIME, Identifiable.Id.SABER_PLORT, 50f, progress: [ProgressType.UNLOCK_OGDEN_MISSIONS]);

        if (SRModLoader.IsModPresent("more_vaccing"))
        {
            MarketRegistry.RegisterSlime(Identifiable.Id.GOLD_SLIME, Identifiable.Id.GOLD_PLORT, 10f);
            MarketRegistry.RegisterSlime(Identifiable.Id.LUCKY_SLIME, Enum.TryParse<Identifiable.Id>("LUCKY_PLORT", out var plort) ? plort : 0, 25f, 250f, 10000f);
        }

        if (SRModLoader.IsModPresent("glitchrancher"))
            MarketRegistry.RegisterSlime(Identifiable.Id.GLITCH_SLIME, Enum.TryParse<Identifiable.Id>("GLITCH_PLORT", out var plort) ? plort : 0, 10f, 60f, 0f, [ProgressType.UNLOCK_VIKTOR_MISSIONS]);
    }

    public override void PostLoad()
    {
        // Delaying the addition of slimes and other non-plort items as sellable things so that plorts are always at the top
        foreach (var (id, price, saturation, progress) in MarketRegistry.SellableItems)
        {
            PlortRegistry.AddEconomyEntry(id, price, saturation); // Create a market entry
            PlortRegistry.AddPlortEntry(id, progress); // Allow progress tracking
        }
    }
}