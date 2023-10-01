using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using AdvancedWorldGen.Base;
using AdvancedWorldGen.BetterVanillaWorldGen.Interface;
using AdvancedWorldGen.UI;
using JSubClasses.UI;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Logs;
using ReLogic.Content;
using Specialseeds1point4.Tiles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Generation;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.ModLoader.UI.Elements;
using Terraria.ObjectData;
using Terraria.UI;
using Terraria.WorldBuilding;

namespace JSubClasses.UI
{
    // This ExampleUIHoverImageButton class inherits from UIImageButton. 
    // Inheriting is a great tool for UI design. 
    // By inheriting, we get the Image drawing, MouseOver sound, and fading for free from UIImageButton
    // We've added some code to allow the Button to show a text tooltip while hovered
    public class UIHoverImageButton : UIImageButton
	{
        // Tooltip text that will be shown on hover
        public Texture2D tex;
        public bool selected = false;

        public UIHoverImageButton(Asset<Texture2D> texture) : base(texture) {
			tex = texture.Value;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch) {

            CalculatedStyle dimensions = GetDimensions();
           
            spriteBatch.Draw(tex, dimensions.Position(), new Rectangle(162,54, 16, 16), Color.White * (!selected ? (base.IsMouseHovering ? 0.5f : 0.2f) : 1f));
            

        }
	}
}
