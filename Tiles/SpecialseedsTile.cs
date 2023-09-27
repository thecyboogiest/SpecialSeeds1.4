using Terraria;
using Terraria.ModLoader;

namespace Specialseeds1point4.Tiles;

internal class SpecialseedsTile : GlobalTile
{
	public string worldSeed = Main.ActiveWorldFileData.Name;

	public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
        Mod awg = ModLoader.GetMod("AdvancedWorldGen");
        
        worldSeed = Main.ActiveWorldFileData.Name;
		SpecialseedsPlayer modPlayer = Main.player[Main.myPlayer].GetModPlayer<SpecialseedsPlayer>();
		if (!(bool)awg.Call("Options Contains", "Fragmented"))
        {
			return;
		}
		if (Main.rand.NextBool())
		{
			if (Main.tile[i + 1, j].TileType == Main.tile[i, j].TileType && WorldGen.SolidTile(i + 1, j))
			{
				modPlayer.BreakTile(i + 1, j);
			}
		}
		else if (Main.tile[i - 1, j].TileType == Main.tile[i, j].TileType && WorldGen.SolidTile(i - 1, j))
		{
			modPlayer.BreakTile(i - 1, j);
		}
		if (Main.rand.NextBool())
		{
			if (Main.tile[i, j + 1].TileType == Main.tile[i, j].TileType && WorldGen.SolidTile(i, j + 1))
			{
				modPlayer.BreakTile(i, j + 1);
			}
		}
		else if (Main.tile[i, j - 1].TileType == Main.tile[i, j].TileType && WorldGen.SolidTile(i, j - 1))
		{
			modPlayer.BreakTile(i, j - 1);
		}
	}
}
