using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;


namespace Specialseeds1point4.Tiles
{
    class CrackedStone : ModTile
    {

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileMerge[Type][TileID.Stone] = true;
            Main.tileMerge[TileID.Stone][Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = false;
            AddMapEntry(new Color(94, 94, 94));

            MinPick = 0;
            
            MineResist = 0.1f;
            SoundType = 21;
            SoundStyle = 2;
            DustType = 1;
            ItemDrop = ItemID.StoneBlock;
            
        }


        

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            noItem = true;
            SpecialseedsPlayer modPlayer = Main.player[Main.myPlayer].GetModPlayer<SpecialseedsPlayer>();

            if(Main.rand.Next(2) == 0)
            {
                if (Main.tile[i + 1, j].type == Type && Main.tile[i + 1, j].IsActive)
                    modPlayer.BreakTile(i + 1, j);
            }
            else
            {
                if (Main.tile[i - 1, j].type == Type && Main.tile[i - 1, j].IsActive)
                    modPlayer.BreakTile(i - 1, j);
            }

            if (Main.rand.Next(2) == 0)
            {
                if (Main.tile[i, j + 1].type == Type && Main.tile[i, j + 1].IsActive)
                    modPlayer.BreakTile(i, j + 1);
            }
            else
            {
                if (Main.tile[i, j - 1].type == Type && Main.tile[i, j - 1].IsActive)
                    modPlayer.BreakTile(i, j - 1);
            }



        }



    }
}