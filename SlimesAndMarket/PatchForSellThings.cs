using HarmonyLib;
using SRML.SR;

namespace SlimesAndMarket;

// This patch is to avoid a sale entry conflict with the sell things mod and handle the config values for market registration in case a dev decides to bypass the registry class for some reason
[HarmonyPatch(typeof(PlortRegistry), nameof(PlortRegistry.RegisterPlort))]
internal static class SkipKookadobaAndGinger
{
    public static bool Prefix(Identifiable.Id id)
    {
        if (id is Identifiable.Id.KOOKADOBA_FRUIT or Identifiable.Id.GINGER_VEGGIE)
            return false;

        if (Config.REGISTER_TARRS && Config.REGISTER_ITEMS && Config.REGISTER_SLIMES && Config.REGISTER_LARGOS && Config.REGISTER_FOODS)
            return true;

        var idString = id.ToString();

        var shouldBlock = (!Config.REGISTER_TARRS && idString.Contains("TARR")) ||
            (!Config.REGISTER_ITEMS && idString.EndsWith("_CRAFT", StringComparison.Ordinal)) ||
            (!Config.REGISTER_SLIMES && idString.EndsWith("_SLIME", StringComparison.Ordinal)) ||
            (!Config.REGISTER_LARGOS && idString.EndsWith("_LARGO", StringComparison.Ordinal)) ||
            (!Config.REGISTER_FOODS && idString.EndsWithAny(MarketRegistry.FoodSuffixes));
        return !shouldBlock;
    }

    private static bool EndsWithAny(this string str, IEnumerable<string> parts)
    {
        foreach (var part in parts)
        {
            if (str.EndsWith(part, StringComparison.Ordinal))
                return true;
        }

        return false;
    }
}