using SRML;

namespace SlimesAndMarket;

public class Main : ModEntryPoint
{
    public static Main Instance { get; private set; }

    public override void PreLoad()
    {
        Instance = this;
        HarmonyInstance.PatchAll();
    }

    public override void Load()
    {
        ConsoleInstance.Log("Loading SlimesAndMarket");

        try // Try the code there, if some error arises then it calls the "catch" function
        {
            ExtraSlimes.VANILLA_SLIMES.ForEach(x => ExtraSlimes.RegisterSlime(x.Item1, x.Item2));

            // Only loading the special slime sales if relevant mods are enabled, because there would be no other way the player would be able to sell them otherwise

            if (SRModLoader.IsModPresent("komikspl_quicksilver_rancher"))
                ExtraSlimes.RegisterSlime(Identifiable.Id.QUICKSILVER_SLIME, Identifiable.Id.QUICKSILVER_PLORT, 5f, 200f, 100f);

            if (SRModLoader.IsModPresent("puresaberslime"))
                ExtraSlimes.RegisterSlime(Identifiable.Id.SABER_SLIME, Identifiable.Id.SABER_PLORT, 50f);

            if (SRModLoader.IsModPresent("more_vaccing"))
            {
                ExtraSlimes.RegisterSlime(Identifiable.Id.GOLD_SLIME, Identifiable.Id.GOLD_PLORT, 10f);
                ExtraSlimes.RegisterSlime(Identifiable.Id.LUCKY_SLIME, 0, 25f, 250f, 125f);
            }
        }
        catch (Exception e) // But if an error was found
        {
            ConsoleInstance.LogError("Slimes and Market mod error: " + e); // Then log the error into the SRML.log file
        }
    }
}