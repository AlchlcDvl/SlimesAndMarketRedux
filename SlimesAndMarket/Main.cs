using HarmonyLib;
using SRML;
using SRML.SR;

namespace SlimesAndMarket;

public sealed class Main : ModEntryPoint
{
    public static Main Instance { get; private set; }

    public static List<EconomyDirector.ValueMap> ValuesToPatch;
    public static List<MarketUI.PlortEntry> PlortsToPatch;

    private static readonly List<(Identifiable.Id, Identifiable.Id, ProgressDirector.ProgressType[])> VANILLA_SLIMES =
    [
        (Identifiable.Id.PINK_SLIME, Identifiable.Id.PINK_PLORT, []),
        (Identifiable.Id.TABBY_SLIME, Identifiable.Id.TABBY_PLORT, []),
        (Identifiable.Id.ROCK_SLIME, Identifiable.Id.ROCK_PLORT, []),
        (Identifiable.Id.PHOSPHOR_SLIME, Identifiable.Id.PHOSPHOR_PLORT, []),
        (Identifiable.Id.RAD_SLIME, Identifiable.Id.RAD_PLORT, [ProgressDirector.ProgressType.UNLOCK_QUARRY]),
        (Identifiable.Id.BOOM_SLIME, Identifiable.Id.BOOM_PLORT, [ProgressDirector.ProgressType.UNLOCK_QUARRY, ProgressDirector.ProgressType.UNLOCK_MOSS]),
        (Identifiable.Id.CRYSTAL_SLIME, Identifiable.Id.CRYSTAL_PLORT, [ProgressDirector.ProgressType.UNLOCK_QUARRY]),
        (Identifiable.Id.HUNTER_SLIME, Identifiable.Id.HUNTER_PLORT, [ProgressDirector.ProgressType.UNLOCK_MOSS]),
        (Identifiable.Id.HONEY_SLIME, Identifiable.Id.HONEY_PLORT, [ProgressDirector.ProgressType.UNLOCK_MOSS]),
        (Identifiable.Id.QUANTUM_SLIME, Identifiable.Id.QUANTUM_PLORT, [ProgressDirector.ProgressType.UNLOCK_RUINS]),
        (Identifiable.Id.DERVISH_SLIME, Identifiable.Id.DERVISH_PLORT, [ProgressDirector.ProgressType.UNLOCK_DESERT]),
        (Identifiable.Id.TANGLE_SLIME, Identifiable.Id.TANGLE_PLORT, [ProgressDirector.ProgressType.UNLOCK_DESERT]),
        (Identifiable.Id.MOSAIC_SLIME, Identifiable.Id.MOSAIC_PLORT, [ProgressDirector.ProgressType.UNLOCK_DESERT]),
        (Identifiable.Id.PUDDLE_SLIME, Identifiable.Id.PUDDLE_PLORT, [ProgressDirector.ProgressType.UNLOCK_QUARRY, ProgressDirector.ProgressType.UNLOCK_MOSS]),
        (Identifiable.Id.FIRE_SLIME, Identifiable.Id.FIRE_PLORT, [ProgressDirector.ProgressType.UNLOCK_DESERT]),
    ];

    public override void PreLoad()
    {
        Instance = this;
        HarmonyInstance.PatchAll();

        ValuesToPatch = AccessTools.Field(typeof(PlortRegistry), "valueMapsToPatch").GetValue(null) as List<EconomyDirector.ValueMap>;
        PlortsToPatch = AccessTools.Field(typeof(PlortRegistry), "plortsToPatch").GetValue(null) as List<MarketUI.PlortEntry>;
    }

    public override void Load()
    {
        VANILLA_SLIMES.ForEach(x => ExtraSlimes.RegisterSlime(x.Item1, x.Item2, progress: x.Item3));

        // Only loading the special slime sales if relevant mods are enabled, because there would be no other way the player would be able to sell them otherwise

        if (SRModLoader.IsModPresent("komikspl_quicksilver_rancher"))
            ExtraSlimes.RegisterSlime(Identifiable.Id.QUICKSILVER_SLIME, Identifiable.Id.QUICKSILVER_PLORT, 5f, 200f, 100f, [ProgressDirector.ProgressType.UNLOCK_MOCHI_MISSIONS]);

        if (SRModLoader.IsModPresent("puresaberslime"))
            ExtraSlimes.RegisterSlime(Identifiable.Id.SABER_SLIME, Identifiable.Id.SABER_PLORT, 50f, progress: [ProgressDirector.ProgressType.UNLOCK_OGDEN_MISSIONS]);

        if (SRModLoader.IsModPresent("more_vaccing"))
        {
            ExtraSlimes.RegisterSlime(Identifiable.Id.GOLD_SLIME, Identifiable.Id.GOLD_PLORT, 10f);
            ExtraSlimes.RegisterSlime(Identifiable.Id.LUCKY_SLIME, 0, 25f, 250f, 125f);
        }
    }
}