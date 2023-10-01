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

namespace Specialseeds1point4.UI
{
    public class CustomSeedTile
    {
        public ReLogic.Content.Asset<Texture2D> texture;
        public int id;
        public bool selected;
        public int myLayer;
        public UIHoverImageButton myButton;

        public CustomSeedTile(Asset<Texture2D> texture, int id)
        {
            this.texture = texture;
            this.id = id;
   
        }
        public CustomSeedTile(CustomSeedTile tile)
        {
            this.texture = tile.texture;
            this.id = tile.id;

        }
    }

    public class CustomLayer
    {
        
        public List<CustomSeedTile> tiles = new();
        

        public CustomLayer()
        {
            Specialseeds1point4.layers.Add(this);
            foreach (CustomSeedTile tile in Specialseeds1point4.validTiles)
            {
                CustomSeedTile newTile = new CustomSeedTile(tile);
                newTile.myLayer = Specialseeds1point4.layers.IndexOf(this);
                this.tiles.Add(newTile);
            }
            
        }

        public void EnableTile(UIMouseEvent evt, UIElement listeningElement)
        {
           

        }
    }

    public class CustomSeedUI : UIState
    {
        private readonly UIState PreviousState;
        private new readonly Option? Parent;
        public Mod mod;

        private UIList layerList;
        private List<UIPanel> panels = new();
        private UIText hoverText;
        public CustomSeedUI(UIState previousState, Option? parent, Mod mod)
        {
            PreviousState = previousState;
            Parent = parent;
            this.mod = mod;
        }

        public override void OnInitialize()
        {
            UIPanel layerUIPanel = new()
            {
                HAlign = 0.5f,
                VAlign = 0.5f,
                Width = new StyleDimension(0, 0.4f),
                Height = new StyleDimension(0, 0.8f),
                BackgroundColor = UICommon.MainPanelBackground
            };
            Append(layerUIPanel);

            hoverText = new("Tile: None", 0.5f, true) { HAlign = 0.5f };
            hoverText.Height = hoverText.MinHeight;
            layerUIPanel.Append(hoverText);
            layerUIPanel.Append(new UIHorizontalSeparator
            {
                Width = new StyleDimension(0f, 1f),
                Top = new StyleDimension(30f, 0f),
                Color = Color.Lerp(Color.White, new Color(63, 65, 151, 255), 0.85f) * 0.9f
            });
            UIScrollbar uiScrollbar = new()
            {
                Height = new StyleDimension(-110f, 1f),
                Top = new StyleDimension(50, 0f),
                HAlign = 1f
                
            };

            layerList = new UIList
            {
                Height = new StyleDimension(-110f, 1f),
                Width = new StyleDimension(-20f, 1f),
                Top = new StyleDimension(50, 0f)
            };
            layerList.SetScrollbar(uiScrollbar);
            layerUIPanel.Append(uiScrollbar);
            layerUIPanel.Append(layerList);

            UITextPanel<string> newLayer = new("New Layer")
            {
                Width = new StyleDimension(0f, 0.1f),
                Top = new StyleDimension(0f, 0.8f),
                HAlign = 0.50f
            };
            newLayer.OnLeftClick += CreateNewLayer;
            newLayer.OnMouseOver += FadedMouseOver;
            newLayer.OnMouseOut += FadedMouseOut;
            Append(newLayer);

            UITextPanel<string> deleteLayer = new("Delete Layer")
            {
                Width = new StyleDimension(0f, 0.1f),
                Top = new StyleDimension(0f, 0.8f),
                HAlign = 0.65f
            };
            deleteLayer.OnLeftClick += DeleteLayer;
            deleteLayer.OnMouseOver += FadedMouseOver;
            deleteLayer.OnMouseOut += FadedMouseOut;
            Append(deleteLayer);

            UITextPanel<string> goBack = new(Language.GetTextValue("UI.Back"))
            {
                Width = new StyleDimension(0f, 0.1f),
                Top = new StyleDimension(0f, 0.8f),
                HAlign = 0.35f
            };
            goBack.OnLeftClick += GoBack;
            goBack.OnMouseOver += FadedMouseOver;
            goBack.OnMouseOut += FadedMouseOut;
            Append(goBack);


        }
        private void DeleteLayer(UIMouseEvent evt, UIElement listeningElement)
        {
            if(Specialseeds1point4.layers.Count > 0)
            {
                Specialseeds1point4.layers.RemoveAt(Specialseeds1point4.layers.Count - 1);
                UIPanel panel = panels[panels.Count - 1];
                panels.Remove(panel);
                layerList._items.Remove(panel);
                panel.Remove();


            }

        }
        private void CreateNewLayer(UIMouseEvent evt, UIElement listeningElement)
        {
            UIPanel customLayerUI = new();
            SetRectangle(customLayerUI, 0, 0, 500, 500, 0.5f);

            UIText layerTitle = new("Custom Layer " + Specialseeds1point4.layers.Count, 0.75f, true) { HAlign = 0.5f };
            layerTitle.Height = layerTitle.MinHeight;
            customLayerUI.Append(layerTitle);
            customLayerUI.Append(new UIHorizontalSeparator
            {
                Width = new StyleDimension(0f, 1f),
                Top = new StyleDimension(43f, 0f),
                Color = Color.Lerp(Color.White, new Color(63, 65, 151, 255), 0.85f) * 0.9f
            });
            panels.Add(customLayerUI);
            TileOptions(customLayerUI);
            layerList.Add(customLayerUI);
            layerList.Recalculate();


        }

        private void GoBack(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuClose);
            Main.MenuUI.SetState(PreviousState);
        }
        public static void FadedMouseOver(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            UIPanel panel = (UIPanel)evt.Target;
            panel.BackgroundColor = new Color(73, 94, 171);
            panel.BorderColor = Colors.FancyUIFatButtonMouseOver;
        }

        public static void FadedMouseOut(UIMouseEvent evt, UIElement listeningElement)
        {
            UIPanel panel = (UIPanel)evt.Target;
            panel.BackgroundColor = new Color(63, 82, 151) * 0.7f;
            panel.BorderColor = Color.Black;
        }
        private void TileOptions(UIElement uiPanel)
        {
           
            UIGrid grid = new UIGrid
            {
                Height = new StyleDimension(-110f, 1f),
                Width = new StyleDimension(-20f, 1f),
                Top = new StyleDimension(50, 0f)
            };
            
            TileList(grid);
            uiPanel.Append(grid);
            


        }

        public void TileList(UIGrid grid)
        {
            CustomLayer layer = new CustomLayer();

            grid.Clear();
            
            foreach (CustomSeedTile tile in layer.tiles)
            {
                string tileName = TileID.Search.GetName(tile.id);
                UIHoverImageButton tileOptionButton = new UIHoverImageButton(tile.texture); 
                SetRectangle(tileOptionButton, left: 0, top: 0, width: 16, height: 16, 0);

                grid.Add(tileOptionButton);
                tile.myButton = tileOptionButton;
                tileOptionButton.OnLeftClick += delegate
                {
                    mod.Logger.Debug(tile.myLayer);
                    if (Main.MenuUI.CurrentState != this || tile.myLayer != Specialseeds1point4.layers.IndexOf(layer) || tile.myButton != tileOptionButton)
                        return;
                    tile.selected = !tile.selected;
                    tileOptionButton.selected = tile.selected;

                    SoundEngine.PlaySound(SoundID.MenuTick);
                    mod.Logger.Debug(Specialseeds1point4.SelectedTiles(layer.tiles).Count + " // " + Specialseeds1point4.layers.IndexOf(layer));
                };

                tileOptionButton.OnMouseOver += delegate
                {
                    hoverText.SetText("Tile: " + tileName);
                };

            }

        }

       


        private void SetRectangle(UIElement uiElement, float left, float top, float width, float height, float HAlign)
        {
            uiElement.Left.Set(left, 0f);
            uiElement.Top.Set(top, 0f);
            uiElement.Width.Set(width, 0f);
            uiElement.Height.Set(height, 0f);
            uiElement.HAlign = HAlign;
            
        }

    }
}
