using SRML;
using SRML.SR;
using SConsole = SRML.Console.Console;
using ProgressType = ProgressDirector.ProgressType;

namespace SlimesAndMarket;

internal sealed class Main : ModEntryPoint
{
    public static SConsole.ConsoleInstance Console;

    private static readonly ProgressType[] AlreadyUnlocked = [];

    private static readonly List<(Identifiable.Id, Identifiable.Id, ProgressType[])> VANILLA_SLIMES =
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

    private static readonly List<(Identifiable.Id, float, ProgressType[])> FOODS =
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

    public override void PreLoad()
    {
        Console = ConsoleInstance;
        HarmonyInstance.PatchAll(typeof(Main).Assembly);
    }

    public override void Load()
    {
        if (Config.DO_NOT_REGISTER_ANYTHING)
            return;

        if (Config.REGISTER_SLIMES)
        {
            foreach (var (slimeId, plortId, progress) in VANILLA_SLIMES)
                MarketRegistry.RegisterSlime(slimeId, plortId, 2.5f, progress: progress);

            // Only loading the special slime sales if relevant mods are enabled, because there would be no other way the player would be able to sell them otherwise

            if (SRModLoader.IsModPresent("komikspl_quicksilver_rancher"))
                MarketRegistry.RegisterSlime(Identifiable.Id.QUICKSILVER_SLIME, Identifiable.Id.QUICKSILVER_PLORT, 5f, 200f, 0f, [ProgressType.UNLOCK_MOCHI_MISSIONS]);

            if (SRModLoader.IsModPresent("puresaberslime"))
                MarketRegistry.RegisterSlime(Identifiable.Id.SABER_SLIME, Identifiable.Id.SABER_PLORT, 50f, progress: [ProgressType.UNLOCK_OGDEN_MISSIONS]);

            if (SRModLoader.IsModPresent("more_vaccing"))
            {
                var luckyExists = Enum.TryParse<Identifiable.Id>("LUCKY_PLORT", out var plort);
                MarketRegistry.RegisterSlime(Identifiable.Id.LUCKY_SLIME, luckyExists ? plort : 0, 5f, luckyExists ? 0f : 250f, luckyExists ? 0f : 10000f);
                MarketRegistry.RegisterSlime(Identifiable.Id.GOLD_SLIME, Identifiable.Id.GOLD_PLORT, 10f);
            }

            if (SRModLoader.IsModPresent("glitchrancher"))
                MarketRegistry.RegisterSlime(Identifiable.Id.GLITCH_SLIME, Enum.TryParse<Identifiable.Id>("GLITCH_PLORT", out var plort) ? plort : 0, 10f, 60f, 0f, [ProgressType.UNLOCK_VIKTOR_MISSIONS]);
        }

        if (!Config.REGISTER_FOODS)
            return;

        foreach (var (id, price, progress) in FOODS)
            MarketRegistry.RegisterFood(id, price, 0f, progress);

        // Conditional sales based on what's there

        var growableExists = SRModLoader.IsModPresent("kookagingergrow");
        MarketRegistry.SetSellable(Identifiable.Id.KOOKADOBA_FRUIT, growableExists ? 30f : 75f, false, 0f, [ProgressType.UNLOCK_OGDEN_MISSIONS]);
        MarketRegistry.SetSellable(Identifiable.Id.GINGER_VEGGIE, growableExists ? 125f : 500f, false, growableExists ? 0f : 10000f, [ProgressType.UNLOCK_DESERT]);
    }

    public override void PostLoad()
    {
        if (Config.DO_NOT_REGISTER_ANYTHING)
            return;

        if (Config.REGISTER_LARGOS)
        {
            foreach (var largoId in Identifiable.LARGO_CLASS)
                MarketRegistry.RegisterLargo(largoId);
        }

        if (Config.REGISTER_TARRS)
        {
            MarketRegistry.RegisterTarr(Identifiable.Id.TARR_SLIME);

            if (SRModLoader.IsModPresent("glitchrancher"))
                MarketRegistry.RegisterTarr(Identifiable.Id.GLITCH_TARR_SLIME, 1.5f, [ProgressType.UNLOCK_VIKTOR_MISSIONS]);

            var count = PlortRegistry.valueMapsToPatch.Count + SceneContext.Instance.EconomyDirector.baseValueMap.Length + MarketRegistry.SellableItems.Count;
            var sum = PlortRegistry.valueMapsToPatch.Sum(x => x.value) + SceneContext.Instance.EconomyDirector.baseValueMap.Sum(x => x.value) + MarketRegistry.SellableItems.Sum(x => x.Item2);
            var avg = sum / count;

            foreach (var (tarr, multiplier, progress) in MarketRegistry.SellableTarrs)
                MarketRegistry.RegisterSlime(tarr, 0, multiplier, avg, 0f, progress);
        }

        // Delaying the addition of slimes and other non-plort items as sellable things so that plorts are always at the top
        foreach (var (id, price, saturation, progress) in MarketRegistry.SellableItems)
        {
            PlortRegistry.AddEconomyEntry(id, price, saturation); // Create a market entry
            PlortRegistry.AddPlortEntry(id, progress ?? AlreadyUnlocked); // Allow progress tracking
        }
    }
}