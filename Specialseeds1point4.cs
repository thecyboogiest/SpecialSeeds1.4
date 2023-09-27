using System.Collections.Generic;
using Terraria.ModLoader;

namespace Specialseeds1point4;
public class Seeds
{
    public static List<string> activatedSeeds = new List<string>();
    
    public const string Custom = "Custom";
    public const string Icemania = "Icemania";
    public const string Junglemania = "Junglemania";
    public const string Desertmania = "Desertmania";
    public const string Meteormania = "Meteormania";
    public const string RainyDay = "RainyDay";
    public const string Cavemania = "Cavemania";
    public const string Nuked = "Nuked";
    public const string Trapmania = "Trapmania";
    public const string Woodlands = "Woodlands";
    public const string Upside = "Upside";

    public static bool Enabled(string seed)
    {
        return activatedSeeds.Contains(seed);
    }

    public static List<string> All()
    {
        return new List<string>() {Custom, Icemania,Junglemania,Desertmania,Meteormania,RainyDay,Cavemania,Nuked,Trapmania,Woodlands,Upside };
    }
}
public class Specialseeds1point4 : Mod
{
    public override void PostSetupContent()
    {
        
        Mod awg = ModLoader.GetMod("AdvancedWorldGen");
        string optionName = "Register Option";
        string specialSeeds = "Special Seeds: ";

        foreach(string seed in Seeds.All())
        {
            awg.Call(optionName, specialSeeds, seed);
        }




    }
}
