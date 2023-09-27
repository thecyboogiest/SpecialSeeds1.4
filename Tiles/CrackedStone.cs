using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Specialseeds1point4.Tiles;

internal class CrackedStone : ModTile
{
	public override void SetStaticDefaults()
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		Main.tileSolid[base.Type] = true;
		Main.tileMergeDirt[base.Type] = true;
		Main.tileMerge[base.Type][1] = true;
		Main.tileMerge[1][base.Type] = true;
		Main.tileBlockLight[base.Type] = true;
		Main.tileLighted[base.Type] = false;
		AddMapEntry(new Color(94, 94, 94));
		base.MinPick = 0;
		base.MineResist = 0.1f;
		base.DustType = 1;
		base.RegisterItemDrop(ItemID.StoneBlock);
	}

    public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
    {
        noItem = true;
        SpecialseedsPlayer modPlayer = Main.player[Main.myPlayer].GetModPlayer<SpecialseedsPlayer>();
        if (Main.rand.NextBool())
        {
            if (Main.tile[i + 1, j].TileType == base.Type)
            {
                modPlayer.BreakTile(i + 1, j);
            }
        }
        else if (Main.tile[i - 1, j].TileType == base.Type)
        {
            modPlayer.BreakTile(i - 1, j);
        }
        if (Main.rand.NextBool())
        {
            if (Main.tile[i, j + 1].TileType == base.Type )
            {
                modPlayer.BreakTile(i, j + 1);
            }
        }
        else if (Main.tile[i, j - 1].TileType == base.Type )
        {
            modPlayer.BreakTile(i, j - 1);
        }
    }
}
