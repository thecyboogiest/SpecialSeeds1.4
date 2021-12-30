﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdvancedWorldGen.Base;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;


namespace Specialseeds1point4.Tiles
{
    class SpecialseedsTile : GlobalTile
    {
        public string worldSeed = Main.ActiveWorldFileData.Name;

        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            worldSeed = Main.ActiveWorldFileData.Name;
            SpecialseedsPlayer modPlayer = Main.player[Main.myPlayer].GetModPlayer<SpecialseedsPlayer>();

            if(API.OptionsContains("Fragmented"))
            {
                if (Main.rand.Next(2) == 0)
                {
                    if (Main.tile[i + 1, j].type == Main.tile[i, j].type && WorldGen.SolidTile(i + 1, j))
                        modPlayer.BreakTile(i + 1, j);
                }
                else
                {
                    if (Main.tile[i - 1, j].type == Main.tile[i, j].type && WorldGen.SolidTile(i - 1, j))
                        modPlayer.BreakTile(i - 1, j);
                }

                if (Main.rand.Next(2) == 0)
                {
                    if (Main.tile[i, j + 1].type == Main.tile[i, j].type && WorldGen.SolidTile(i, j + 1))
                        modPlayer.BreakTile(i, j + 1);
                }
                else
                {
                    if (Main.tile[i, j - 1].type == Main.tile[i, j].type && WorldGen.SolidTile(i, j - 1))
                        modPlayer.BreakTile(i, j - 1);
                }

            }

        }



    }
}