using AdvancedWorldGen.Base;
using AdvancedWorldGen.BetterVanillaWorldGen.Interface;
using AdvancedWorldGen.UI;
using Microsoft.Xna.Framework;
using Specialseeds1point4.UI;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using log4net.Repository.Hierarchy;
using System.Linq;
using Terraria.DataStructures;
using System.IO;

namespace Specialseeds1point4;
public class Seeds
{
    public static List<string> activatedSeeds = new List<string>();


    public const string Icemania = "Icemania";
    public const string Junglemania = "Junglemania";
    public const string Desertmania = "Desertmania";
    public const string Meteormania = "Meteormania";
    public const string RainyDay = "RainyDay";
    public const string Cavemania = "Cavemania";
    public const string Nuked = "Nuked";
    public const string Trapmania = "Trapmania";
    public const string Woodlands = "Woodlands";
    public const string Upside = "Upside";

    public static bool Enabled(string seed)
    {
        return activatedSeeds.Contains(seed);
    }

    public static List<string> All()
    {
        return new List<string>() {Icemania,Junglemania,Desertmania,Meteormania,RainyDay,Cavemania,Nuked,Trapmania,Woodlands,Upside };
    }
}
public class Specialseeds1point4 : Mod
{
    public static List<CustomSeedTile> allTiles = new();
    public static List<CustomSeedTile> validTiles = new();

    public static List<CustomLayer> layers = new List<CustomLayer>();

    public static int[] TileIDs = { 0, 1, 2, 23, 25, 37, 38, 39, 45, 46, 47, 51, 53, 54, 56, 57, 58, 59, 60, 75, 76, 80, 109, 112, 116, 117, 118, 119, 121, 122, 123, 140, 148, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 170, 175, 176, 177, 179, 180, 181, 182, 183, 189, 191, 196, 198, 199, 202, 203, 206, 208, 224, 225, 226, 234, 248, 250, 253, 262, 263, 264, 265, 266, 267, 268, 273, 284, 311, 312, 313, 315, 321, 322, 325, 326, 327, 328, 336, 340, 341, 342, 343, 344, 345, 346, 347, 348, 367, 368, 370, 379, 381, 383, 396, 397, 404, 460, 472, 473, 478, 479, 495, 500, 501, 502, 503, 534, 536, 539, 546, 571, };


    

    public CustomSeedUI seedUI = null!;

   
    public override void HandlePacket(BinaryReader reader, int whoAmI)
    {
        int count = reader.ReadInt32();
        for (int i = 0; i < count; i++)
            Seeds.activatedSeeds.Add(reader.ReadString());
    }

    public override void PostSetupContent()
    {
        Mod awg = ModLoader.GetMod("AdvancedWorldGen");
        foreach(string seed in Seeds.All())
            awg.Call("Register Option", "SpecialSeeds", seed);

    }


    public override void Load()
    {

        for (int t = 0; t < TileID.Count; t++)
            allTiles.Add(new CustomSeedTile(Main.Assets.Request<Texture2D>("Images/Tiles_" + t), t));

        foreach (CustomSeedTile tile in allTiles)
            for (int i = 0; i < TileIDs.Count(); i++)
                if (tile.id == TileIDs[i])
                    validTiles.Add(tile);



        Logger.Debug(validTiles.Count);

        On_UIWorldCreation.AddDescriptionPanel += TweakWorldGenUI;
    }


    public static List<CustomSeedTile> SelectedTiles(List<CustomSeedTile> tiles)
    {
        List<CustomSeedTile> selected = new List<CustomSeedTile>();
        selected.Clear();
        foreach (CustomSeedTile tile in tiles)
            if (tile.selected)
                selected.Add(tile);
        

        return selected;
    }

    public void TweakWorldGenUI(On_UIWorldCreation.orig_AddDescriptionPanel origAddDescriptionPanel,UIWorldCreation self, UIElement container, float accumulatedHeight, string tagGroup)
    {
        origAddDescriptionPanel(self, container, accumulatedHeight, tagGroup);

        UICharacterNameButton characterNameButton = VanillaInterface.SeedPlate(self).Value;
        characterNameButton.Width.Pixels -= 48;

        GroupOptionButton<bool> groupOptionButton = new(true, null, null, Color.White, null)
        {
            Width = new StyleDimension(40f, 0f),
            Height = new StyleDimension(40f, 0f),
            Top = characterNameButton.Top,
            Left = new StyleDimension(-176f, 1f),
            ShowHighlightWhenSelected = false,
            PaddingTop = 4f,
            PaddingLeft = 4f
        };

        VanillaInterface.IconTexture(groupOptionButton).Value = Assets.Request<Texture2D>($"UI/CustomTree");

        groupOptionButton.OnLeftClick += ToCustomMenu;

        container.Append(groupOptionButton);
        seedUI = new CustomSeedUI(self, null, this);

    }


    public void ToCustomMenu(UIMouseEvent evt, UIElement listeningElement)
    {
        SoundEngine.PlaySound(SoundID.MenuOpen);
        Main.MenuUI.SetState(seedUI);
    }
}
