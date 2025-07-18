using HarmonyLib;
using SRML.SR;

namespace SlimesAndMarket;

// This patch is to avoid a sale entry conflict with the sell things mod
[HarmonyPatch(typeof(PlortRegistry), nameof(PlortRegistry.RegisterPlort))]
internal static class SkipKookadoba
{
    public static bool Prefix(Identifiable.Id id) => id != Identifiable.Id.KOOKADOBA_FRUIT;
}