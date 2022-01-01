using System.IO;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Generation;
using System.Linq;
using Terraria.IO;
using Specialseeds1point4.Tiles;
using Terraria.Audio;
using AdvancedWorldGen.Base;
using Terraria.ModLoader.IO;
using System;

namespace Specialseeds1point4
{
    public class ListBoss : TagSerializable
    {
        public static readonly Func<TagCompound, ListBoss> DESERIALIZER = Load;
        public int type;
        public string name;



        public TagCompound SerializeData()
        {
            return new TagCompound
            {
                ["type"] = type,
                ["name"] = name
            };
        }

        public static ListBoss Load(TagCompound tag)
        {
            var LoadListBoss = new ListBoss
            {
                type = tag.GetInt("type"),
                name = tag.GetString("name")
            };
            return LoadListBoss;
        }

    }


    public class SpecialseedsWorld : ModSystem
    {

        //public Mod advencedWorldGen = ModLoader.GetMod("AdvencedWorldGen");
        public List<int> seedsToDo = new();
        public int currentSelectedSeed;
        public int worldSpikes;
        public int worldBaubles;
        public int meteors;
        public int zone;
        public static List<ListBoss> bossList = new();
        public static List<int> realIDBossList = new();
        public static List<int> IDBossList = new();
        public static int currentBossInList;
        



        public static void RandomizeBosses()
        {

            for (int i = 0; i < bossList.Count * 2; i++)
            {
                int bossID = Main.rand.Next(bossList.Count);
                int zeroBossID = Main.rand.Next(bossList.Count);

                if (bossID != 0)
                {
                    bossList.Add(bossList.ElementAt(bossID));

                    bossList.RemoveAt(bossID + 1);

                }
                else
                {

                    bossList.Insert(zeroBossID, bossList.ElementAt(0));

                    bossList.RemoveAt(0);

                }



            }


            for (int t = 0; t <= bossList.Count; t++)
            {
                IDBossList.Add(bossList.ElementAt(t).type);
                
            }
        }




        public override void SaveWorldData(TagCompound tag)
        {
            tag["savedBossList"] = bossList;
            tag["savedIDBossList"] = IDBossList;
            tag["savedRealIDBossList"] = realIDBossList;
        }

        public override void LoadWorldData(TagCompound tag)
        {

            bossList = tag.GetList<ListBoss>("savedBossList").ToList();
            IDBossList = tag.GetList<int>("savedIDBossList").ToList();
            realIDBossList = tag.GetList<int>("savedRealIDBossList").ToList();
        }



        public override void PostUpdatePlayers()
        {


            for (int i = 0; i < Main.PlayerList.Count; i++)
            {
                Player player = Main.player[i];
                SceneMetrics metrics = Main.SceneMetrics;
                if (seedsToDo.Contains(4) && player.Center.Y <= Main.worldSurface * 16)
                {


                    player.AddBuff(BuffID.WaterCandle, 55);
                    player.jumpSpeedBoost *= 2;
                    player.ZoneMeteor = true;
                    metrics.MeteorTileCount = 169;
                    if (player.velocity.Y > player.maxFallSpeed / 1.7f)
                    {
                        player.velocity.Y = player.maxFallSpeed / 1.7f;

                    }
                }

            }



        }


        public static void ModifyBossList()
        {

            NPC npc = new();
            for (int i = 1; i < 668; i++)
            {
                npc.SetDefaults(i);
                
                if (npc.boss)
                {
                    realIDBossList.Add(i);
                    ListBoss boss = new()
                    {
                        type = i,
                        name = npc.GivenName
                    };
                    bossList.Add(boss);
                    
                    
                }
                
            }
            RandomizeBosses();

        }




        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            if (API.OptionsContains("Bossmania") && bossList.Count == 0)
                ModifyBossList();
            if (API.OptionsContains("Icemania"))
                seedsToDo.Add(1);
            if (API.OptionsContains("Junglemania"))
                seedsToDo.Add(2);
            if (API.OptionsContains("Desertmania"))
                seedsToDo.Add(3);
            if (API.OptionsContains("Meteormania"))
                seedsToDo.Add(4);
            if (API.OptionsContains("RainyDay"))
                seedsToDo.Add(5);
            if (API.OptionsContains("Cavemania"))
                seedsToDo.Add(6);
            if (API.OptionsContains("Nuked"))
                seedsToDo.Add(7);
            if (API.OptionsContains("Trapmania"))
                seedsToDo.Add(8);

            int genIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Shinies"));
            if (Main.maxTilesX < 6000)
                worldBaubles = Main.rand.Next(30, 43);
            else if (Main.maxTilesX < 8000)
                worldBaubles = Main.rand.Next(43, 53);
            else
                worldBaubles = Main.rand.Next(50, 63);

            if (API.OptionsContains("Nuked"))
            {
                tasks.RemoveAt(tasks.FindIndex(genpass => genpass.Name.Equals("Grass")));
                tasks.RemoveAt(tasks.FindIndex(genpass => genpass.Name.Equals("Spreading Grass")));
                tasks.RemoveAt(tasks.FindIndex(genpass => genpass.Name.Equals("Planting Trees")));

            }
            if (API.OptionsContains("Trapmania"))
            {
                tasks.RemoveAt(tasks.FindIndex(genpass => genpass.Name.Equals("Wavy Caves")));
                tasks.Insert(tasks.FindIndex(genpass => genpass.Name.Equals("Traps")) + 1, new PassLegacy("Traps 2", Traps2));
                tasks.Insert(tasks.FindIndex(genpass => genpass.Name.Equals("Traps")) + 1, new PassLegacy("Traps 2", Traps2));
                tasks.Insert(tasks.FindIndex(genpass => genpass.Name.Equals("Traps")) + 1, new PassLegacy("Traps 2", Traps2));

            }
                

            genIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Small Holes")) - 1;
            tasks.Insert(genIndex, new PassLegacy("Seeds main", GenerateWorld));

            genIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Small Holes")) + 1;
            tasks.Insert(genIndex, new PassLegacy("Small Hole Extras", SmallHoleExtras));

            genIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Hives")) + 1;
            tasks.Insert(genIndex, new PassLegacy("World Extras", GenerateExtras));
            if (API.OptionsContains("CUS"))
            {

                genIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Seeds main")) + 1;
                tasks.Insert(genIndex, new PassLegacy("Custom World Generation", CustomWorld));

            }



        }

        private void Traps2(GenerationProgress progress, GameConfiguration gameConfiguration)
        {
            if (!WorldGen.notTheBees)
            {
                WorldGen.placingTraps = true;
                progress.Message = Lang.gen[34].Value;
                float num336 = (float)Main.maxTilesX * 0.05f;
                if (WorldGen.getGoodWorldGen)
                {
                    num336 *= 1.5f;
                }
                for (int num337 = 0; (float)num337 < num336; num337++)
                {
                    progress.Set((float)num337 / num336 / 2f);
                    for (int num338 = 0; num338 < 1150; num338++)
                    {
                        int num339 = WorldGen.genRand.Next(200, Main.maxTilesX - 200);
                        int num340 = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY - 210);
                        while (WorldGen.oceanDepths(num339, num340))
                        {
                            num339 = WorldGen.genRand.Next(200, Main.maxTilesX - 200);
                            num340 = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY - 210);
                        }
                        if (Main.tile[num339, num340].wall == 0 && WorldGen.placeTrap(num339, num340))
                        {
                            break;
                        }
                    }
                }
                num336 = (float)Main.maxTilesX * 0.003f;
                if (WorldGen.getGoodWorldGen)
                {
                    num336 *= 1.5f;
                }
                for (int num341 = 0; (float)num341 < num336; num341++)
                {
                    progress.Set((float)num341 / num336 / 2f + 0.5f);
                    for (int num342 = 0; num342 < 20000; num342++)
                    {
                        int num343 = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.15), (int)((double)Main.maxTilesX * 0.85));
                        int num344 = WorldGen.genRand.Next((int)Main.worldSurface + 20, Main.maxTilesY - 210);
                        if (Main.tile[num343, num344].wall == 187 && WorldGen.PlaceSandTrap(num343, num344))
                        {
                            break;
                        }
                    }
                }
                if (WorldGen.drunkWorldGen)
                {
                    for (int num345 = 0; num345 < 8; num345++)
                    {
                        progress.Message = Lang.gen[34].Value;
                        num336 = 100f;
                        for (int num346 = 0; (float)num346 < num336; num346++)
                        {
                            progress.Set((float)num346 / num336);

                        }
                    }
                }
                WorldGen.placingTraps = false;
            }
        }

        private void SmallHoleExtras(GenerationProgress progress, GameConfiguration gameConfiguration)
        {
            if (seedsToDo.Contains(6))
                GenerateRandomSpikes(-40, ((Main.maxTilesY / 2) + 250), TileID.Stone, TileID.Dirt, true, WallID.Cave2Unsafe, WallID.CaveUnsafe, false, -0.5f, 0.7f, false, 2, 80, 6, 14);

        }

        private void CustomWorld(GenerationProgress progress, GameConfiguration gameConfiguration)
        {

            //CUS,006,076,t
            // 123456789
            /*
            int floorTileID = System.Convert.ToInt32(worldSeed.Substring(4, 3));
            int altFloorTileID = System.Convert.ToInt32(worldSeed.Substring(8, 3));
            bool altTile = false;
            if (worldSeed.Substring(12, 1) == "T")
                altTile = true;
            
            GenerateFloor((ushort)floorTileID, altTile, (ushort)altFloorTileID);
            */

        }


        private void GenerateWorld(GenerationProgress progress, GameConfiguration gameConfiguration)
        {
            progress.Message = "Breaking The Balance";

            for (int e = 0; e < seedsToDo.Count; e++)
            {
                currentSelectedSeed = seedsToDo.ElementAt(e);
                switch (currentSelectedSeed)
                {
                    case 1:
                        {

                            for (int i = 0; i < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 6E-05); i++)
                            {

                                //Snow

                                WorldGen.TileRunner(
                                    WorldGen.genRand.Next(WorldGen.genRand.Next(500, 700), Main.maxTilesX - WorldGen.genRand.Next(500, 700)), WorldGen.genRand.Next(0, (int)WorldGen.worldSurface + 350), (double)WorldGen.genRand.Next(100, 200), 15, TileID.SnowBlock, false, 0f, 0f, false, true);
                                WorldGen.TileRunner(
                                    WorldGen.genRand.Next((Main.maxTilesX / 2) - WorldGen.genRand.Next(100, 500), (Main.maxTilesX / 2) + WorldGen.genRand.Next(100, 500)), WorldGen.genRand.Next(0, Main.maxTilesY + 300), (double)WorldGen.genRand.Next(200, 350), 35, TileID.SnowBlock, false, 0f, 0f, false, true);


                            }



                            for (int z = 0; z < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 6E-05); z++)
                            {

                                //Ice

                                //Left
                                WorldGen.TileRunner(
                                    WorldGen.genRand.Next(WorldGen.genRand.Next(300, 500), (Main.maxTilesX / 2) - WorldGen.genRand.Next(300, 500)), WorldGen.genRand.Next((int)WorldGen.worldSurface + 150, Main.maxTilesY - WorldGen.genRand.Next(300, 500)), (double)WorldGen.genRand.Next(80, 110), 1, TileID.IceBlock, true, 0f, 0f, true, true);
                                //Right
                                WorldGen.TileRunner(
                                    WorldGen.genRand.Next((Main.maxTilesX / 2) + WorldGen.genRand.Next(300, 500), Main.maxTilesX - WorldGen.genRand.Next(300, 500)), WorldGen.genRand.Next((int)WorldGen.worldSurface + 150, Main.maxTilesY - WorldGen.genRand.Next(300, 500)), (double)WorldGen.genRand.Next(80, 110), 1, TileID.IceBlock, true, 0f, 0f, false, true);

                            }
                            break;
                        }
                    case 2:
                        {

                            for (int i = 0; i < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 6E-05); i++)
                            {
                                //Mud

                                //Caves
                                WorldGen.TileRunner(
                                    WorldGen.genRand.Next(WorldGen.genRand.Next(300, 500), Main.maxTilesX - WorldGen.genRand.Next(300, 500)), WorldGen.genRand.Next((int)WorldGen.worldSurface + WorldGen.genRand.Next(100, 200), Main.maxTilesY - WorldGen.genRand.Next(100, 300)), (double)WorldGen.genRand.Next(130, 190), 2, TileID.Mud, false, 0f, 0f, false, true);

                                //Spawn
                                WorldGen.TileRunner(
                                    WorldGen.genRand.Next((Main.maxTilesX / 2) - WorldGen.genRand.Next(100, 300), (Main.maxTilesX / 2) + WorldGen.genRand.Next(100, 300)), WorldGen.genRand.Next((int)WorldGen.worldSurface, (int)WorldGen.worldSurface + WorldGen.genRand.Next(300, 400)), (double)WorldGen.genRand.Next(130, 190), 2, TileID.Mud, false, 0f, 0f, false, true);


                                //Hives
                                WorldGen.TileRunner(
                                    WorldGen.genRand.Next(WorldGen.genRand.Next(300, 500), Main.maxTilesX - WorldGen.genRand.Next(300, 500)), WorldGen.genRand.Next((int)WorldGen.worldSurface + WorldGen.genRand.Next(100, 200), Main.maxTilesY - WorldGen.genRand.Next(300, 500)), (double)WorldGen.genRand.Next(50, 90), 1, TileID.Hive, false, 0f, 0f, false, true);



                            }
                            break;

                        }
                    case 3:
                        {


                            for (int i = 0; i < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 6E-05); i++)
                            {
                                //Sand



                                //Surface
                                WorldGen.TileRunner(
                                    WorldGen.genRand.Next(WorldGen.genRand.Next(500, 700), Main.maxTilesX - WorldGen.genRand.Next(500, 700)), WorldGen.genRand.Next(0, (int)WorldGen.worldSurface + 350), (double)WorldGen.genRand.Next(100, 200), 15, TileID.Sand, false, 0f, 0f, false, true);

                                //Spawn
                                WorldGen.TileRunner(
                                    WorldGen.genRand.Next((Main.maxTilesX / 2) - WorldGen.genRand.Next(100, 500), (Main.maxTilesX / 2) + WorldGen.genRand.Next(100, 500)), WorldGen.genRand.Next(0, Main.maxTilesY + 300), (double)WorldGen.genRand.Next(200, 350), 35, TileID.Sand, false, 0f, 0f, false, true);

                                //Sandstone Clumps
                                WorldGen.TileRunner(
                                    WorldGen.genRand.Next(WorldGen.genRand.Next(300, 500), Main.maxTilesX - WorldGen.genRand.Next(300, 500)), WorldGen.genRand.Next((int)WorldGen.worldSurface + WorldGen.genRand.Next(100, 200), Main.maxTilesY - WorldGen.genRand.Next(300, 500)), (double)WorldGen.genRand.Next(50, 90), 1, TileID.Sandstone, false, 0f, 0f, false, true);




                            }

                            GenerateBauble(TileID.HardenedSand, 15, 35, 2);
                            GenerateBauble(TileID.Sandstone, 25, 35, 4);
                            break;
                        }
                    case 4:
                        {



                            for (int i = 0; i < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 6E-05); i++)
                            {


                                //Spawn
                                WorldGen.TileRunner(
                                    WorldGen.genRand.Next((Main.maxTilesX / 2) - WorldGen.genRand.Next(100, 500), (Main.maxTilesX / 2) + WorldGen.genRand.Next(100, 500)), WorldGen.genRand.Next(0, Main.maxTilesY + 300), (double)WorldGen.genRand.Next(200, 350), 35, TileID.Stone, false, 0f, 0f, false, true);

                                //Gran / Marb

                                //Left
                                WorldGen.TileRunner(
                                    WorldGen.genRand.Next(WorldGen.genRand.Next(300, 500), (Main.maxTilesX / 2) - WorldGen.genRand.Next(300, 500)), WorldGen.genRand.Next((int)WorldGen.worldSurface + 95, (int)WorldGen.worldSurface + 350), (double)WorldGen.genRand.Next(40, 60), 2, TileID.Granite, false, 0f, 0f, false, true);
                                //Right
                                WorldGen.TileRunner(
                                    WorldGen.genRand.Next((Main.maxTilesX / 2) + WorldGen.genRand.Next(300, 500), Main.maxTilesX - WorldGen.genRand.Next(300, 500)), WorldGen.genRand.Next((int)WorldGen.worldSurface + 95, (int)WorldGen.worldSurface + 350), (double)WorldGen.genRand.Next(40, 60), 2, TileID.Granite, false, 0f, 0f, false, true);
                                //Left
                                WorldGen.TileRunner(
                                    WorldGen.genRand.Next(WorldGen.genRand.Next(300, 500), (Main.maxTilesX / 2) - WorldGen.genRand.Next(300, 500)), WorldGen.genRand.Next((int)WorldGen.worldSurface + 95, (int)WorldGen.worldSurface + 350), (double)WorldGen.genRand.Next(40, 60), 2, TileID.Marble, false, 0f, 0f, false, true);
                                //Right
                                WorldGen.TileRunner(
                                    WorldGen.genRand.Next((Main.maxTilesX / 2) + WorldGen.genRand.Next(300, 500), Main.maxTilesX - WorldGen.genRand.Next(300, 500)), WorldGen.genRand.Next((int)WorldGen.worldSurface + 95, (int)WorldGen.worldSurface + 350), (double)WorldGen.genRand.Next(40, 60), 2, TileID.Marble, false, 0f, 0f, false, true);

                                //Meteor

                                //Left
                                WorldGen.TileRunner(
                                    WorldGen.genRand.Next(WorldGen.genRand.Next(300, 500), (Main.maxTilesX / 2) - WorldGen.genRand.Next(300, 500)), WorldGen.genRand.Next((int)WorldGen.worldSurface + WorldGen.genRand.Next(100, 200), Main.maxTilesY - WorldGen.genRand.Next(300, 500)), (double)WorldGen.genRand.Next(40, 60), 2, TileID.Meteorite, false, 0f, 0f, false, true);
                                //Right
                                WorldGen.TileRunner(
                                    WorldGen.genRand.Next((Main.maxTilesX / 2) + WorldGen.genRand.Next(300, 500), Main.maxTilesX - WorldGen.genRand.Next(300, 500)), WorldGen.genRand.Next((int)WorldGen.worldSurface + WorldGen.genRand.Next(100, 200), Main.maxTilesY - WorldGen.genRand.Next(300, 500)), (double)WorldGen.genRand.Next(40, 60), 2, TileID.Meteorite, false, 0f, 0f, false, true);

                                //Obsidian

                                //Left
                                WorldGen.TileRunner(
                                    WorldGen.genRand.Next(WorldGen.genRand.Next(300, 500), (Main.maxTilesX / 2) - WorldGen.genRand.Next(300, 500)), WorldGen.genRand.Next((int)WorldGen.worldSurface + WorldGen.genRand.Next(100, 200), Main.maxTilesY - WorldGen.genRand.Next(300, 500)), (double)WorldGen.genRand.Next(20, 40), 2, TileID.Obsidian, false, 0f, 0f, false, true);
                                //Right
                                WorldGen.TileRunner(
                                    WorldGen.genRand.Next((Main.maxTilesX / 2) + WorldGen.genRand.Next(300, 500), Main.maxTilesX - WorldGen.genRand.Next(300, 500)), WorldGen.genRand.Next((int)WorldGen.worldSurface + WorldGen.genRand.Next(100, 200), Main.maxTilesY - WorldGen.genRand.Next(300, 500)), (double)WorldGen.genRand.Next(20, 40), 2, TileID.Obsidian, false, 0f, 0f, false, true);



                            }



                            break;
                        }
                    case 5:
                        {

                            for (int i = 0; i < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 6E-05); i++)
                            {

                                //Clouds

                                //Left
                                WorldGen.TileRunner(
                                    WorldGen.genRand.Next(WorldGen.genRand.Next(300, 500), (Main.maxTilesX / 2) - WorldGen.genRand.Next(300, 500)), WorldGen.genRand.Next(60, 125), (double)WorldGen.genRand.Next(10, 30), 1, TileID.Cloud, true, 0f, 0f, false, true);
                                //Right
                                WorldGen.TileRunner(
                                    WorldGen.genRand.Next((Main.maxTilesX / 2) + WorldGen.genRand.Next(300, 500), Main.maxTilesX - WorldGen.genRand.Next(300, 500)), WorldGen.genRand.Next(60, 125), (double)WorldGen.genRand.Next(10, 30), 1, TileID.Cloud, true, 0f, 0f, false, true);
                                //Middle
                                WorldGen.TileRunner(
                                    WorldGen.genRand.Next((Main.maxTilesX / 2) - WorldGen.genRand.Next(300, 500), (Main.maxTilesX / 2) + WorldGen.genRand.Next(300, 500)), WorldGen.genRand.Next(100, 125), (double)WorldGen.genRand.Next(10, 30), 2, TileID.Cloud, true, 0f, 0f, false, true);

                                //Rain Clouds

                                //Left
                                WorldGen.TileRunner(
                                    WorldGen.genRand.Next(WorldGen.genRand.Next(300, 500), (Main.maxTilesX / 2) - WorldGen.genRand.Next(300, 500)), WorldGen.genRand.Next(100, 150), (double)WorldGen.genRand.Next(1, 20), 1, TileID.RainCloud, true, 0f, 0f, false, true);
                                //Right
                                WorldGen.TileRunner(
                                    WorldGen.genRand.Next((Main.maxTilesX / 2) + WorldGen.genRand.Next(300, 500), Main.maxTilesX - WorldGen.genRand.Next(300, 500)), WorldGen.genRand.Next(100, 150), (double)WorldGen.genRand.Next(1, 20), 1, TileID.RainCloud, true, 0f, 0f, false, true);
                            }



                            break;
                        }
                    case 6:
                        {

                            GenerateSpike((Main.maxTilesX / 2), (Main.maxTilesY / 2), ((Main.maxTilesY / 8)), ((Main.maxTilesY / 4)), TileID.Marble, TileID.Granite, true, WallID.GraniteUnsafe, WallID.MarbleUnsafe, false, 0.4f, 1.4f, true, 2, 0);
                            for (int num658 = 0; num658 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.00013); num658++)
                            {
                                float value17 = (float)((double)num658 / ((double)(Main.maxTilesX * Main.maxTilesY) * 0.00013));
                                progress.Set(value17);
                                if (WorldGen.rockLayerHigh <= (double)Main.maxTilesY)
                                {
                                    int type7 = -1;
                                    if (WorldGen.genRand.Next(10) == 0)
                                    {
                                        type7 = -2;
                                    }
                                    WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)WorldGen.rockLayerHigh, Main.maxTilesY), WorldGen.genRand.Next(15, 35), WorldGen.genRand.Next(20, 600), type7);
                                }
                            }


                            break;
                        }
                    case 7:
                        {



                            GenerateFloor(TileID.Dirt);
                            for (int num658 = 0; num658 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.00013); num658++)
                            {
                                float value17 = (float)((double)num658 / ((double)(Main.maxTilesX * Main.maxTilesY) * 0.00013));
                                progress.Set(value17);
                                if (WorldGen.worldSurface - 100 <= (double)Main.maxTilesY)
                                {
                                    int type7 = -1;
                                    if (WorldGen.genRand.Next(10) == 0)
                                    {
                                        type7 = -2;
                                    }
                                    WorldGen.TileRunner(WorldGen.genRand.Next((Main.maxTilesX / 2) - 800, (Main.maxTilesX / 2) + 800), WorldGen.genRand.Next((int)WorldGen.worldSurface - 100, (Main.maxTilesY / 2) + 300), WorldGen.genRand.Next(15, 55), 5, type7);


                                }
                            }
                            break;
                        }
                    case 8:
                        {

                            for (int num42069 = 0; num42069 < Main.maxTilesY; num42069 += 40)
                            {
                                for (int num69420 = 0; num69420 < Main.maxTilesX; num69420 += 40)
                                {
                                    WorldGen.TileRunner(num69420, num42069, WorldGen.genRand.Next(num42069 / 80), WorldGen.genRand.Next(20, 400), ModContent.TileType<CrackedStone>());
                                }
                            }

                            for (int num658 = 0; num658 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.00013); num658++)
                            {

                                float value17 = (float)((double)num658 / ((double)(Main.maxTilesX * Main.maxTilesY) * 0.00013));
                                progress.Set(value17);
                                if (WorldGen.rockLayerHigh <= (double)Main.maxTilesY)
                                {
                                    int type7 = -1;
                                    if (WorldGen.genRand.Next(10) == 0)
                                    {
                                        type7 = -2;
                                    }
                                    int x = WorldGen.genRand.Next(0, Main.maxTilesX);
                                    int y = WorldGen.genRand.Next((int)WorldGen.rockLayerHigh, Main.maxTilesY);
                                    int scale = WorldGen.genRand.Next(20, 25);
                                    int strength = WorldGen.genRand.Next(10, 40);
                                    WorldGen.TileRunner(x, y, scale + 5, strength, TileID.WoodenSpikes);
                                    WorldGen.TileRunner(x, y, scale, strength, type7);
                                }
                            }
                            break;
                        }

                }

            }

            seedsToDo.Clear();

        }

        private void GenerateExtras(GenerationProgress progress, GameConfiguration gameConfiguration)
        {
            progress.Message = "Breaking The Rest";
            for (int e = 0; e < seedsToDo.Count; e++)  
            {

                currentSelectedSeed = seedsToDo.ElementAt(e);
                switch (currentSelectedSeed)
                {


                    case 1:
                        {

                            GenerateRandomSpikes(115, 115, TileID.IceBlock, TileID.IceBrick, true, WallID.IceBrick, WallID.SnowWallUnsafe, true, -0.4f, 0.7f, true, 22, 50, 9, 14);
                            GenerateBauble(TileID.IceBlock, 15, 35, 2);
                            break;
                        }
                    case 2:
                        {

                            GenerateBauble(TileID.Hive, 35, 45, 1);
                            break;
                        }
                    case 3:
                        {


                            //Baubles
                            GenerateRandomSpikes(45, 115, TileID.SandStoneSlab, TileID.SandstoneBrick, false, WallID.SandstoneBrick, WallID.SandstoneBrick, false, 0.4f, 1.4f, true, 2, 80, 2, 4);

                            break;
                        }
                    case 4:
                        {


                            GenerateRandomSpikes(45, 115, TileID.Stone, TileID.MeteoriteBrick, false, WallID.MeteoriteBrick, WallID.MeteoriteBrick, false, 1, 1, true, 2, 80, 2, 4);


                            //Meteors

                            WorldGen.spawnMeteor = true;
                            if (Main.maxTilesX > 6000)
                                meteors = Main.rand.Next(31, 42);
                            else if (Main.maxTilesX > 8000)
                                meteors = Main.rand.Next(41, 50);
                            else
                                meteors = Main.rand.Next(48, 58);

                            for (int r = 0; r < meteors; r++)
                            {

                                int num1 = 0;
                                int num2 = Main.rand.Next((Main.maxTilesX / 2) + 150, Main.maxTilesX - 150);
                                int num3 = Main.rand.Next(150, (Main.maxTilesX / 2) - 150);



                                for (int s = 0; s < Main.maxTilesY; s++)
                                {
                                    if (r < meteors / 2)
                                    {

                                        if (!WorldGen.SolidTile(num2, num1))
                                        {
                                            num1++;
                                        }
                                        else
                                        {
                                            WorldGen.meteor(num2, num1);
                                            break;
                                        }

                                    }
                                    else
                                    {

                                        if (!WorldGen.SolidTile(num3, num1))
                                        {
                                            num1++;
                                        }
                                        else
                                        {
                                            WorldGen.meteor(num3, num1);
                                            break;
                                        }

                                    }

                                }



                            }

                            break;
                        }
                    case 8:
                        {

                            for (int t = 0; t < Main.maxTilesX / 8; t++)
                            {
                                int num1 = 0;
                                int num2 = Main.rand.Next(350, Main.maxTilesX - 350);

                                float value17 = (float)((double)t / (Main.maxTilesX / 8));
                                progress.Set(value17);


                                for (int s = 0; s < Main.maxTilesY; s++)
                                {



                                    if (s < Main.maxTilesY - 200)
                                    {

                                        if (Main.tile[num2, num1].LiquidType != 1)
                                        {
                                            num1++;
                                        }
                                        else
                                        {

                                            if (WorldGen.SolidTile(num2, num1 - 10))
                                            {

                                                WorldGen.TileRunner(num2, num1 - 10, (double)WorldGen.genRand.Next(20, 40), 2, ModContent.TileType<CrackedStone>(), false, 0f, 0f, true, true);

                                            }
                                            else
                                            {
                                                for (int i = num1 - 10; i <= num1 - 5; i++)
                                                {

                                                    for (int r = num2 - 40; r < num2 + 40; r++)
                                                    {

                                                        if (Main.rand.Next(15) != 0)
                                                            WorldGen.PlaceTile(r, i, ModContent.TileType<CrackedStone>());

                                                    }

                                                }

                                            }



                                            break;
                                        }
                                    }
                                    else
                                    {
                                        num2 = Main.rand.Next(150, Main.maxTilesX - 150);
                                        s = 0;
                                    }


                                }

                            }

                            break;
                            /*

                            for (int t = 0; t < Main.rand.Next(10); t++)
                            {

                                int X = Main.rand.Next(400, 4000);
                                int Y = 500;


                                for (int i = Y; i < Y + 20; i++)
                                {

                                    for (int r = X; r < X + 40; r++)
                                    {



                                        if (r > Y && r < Y + 40)
                                        {
                                            Main.tile[r, i].ClearTile();
                                            if (i < Y + 5)
                                                WorldGen.PlaceTile(r, i, ModContent.TileType<CrackedStone>());
                                            else if (i < Y + 19 && i > Y + 10)
                                                WorldGen.PlaceLiquid(r, i, 1, 255);
                                            else
                                                WorldGen.PlaceTile(r, i, 1);
                                        }
                                        else
                                            WorldGen.PlaceTile(r, i, 1);

                                    }

                                }

                            }

                            */

                        }

                }
            }


            seedsToDo.Clear();


        }











        public static void GenerateFloor(ushort mainTile, bool useAltTile = false, ushort altTile = TileID.Dirt)
        {
            int bottomLeft = 0;
            int bottomRight = 0;

            for (int r = 0; r < Main.maxTilesY - (Main.maxTilesY / 4); r++)
            {
                

                for (int s = 0; s < Main.maxTilesX; s++)
                {


                    if (WorldGen.SolidTile(s, r))
                    {
                        
                         if (s == 599 + r || s == Main.maxTilesX - r - 599)
                            if (Main.rand.Next(16) != 0)
                                WorldGen.TileRunner(s, r, WorldGen.genRand.Next(15, 125), 1, mainTile, false, 0f, 0f, false, true);


                        

                         
                        if (s > 600 + r + Main.rand.Next(-50, 50) && s < Main.maxTilesX - 600 - r + Main.rand.Next(-50, 50))
                        {

                            if (bottomLeft == 0 && r == Main.maxTilesY - (Main.maxTilesY / 4) - 1)
                                bottomLeft = s;
                            bottomRight = s;

                            if (useAltTile)
                            {

                                if (Main.rand.Next(3) != 0)
                                {
                                    Main.tile[s, r].type = mainTile;
                                    Main.tile[s, r].IsHalfBlock = false;
                                    Main.tile[s, r].Slope = 0;
                                }
                                else
                                {
                                    Main.tile[s, r].type = altTile;
                                    Main.tile[s, r].IsHalfBlock = false;
                                    Main.tile[s, r].Slope = 0;
                                }

                            }
                            else
                            {

                                Main.tile[s, r].type = mainTile;
                                Main.tile[s, r].IsHalfBlock = false;
                                Main.tile[s, r].Slope = 0;

                            }

                        }


                    }
                        
                    







                }

            }

            for (int j = 0; j < bottomRight; j++)
            {

                if (j > bottomLeft)
                    WorldGen.TileRunner(j, Main.maxTilesY - (Main.maxTilesY / 4), WorldGen.genRand.Next(15, 125), 1, mainTile, true, 0f, 0f, false, true);

            }

        }

        public void GenerateBauble(int type, int sizeMin, int sizeMax, int strength)
        {


            for (int r = 0; r < worldBaubles; r++)
            {


                int num1 = 200;
                int num3 = Main.rand.Next(600, Main.maxTilesX - 600);



                for (int s = 0; s < Main.maxTilesY; s++)
                {


                    if (!WorldGen.SolidTile(num3, num1))
                    {
                        num1++;
                    }
                    else
                    {
                        WorldGen.TileRunner(num3, num1, (double)WorldGen.genRand.Next(sizeMin, sizeMax), strength, type, false, 0f, 0f, false, true);
                        break;

                    }



                }

            }


        }

        public void GenerateRandomSpikes(int spikeHieghtFromCenter, int spikeDepthFromCenter, ushort tileType, ushort altTileType, bool displacement ,ushort wallType, ushort altWalltype, bool generatePlatform, float minWide, float maxWide, bool generateChest, int chestType, int chestDepth, int worldSpikesMin, int worldSpikesMax)
        {

            if (Main.maxTilesX < 6000)
                worldSpikes = Main.rand.Next(worldSpikesMin, worldSpikesMax);
            else if (Main.maxTilesX < 8000)
                worldSpikes = Main.rand.Next(worldSpikesMin * 2, worldSpikesMax * 2);
            else
                worldSpikes = Main.rand.Next(worldSpikesMin * 4, worldSpikesMax * 4);

            for (int r = 0; r < worldSpikes; r++)
            {


                int num1 = 200;
                int num3 = Main.rand.Next(600, Main.maxTilesX - 600);



                for (int s = 0; s < Main.maxTilesY; s++)
                {


                    if (!WorldGen.SolidTile(num3, num1))
                    {
                        num1++;
                    }
                    else
                    {

                        GenerateSpike(num3, num1, spikeHieghtFromCenter, spikeDepthFromCenter, tileType, altTileType, displacement, wallType, altWalltype, generatePlatform, minWide, maxWide, generateChest, chestType, chestDepth);
                        break;

                    }



                }

            }

        }

        public static void GenerateSpike(int d, int e, int spikeHieghtFromCenter, int spikeDepthFromCenter, ushort tileType, ushort altTileType,bool displacement, ushort wallType, ushort altWalltype, bool generatePlatform, float minWide, float maxWide, bool generateChest, int chestType, int chestDepth)
        {


            int X = d;
            int Y = e;
            int num7 = Y - spikeHieghtFromCenter;
            int num4 = 1;
            int num5 = Y + spikeDepthFromCenter;
            float num6 = 0;


            for (int k = num7; k < num5; k++)
            {

                if (generatePlatform)
                {
                    WorldGen.TileRunner(
                WorldGen.genRand.Next(X - 120, X + 120), WorldGen.genRand.Next(num5, num5 + 20), (double)WorldGen.genRand.Next(15, 45), 1, altTileType, true, 0f, 0f, false, true);
                    WorldGen.TileRunner(
                    WorldGen.genRand.Next(X - 135, X + 135), WorldGen.genRand.Next(num5, num5 + 20), (double)WorldGen.genRand.Next(1, 25), 1, tileType, true, 0f, 0f, false, true);
                }

                for (int l = X - num4; l < X + num4 - 1; l++)
                {

                    if (generateChest)
                    {

                        if (l < X + 5 && l > X - 6 && k < Y + chestDepth && k > Y + (chestDepth - 5))
                        {
                            WorldGen.PlaceWall(l, k, wallType, true);
                            Main.tile[l, k].ClearTile();

                        }
                        else
                        {

                            if (Main.rand.Next(1) == 0)
                                WorldGen.PlaceWall(l, k, wallType, true);
                            else
                                WorldGen.PlaceWall(l, k, altWalltype, true);



                            if (CheckValidTile(l, k))
                            {
                                
                                Main.tile[l, k].type = tileType;
                                Main.tile[l, k].IsActive = true;
                                Main.tile[l, k].IsHalfBlock = false;
                                Main.tile[l, k].Slope = 0;
                            }

                            if (displacement)
                            {

                                int blockX = l + Main.rand.Next(-2, 2);
                                if ((blockX < X + 5 && blockX > X - 6 && k < Y + chestDepth && k > Y + (chestDepth - 5)))
                                {
                                    if (CheckValidTile(l, k))
                                    {

                                        Main.tile[l, k].type = altTileType;
                                        Main.tile[l, k].IsActive = true;
                                        Main.tile[l, k].IsHalfBlock = false;
                                        Main.tile[l, k].Slope = 0;

                                    }

                                }
                                else
                                {
                                    if (CheckValidTile(blockX, k))
                                    {
                                        Main.tile[blockX, k].type = altTileType;
                                        Main.tile[blockX, k].IsActive = true;
                                        Main.tile[blockX, k].IsHalfBlock = false;
                                        Main.tile[blockX, k].Slope = 0;
                                    }

                                }

                            }
                            else
                            {
                                if(Main.rand.Next(2) == 0)
                                {

                                    if (CheckValidTile(l, k))
                                    {

                                        Main.tile[l, k].type = altTileType;
                                        Main.tile[l, k].IsActive = true;
                                        Main.tile[l, k].IsHalfBlock = false;
                                        Main.tile[l, k].Slope = 0;

                                    }

                                }
                                

                            }

                            

                        }

                    }
                    else
                    {

                        if (CheckValidTile(l, k))
                        {
                            

                            Main.tile[l, k].type = tileType;
                            Main.tile[l, k].IsActive = true;
                            Main.tile[l, k].IsHalfBlock = false;
                            Main.tile[l, k].Slope = 0;
                        }


                        if (displacement)
                        {

                            int blockX = l + Main.rand.Next(-2, 2);
                            if (CheckValidTile(blockX, k))
                            {
                                Main.tile[blockX, k].type = altTileType;
                                Main.tile[blockX, k].IsActive = true;
                                Main.tile[blockX, k].IsHalfBlock = false;
                                Main.tile[blockX, k].Slope = 0;
                            }

                        }
                        else
                        {

                            if (Main.rand.Next(2) == 0)
                            {

                                if (CheckValidTile(l, k))
                                {

                                    Main.tile[l, k].type = altTileType;
                                    Main.tile[l, k].IsActive = true;
                                    Main.tile[l, k].IsHalfBlock = false;
                                    Main.tile[l, k].Slope = 0;

                                }

                            }

                        }
                        

                    }


                }
                // -0.4f, 0.7f
                num6 += Main.rand.NextFloat(minWide, maxWide);
                num4 = (int)num6;
                


            }
            
            if(generateChest)
            {
                //22
                WorldGen.AddBuriedChest(X, Y + chestDepth, 0, true, chestType);

            }

        }
        public static bool CheckValidTile(int n, int m)
        {

            if (Main.tile[n, m].type != TileID.BlueDungeonBrick || Main.tile[n, m].type != TileID.GreenDungeonBrick || Main.tile[n, m].type != TileID.PinkDungeonBrick)
                return true;

            return false;

        }

    }



}



/*

if (API.OptionsContains("")
    {


    }

        public void GeneratePaintedFloor(byte mainPaint, bool useAltColor = false, byte altPaint = PaintID.Black)
{

for (int r = 0; r < Main.maxTilesY - (Main.maxTilesY / 4); r++)
{


    for (int s = 0; s < Main.maxTilesX; s++)
    {


        if (WorldGen.SolidTile(s, r))
        {







            if (s > 600 + r + Main.rand.Next(-50, 50) && s < Main.maxTilesX - 600 - r + Main.rand.Next(-50, 50))
            {



                if (useAltColor)
                {

                    if (Main.rand.Next(3) != 0)
                    {
                        Main.tile[s, r].color(mainPaint);
                    }
                    else
                    {

                        Main.tile[s, r].color(altPaint);
                    }

                }
                else
                {

                    Main.tile[s, r].color(mainPaint);

                }

            }


        }









    }

}


}

 */