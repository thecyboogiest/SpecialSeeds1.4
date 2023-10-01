using System;
using System.Collections.Generic;
using System.Linq;
using AdvancedWorldGen.Base;
using Microsoft.Xna.Framework;
using MonoMod.Core.Platforms;
using Specialseeds1point4.Tiles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Specialseeds1point4;

public class SpecialseedsPlayer : ModPlayer
{
	public string worldSeed = Main.ActiveWorldFileData.Name;

	public int tickTimer;

	private int tickTime = 60;
	private float veloPain;

	public int bossesDefeated;

	public float breakStoneCooldown;

	public List<BreakTile> tilesToBreak = new List<BreakTile>();

   
    public override void PostUpdateEquips()
    {
        if (Seeds.Enabled(Seeds.Upside))
        {
            Player.gravControl = true;
            Player.controlUp = false;
            Player.gravDir = -1f;
        }
    }

    public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
    {
        if (!newPlayer)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write(Seeds.activatedSeeds.Count);
            foreach (string seed in Seeds.activatedSeeds)
                packet.Write(seed);

        }

    }


   

    public override void PostUpdate()
	{
        //Main.NewText(SpecialseedsWorld.Seeds.activatedSeeds.Count);

        if (tilesToBreak.Count > 0)
		{
			if (MathF.Abs(tilesToBreak.FirstOrDefault().playerX - tilesToBreak.FirstOrDefault().x) < 15f)
			{
				WorldGen.KillTile(tilesToBreak.FirstOrDefault().x, tilesToBreak.FirstOrDefault().y);
			}
			tilesToBreak.RemoveAt(0);
		}
		
        

        if (Seeds.Enabled(Seeds.Nuked))
		{

			if (base.Player.statLife > 0 && (base.Player.wet || ((double)base.Player.Center.Y <= (Main.worldSurface - 25.0) * 16.0 && base.Player.Center.X >= (float)((Main.maxTilesX / 2 - 400) * 16) && base.Player.Center.X <= (float)((Main.maxTilesX / 2 + 400) * 16))))
			{
				
                if (tickTimer >= tickTime)
                {
                    SoundEngine.PlaySound(new SoundStyle($"{nameof(Specialseeds1point4)}/Sounds/Geiger"), base.Player.Center);

                    tickTimer = 0;
                    if (tickTime <= 20)
                    {
                        NukeDamage();
                        
                    }
                    if (tickTime > 5)
                        tickTime = (int)((float)tickTime * 0.95f);
                }
                else
                    tickTimer++;


            }
            else
			{
                tickTime = 60;
                tickTimer = 0;
            }
		}
        if (Seeds.Enabled(Seeds.Upside))
        {
            if((double)base.Player.Center.Y / 16 <= 45)
            {
                base.Player.KillMe(PlayerDeathReason.ByCustomReason(base.Player.name + " was killed by falling upwards"), 10000, 1);
            }
        }
        if (Seeds.Enabled(Seeds.Trapmania))
		{
            if ((double)base.Player.Center.Y > Main.worldSurface)
            {
                if (breakStoneCooldown > 0f)
                {
                    breakStoneCooldown -= 0.1f;
                }
                else
                {
                    breakStoneCooldown = 0f;
                }
            }
            if (breakStoneCooldown == 0f)
            {
                if (Main.tile[(int)base.Player.Center.X / 16, (int)(base.Player.Center.Y / 16f) + 2].TileType == ModContent.TileType<CrackedStone>())
                {
                    WorldGen.KillTile((int)base.Player.Center.X / 16, (int)(base.Player.Center.Y / 16f) + 2);
                    for (int i = 0; (float)i < base.Player.velocity.Y; i++)
                    {
                        for (int r = -3; r < 3; r++)
                        {
                            if (r > -1 && r < 1)
                            {
                                if (Main.tile[(int)(base.Player.Center.X / 16f) + r, (int)(base.Player.Center.Y / 16f) + i].TileType == ModContent.TileType<CrackedStone>())
                                {
                                    BreakTile((int)(base.Player.Center.X / 16f) + r, (int)(base.Player.Center.Y / 16f) + i);
                                }
                            }
                            else if (!Main.rand.NextBool(2) && Main.tile[(int)(base.Player.Center.X / 16f) + r, (int)(base.Player.Center.Y / 16f) + i].TileType == ModContent.TileType<CrackedStone>())
                            {
                                BreakTile((int)(base.Player.Center.X / 16f) + r, (int)(base.Player.Center.Y / 16f) + i);
                            }
                        }
                    }
                    breakStoneCooldown = 5f;
                }
                else
                {
                    breakStoneCooldown = 0f;
                }
            }
            else if (Main.rand.NextBool(120) && breakStoneCooldown == 0f)
            {
                if (Main.tile[(int)base.Player.Center.X / 16, (int)(base.Player.Center.Y / 16f) + 2].TileType == ModContent.TileType<CrackedStone>())
                {
                    BreakTile((int)base.Player.Center.X / 16, (int)(base.Player.Center.Y / 16f) + 2);
                    BreakTile((int)(base.Player.Center.X / 16f) - 1, (int)(base.Player.Center.Y / 16f) + 3);
                    BreakTile((int)(base.Player.Center.X / 16f) + 1, (int)(base.Player.Center.Y / 16f) + 3);
                    breakStoneCooldown = 2f;
                }
                else
                {
                    breakStoneCooldown = 0f;
                }
            }
            if (base.Player.HasBuff(111))
            {
                base.Player.ClearBuff(111);
            }
        }
		
	}

	private void NukeDamage()
	{
        base.Player.statLife -= 2;
        CombatText.NewText(new Rectangle((int)base.Player.position.X, (int)base.Player.position.Y, base.Player.width, base.Player.height), CombatText.LifeRegen, 2, dramatic: false, dot: true);
        if (base.Player.statLife <= 0 && base.Player.whoAmI == Main.myPlayer)
        {
            base.Player.KillMe(PlayerDeathReason.ByOther(9), 10.0, 0);
        }
    }

    public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
    {
       
    }

    public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
    {
        if (proj.friendly)
        {
            base.Player.statLife -= hurtInfo.Damage / 2 + hurtInfo.Damage / 6 * bossesDefeated;
            CombatText.NewText(new Rectangle((int)base.Player.position.X, (int)base.Player.position.Y, base.Player.width, base.Player.height), CombatText.LifeRegen, hurtInfo.Damage / 2, dramatic: false, dot: true);
            if (base.Player.statLife <= 0 && base.Player.whoAmI == Main.myPlayer)
            {
                base.Player.KillMe(PlayerDeathReason.ByOther(5), 10.0, 0);
            }
        }
    }

    public void BreakTile(int i, int j)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		BreakTile tileToBreak = new BreakTile();
		tileToBreak.setXY(i, j, (int)base.Player.Center.X / 16, (int)base.Player.Center.Y / 16);
		if (!tilesToBreak.Contains(tileToBreak))
		{
			tilesToBreak.Add(tileToBreak);
		}
	}

    public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
    {
        tickTime = 60;
        tickTimer = 0;
    }
}
