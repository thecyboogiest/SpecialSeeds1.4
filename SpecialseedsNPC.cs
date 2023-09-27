using System;
using System.Collections.Generic;
using AdvancedWorldGen.Base;
using Microsoft.Xna.Framework;
using MonoMod.Core.Platforms;
using Terraria;
using Terraria.ModLoader;

namespace Specialseeds1point4;

public class SpecialseedsNPC : GlobalNPC
{
	public string worldSeed = Main.ActiveWorldFileData.Name;

	private int cooldown;

	private int cooldownCount = 50;

	private bool modified;

	public override bool InstancePerEntity => true;

	public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
	{
        Mod awg = ModLoader.GetMod("AdvancedWorldGen");

        worldSeed = Main.ActiveWorldFileData.Name;
		if (!(bool)awg.Call("Options Contains", "NPCmania"))
		{
			return;
		}
		spawnInfo.SpawnTileType = 0;
		NPC npc = new NPC();
		for (int i = 1; i < 668; i++)
		{
			npc.CloneDefaults(i);
			if (!npc.boss)
			{
				pool.Add(i, Main.rand.Next(900, 1100));
			}
		}
	}

	public override void SetDefaults(NPC npc)
	{
        base.SetDefaults(npc);
        if (!modified)
        {
            modified = true;
            
        }
        
    }

	public override void AI(NPC npc)
	{
        base.AI(npc);
    }

    public override void HitEffect(NPC npc, NPC.HitInfo hit)
    {
       
    }

    public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
    {
        
	}

    public override void ApplyDifficultyAndPlayerScaling(NPC npc, int numPlayers, float balance, float bossAdjustment)
    {
       
    }

    public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
    {
        
    }
}
