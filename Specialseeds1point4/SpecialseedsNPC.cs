using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;
using System.IO;
using AdvancedWorldGen.Base;
using Terraria.ModLoader.IO;


namespace Specialseeds1point4
{
    public class SpecialseedsNPC : GlobalNPC
    {
        
        public string worldSeed = Main.ActiveWorldFileData.Name;
        private int cooldown;
        private int cooldownCount = 50;
        private bool modified;
        public override bool InstancePerEntity => true;

        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            worldSeed = Main.ActiveWorldFileData.Name;
            if (API.OptionsContains("NPCmania"))
            {
                spawnInfo.spawnTileType = 0;
                NPC npc = new NPC();
                for(int i = 1; i < 668; i++)
                {
                    npc.CloneDefaults(i);
                    if(!npc.boss)
                        pool.Add(i, Main.rand.Next(900, 1100));
                    
                }
                    
            }
        }

        public override void SetDefaults(NPC npc)
        {
            Player player = Main.player[Main.myPlayer];
            
            if (!modified)
            {
                worldSeed = Main.ActiveWorldFileData.Name;
                modified = true;
                if (API.OptionsContains("Gigantism"))
                {
                    if (!npc.boss)
                    {
                        npc.scale *= 2;
                        npc.width *= 2;
                        npc.height *= 2;
                        npc.lifeMax *= 2;
                        npc.life *= 2;
                        npc.damage *= 2;
                        npc.position.Y -= npc.height;
                    }
                    else
                    {
                        npc.scale *= 2;
                        npc.width *= 2;
                        npc.height *= 2;

                    }

                }
                if (API.OptionsContains("Slimed"))
                {
                    if (!npc.boss && !npc.buffImmune[31])
                    {
                        npc.aiStyle = 1;
                        npc.noGravity = false;

                    }
                    npc.AddBuff(137, 9999);

                }
                if (API.OptionsContains("Healthy"))
                {
                    npc.lifeMax *= 5;
                    npc.life *= 5;
                    npc.defense = 0;
                    npc.color = new Color(0, 255, 0, 0);


                }




            }

            npc.netUpdate = true;
        }




        public override void AI(NPC npc)
        {
            worldSeed = Main.ActiveWorldFileData.Name;
            if (API.OptionsContains("NPCmania"))
            {

                if (!npc.boss)
                {
                    if(!Main.hardMode)
                        npc.life = (npc.life / 4);

                    if (Main.hardMode)
                        npc.life = (npc.life / 2);

                }

            }




                
            if (API.OptionsContains("Lifesteal"))
            {

                if (cooldown < cooldownCount)
                    cooldown++;
                else
                    cooldown = cooldownCount;

            }

            npc.netUpdate = true;
        }

        public override void OnKill(NPC npc)
        {
            if (API.OptionsContains("Lifesteal"))
                if (Main.rand.Next(10) == 0)
                {
                    Item.NewItem(npc.getRect(), ItemType<LifeGelly>(), 1);

                }

            

            for (int i = 0; i < SpecialseedsWorld.bossList.Count; i++)
            {
                if (SpecialseedsWorld.bossList.ElementAt(i).type == npc.type)
                {
                    SpecialseedsWorld.bossList.RemoveAt(i);
                    Main.NewText("Current boss: " + SpecialseedsWorld.bossList.ElementAt(0).type);
                }
                    

            }
            

        }

        public override void HitEffect(NPC npc, int hitDirection, double damage)
        {
            worldSeed = Main.ActiveWorldFileData.Name;
            if (API.OptionsContains("Healthy"))
            {
                float life = npc.life;
                float lifeMax = npc.lifeMax;
                int lifeHue = (int)Math.Floor(355 * (life / lifeMax));
                npc.color = new Color(355 - lifeHue, lifeHue, 0, 0); ;


            }
            else if (API.OptionsContains("TBD"))
            {

                float life = npc.life;
                float lifeMax = npc.lifeMax;
                npc.alpha = 255 - (int)Math.Floor(255 * (life / lifeMax));


            }
            npc.netUpdate = true;

        }


        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            
            if (API.OptionsContains("Murdermania"))
            {
                spawnRate *= 50;
                maxSpawns *= 4;

            }
            
        }

        public override void ScaleExpertStats(NPC npc, int numPlayers, float bossLifeScale)
        {
            if (numPlayers <= 10)
                cooldownCount -= (numPlayers * 5);
            else
                cooldownCount = 10;
            npc.netUpdate = true;
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            

            worldSeed = Main.ActiveWorldFileData.Name;
            if (API.OptionsContains("Lifesteal"))
            {

                if (cooldown == cooldownCount)
                {
                    if (npc.life == npc.lifeMax)
                    {
                        npc.lifeMax += (npc.damage * 2);
                        npc.life = npc.lifeMax;
                    }
                        else
                        npc.life += (npc.damage * 2);
                    cooldown = 0;
                }

            }
            npc.netUpdate = true;

        }

        







    }


    public class LifeGelly : ModItem
    {


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Life Gelly"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            Tooltip.SetDefault("Permanently increases maximum life by 20 \nOnly applies if under 200 maximum life");
        }

        public override void SetDefaults()
        {
            Item.consumable = true;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item4;
            Item.maxStack = 30;


        }

        public override bool CanUseItem(Player player)
        {
            if (player.statLifeMax + 20 < 200)
                player.statLifeMax += 20;
            else if (player.statLifeMax < 200)
                player.statLifeMax = 200;
            else
                return false;
            Item.stack--;
            return true;
        }





    }
}



