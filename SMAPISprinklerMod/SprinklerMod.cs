using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;

namespace BetterSprinklers
{
    public class SprinklerMod : Mod
    {
        /*********
        ** Properties
        *********/
        /// <summary>The maximum grid size.</summary>
        private readonly int MaxGridSize = 19;

        private int[] validSprinklers;

        private static IModHelper Helper;
        private static Dictionary<string, string> oldCraftingRecipes;
        private static Dictionary<int, string> oldObjectInfo;
        private static Texture2D buildingPlacementTiles;
        private static int[,] scarecrowGrid;
        private static bool gridKeyHeldDown;


        /*********
        ** Accessors
        *********/
        public static SprinklerModConfig ModConfig { get; private set; }
        public static bool extraInfoActive; //deliberately public, so other mods can read it.


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            SprinklerMod.Helper = helper;
            ModConfig = helper.ReadConfig<SprinklerModConfig>();
            oldCraftingRecipes = null;
            oldObjectInfo = null;
            extraInfoActive = false;

            TimeEvents.DayOfMonthChanged += Event_ChangedDayOfMonth;
            GameEvents.LoadContent += Event_LoadContent;
            GameEvents.UpdateTick += Event_UpdateTick;
            GraphicsEvents.OnPreRenderHudEvent += Event_PreRenderHud;

            scarecrowGrid = new int[this.MaxGridSize, this.MaxGridSize];
            int scarecrowCenterValue = this.MaxGridSize / 2;
            Vector2 scarecrowCenter = new Vector2(scarecrowCenterValue, scarecrowCenterValue);
            for (int x = 0; x < this.MaxGridSize; x++)
            {
                for (int y = 0; y < this.MaxGridSize; y++)
                {
                    Vector2 vector = new Vector2(x, y);
                    scarecrowGrid[x, y] = Vector2.Distance(vector, scarecrowCenter) < 9f ? 1 : 0;
                }
            }
        }

        public static void UpdatePrices()
        {
            if (oldCraftingRecipes == null)
            {
                oldCraftingRecipes = CraftingRecipe.craftingRecipes;
                oldObjectInfo = Game1.objectInformation;
            }
            else
            {
                CraftingRecipe.craftingRecipes = oldCraftingRecipes;
                Game1.objectInformation = oldObjectInfo;
            }

            var newCraftingRecipes = new Dictionary<string, string>();
            string[] infoSplit;
            foreach (KeyValuePair<string, string> craftingRecipe in CraftingRecipe.craftingRecipes)
            {
                if (craftingRecipe.Key.Contains("prinkler"))
                {
                    infoSplit = craftingRecipe.Value.Split('/');
                    int sprinklerSheet = int.Parse(infoSplit[2]);
                    int multiplier = ModConfig.SprinklerPrices[sprinklerSheet];
                    string[] ingredientsSplit = infoSplit[0].Split(' ');
                    for (int i = 1; i < ingredientsSplit.Length; i += 2)
                        ingredientsSplit[i] = (int.Parse(ingredientsSplit[i]) * multiplier).ToString();
                    infoSplit[0] = string.Join(" ", ingredientsSplit);
                    newCraftingRecipes[craftingRecipe.Key] = string.Join("/", infoSplit);
                }
                else
                    newCraftingRecipes[craftingRecipe.Key] = craftingRecipe.Value;
            }

            Dictionary<int, string> newObjectInfo = new Dictionary<int, string>();
            foreach (KeyValuePair<int, string> objectInfo in Game1.objectInformation)
            {
                if (ModConfig.SprinklerPrices.ContainsKey(objectInfo.Key))
                {
                    int multiplier = ModConfig.SprinklerPrices[objectInfo.Key];
                    infoSplit = objectInfo.Value.Split('/');
                    infoSplit[1] = (int.Parse(infoSplit[1]) * multiplier).ToString();
                    newObjectInfo[objectInfo.Key] = string.Join("/", infoSplit);
                }
                else
                    newObjectInfo[objectInfo.Key] = objectInfo.Value;
            }

            CraftingRecipe.craftingRecipes = newCraftingRecipes;
            Game1.objectInformation = newObjectInfo;
        }

        /// <summary>Save the current configuration settings.</summary>
        internal static void SaveConfig()
        {
            SprinklerMod.Helper.WriteConfig(SprinklerMod.ModConfig);
        }


        /*********
        ** Private methods
        *********/
        private static void RenderSprinklerHighlight(int sprinklerID, Vector2 centerTile)
        {
            SprinklerMod.RenderHighlight(centerTile, ModConfig.SprinklerShapes[sprinklerID]);
        }

        private static void RenderScarecrowHighlight(Vector2 centerTile)
        {
            SprinklerMod.RenderHighlight(centerTile, scarecrowGrid);
        }

        private static void RenderHighlight(Vector2 centerTile, int[,] grid)
        {
            SprinklerMod.ForGridTiles(centerTile, grid, (tilePos, gridPos) =>
            {
                if (grid[(int)gridPos.X, (int)gridPos.Y] > 0)
                    Game1.spriteBatch.Draw(buildingPlacementTiles, Game1.GlobalToLocal(Game1.viewport, tilePos * Game1.tileSize), Game1.getSourceRectForStandardTileSheet(buildingPlacementTiles, 0), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.999f);
            });
        }

        private static void RenderGrid()
        {
            int startingX = -Game1.viewport.X % Game1.tileSize;
            float startingY = -(float)Game1.viewport.Y % Game1.tileSize;
            for (int x = startingX; x < Game1.graphics.GraphicsDevice.Viewport.Width; x += Game1.tileSize)
            {
                Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(x, (int)startingY, 1, Game1.graphics.GraphicsDevice.Viewport.Height), ModConfig.GridColour);
            }
            for (float y = startingY; y < Game1.graphics.GraphicsDevice.Viewport.Height; y += Game1.tileSize)
            {
                Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(startingX, (int)y, Game1.graphics.GraphicsDevice.Viewport.Width, 1), ModConfig.GridColour);
            }
        }

        private static void Event_PreRenderHud(object sender, EventArgs e)
        {
            if (buildingPlacementTiles == null) buildingPlacementTiles = Game1.content.Load<Texture2D>("LooseSprites\\buildingPlacementTiles");

            if (Game1.activeClickableMenu == null && Game1.CurrentEvent == null && Game1.gameMode == Game1.playingGameMode)
            {
                Vector2 mousePositionTile = new Vector2((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize, (Game1.viewport.Y + Game1.getOldMouseY()) / Game1.tileSize);

                if (Game1.player.ActiveObject != null)
                {
                    if (ModConfig.SprinklerShapes.ContainsKey(Game1.player.ActiveObject.parentSheetIndex))
                    {
                        RenderSprinklerHighlight(Game1.player.ActiveObject.parentSheetIndex, mousePositionTile);
                        return; //don't want mess with people trying to place things.
                    }

                    if (Game1.player.ActiveObject.bigCraftable && Game1.player.ActiveObject.Name.Contains("arecrow"))
                    {
                        RenderScarecrowHighlight(mousePositionTile);
                        return; //don't want to mess with people trying to place things.
                    }
                }

                if (extraInfoActive)
                {
                    if (Game1.currentLocation.objects.ContainsKey(mousePositionTile))
                    {
                        SObject hoveredObject = Game1.currentLocation.objects[mousePositionTile];

                        if (ModConfig.SprinklerShapes.ContainsKey(hoveredObject.parentSheetIndex))
                        {
                            RenderSprinklerHighlight(hoveredObject.parentSheetIndex, mousePositionTile);
                        }

                        if (hoveredObject.bigCraftable && hoveredObject.Name.Contains("arecrow"))
                        {
                            RenderScarecrowHighlight(mousePositionTile);
                        }
                    }

                    RenderGrid();
                }
            }


        }

        private static void Event_ChangedDayOfMonth(object sender, EventArgs e)
        {
            foreach (GameLocation location in Game1.locations)
            {
                foreach (KeyValuePair<Vector2, SObject> objectPair in location.objects)
                {
                    int targetID = objectPair.Value.parentSheetIndex;
                    Vector2 targetTile = objectPair.Key;
                    if (ModConfig.SprinklerShapes.ContainsKey(targetID))
                    {
                        int[,] grid = ModConfig.SprinklerShapes[targetID];
                        SprinklerMod.ForGridTiles(targetTile, grid, (tilePos, gridPos) =>
                        {
                            if (grid[(int)gridPos.X, (int)gridPos.Y] > 0 && location.terrainFeatures.TryGetValue(tilePos, out TerrainFeature terrainFeature) && terrainFeature is HoeDirt dirt)
                                dirt.state = 1;
                        });
                    }
                }
            }
        }

        private static void Event_LoadContent(object sender, EventArgs e)
        {
            UpdatePrices();
        }

        private static void Event_UpdateTick(object sender, EventArgs e)
        {
            if (Game1.activeClickableMenu == null && Game1.CurrentEvent == null)
            {
                KeyboardState currentKeyboardState = Keyboard.GetState();
                if (currentKeyboardState.IsKeyDown(ModConfig.ConfigKey))
                {
                    Game1.activeClickableMenu = new SprinklerShapeEditMenu();
                }

                if (currentKeyboardState.IsKeyDown(ModConfig.HighlightKey))
                {
                    if (gridKeyHeldDown == false) extraInfoActive = !extraInfoActive;
                    gridKeyHeldDown = true;
                }
                else
                {
                    gridKeyHeldDown = false;
                }
            }
        }

        /// <summary>Get a tile grid centered on the given tile position.</summary>
        /// <param name="centerTile">The center tile position.</param>
        /// <param name="grid">The grid to get.</param>
        /// <param name="perform">The action to perform for each tile, given the tile position and grid position.</param>
        private static void ForGridTiles(Vector2 centerTile, int[,] grid, Action<Vector2, Vector2> perform)
        {
            int arrayHalfSizeX = grid.GetLength(0) / 2;
            int arrayHalfSizeY = grid.GetLength(1) / 2;
            int minX = (int)centerTile.X - arrayHalfSizeX;
            int minY = (int)centerTile.Y - arrayHalfSizeY;
            int maxX = (int)centerTile.X + arrayHalfSizeX;
            int maxY = (int)centerTile.Y + arrayHalfSizeY;

            for (int gridX = 0, x = minX; x <= maxX; x++, gridX++)
            {
                for (int gridY = 0, y = minY; y <= maxY; y++, gridY++)
                    perform(new Vector2(x, y), new Vector2(gridX, gridY));
            }
        }
    }
}
