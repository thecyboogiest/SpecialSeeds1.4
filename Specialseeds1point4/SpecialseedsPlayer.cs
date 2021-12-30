using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdvancedWorldGen.Base;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.Audio;
using Specialseeds1point4.Tiles;
using Terraria.ModLoader.IO;

namespace Specialseeds1point4
{
    public class BreakTile
    {

        public int x;
        public int y;

        public int playerX;
        public int playerY;
        public void setXY(int i, int j, int g, int h)
        {
            x = i;
            y = j;
            playerX = g;
            playerY = h;

        }
    }

    public class SpecialseedsPlayer : ModPlayer
    {

        public string worldSeed = Main.ActiveWorldFileData.Name;
        public float tickSpeed = 60;
        public int tickTimer = 0;
        private float veloPain;
        public int bossesDefeated;
        public float breakStoneCooldown;
        public List<BreakTile> tilesToBreak = new List<BreakTile>();

        


        public override void PostUpdate()
        {
            worldSeed = Main.ActiveWorldFileData.Name;
            
            if(tilesToBreak.Count > 0)
            {
                if(MathF.Abs(tilesToBreak.FirstOrDefault().playerX - tilesToBreak.FirstOrDefault().x) < 15)
                    WorldGen.KillTile(tilesToBreak.FirstOrDefault().x, tilesToBreak.FirstOrDefault().y);
                tilesToBreak.RemoveAt(0);

            }
            if (API.OptionsContains("Velocity"))
            {

                if (Player.velocity.X == 0)
                {
                    veloPain += 0.01f;
                    Player.statLife -= (int)(1 + veloPain);
                    CombatText.NewText(new Rectangle((int)Player.position.X, (int)Player.position.Y, Player.width, Player.height), CombatText.LifeRegen, 1, dramatic: false, dot: true);
                    if (Player.statLife <= 0 && Player.whoAmI == Main.myPlayer)
                    {
                        Player.KillMe(PlayerDeathReason.ByOther(5), 10.0, 0);
                    }

                }
                else
                    veloPain = 0;

            }

            



            if (API.OptionsContains("Nuked"))
            {

                if (Player.statLife <= 0)
                {
                    tickSpeed = 60;
                    tickTimer = 0;
                }

                if (Player.wet || (Player.Center.Y <= (Main.worldSurface - 25) * 16 && Player.Center.X >= ((Main.maxTilesX / 2) - 400) * 16 && Player.Center.X <= ((Main.maxTilesX / 2) + 400) * 16))
                {
                    if ((tickTimer == (int)(tickSpeed / 2) || Main.rand.Next(((int)tickSpeed) * 4) == 0) && tickSpeed <= 30)
                    {


                        Player.statLife -= 2;
                        CombatText.NewText(new Rectangle((int)Player.position.X, (int)Player.position.Y, Player.width, Player.height), CombatText.LifeRegen, 2, dramatic: false, dot: true);
                        if (Player.statLife <= 0 && Player.whoAmI == Main.myPlayer)
                        {
                            Player.KillMe(PlayerDeathReason.ByOther(9), 10.0, 0);
                        }


                    }



                    if (tickSpeed > 15)
                        tickSpeed -= 0.1f;


                    if (tickTimer >= tickSpeed)
                    {
                        SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(ModLoader.GetMod("Specialseeds1point4"), "Sounds/GeigerCounterSound"), Player.Center);

                        tickTimer = Main.rand.Next(0, 14);

                    }
                    else
                        tickTimer++;




                }
                else
                {
                    tickSpeed = 60;
                    tickTimer = 0;
                }



            }


            if (API.OptionsContains("Trapmania"))
            {

                if(Player.Center.Y > WorldGen.worldSurface)
                {

                    if (breakStoneCooldown > 0)
                        breakStoneCooldown -= 0.1f;
                    else breakStoneCooldown = 0;

                }


                if (breakStoneCooldown == 0)
                {
                    if (Main.tile[(int)Player.Center.X / 16, (int)(Player.Center.Y / 16) + 2].type == ModContent.TileType<CrackedStone>())
                    {

                        WorldGen.KillTile((int)Player.Center.X / 16, (int)(Player.Center.Y / 16) + 2);

                        for (int i = 0; i < Player.velocity.Y; i++)
                        {

                            for (int r = -3; r < 3; r++)
                            {

                                if (r > -1 && r < 1)
                                {

                                    if (Main.tile[(int)(Player.Center.X / 16) + r, (int)(Player.Center.Y / 16) + i].type == ModContent.TileType<CrackedStone>())
                                        BreakTile((int)(Player.Center.X / 16) + r, (int)(Player.Center.Y / 16) + i);
                                }
                                else
                                {

                                    if (Main.rand.Next(2) != 0)
                                    {

                                        if (Main.tile[(int)(Player.Center.X / 16) + r, (int)(Player.Center.Y / 16) + i].type == ModContent.TileType<CrackedStone>())
                                            BreakTile((int)(Player.Center.X / 16) + r, (int)(Player.Center.Y / 16) + i);

                                    }

                                }


                            }

                        }

                        breakStoneCooldown = 5;

                    }
                    else
                        breakStoneCooldown = 0;

                }
                else if (Main.rand.Next(120) == 0 && breakStoneCooldown == 0)
                {
                    if (Main.tile[(int)Player.Center.X / 16, (int)(Player.Center.Y / 16) + 2].type == ModContent.TileType<CrackedStone>())
                    {

                        BreakTile((int)Player.Center.X / 16, (int)(Player.Center.Y / 16) + 2);

                        BreakTile((int)(Player.Center.X / 16) - 1, (int)(Player.Center.Y / 16) + 3);

                        BreakTile((int)(Player.Center.X / 16) + 1, (int)(Player.Center.Y / 16) + 3);

                        breakStoneCooldown = 2;

                    }
                    else
                        breakStoneCooldown = 0;
                }


                if (Player.HasBuff(BuffID.Dangersense))
                    Player.ClearBuff(BuffID.Dangersense);
            }


        }
        public override void OnHitByNPC(NPC npc, int damage, bool crit)
        {
            worldSeed = Main.ActiveWorldFileData.Name;
            if (API.OptionsContains("Lifesteal"))
            {
                
                if (Player.statLifeMax > damage + 25)
                {
                    
                    Player.statLifeMax = Player.statLife;

                }
                else
                    Player.statLifeMax = 25;

                if (Player.statLifeMax < 25)
                    Player.statLifeMax = 25;

            }

            if (API.OptionsContains("Nuked"))
            {

                Player.AddBuff(BuffID.Poisoned, 169);

            }


        }

        public override void OnHitByProjectile(Projectile proj, int damage, bool crit)
        {
            if(proj.friendly)
            {
                Player.statLife -= (damage / 2) + (damage / 6 * bossesDefeated);
                CombatText.NewText(new Rectangle((int)Player.position.X, (int)Player.position.Y, Player.width, Player.height), CombatText.LifeRegen, damage / 2, dramatic: false, dot: true);
                if (Player.statLife <= 0 && Player.whoAmI == Main.myPlayer)
                {
                    Player.KillMe(PlayerDeathReason.ByOther(5), 10.0, 0);
                }
            }
        }

        public void BreakTile(int i, int j)
        {
            BreakTile tileToBreak = new BreakTile();
            tileToBreak.setXY(i, j, (int)Player.Center.X / 16, (int)Player.Center.Y / 16);
            if(!tilesToBreak.Contains(tileToBreak))
                tilesToBreak.Add(tileToBreak);
            //Main.NewText(tileToBreak.x + ", " + tileToBreak.y);

        }







    }
}
