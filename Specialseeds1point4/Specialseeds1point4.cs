using AdvancedWorldGen.Base;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Specialseeds1point4
{
	public class Specialseeds1point4 : Mod
	{
        
        public override void PostSetupContent()
        {
            API.RegisterOption(this, "Icemania", false);
            API.RegisterOption(this, "Junglemania", false);
            API.RegisterOption(this, "Desertmania", false);
            API.RegisterOption(this, "Meteormania", false);
            API.RegisterOption(this, "RainyDay", false);
            API.RegisterOption(this, "Cavemania", false);
            API.RegisterOption(this, "Nuked", false);
            API.RegisterOption(this, "Trapmania", false);
            API.RegisterOption(this, "Fragmented", false);
            API.RegisterOption(this, "Velocity", false);
            API.RegisterOption(this, "Lifesteal", false);
            API.RegisterOption(this, "NPCmania", false);
            API.RegisterOption(this, "Gigantism", false);
            API.RegisterOption(this, "Slimed", false);
            API.RegisterOption(this, "Healthy", false);
            API.RegisterOption(this, "Murdermania", false);
            API.RegisterOption(this, "Bossmania", false);
        }


    }
}