using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Specialseeds1point4;

public class LifeGelly : ModItem
{
	public override void SetStaticDefaults()
	{
        
	}

	public override void SetDefaults()
	{
		base.Item.consumable = true;
		base.Item.useTime = 15;
		base.Item.useAnimation = 15;
		base.Item.useStyle = 4;
		base.Item.UseSound = SoundID.Item4;
		base.Item.maxStack = 30;
	}

	public override bool CanUseItem(Player player)
	{
		if (player.statLifeMax + 20 < 200)
		{
			player.statLifeMax += 20;
		}
		else
		{
			if (player.statLifeMax >= 200)
			{
				return false;
			}
			player.statLifeMax = 200;
		}
		base.Item.stack--;
		return true;
	}
}
