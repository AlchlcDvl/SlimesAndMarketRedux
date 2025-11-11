using HarmonyLib;
using SRML.SR;

namespace SlimesAndMarket;

// This patch is to avoid a sale entry conflict with the sell things mod
[HarmonyPatch(typeof(PlortRegistry), nameof(PlortRegistry.RegisterPlort))]
internal static class SkipKookadobaAndGinger
{
    public static bool Prefix(Identifiable.Id id)
    {
        var idString = id.ToString();
        return id is not (Identifiable.Id.KOOKADOBA_FRUIT or Identifiable.Id.GINGER_VEGGIE)
            || (!Config.REGISTER_TARRS && idString.Contains("TARR"))
            || (!Config.REGISTER_ITEMS && idString.EndsWith("_CRAFT", StringComparison.Ordinal))
            || (!Config.REGISTER_SLIMES && idString.EndsWith("_SLIME", StringComparison.Ordinal))
            || (!Config.REGISTER_LARGOS && idString.EndsWith("_LARGO", StringComparison.Ordinal))
            || (!Config.REGISTER_FOODS && idString.EndsWithAny(MarketRegistry.FoodSuffixes));
    }

    private static bool EndsWithAny(this string @string, IEnumerable<string> parts) => parts.Any(x => @string.EndsWith(x, StringComparison.Ordinal));
}