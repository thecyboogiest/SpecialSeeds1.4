using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Specialseeds1point4
{
    public class SpecialseedsItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public override GlobalItem Clone(Item item, Item itemClone)
        {
            return base.Clone(item, itemClone);
        }

        public string worldSeed = Main.ActiveWorldFileData.Name;

        public override void SetDefaults(Item item)
        {
            worldSeed = Main.ActiveWorldFileData.Name;
        }




        public override void UseStyle(Item item, Player player, Rectangle heldItemFrame)
        {
            if (item.type == ItemID.LifeCrystal)
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
}
