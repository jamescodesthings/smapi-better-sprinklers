using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        /// <summary>The mod configuration.</summary>
        private SprinklerModConfig Config;

        private Dictionary<string, string> OldCraftingRecipes;
        private Dictionary<int, string> OldObjectInfo;
        private Texture2D BuildingPlacementTiles;
        private int[,] ScarecrowGrid;

        /// <summary>Whether to show a grid overlay and highlight the coverage for sprinklers or scarecrows under the cursor.</summary>
        private bool ShowInfoOverlay;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // initialise
            this.Config = helper.ReadConfig<SprinklerModConfig>();
            this.OldCraftingRecipes = new Dictionary<string, string>(CraftingRecipe.craftingRecipes);
            this.OldObjectInfo = new Dictionary<int, string>(Game1.objectInformation);
            this.ScarecrowGrid = this.GetScarecrowGrid();
            this.BuildingPlacementTiles = Game1.content.Load<Texture2D>("LooseSprites\\buildingPlacementTiles");
            this.UpdatePrices();

            // set up events
            TimeEvents.AfterDayStarted += this.Event_AfterDayStarted;
            GraphicsEvents.OnPreRenderHudEvent += this.Event_PreRenderHud;
            ControlEvents.KeyPressed += this.Event_OnKeyPressed;
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

        /// <summary>The method called when a new day starts.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void Event_AfterDayStarted(object sender, EventArgs e)
        {
            this.RunSprinklers();
        }

        /// <summary>The method after the game updates its state (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void Event_OnKeyPressed(object sender, EventArgsKeyPressed e)
        {
            if (Game1.activeClickableMenu != null || Game1.CurrentEvent != null)
                return;

            // show menu
            if (e.KeyPressed == this.Config.ConfigKey)
                Game1.activeClickableMenu = new SprinklerShapeEditMenu(this.Config, this.SaveChanges);

            // toggle overlay
            else if (e.KeyPressed == this.Config.HighlightKey)
                this.ShowInfoOverlay = !this.ShowInfoOverlay;
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
            // reset game data
            CraftingRecipe.craftingRecipes = new Dictionary<string, string>(this.OldCraftingRecipes);
            Game1.objectInformation = new Dictionary<int, string>(this.OldObjectInfo);

            // recalculate sprinkler crafting resources
            foreach (string recipeKey in CraftingRecipe.craftingRecipes.Keys.ToArray())
            {
                if (!recipeKey.Contains("prinkler"))
                    continue;

                // multiply resource costs
                string[] fields = CraftingRecipe.craftingRecipes[recipeKey].Split('/');
                int sprinklerID = int.Parse(fields[2]);
                string[] ingredients = fields[0].Split(' ');
                int multiplier = this.Config.SprinklerPrices[sprinklerID];
                for (int i = 1; i < ingredients.Length; i += 2)
                    ingredients[i] = (int.Parse(ingredients[i]) * multiplier).ToString();
                fields[0] = string.Join(" ", ingredients);

                // update game data
                CraftingRecipe.craftingRecipes[recipeKey] = string.Join("/", fields);
            }

            // recalculate sale price
            foreach (int itemID in Game1.objectInformation.Keys.ToArray())
            {
                if (!this.Config.SprinklerPrices.ContainsKey(itemID))
                    continue;

                // multiply cost
                int multiplier = this.Config.SprinklerPrices[itemID];
                string[] fields = Game1.objectInformation[itemID].Split('/');
                fields[1] = (int.Parse(fields[1]) * multiplier).ToString();

                // update game data
                Game1.objectInformation[itemID] = string.Join("/", fields);
            }
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
            if (this.ShowInfoOverlay)
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
