using SRML.Config.Attributes;

namespace SlimesAndMarket;

[ConfigFile("SlimesAndMarket")]
internal static class Config
{
    internal static bool REGISTER_SLIMES = true;
    internal static bool REGISTER_LARGOS = true;
    internal static bool REGISTER_TARRS = true;
    internal static bool REGISTER_FOODS = true;
    internal static bool REGISTER_ITEMS = true;

    internal static bool DO_NOT_REGISTER_ANYTHING = false;
}