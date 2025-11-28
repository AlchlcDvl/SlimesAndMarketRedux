using SRML.Config.Attributes;

namespace SlimesAndMarket;

[ConfigFile("SlimesAndMarket")]
internal static class Config
{
    [ConfigComment("Enables the registration of slimes to the market")]
    public static bool REGISTER_SLIMES = true;

    [ConfigComment("Enables the registration of largos to the market")]
    public static bool REGISTER_LARGOS = true;

    [ConfigComment("Enables the registration of tarrs to the market")]
    public static bool REGISTER_TARRS = true;

    [ConfigComment("Enables the registration of foods to the market")]
    public static bool REGISTER_FOODS = true;

    [ConfigComment("Enables the registration of slime science items to the market")]
    public static bool REGISTER_ITEMS = true;

    [ConfigComment("Master control to disable registration of anything. Don't know why you'd want to use this, but it was requested")]
    public static bool DO_NOT_REGISTER_ANYTHING = false;
}