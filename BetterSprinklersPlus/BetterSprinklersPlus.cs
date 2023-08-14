using System;
using System.Diagnostics.CodeAnalysis;
using BetterSprinklersPlus.Framework;
using BetterSprinklersPlus.Framework.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;

namespace BetterSprinklersPlus
{
  /// <summary>
  /// Better Sprinklers Plus
  /// </summary>
  // ReSharper disable once UnusedType.Global
  public class BetterSprinklersPlus : Mod
  {
    /// <summary>The mod configuration.</summary>
    private BetterSprinklersPlusConfig Config;

    private Texture2D BuildingPlacementTiles;

    /// <summary>Whether to show a grid overlay and highlight the coverage for sprinklers or scarecrows under the cursor.</summary>
    private bool ShowInfoOverlay;

    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
      Config = BetterSprinklersPlusConfig.ReadConfig(helper);

      BuildingPlacementTiles = Helper.GameContent.Load<Texture2D>("LooseSprites/buildingPlacementTiles");

      SetUpEvents();
    }
      
    /// <summary>
    /// Sets up events
    /// </summary>
    private void SetUpEvents()
    {
      Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
      Helper.Events.GameLoop.DayStarted += OnDayStarted;
      Helper.Events.Display.RenderedWorld += OnRenderedWorld;
      Helper.Events.Input.ButtonPressed += OnButtonPressed;
    }

    /// <summary>Get an API that other mods can access. This is always called after <see cref="Entry" />.</summary>
    public override object GetApi()
    {
      return new BetterSprinklersPlusApi(Config, BetterSprinklersPlusConfig.MaxGridSize);
    }

    private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
    {
      BetterSprinklersPlusConfig.SetupGenericModConfigMenu(Helper, ModManifest, Monitor, () =>
      {
        Config = BetterSprinklersPlusConfig.ReadConfig(Helper);
      });
    }

    /// <summary>Raised after drawing the world.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnRenderedWorld(object sender, RenderedWorldEventArgs e)
    {
        RenderHighlight();
    }

    /// <summary>Raised after the game begins a new day (including when the player loads a save).</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnDayStarted(object sender, DayStartedEventArgs e)
    {
      RunSprinklers();
    }

    /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
    {
      if (Game1.activeClickableMenu != null || Game1.CurrentEvent != null)
        return;

      if (e.Button == Config.ShowSprinklerEditKey)
      {
        ShowSprinklerEditMenu();
        return;
      }

      if (e.Button == Config.ShowOverlayKey)
      {
        ToggleOverlay();
      }
    }

    private void ShowSprinklerEditMenu()
    {
      Game1.activeClickableMenu = new SprinklerShapeEditMenu(Config, () =>
      {
        BetterSprinklersPlusConfig.SaveChanges(Helper, Config);
      });
    }
    
    private void ToggleOverlay()
    {
      ShowInfoOverlay = !ShowInfoOverlay;
    }

    /// <summary>Run all sprinklers.</summary>
    private void RunSprinklers()
    {
      Monitor.Log("Running Sprinklers", LogLevel.Info);
      if (Config.BalancedMode == (int)BetterSprinklersPlusConfig.BalancedModeOptions.Off)
      {
        Monitor.VerboseLog("Balanced mode is off, just water");
        if (Config.BalancedModeCostMessage)
        {
          Game1.addHUDMessage(new HUDMessage("Your sprinklers have run.", Color.Green, 5000f));
        }

        WaterAll();
        return;
      }

      Monitor.VerboseLog("Balanced Mode is on, calculating cost");
      var tilesWeCanWater = GetCountOfTilesWeCanWater();
      var affordable = GetTilesWeCanAffordToWater();
      int cost;

      if (tilesWeCanWater > affordable)
      {
        Monitor.VerboseLog(
          $"We can only afford to water {affordable} tiles, but there are {tilesWeCanWater} tiles to water");
        // ReSharper disable once ConvertIfStatementToSwitchStatement
        if (Config.CannotAfford == (int)BetterSprinklersPlusConfig.CannotAffordOptions.DoNotWater)
        {
          Monitor.VerboseLog("Do not water is set, unwatering.");
          UnwaterAll();
          cost = CalculateCost(tilesWeCanWater);
          if (Config.BalancedModeCostMessage || Config.BalancedModeCannotAffordWarning)
          {
            Game1.addHUDMessage(new HUDMessage($"You could not to run your sprinklers today ({cost}G).",
              Color.Green, 5000f));
          }

          return;
        }

        if (Config.CannotAfford == (int)BetterSprinklersPlusConfig.CannotAffordOptions.CutOff)
        {
          cost = CalculateCost(affordable);
          WaterAllWeCanAfford(affordable);
          DeductCost(cost);
          Monitor.VerboseLog($"Sprinklers have run ({cost}G), Could not finish watering.");
          if (Config.BalancedModeCostMessage)
          {
            Game1.addHUDMessage(new HUDMessage(
              $"Your sprinklers have run ({cost}G), Could not finish watering.", Color.Green, 5000f));
          }
          else if (Config.BalancedModeCannotAffordWarning)
          {
            Game1.addHUDMessage(new HUDMessage("Could not finish watering.", Color.Green, 5000f));
          }

          return;
        }
      }

      // Otherwise, water everything and deduct cost
      WaterAll();
      cost = CalculateCost(tilesWeCanWater);
      DeductCost(cost);
      Monitor.VerboseLog($"Sprinklers have run ({cost}G).");
      if (Config.BalancedModeCostMessage)
      {
        Game1.addHUDMessage(new HUDMessage($"Your sprinklers have run ({cost}G).", Color.Green, 5000f));
      }
    }
    
    private void WaterAll()
    {
      foreach (var location in LocationHelper.GetAllBuildableLocations())
      {
        foreach (var (tile, sprinkler) in location.AllSprinklers())
        {
          Monitor.VerboseLog($"Processing Sprinkler at {tile.X}x{tile.Y}: {sprinkler.ParentSheetIndex}");
          sprinkler.ForCoveredTiles(Config, tile, t => WaterTile(location, t));
        }
      }
    }

    private void UnwaterAll()
    {
      foreach (var location in LocationHelper.GetAllBuildableLocations())
      {
        foreach (var (tile, sprinkler) in location.AllSprinklers())
        {
          Monitor.VerboseLog($"Processing Sprinkler at {tile.X}x{tile.Y}: {sprinkler.ParentSheetIndex}");
          sprinkler.ForCoveredTiles(Config, tile, t => UnwaterTile(location, t));
        }
      }
    }

    private void WaterAllWeCanAfford(int affordable)
    {
      var current = 0;
      foreach (var location in LocationHelper.GetAllBuildableLocations())
      {
        foreach (var (tile, sprinkler) in location.AllSprinklers())
        {
          sprinkler.ForCoveredTiles(Config, tile, _ =>
          {
            if (current < affordable)
            {
              WaterTile(location, tile);
            }
            else
            {
              UnwaterTile(location, tile);
            }

            current++;
          });
        }
      }
    }

    private void WaterTile(GameLocation location, Vector2 tile)
    {
      SetTileWateredValue(location, tile, 1);
    }

    private void UnwaterTile(GameLocation location, Vector2 tile)
    {
      SetTileWateredValue(location, tile, 0);
    }

    private void SetTileWateredValue(GameLocation location, Vector2 tile, int value)
    {
      if (!location.terrainFeatures.TryGetValue(tile, out var terrainFeature))
      {
        Monitor.VerboseLog($"could not get feature at: {tile.X}x{tile.Y}");
        return;
      }

      if (!terrainFeature.IsDirt()) return;

      var dirt = (HoeDirt)terrainFeature;
      dirt.state.Value = value;
    }
    
    private int GetTilesWeCanAffordToWater()
    {
      var costPerTile = GetCostPerTile();
      var canAfford = costPerTile == 0f ? int.MaxValue : (int)Math.Floor(Game1.player.Money / costPerTile);
      return canAfford;
    }

    private static void DeductCost(int cost)
    {
      if (Game1.player.Money - cost >= 0)
      {
        Game1.player.Money -= cost;
      }
      else
      {
        Game1.player.Money = 0;
      }
    }

    /// <summary>
    /// Calculates the cost of watering x tiles
    /// </summary>
    /// <param name="count">The number of tiles</param>
    /// <returns>The cost in G</returns>
    private int CalculateCost(int count)
    {
      var costPerTile = GetCostPerTile();
      var cost = (int)Math.Round(count * costPerTile);
      return cost;
    }
    
    private float GetCostPerTile()
    {
      try
      {
        return BetterSprinklersPlusConfig.BalancedModeOptionsMultipliers[Config.BalancedMode];
      }
      catch (Exception e)
      {
        Monitor.Log($"Error getting cost per tile {e.Message}", LogLevel.Error);
        return 0f;
      }
    }

    private int GetCountOfTilesWeCanWater()
    {
      var count = 0;
      foreach (var location in LocationHelper.GetAllBuildableLocations())
      {
        foreach (var (tile, sprinkler) in location.AllSprinklers())
        {
          sprinkler.ForCoveredTiles(Config, tile, _ =>
          {
            if (Config.BalancedModeCostsMoneyOnAnyTile)
            {
              count++;
            }
            else if (CanWaterTile(location, tile))
            {
              count++;
            }
          });
        }
      }

      return count;
    }

    private bool CanWaterTile(GameLocation location, Vector2 tile)
    {
      if (!location.terrainFeatures.TryGetValue(tile, out var terrainFeature))
      {
        Monitor.VerboseLog($"could not get feature at: {tile.X}x{tile.Y}");
        return false;
      }

      return terrainFeature.IsDirt();
    }

    /// <summary>Highlight coverage for sprinklers and scarecrows based on the current context.</summary>
    [SuppressMessage("ReSharper", "PossibleLossOfFraction",
      Justification = "The decimals are deliberately truncated during conversion to tile coordinates.")]
    private void RenderHighlight()
    {
      if (!(Context.IsWorldReady && Game1.activeClickableMenu == null && Game1.CurrentEvent == null))
      {
        return;
      }

      var cursorPos = Helper.Input.GetCursorPosition();
      var mouseTile = new Vector2((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize,
        (Game1.viewport.Y + Game1.getOldMouseY()) / Game1.tileSize);

      HighlightCoverageForHeldObject();
      HighlightCoverageForObjectUnderCursor(mouseTile, cursorPos);
    }

    private void HighlightCoverageForHeldObject()
    {
      if (!Config.OverlayEnabledOnPlace) return;
      
      var heldObject = Game1.player.ActiveObject;
      if (heldObject == null) return;
        
      // accounts for controller mode
      var tileToHighlightFrom = PlacementHelper.GetPlacementPosition();
      if (tileToHighlightFrom == null) return;

      if (SprinklerHelper.IsSprinkler(heldObject))
      {
        RenderSprinklerHighlight(heldObject.ParentSheetIndex, (Vector2)tileToHighlightFrom);
      }

      if (ScarecrowHelper.IsScarecrow(heldObject))
      {
        RenderScarecrowHighlight((Vector2)tileToHighlightFrom);
      }
    }

    private void HighlightCoverageForObjectUnderCursor(Vector2 mouseTile, ICursorPosition cursorPos)
    {
      if (!ShowInfoOverlay) return;
      
      if (Game1.currentLocation.objects.TryGetValue(mouseTile, out var objUnderMouse))
      {
        if (SprinklerHelper.IsSprinkler(objUnderMouse)){
          RenderSprinklerHighlight(objUnderMouse.ParentSheetIndex, cursorPos.Tile);
        }
        else if (ScarecrowHelper.IsScarecrow(objUnderMouse))
        {
          RenderScarecrowHighlight(cursorPos.Tile);
        }
      }

      if (this.Config.GridColour == Color.Transparent) return;
      
      RenderGrid();
    }

    /// <summary>Highlight coverage for a sprinkler.</summary>
    /// <param name="sprinklerId">The sprinkler ID.</param>
    /// <param name="tile">The sprinkler tile.</param>
    private void RenderSprinklerHighlight(int sprinklerId, Vector2 tile)
    {
      RenderHighlight(tile, Config.SprinklerShapes[sprinklerId]);
    }

    /// <summary>Highlight coverage for a scarecrow.</summary>
    /// <param name="tile">The scarecrow tile.</param>
    private void RenderScarecrowHighlight(Vector2 tile)
    {
      var scarecrowGrid = ScarecrowHelper.GetScarecrowGrid();
      RenderHighlight(tile, scarecrowGrid);
    }

    /// <summary>Highlight coverage based on a given grid.</summary>
    /// <param name="centerTile">The tile at the center of the grid.</param>
    /// <param name="grid">The grid indicating which tiles to highlight.</param>
    private void RenderHighlight(Vector2 centerTile, int[,] grid)
    {
      GridHelper.ForCoveredTiles(centerTile, grid,
        tilePos =>
        {
          Game1.spriteBatch.Draw(
            BuildingPlacementTiles,
            Game1.GlobalToLocal(Game1.viewport, tilePos * Game1.tileSize),
            Game1.getSourceRectForStandardTileSheet(BuildingPlacementTiles, 0), Color.White, 0f,
            Vector2.Zero, 1f,
            SpriteEffects.None, 0.999f);
        });
    }

    /// <summary>Render a grid showing the tile layout.</summary>
    private void RenderGrid()
    {
      var startingX = -Game1.viewport.X % Game1.tileSize;
      var startingY = -(float)Game1.viewport.Y % Game1.tileSize;
      for (var x = startingX; x < Game1.graphics.GraphicsDevice.Viewport.Width; x += Game1.tileSize)
        Game1.spriteBatch.Draw(Game1.staminaRect,
          new Rectangle(x, (int)startingY, 1, Game1.graphics.GraphicsDevice.Viewport.Height + Game1.tileSize),
          Config.GridColour);
      for (var y = startingY; y < Game1.graphics.GraphicsDevice.Viewport.Height; y += Game1.tileSize)
        Game1.spriteBatch.Draw(Game1.staminaRect,
          new Rectangle(startingX, (int)y, Game1.graphics.GraphicsDevice.Viewport.Width + Game1.tileSize, 1),
          Config.GridColour);
    }
  }
}