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

        private int[] ValidSprinklers;

        private Dictionary<string, string> OldCraftingRecipes;
        private Dictionary<int, string> OldObjectInfo;
        private Texture2D BuildingPlacementTiles;
        private int[,] ScarecrowGrid;
        private bool GridKeyHeldDown;

        private SprinklerModConfig Config;
        private bool ExtraInfoActive; //deliberately public, so other mods can read it.


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // initialise
            this.Config = helper.ReadConfig<SprinklerModConfig>();
            this.OldCraftingRecipes = null;
            this.OldObjectInfo = null;
            this.ExtraInfoActive = false;

            // set up grids
            this.ScarecrowGrid = new int[this.MaxGridSize, this.MaxGridSize];
            int scarecrowCenterValue = this.MaxGridSize / 2;
            Vector2 scarecrowCenter = new Vector2(scarecrowCenterValue, scarecrowCenterValue);
            for (int x = 0; x < this.MaxGridSize; x++)
            {
                for (int y = 0; y < this.MaxGridSize; y++)
                {
                    Vector2 vector = new Vector2(x, y);
                    this.ScarecrowGrid[x, y] = Vector2.Distance(vector, scarecrowCenter) < 9f ? 1 : 0;
                }
            }

            // set up events
            TimeEvents.DayOfMonthChanged += Event_ChangedDayOfMonth;
            GameEvents.LoadContent += Event_LoadContent;
            GameEvents.UpdateTick += Event_UpdateTick;
            GraphicsEvents.OnPreRenderHudEvent += Event_PreRenderHud;
        }


        /*********
        ** Private methods
        *********/
        private void UpdatePrices()
        {
            if (this.OldCraftingRecipes == null)
            {
                this.OldCraftingRecipes = CraftingRecipe.craftingRecipes;
                OldObjectInfo = Game1.objectInformation;
            }
            else
            {
                CraftingRecipe.craftingRecipes = this.OldCraftingRecipes;
                Game1.objectInformation = OldObjectInfo;
            }

            var newCraftingRecipes = new Dictionary<string, string>();
            string[] infoSplit;
            foreach (KeyValuePair<string, string> craftingRecipe in CraftingRecipe.craftingRecipes)
            {
                if (craftingRecipe.Key.Contains("prinkler"))
                {
                    infoSplit = craftingRecipe.Value.Split('/');
                    int sprinklerSheet = int.Parse(infoSplit[2]);
                    int multiplier = Config.SprinklerPrices[sprinklerSheet];
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
                if (Config.SprinklerPrices.ContainsKey(objectInfo.Key))
                {
                    int multiplier = Config.SprinklerPrices[objectInfo.Key];
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
        private void SaveConfig()
        {
            this.Helper.WriteConfig(this.Config);
        }

        private void RenderSprinklerHighlight(int sprinklerID, Vector2 centerTile)
        {
            this.RenderHighlight(centerTile, this.Config.SprinklerShapes[sprinklerID]);
        }

        private void RenderScarecrowHighlight(Vector2 centerTile)
        {
            this.RenderHighlight(centerTile, this.ScarecrowGrid);
        }

        private void RenderHighlight(Vector2 centerTile, int[,] grid)
        {
            this.ForGridTiles(centerTile, grid, (tilePos, gridPos) =>
            {
                if (grid[(int)gridPos.X, (int)gridPos.Y] > 0)
                    Game1.spriteBatch.Draw(this.BuildingPlacementTiles, Game1.GlobalToLocal(Game1.viewport, tilePos * Game1.tileSize), Game1.getSourceRectForStandardTileSheet(this.BuildingPlacementTiles, 0), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.999f);
            });
        }

        private void RenderGrid()
        {
            int startingX = -Game1.viewport.X % Game1.tileSize;
            float startingY = -(float)Game1.viewport.Y % Game1.tileSize;
            for (int x = startingX; x < Game1.graphics.GraphicsDevice.Viewport.Width; x += Game1.tileSize)
                Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(x, (int)startingY, 1, Game1.graphics.GraphicsDevice.Viewport.Height), Config.GridColour);
            for (float y = startingY; y < Game1.graphics.GraphicsDevice.Viewport.Height; y += Game1.tileSize)
                Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(startingX, (int)y, Game1.graphics.GraphicsDevice.Viewport.Width, 1), Config.GridColour);
        }

        private void Event_PreRenderHud(object sender, EventArgs e)
        {
            if (this.BuildingPlacementTiles == null)
                this.BuildingPlacementTiles = Game1.content.Load<Texture2D>("LooseSprites\\buildingPlacementTiles");

            if (Game1.activeClickableMenu == null && Game1.CurrentEvent == null && Game1.gameMode == Game1.playingGameMode)
            {
                Vector2 mouseTile = new Vector2((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize, (Game1.viewport.Y + Game1.getOldMouseY()) / Game1.tileSize);

                if (Game1.player.ActiveObject != null)
                {
                    if (Config.SprinklerShapes.ContainsKey(Game1.player.ActiveObject.parentSheetIndex))
                    {
                        this.RenderSprinklerHighlight(Game1.player.ActiveObject.parentSheetIndex, mouseTile);
                        return; //don't want mess with people trying to place things.
                    }

                    if (Game1.player.ActiveObject.bigCraftable && Game1.player.ActiveObject.Name.Contains("arecrow"))
                    {
                        this.RenderScarecrowHighlight(mouseTile);
                        return; //don't want to mess with people trying to place things.
                    }
                }

                if (this.ExtraInfoActive)
                {
                    if (Game1.currentLocation.objects.ContainsKey(mouseTile))
                    {
                        SObject target = Game1.currentLocation.objects[mouseTile];
                        if (Config.SprinklerShapes.ContainsKey(target.parentSheetIndex))
                            this.RenderSprinklerHighlight(target.parentSheetIndex, mouseTile);
                        else if (target.bigCraftable && target.Name.Contains("arecrow"))
                            this.RenderScarecrowHighlight(mouseTile);
                    }

                    this.RenderGrid();
                }
            }
        }

        private void Event_ChangedDayOfMonth(object sender, EventArgs e)
        {
            foreach (GameLocation location in Game1.locations)
            {
                foreach (KeyValuePair<Vector2, SObject> objectPair in location.objects)
                {
                    int targetID = objectPair.Value.parentSheetIndex;
                    Vector2 targetTile = objectPair.Key;
                    if (Config.SprinklerShapes.ContainsKey(targetID))
                    {
                        int[,] grid = Config.SprinklerShapes[targetID];
                        this.ForGridTiles(targetTile, grid, (tilePos, gridPos) =>
                        {
                            if (grid[(int)gridPos.X, (int)gridPos.Y] > 0 && location.terrainFeatures.TryGetValue(tilePos, out TerrainFeature terrainFeature) && terrainFeature is HoeDirt dirt)
                                dirt.state = 1;
                        });
                    }
                }
            }
        }

        private void Event_LoadContent(object sender, EventArgs e)
        {
            this.UpdatePrices();
        }

        private void Event_UpdateTick(object sender, EventArgs e)
        {
            if (Game1.activeClickableMenu == null && Game1.CurrentEvent == null)
            {
                KeyboardState currentKeyboardState = Keyboard.GetState();
                if (currentKeyboardState.IsKeyDown(this.Config.ConfigKey))
                {
                    Game1.activeClickableMenu = new SprinklerShapeEditMenu(this.Config, () =>
                    {
                        this.SaveConfig();
                        Game1.addHUDMessage(new HUDMessage("Sprinkler Configurations Saved", Color.Green, 3500f));
                        this.UpdatePrices();
                    });
                }

                if (currentKeyboardState.IsKeyDown(this.Config.HighlightKey))
                {
                    if (GridKeyHeldDown == false)
                        this.ExtraInfoActive = !this.ExtraInfoActive;
                    this.GridKeyHeldDown = true;
                }
                else
                    this.GridKeyHeldDown = false;
            }
        }

        /// <summary>Get a tile grid centered on the given tile position.</summary>
        /// <param name="centerTile">The center tile position.</param>
        /// <param name="grid">The grid to get.</param>
        /// <param name="perform">The action to perform for each tile, given the tile position and grid position.</param>
        private void ForGridTiles(Vector2 centerTile, int[,] grid, Action<Vector2, Vector2> perform)
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
