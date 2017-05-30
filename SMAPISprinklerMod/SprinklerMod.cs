using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

        private Dictionary<string, string> OldCraftingRecipes;
        private Dictionary<int, string> OldObjectInfo;
        private Texture2D BuildingPlacementTiles;
        private int[,] ScarecrowGrid;
        private bool GridKeyHeldDown;

        private SprinklerModConfig Config;
        private bool ExtraInfoActive;


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
            this.ScarecrowGrid = this.GetScarecrowGrid();
            this.BuildingPlacementTiles = Game1.content.Load<Texture2D>("LooseSprites\\buildingPlacementTiles");

            // set up events
            TimeEvents.DayOfMonthChanged += Event_ChangedDayOfMonth;
            GameEvents.LoadContent += Event_LoadContent;
            GameEvents.UpdateTick += Event_UpdateTick;
            GraphicsEvents.OnPreRenderHudEvent += Event_PreRenderHud;
        }


        /*********
        ** Private methods
        *********/
        /****
        ** Event handlers
        ****/
        /// <summary>The method called before the game draws the HUD.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void Event_PreRenderHud(object sender, EventArgs e)
        {
            if (Context.IsWorldReady && Game1.activeClickableMenu == null && Game1.CurrentEvent == null)
                this.RenderHighlight();
        }

        /// <summary>The method called when the day-of-month value changes.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void Event_ChangedDayOfMonth(object sender, EventArgs e)
        {
            this.RunSprinklers();
        }

        /// <summary>The method called when the game is loaded.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void Event_LoadContent(object sender, EventArgs e)
        {
            this.UpdatePrices();
        }

        /// <summary>The method after the game updates its state (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void Event_UpdateTick(object sender, EventArgs e)
        {
            if (Game1.activeClickableMenu == null && Game1.CurrentEvent == null)
            {
                KeyboardState currentKeyboardState = Keyboard.GetState();
                if (currentKeyboardState.IsKeyDown(this.Config.ConfigKey))
                    Game1.activeClickableMenu = new SprinklerShapeEditMenu(this.Config, this.SaveChanges);
                else if (currentKeyboardState.IsKeyDown(this.Config.HighlightKey))
                {
                    if (this.GridKeyHeldDown == false)
                        this.ExtraInfoActive = !this.ExtraInfoActive;
                    this.GridKeyHeldDown = true;
                }
                else
                    this.GridKeyHeldDown = false;
            }
        }

        /****
        ** Methods
        ****/
        /// <summary>Get the scarecrow layout grid.</summary>
        private int[,] GetScarecrowGrid()
        {
            int[,] grid = new int[this.MaxGridSize, this.MaxGridSize];
            int scarecrowCenterValue = this.MaxGridSize / 2;
            Vector2 scarecrowCenter = new Vector2(scarecrowCenterValue, scarecrowCenterValue);
            for (int x = 0; x < this.MaxGridSize; x++)
            {
                for (int y = 0; y < this.MaxGridSize; y++)
                {
                    Vector2 vector = new Vector2(x, y);
                    grid[x, y] = Vector2.Distance(vector, scarecrowCenter) < 9f ? 1 : 0;
                }
            }
            return grid;
        }

        /// <summary>Save changes to the mod configuration.</summary>
        private void SaveChanges()
        {
            this.Helper.WriteConfig(this.Config);
            Game1.addHUDMessage(new HUDMessage("Sprinkler Configurations Saved", Color.Green, 3500f));
            this.UpdatePrices();
        }

        /// <summary>Update the sprinkler crafting costs based on the current mod configuration.</summary>
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
                    int multiplier = this.Config.SprinklerPrices[sprinklerSheet];
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
                if (this.Config.SprinklerPrices.ContainsKey(objectInfo.Key))
                {
                    int multiplier = this.Config.SprinklerPrices[objectInfo.Key];
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

        /// <summary>Run all sprinklers.</summary>
        private void RunSprinklers()
        {
            foreach (GameLocation location in Game1.locations)
            {
                foreach (KeyValuePair<Vector2, SObject> objectPair in location.objects)
                {
                    int targetID = objectPair.Value.parentSheetIndex;
                    Vector2 targetTile = objectPair.Key;
                    if (this.Config.SprinklerShapes.ContainsKey(targetID))
                    {
                        int[,] grid = this.Config.SprinklerShapes[targetID];
                        this.ForGridTiles(targetTile, grid, (tilePos, gridPos) =>
                        {
                            if (grid[(int)gridPos.X, (int)gridPos.Y] > 0 && location.terrainFeatures.TryGetValue(tilePos, out TerrainFeature terrainFeature) && terrainFeature is HoeDirt dirt)
                                dirt.state = 1;
                        });
                    }
                }
            }
        }

        /// <summary>Highlight coverage for sprinklers and scarecrows based on the current context.</summary>
        [SuppressMessage("ReSharper", "PossibleLossOfFraction", Justification = "The decimals are deliberately truncated during conversion to tile coordinates.")]
        private void RenderHighlight()
        {
            // get context
            Vector2 mouseTile = new Vector2((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize, (Game1.viewport.Y + Game1.getOldMouseY()) / Game1.tileSize);
            SObject heldItem = Game1.player.ActiveObject;

            // highlight coverage for held item
            if (heldItem != null)
            {
                if (this.Config.SprinklerShapes.ContainsKey(heldItem.parentSheetIndex))
                {
                    this.RenderSprinklerHighlight(heldItem.parentSheetIndex, mouseTile);
                    return;
                }
                if (heldItem.bigCraftable && heldItem.Name.Contains("arecrow"))
                {
                    this.RenderScarecrowHighlight(mouseTile);
                    return;
                }
            }

            // highlight coverage for item under cursor
            if (this.ExtraInfoActive)
            {
                if (Game1.currentLocation.objects.TryGetValue(mouseTile, out SObject target))
                {
                    if (this.Config.SprinklerShapes.ContainsKey(target.parentSheetIndex))
                        this.RenderSprinklerHighlight(target.parentSheetIndex, mouseTile);
                    else if (target.bigCraftable && target.Name.Contains("arecrow"))
                        this.RenderScarecrowHighlight(mouseTile);
                }
                this.RenderGrid();
            }
        }

        /// <summary>Highlight coverage for a sprinkler.</summary>
        /// <param name="sprinklerID">The sprinkler ID.</param>
        /// <param name="tile">The sprinkler tile.</param>
        private void RenderSprinklerHighlight(int sprinklerID, Vector2 tile)
        {
            this.RenderHighlight(tile, this.Config.SprinklerShapes[sprinklerID]);
        }

        /// <summary>Highlight coverage for a scarecrow.</summary>
        /// <param name="tile">The scarecrow tile.</param>
        private void RenderScarecrowHighlight(Vector2 tile)
        {
            this.RenderHighlight(tile, this.ScarecrowGrid);
        }

        /// <summary>Highlight coverage based on a given grid.</summary>
        /// <param name="centerTile">The tile at the center of the grid.</param>
        /// <param name="grid">The grid indicating which tiles to highlight.</param>
        private void RenderHighlight(Vector2 centerTile, int[,] grid)
        {
            this.ForGridTiles(centerTile, grid, (tilePos, gridPos) =>
            {
                if (grid[(int)gridPos.X, (int)gridPos.Y] > 0)
                    Game1.spriteBatch.Draw(this.BuildingPlacementTiles, Game1.GlobalToLocal(Game1.viewport, tilePos * Game1.tileSize), Game1.getSourceRectForStandardTileSheet(this.BuildingPlacementTiles, 0), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.999f);
            });
        }

        /// <summary>Render a grid showing the tile layout.</summary>
        private void RenderGrid()
        {
            int startingX = -Game1.viewport.X % Game1.tileSize;
            float startingY = -(float)Game1.viewport.Y % Game1.tileSize;
            for (int x = startingX; x < Game1.graphics.GraphicsDevice.Viewport.Width; x += Game1.tileSize)
                Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(x, (int)startingY, 1, Game1.graphics.GraphicsDevice.Viewport.Height), this.Config.GridColour);
            for (float y = startingY; y < Game1.graphics.GraphicsDevice.Viewport.Height; y += Game1.tileSize)
                Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(startingX, (int)y, Game1.graphics.GraphicsDevice.Viewport.Width, 1), this.Config.GridColour);
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
