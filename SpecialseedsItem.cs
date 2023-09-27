using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Specialseeds1point4;

public class SpecialseedsItem : GlobalItem
{
	public string worldSeed = Main.ActiveWorldFileData.Name;

	public override bool InstancePerEntity => true;

	public override GlobalItem Clone(Item item, Item itemClone)
	{
		return base.Clone(item, itemClone);
	}

	public override void SetDefaults(Item item)
	{
		worldSeed = Main.ActiveWorldFileData.Name;
	}

	public override void UseStyle(Item item, Player player, Rectangle heldItemFrame)
	{
		if (item.type == 29)
		{
			if (player.statLifeMax > 400)
			{
				player.statLifeMax = 400;
			}
			if (player.statLife > player.statLifeMax)
			{
				player.statLife = player.statLifeMax;
			}
		}
	}
}
