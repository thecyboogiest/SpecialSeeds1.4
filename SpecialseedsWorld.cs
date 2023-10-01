using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using AdvancedWorldGen.Base;
using Humanizer;
using Microsoft.Xna.Framework;
using MonoMod.Logs;
using Specialseeds1point4.Tiles;
using Specialseeds1point4.UI;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using Terraria.WorldBuilding;

namespace Specialseeds1point4;

public class SpecialseedsWorld : ModSystem
{

	public string currentSelectedSeed;

	public int worldSpikes;

	public int worldBaubles;

	public int meteors;

	public int zone;


	public override void PostUpdatePlayers()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < Main.PlayerList.Count(); i++)
		{
			Player player = Main.player[i];
			SceneMetrics metrics = Main.SceneMetrics;
			if (Seeds.Enabled(Seeds.Meteormania) && (double)player.Center.Y <= Main.worldSurface * 16.0)
			{
				player.AddBuff(86, 55);
				player.jumpSpeedBoost *= 2f;
				player.ZoneMeteor = true;
				metrics.MeteorTileCount = 169;
				if (player.velocity.Y > player.maxFallSpeed / 1.7f)
				{
					player.velocity.Y = player.maxFallSpeed / 1.7f;
				}
			}


		}
	}
    public override void PreWorldGen()
    {
        
        
    }
    
    
    public override void LoadWorldData(TagCompound tag)
    {
        if (tag.ContainsKey("activatedSeeds"))
        {
            List<string> acts = tag["activatedSeeds"] as List<string>;
            Seeds.activatedSeeds = acts;
        }


    }
    public override void SaveWorldData(TagCompound tag)
    {
        tag["activatedSeeds"] = Seeds.activatedSeeds;
    }

    public override void PostWorldGen()
    {

    }

    public override void OnWorldUnload()
    {
        Seeds.activatedSeeds.Clear();
    }

    public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
    {
        Mod awg = ModLoader.GetMod("AdvancedWorldGen");

        bool CheckOptionContains(string seedName)
        {
            return (bool)awg.Call("Options Contains", seedName);
        }


        foreach (string seed in Seeds.All())
        {
            if (CheckOptionContains(seed))
                Seeds.activatedSeeds.Add(seed);
        }

        int genIndex = tasks.FindIndex((GenPass genpass) => genpass.Name.Equals("Shinies"));
        if (Main.maxTilesX < 6000)
        {
            worldBaubles = Main.rand.Next(30, 43);
        }
        else if (Main.maxTilesX < 8000)
        {
            worldBaubles = Main.rand.Next(43, 53);
        }
        else
        {
            worldBaubles = Main.rand.Next(50, 63);
        }
        if (CheckOptionContains("Nuked"))
        {
            tasks.RemoveAt(tasks.FindIndex((GenPass genpass) => genpass.Name.Equals("Grass")));
            tasks.RemoveAt(tasks.FindIndex((GenPass genpass) => genpass.Name.Equals("Spreading Grass")));
            tasks.RemoveAt(tasks.FindIndex((GenPass genpass) => genpass.Name.Equals("Planting Trees")));
        }
        if (CheckOptionContains("Trapmania"))
        {
            tasks.RemoveAt(tasks.FindIndex((GenPass genpass) => genpass.Name.Equals("Wavy Caves")));
            tasks.Insert(tasks.FindIndex((GenPass genpass) => genpass.Name.Equals("Traps")) + 1, new PassLegacy("Traps 2", Traps2));
            tasks.Insert(tasks.FindIndex((GenPass genpass) => genpass.Name.Equals("Traps")) + 1, new PassLegacy("Traps 2", Traps2));
            tasks.Insert(tasks.FindIndex((GenPass genpass) => genpass.Name.Equals("Traps")) + 1, new PassLegacy("Traps 2", Traps2));
        }
        genIndex = tasks.FindIndex((GenPass genpass) => genpass.Name.Equals("Small Holes")) - 1;
        tasks.Insert(genIndex, new PassLegacy("Seeds main", GenerateWorld));
        genIndex = tasks.FindIndex((GenPass genpass) => genpass.Name.Equals("Small Holes")) + 1;
        tasks.Insert(genIndex, new PassLegacy("Small Hole Extras", SmallHoleExtras));
        genIndex = tasks.FindIndex((GenPass genpass) => genpass.Name.Equals("Hives")) + 1;
        tasks.Insert(genIndex, new PassLegacy("World Extras", GenerateExtras));
        genIndex = tasks.FindIndex((GenPass genpass) => genpass.Name.Equals("Final Cleanup")) - 1;
        tasks.Insert(genIndex, new PassLegacy("Post Cleanup", PostCleanUpTasks));
        if (Specialseeds1point4.layers.Count > 0)
        {
            genIndex = tasks.FindIndex((GenPass genpass) => genpass.Name.Equals("Seeds main")) + 1;
            tasks.Insert(genIndex, new PassLegacy("Custom World Generation", CustomWorld));
        }
    }

    private void Traps2(GenerationProgress progress, GameConfiguration gameConfiguration)
	{
		if (WorldGen.notTheBees)
		{
			return;
		}
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
				if (Main.tile[num339, num340].WallType == 0 && WorldGen.placeTrap(num339, num340))
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
				if (Main.tile[num343, num344].WallType == 187 && WorldGen.PlaceSandTrap(num343, num344))
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

	private void SmallHoleExtras(GenerationProgress progress, GameConfiguration gameConfiguration)
	{
		
	}

	private void CustomWorld(GenerationProgress progress, GameConfiguration gameConfiguration)
	{
        progress.Message = "Custom World Generation";

        List<int> tileIdsToPlace = new();
        for(int l = 0; l < Specialseeds1point4.layers.Count; l++)
        {

            if (Specialseeds1point4.layers[l].tiles.Count > 0)
            {
                foreach (CustomSeedTile tile in Specialseeds1point4.SelectedTiles(Specialseeds1point4.layers[l].tiles))
                {
                    if (tile.selected)
                        tileIdsToPlace.Add(tile.id);
                    tile.selected = false;

                }

                int layerId = l;
                int layerHeight = ((Main.maxTilesY / Specialseeds1point4.layers.Count) * layerId) + 50;

                if (tileIdsToPlace.Count > 0)
                    for (int i = 20; i < Main.maxTilesX - 20; i += 5)
                        for (int j = 20; j < Main.maxTilesY; j++)
                        {
                            int sin = (int)(Math.Sin(i / 60f) * 30);
                            int pointBeforeCheck = layerHeight + sin;
                            int point = pointBeforeCheck < 20 ? 20 : (pointBeforeCheck > Main.maxTilesY - 20 ? Main.maxTilesY - 20 : pointBeforeCheck);

                            if (j >= point)
                                WorldGen.TileRunner(i, j, 10, 1, tileIdsToPlace[Main.rand.Next(0, tileIdsToPlace.Count)], false, 0, 0, false, true);
                        }
                tileIdsToPlace.Clear();
            }

        }


    }

    private void GenerateWorld(GenerationProgress progress, GameConfiguration gameConfiguration)
	{
		progress.Message = "Breaking The Balance";
		if(Seeds.Enabled(Seeds.Icemania))
			{
            for (int i = 0; i < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 6E-05); i++)
            {
                WorldGen.TileRunner(WorldGen.genRand.Next(WorldGen.genRand.Next(500, 700), Main.maxTilesX - WorldGen.genRand.Next(500, 700)), WorldGen.genRand.Next(0, (int)Main.worldSurface + 350), WorldGen.genRand.Next(100, 200), 15, 147);
                WorldGen.TileRunner(WorldGen.genRand.Next(Main.maxTilesX / 2 - WorldGen.genRand.Next(100, 500), Main.maxTilesX / 2 + WorldGen.genRand.Next(100, 500)), WorldGen.genRand.Next(0, Main.maxTilesY + 300), WorldGen.genRand.Next(200, 350), 35, 147);
            }
            for (int z = 0; z < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 6E-05); z++)
            {
                WorldGen.TileRunner(WorldGen.genRand.Next(WorldGen.genRand.Next(300, 500), Main.maxTilesX / 2 - WorldGen.genRand.Next(300, 500)), WorldGen.genRand.Next((int)Main.worldSurface + 150, Main.maxTilesY - WorldGen.genRand.Next(300, 500)), WorldGen.genRand.Next(80, 110), 1, 161, addTile: true, 0f, 0f, noYChange: true);
                WorldGen.TileRunner(WorldGen.genRand.Next(Main.maxTilesX / 2 + WorldGen.genRand.Next(300, 500), Main.maxTilesX - WorldGen.genRand.Next(300, 500)), WorldGen.genRand.Next((int)Main.worldSurface + 150, Main.maxTilesY - WorldGen.genRand.Next(300, 500)), WorldGen.genRand.Next(80, 110), 1, 161, addTile: true);
            }
        }
        if (Seeds.Enabled(Seeds.Junglemania))
        {
            for (int j = 0; j < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 6E-05); j++)
            {
                WorldGen.TileRunner(WorldGen.genRand.Next(WorldGen.genRand.Next(300, 500), Main.maxTilesX - WorldGen.genRand.Next(300, 500)), WorldGen.genRand.Next((int)Main.worldSurface + WorldGen.genRand.Next(100, 200), Main.maxTilesY - WorldGen.genRand.Next(100, 300)), WorldGen.genRand.Next(130, 190),2, 59);
                WorldGen.TileRunner(WorldGen.genRand.Next(Main.maxTilesX / 2 - WorldGen.genRand.Next(100, 300), Main.maxTilesX / 2 + WorldGen.genRand.Next(100, 300)), WorldGen.genRand.Next((int)Main.worldSurface, (int)Main.worldSurface + WorldGen.genRand.Next(300, 400)), WorldGen.genRand.Next(130, 190), 2, 59);
                WorldGen.TileRunner(WorldGen.genRand.Next(WorldGen.genRand.Next(300, 500), Main.maxTilesX - WorldGen.genRand.Next(300, 500)), WorldGen.genRand.Next((int)Main.worldSurface + WorldGen.genRand.Next(100, 200), Main.maxTilesY - WorldGen.genRand.Next(300, 500)), WorldGen.genRand.Next(50, 90), 1, 225);
            }
        }
        if (Seeds.Enabled(Seeds.Desertmania))
        {
            for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 6E-05); k++)
            {
                WorldGen.TileRunner(WorldGen.genRand.Next(WorldGen.genRand.Next(500, 700), Main.maxTilesX - WorldGen.genRand.Next(500, 700)), WorldGen.genRand.Next(0, (int)Main.worldSurface + 350), WorldGen.genRand.Next(100, 200), 15, 53);
                WorldGen.TileRunner(WorldGen.genRand.Next(Main.maxTilesX / 2 - WorldGen.genRand.Next(100, 500), Main.maxTilesX / 2 + WorldGen.genRand.Next(100, 500)), WorldGen.genRand.Next(0, Main.maxTilesY + 300), WorldGen.genRand.Next(200, 350), 35, 53);
                WorldGen.TileRunner(WorldGen.genRand.Next(WorldGen.genRand.Next(300, 500), Main.maxTilesX - WorldGen.genRand.Next(300, 500)), WorldGen.genRand.Next((int)Main.worldSurface + WorldGen.genRand.Next(100, 200), Main.maxTilesY - WorldGen.genRand.Next(300, 500)), WorldGen.genRand.Next(50, 90), 1, 396);
            }
            GenerateBauble(397, 15, 35, 2);
            GenerateBauble(396, 25, 35, 4);
        }
        if (Seeds.Enabled(Seeds.Meteormania))
        {
            for (int l = 0; l < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 6E-05); l++)
            {
                WorldGen.TileRunner(WorldGen.genRand.Next(Main.maxTilesX / 2 - WorldGen.genRand.Next(100, 500), Main.maxTilesX / 2 + WorldGen.genRand.Next(100, 500)), WorldGen.genRand.Next(0, Main.maxTilesY + 300), WorldGen.genRand.Next(200, 350), 35, 1);
                WorldGen.TileRunner(WorldGen.genRand.Next(WorldGen.genRand.Next(300, 500), Main.maxTilesX / 2 - WorldGen.genRand.Next(300, 500)), WorldGen.genRand.Next((int)Main.worldSurface + 95, (int)Main.worldSurface + 350), WorldGen.genRand.Next(40, 60), 2, 368);
                WorldGen.TileRunner(WorldGen.genRand.Next(Main.maxTilesX / 2 + WorldGen.genRand.Next(300, 500), Main.maxTilesX - WorldGen.genRand.Next(300, 500)), WorldGen.genRand.Next((int)Main.worldSurface + 95, (int)Main.worldSurface + 350), WorldGen.genRand.Next(40, 60), 2, 368);
                WorldGen.TileRunner(WorldGen.genRand.Next(WorldGen.genRand.Next(300, 500), Main.maxTilesX / 2 - WorldGen.genRand.Next(300, 500)), WorldGen.genRand.Next((int)Main.worldSurface + 95, (int)Main.worldSurface + 350), WorldGen.genRand.Next(40, 60), 2, 367);
                WorldGen.TileRunner(WorldGen.genRand.Next(Main.maxTilesX / 2 + WorldGen.genRand.Next(300, 500), Main.maxTilesX - WorldGen.genRand.Next(300, 500)), WorldGen.genRand.Next((int)Main.worldSurface + 95, (int)Main.worldSurface + 350), WorldGen.genRand.Next(40, 60), 2, 367);
                WorldGen.TileRunner(WorldGen.genRand.Next(WorldGen.genRand.Next(300, 500), Main.maxTilesX / 2 - WorldGen.genRand.Next(300, 500)), WorldGen.genRand.Next((int)Main.worldSurface + WorldGen.genRand.Next(100, 200), Main.maxTilesY - WorldGen.genRand.Next(300, 500)), WorldGen.genRand.Next(40, 60), 2, 37);
                WorldGen.TileRunner(WorldGen.genRand.Next(Main.maxTilesX / 2 + WorldGen.genRand.Next(300, 500), Main.maxTilesX - WorldGen.genRand.Next(300, 500)), WorldGen.genRand.Next((int)Main.worldSurface + WorldGen.genRand.Next(100, 200), Main.maxTilesY - WorldGen.genRand.Next(300, 500)), WorldGen.genRand.Next(40, 60), 2, 37);
                WorldGen.TileRunner(WorldGen.genRand.Next(WorldGen.genRand.Next(300, 500), Main.maxTilesX / 2 - WorldGen.genRand.Next(300, 500)), WorldGen.genRand.Next((int)Main.worldSurface + WorldGen.genRand.Next(100, 200), Main.maxTilesY - WorldGen.genRand.Next(300, 500)), WorldGen.genRand.Next(20, 40), 2, 56);
                WorldGen.TileRunner(WorldGen.genRand.Next(Main.maxTilesX / 2 + WorldGen.genRand.Next(300, 500), Main.maxTilesX - WorldGen.genRand.Next(300, 500)), WorldGen.genRand.Next((int)Main.worldSurface + WorldGen.genRand.Next(100, 200), Main.maxTilesY - WorldGen.genRand.Next(300, 500)), WorldGen.genRand.Next(20, 40), 2, 56);
            }
        }
        if (Seeds.Enabled(Seeds.RainyDay))
        {
            for (int m = 0; m < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 6E-05); m++)
            {
                WorldGen.TileRunner(WorldGen.genRand.Next(WorldGen.genRand.Next(300, 500), Main.maxTilesX / 2 - WorldGen.genRand.Next(300, 500)), WorldGen.genRand.Next(60, 125), WorldGen.genRand.Next(10, 30), 1, 189, addTile: true);
                WorldGen.TileRunner(WorldGen.genRand.Next(Main.maxTilesX / 2 + WorldGen.genRand.Next(300, 500), Main.maxTilesX - WorldGen.genRand.Next(300, 500)), WorldGen.genRand.Next(60, 125), WorldGen.genRand.Next(10, 30), 1, 189, addTile: true);
                WorldGen.TileRunner(WorldGen.genRand.Next(Main.maxTilesX / 2 - WorldGen.genRand.Next(300, 500), Main.maxTilesX / 2 + WorldGen.genRand.Next(300, 500)), WorldGen.genRand.Next(100, 125), WorldGen.genRand.Next(10, 30), 2, 189, addTile: true);
                WorldGen.TileRunner(WorldGen.genRand.Next(WorldGen.genRand.Next(300, 500), Main.maxTilesX / 2 - WorldGen.genRand.Next(300, 500)), WorldGen.genRand.Next(100, 150), WorldGen.genRand.Next(1, 20), 1, 196, addTile: true);
                WorldGen.TileRunner(WorldGen.genRand.Next(Main.maxTilesX / 2 + WorldGen.genRand.Next(300, 500), Main.maxTilesX - WorldGen.genRand.Next(300, 500)), WorldGen.genRand.Next(100, 150), WorldGen.genRand.Next(1, 20), 1, 196, addTile: true);
            }
        }
        if (Seeds.Enabled(Seeds.Cavemania))
        {
            GenerateSpike(Main.maxTilesX / 2, Main.maxTilesY / 2, Main.maxTilesY / 8, Main.maxTilesY / 4, 367, 368, true, 2, 0);
            for (int num42070 = 0; num42070 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.00013); num42070++)
            {
                float value17 = (float)((double)num42070 / ((double)(Main.maxTilesX * Main.maxTilesY) * 0.00013));
                progress.Set(value17);
                if (Main.rockLayer <= (double)Main.maxTilesY)
                {
                    int type7 = -1;
                    if (WorldGen.genRand.Next(10) == 0)
                    {
                        type7 = -2;
                    }
                    WorldGen.TileRunner(WorldGen.genRand.Next(100, Main.maxTilesX - 100), WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY - 100), WorldGen.genRand.Next(15, 35), WorldGen.genRand.Next(20, 600), type7);
                }
            }
        }
        if (Seeds.Enabled(Seeds.Nuked))
        {
            GenerateFloor(0, useAltTile: false, 0);
            for (int num42071 = 0; num42071 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.00013); num42071++)
            {
                float value18 = (float)((double)num42071 / ((double)(Main.maxTilesX * Main.maxTilesY) * 0.00013));
                progress.Set(value18);
                if (Main.worldSurface - 100.0 <= (double)Main.maxTilesY)
                {
                    int type8 = -1;
                    if (WorldGen.genRand.Next(10) == 0)
                    {
                        type8 = -2;
                    }
                    WorldGen.TileRunner(WorldGen.genRand.Next(Main.maxTilesX / 2 - 800, Main.maxTilesX / 2 + 800), WorldGen.genRand.Next((int)Main.worldSurface - 100, Main.maxTilesY / 2 + 300), WorldGen.genRand.Next(15, 55), 5, type8);
                }
            }
        }
        if (Seeds.Enabled(Seeds.Trapmania))
        {
            for (int num42069 = 0; num42069 < Main.maxTilesY; num42069 += 40)
            {
                for (int num42073 = 0; num42073 < Main.maxTilesX; num42073 += 40)
                {
                    WorldGen.TileRunner(num42073, num42069, WorldGen.genRand.Next(num42069 / 80), WorldGen.genRand.Next(20, 400), ModContent.TileType<CrackedStone>());
                }
            }
            for (int num42072 = 0; num42072 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.00013); num42072++)
            {
                float value19 = (float)((double)num42072 / ((double)(Main.maxTilesX * Main.maxTilesY) * 0.00013));
                progress.Set(value19);
                if (Main.rockLayer <= (double)Main.maxTilesY)
                {
                    int type9 = -1;
                    if (WorldGen.genRand.NextBool(10))
                    {
                        type9 = -2;
                    }
                    int i2 = WorldGen.genRand.Next(0, Main.maxTilesX);
                    int y = WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY);
                    int scale = WorldGen.genRand.Next(20, 25);
                    int strength = WorldGen.genRand.Next(10, 40);
                    WorldGen.TileRunner(i2, y, scale + 5, strength, 232);
                    WorldGen.TileRunner(i2, y, scale, strength, type9);
                }
            }
        }
        if (Seeds.Enabled(Seeds.Woodlands))
        {
            int treeTip = (int)Main.worldSurface - 100 > 10 ? (int)Main.worldSurface - 100 : 10;
            for (int l = 0; l < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 6E-05); l++)
                WorldGen.TileRunner(WorldGen.genRand.Next(WorldGen.genRand.Next(500, 700), Main.maxTilesX - WorldGen.genRand.Next(500, 700)), WorldGen.genRand.Next(0, (int)Main.worldSurface + 350), WorldGen.genRand.Next(100, 200), 15, TileID.LeafBlock);

           

            for (int j = treeTip; j < Main.maxTilesY - 100; j++)
                WorldGen.TileRunner((Main.maxTilesX / 2) + (int)(MathF.Sin(j / 10f) / 0.1f), j, j / 20, 15, TileID.LivingWood, true, 0,0, false, true);
        }

        
    }
    
	

	private void GenerateExtras(GenerationProgress progress, GameConfiguration gameConfiguration)
	{
        Mod.Logger.Debug("Special Seeds to do: " + Seeds.activatedSeeds.Count.ToString());

        foreach (string seed in Seeds.activatedSeeds)
        {
            Mod.Logger.Debug(seed );
        }
        progress.Message = "Breaking The Rest";
        if (Seeds.Enabled(Seeds.Icemania))
        {
            //GenerateSpike(Main.maxTilesX / 2, Main.maxTilesY / 2, 50, 20, TileID.Stone, TileID.Dirt,false, WallID.Dirt, WallID.Stone, false, 0,0, false, 0, 0);
            GenerateRandomSpikes(100, 50, TileID.IceBlock, TileID.SnowBlock, 4, 8, true, 22, 50);
            GenerateBauble(161, 15, 35, 2);
        }
        if (Seeds.Enabled(Seeds.Junglemania))
            GenerateBauble(225, 35, 45, 1);
        if (Seeds.Enabled(Seeds.Desertmania))
            GenerateRandomSpikes(45, 115, 274, 151, 1, 8, true, 2, 80);
        if (Seeds.Enabled(Seeds.Meteormania))
        {
            GenerateRandomSpikes(45, 115, 1, 370, 2, 4, generateChest: true, 2, 80);
            WorldGen.spawnMeteor = true;
            if (Main.maxTilesX > 6000)
            {
                meteors = Main.rand.Next(31, 42);
            }
            else if (Main.maxTilesX > 8000)
            {
                meteors = Main.rand.Next(41, 50);
            }
            else
            {
                meteors = Main.rand.Next(48, 58);
            }
            for (int r = 0; r < meteors; r++)
            {
                int num1 = 0;
                int num3 = Main.rand.Next(Main.maxTilesX / 2 + 150, Main.maxTilesX - 150);
                int num5 = Main.rand.Next(150, Main.maxTilesX / 2 - 150);
                for (int s = 0; s < Main.maxTilesY; s++)
                {
                    if (r < meteors / 2)
                    {
                        if (WorldGen.SolidTile(num3, num1))
                        {
                            WorldGen.meteor(num3, num1);
                            break;
                        }
                        num1++;
                    }
                    else
                    {
                        if (WorldGen.SolidTile(num5, num1))
                        {
                            WorldGen.meteor(num5, num1);
                            break;
                        }
                        num1++;
                    }
                }
            }
        }
        if (Seeds.Enabled(Seeds.Trapmania))
        {
            for (int t = 0; t < Main.maxTilesX / 8; t++)
            {
                int num2 = 0;
                int num4 = Main.rand.Next(350, Main.maxTilesX - 350);
                float value17 = (float)((double)t / (double)(Main.maxTilesX / 8));
                progress.Set(value17);
                for (int s2 = 0; s2 < Main.maxTilesY; s2++)
                {
                    if (s2 < Main.maxTilesY - 200)
                    {
                        if (Main.tile[num4, num2].LiquidType == 1)
                        {
                            if (WorldGen.SolidTile(num4, num2 - 10))
                            {
                                WorldGen.TileRunner(num4, num2 - 10, WorldGen.genRand.Next(20, 40), 2, ModContent.TileType<CrackedStone>(), addTile: false, 0f, 0f, noYChange: true);
                                break;
                            }
                            for (int i = num2 - 10; i <= num2 - 5; i++)
                            {
                                for (int r2 = num4 - 40; r2 < num4 + 40; r2++)
                                {
                                    if (!Main.rand.NextBool(15))
                                    {
                                        if(CheckValidTile(r2, i))
                                            WorldGen.PlaceTile(r2, i, ModContent.TileType<CrackedStone>());
                                    }
                                }
                            }
                            break;
                        }
                        num2++;
                    }
                    else
                    {
                        num4 = Main.rand.Next(150, Main.maxTilesX - 150);
                        s2 = 0;
                    }
                }
            }
        }

        if (Seeds.Enabled(Seeds.Woodlands))
        {
            int treeTip = (int)Main.worldSurface - 100 > 10 ? (int)Main.worldSurface - 100 : 10;
            

            for (int l = 0; l < 100; l++)
                WorldGen.TileRunner((Main.maxTilesX / 2) + Main.rand.Next(-50, 50), treeTip + Main.rand.Next(-50, 50), WorldGen.genRand.Next(50, 100), 15, TileID.LeafBlock, true, 0, 10, false, true);

            for (int l = 0; l < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 6E-04); l++)
            {
                int startX = WorldGen.genRand.Next(WorldGen.genRand.Next(500, 700), Main.maxTilesX - WorldGen.genRand.Next(500, 700));
                int startY = WorldGen.genRand.Next(0, (int)Main.worldSurface + 500);
                int endX = startX + WorldGen.genRand.Next(-40, 41);
                int endY = startY + WorldGen.genRand.Next(-40, 41);
                GenerateBranch(startX, startY, endX, endY);
            }



        }
    }

    private void PostCleanUpTasks(GenerationProgress progress, GameConfiguration gameConfiguration)
    {
        if (Seeds.Enabled(Seeds.Upside))
        {
            WorldGen.PlaceTile(Main.spawnTileX + 1, Main.spawnTileY - 5, TileID.Stone);
            WorldGen.PlaceTile(Main.spawnTileX, Main.spawnTileY - 5, TileID.Stone);
            WorldGen.PlaceTile(Main.spawnTileX - 1, Main.spawnTileY - 5, TileID.Stone);
            WorldGen.PlaceTile(Main.spawnTileX + 2, Main.spawnTileY - 5, TileID.Stone);
            WorldGen.PlaceTile(Main.spawnTileX - 2, Main.spawnTileY - 5, TileID.Stone);


        }
    }

	public void GenerateFloor(ushort mainTile, bool useAltTile = false, ushort altTile = 0)
	{
		//IL_00e1: Expected native int or pointer, but got O
		//IL_0119: Expected native int or pointer, but got O
		//IL_0151: Expected native int or pointer, but got O
		int bottomLeft = 0;
		int bottomRight = 0;
		for (int r = 0; r < Main.maxTilesY - Main.maxTilesY / 4; r++)
		{
			for (int s = 0; s < Main.maxTilesX; s++)
			{
				if (!WorldGen.SolidTile(s, r))
				{
					continue;
				}
				if ((s == 599 + r || s == Main.maxTilesX - r - 599) && Main.rand.Next(16) != 0)
				{
					WorldGen.TileRunner(s, r, WorldGen.genRand.Next(15, 125), 1, mainTile);
				}
				if (s <= 600 + r + Main.rand.Next(-50, 50) || s >= Main.maxTilesX - 600 - r + Main.rand.Next(-50, 50))
				{
					continue;
				}
				if (bottomLeft == 0 && r == Main.maxTilesY - Main.maxTilesY / 4 - 1)
				{
					bottomLeft = s;
				}
				bottomRight = s;

                if (useAltTile && Main.rand.NextBool(3))
                    Main.tile[s, r].ResetToType(altTile);
				else
                    Main.tile[s, r].ResetToType(mainTile);

            }
        }
		for (int i = 0; i < bottomRight; i++)
		{
			if (i > bottomLeft)
			{
				WorldGen.TileRunner(i, Main.maxTilesY - Main.maxTilesY / 4, WorldGen.genRand.Next(15, 125), 1, mainTile, addTile: true);
			}
		}
	}

	public void GenerateBauble(int type, int sizeMin, int sizeMax, int strength)
	{
		for (int r = 0; r < worldBaubles; r++)
		{
			int num1 = 200;
			int num2 = Main.rand.Next(600, Main.maxTilesX - 600);
			for (int s = 0; s < Main.maxTilesY; s++)
			{
				if (!WorldGen.SolidTile(num2, num1))
				{
					num1++;
					continue;
				}
				WorldGen.TileRunner(num2, num1, WorldGen.genRand.Next(sizeMin, sizeMax), strength, type);
				break;
			}
		}
	}

    public void GenerateBranch(int startX, int startY, int endX, int endY)
    {
        int distance = (int)Vector2.Distance(new Vector2(startX, startY), new Vector2(endX, endY));
        Vector2 step = (new Vector2(startX, startY) - new Vector2(endX, endY)) / distance;
        int strength = WorldGen.genRand.Next(2, 8);
        for (int d = 0; d <= distance; d++)
        {
            Vector2 pos = new Vector2(startX, startY) + (step * d);

            WorldGen.TileRunner((int)pos.X, (int)pos.Y, strength, 1, TileID.LivingWood);

        }


    }

    public void GenerateRandomSpikes(int spikeHeight, int spikeWidth, ushort tileType, ushort altTileType, int worldSpikesMin = 2, int worldSpikesMax = 6,  bool generateChest = false, int chestType = 0, int chestDepth = 50)
	{

        if (Main.maxTilesX < 6000)
		{
			worldSpikes = Main.rand.Next(worldSpikesMin, worldSpikesMax);
		}
		else if (Main.maxTilesX < 8000)
		{
			worldSpikes = Main.rand.Next(worldSpikesMin * 2, worldSpikesMax * 2);
		}
		else
		{
			worldSpikes = Main.rand.Next(worldSpikesMin * 4, worldSpikesMax * 4);
		}
		for (int r = 0; r < worldSpikes; r++)
		{
			
			int num2 = Main.rand.Next(600, Main.maxTilesX - 600);
			for (int s = 350; s < Main.maxTilesY; s++)
			{
				if (!WorldGen.SolidTile(num2, s))
				{
					s++;
					continue;
				}
                int fourthWidth = spikeWidth / 4;

                int fourthHeight = spikeHeight / 4;
                GenerateSpike(num2, s, spikeHeight + Main.rand.Next(-fourthHeight, fourthHeight), spikeWidth + Main.rand.Next(-fourthHeight, fourthHeight), tileType, altTileType, generateChest, chestType, chestDepth);
                break;
			}
		}
	}

	public void GenerateSpike(int x, int y, int spikeHeight, int spikeWidth, ushort tileType, ushort altTileType, bool generateChest = false, int chestType = 0, int chestDepth = 5)
	{
		int halfWidth = (spikeWidth / 2);
        int halfHeight = (spikeHeight / 2);

        for (int i = 0; i < spikeWidth; i++)
		{
			int point = (int)MathF.Round((-MathF.Abs(i - halfWidth) + halfWidth) * (spikeHeight / halfWidth));
            for (int j = 0; j < point; j++)
            {
                int xPos = x + i;
                int yPos = y + (spikeHeight / 2) - j;
                

                if(xPos > 0 && xPos < Main.maxTilesX && yPos > 0 && yPos < Main.maxTilesY)
                {
                    if (generateChest && j > spikeWidth - 5 && j < spikeWidth + 5 && i > halfHeight - chestDepth - 5 && i < halfHeight - chestDepth)
                    {
                        WorldGen.KillTile(xPos, yPos, false, false, true);
                        
                        continue;
                    }
                    float alt = j / (spikeHeight / 8);
                    if (Main.rand.NextFloat(alt) <= 0.5f)
                        WorldGen.PlaceTile(xPos, yPos, altTileType, false, true);
                    else
                        WorldGen.PlaceTile(xPos, yPos, tileType, false, true);
                }
            }

        }
        if(generateChest)
            WorldGen.AddBuriedChest(halfWidth + x, y - halfHeight + chestDepth, 0, notNearOtherChests: true, chestType, trySlope: false, 0);


    }

	public bool CheckValidTile(int n, int m)
	{
		if (Main.tile[n, m].TileType != 41 || Main.tile[n, m].TileType != 43 || Main.tile[n, m].TileType != 44)
		{
			return true;
		}
		return false;
	}
}
