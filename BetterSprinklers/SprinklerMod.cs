using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using BetterSprinklers.Framework;
using GenericModConfigMenu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;

namespace BetterSprinklers
{
  public class SprinklerMod : Mod
  {
    /*********
     ** Fields
     *********/
    /// <summary>The asset key for the crafting recipes.</summary>
    public const string RecipeDataKey = "Data/CraftingRecipes";

    /// <summary>The asset key for the object data.</summary>
    public const string ObjectDataKey = "Data/ObjectInformation";

    /// <summary>The maximum grid size.</summary>
    private readonly int MaxGridSize = 19;

    /// <summary>The mod configuration.</summary>
    private SprinklerModConfig Config;

    private Texture2D BuildingPlacementTiles;
    private int[,] ScarecrowGrid;

    /// <summary>Whether to show a grid overlay and highlight the coverage for sprinklers or scarecrows under the cursor.</summary>
    private bool ShowInfoOverlay;

    private List<int> SprinklerIds = new()
    {
      599,
      621,
      645
    };


    /*********
     ** Public methods
     *********/
    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
      // initialise
      this.Config = helper.ReadConfig<SprinklerModConfig>();
      this.ScarecrowGrid = this.GetScarecrowGrid();

      this.BuildingPlacementTiles = helper.GameContent.Load<Texture2D>("LooseSprites/buildingPlacementTiles");

      // set up events
      helper.Events.Display.RenderedWorld += this.OnRenderedWorld;
      helper.Events.GameLoop.DayStarted += this.OnDayStarted;
      helper.Events.Input.ButtonPressed += this.OnButtonPressed;

      // set up config
      helper.Events.GameLoop.GameLaunched += OnGameLaunched;
    }

    /// <summary>Get an API that other mods can access. This is always called after <see cref="Entry" />.</summary>
    public override object GetApi()
    {
      return new BetterSprinklersApi(this.Config, this.MaxGridSize);
    }

    private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
    {
      // get Generic Mod Config Menu's API (if it's installed)
      var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
      if (configMenu is null)
        return;

      // register mod
      configMenu.Register(
        mod: this.ModManifest,
        reset: () => this.Config = new SprinklerModConfig(),
        save: () => this.Helper.WriteConfig(this.Config)
      );

      configMenu.AddSectionTitle(mod: this.ModManifest, () => "Balance:");

      configMenu.AddTextOption(
        mod: this.ModManifest,
        name: () => "Mode",
        tooltip: () => "Changes the amount that water costs.",
        getValue: () =>
        {
          try
          {
            return SprinklerModConfig.BalancedModeOptionsText[(int)this.Config.BalancedMode] ?? "Off";
          }
          catch (Exception exception)
          {
            this.Monitor.Log($"Error Getting Balanced Mode option {this.Config.BalancedMode}: {exception.Message}",
              LogLevel.Error);
            return "Off";
          }
        },
        setValue: value =>
        {
          try
          {
            var index = Array.IndexOf(SprinklerModConfig.BalancedModeOptionsText, value);
            if (index == -1) index = 0;
            this.Config.BalancedMode = index;
          }
          catch (Exception exception)
          {
            this.Monitor.Log($"Error Setting Balanced Mode option {value}: {exception.Message}", LogLevel.Error);
            this.Config.BalancedMode = (int)SprinklerModConfig.BalancedModeOptions.Off;
          }
        },
        allowedValues: SprinklerModConfig.BalancedModeOptionsText
      );

      configMenu.AddTextOption(
        mod: this.ModManifest,
        name: () => "Can't Afford",
        tooltip: () => "What to do when you can't afford the cost",
        getValue: () =>
        {
          try
          {
            return SprinklerModConfig.CannotAffordOptionsText[(int)this.Config.CannotAfford] ?? "Off";
          }
          catch (Exception exception)
          {
            this.Monitor.Log($"Error Getting Can't Afford option {this.Config.CannotAfford}: {exception.Message}",
              LogLevel.Error);
            return "Off";
          }
        },
        setValue: value =>
        {
          try
          {
            var index = Array.IndexOf(SprinklerModConfig.CannotAffordOptionsText, value);
            if (index == -1) index = 0;
            this.Config.CannotAfford = index;
          }
          catch (Exception exception)
          {
            this.Monitor.Log($"Error Setting Can't Afford option {value}: {exception.Message}",
              LogLevel.Error);
            this.Config.CannotAfford = (int)SprinklerModConfig.CannotAffordOptions.Off;
          }
        },
        allowedValues: SprinklerModConfig.CannotAffordOptionsText
      );

      configMenu.AddBoolOption(
        mod: this.ModManifest,
        name: () => "Water costs on any tile",
        tooltip: () => "Water costs money even if it waters a non-waterable tile.",
        getValue: () => this.Config.BalancedModeCostsMoneyOnAnyTile,
        setValue: value => this.Config.BalancedModeCostsMoneyOnAnyTile = value
      );

      configMenu.AddBoolOption(
        mod: this.ModManifest,
        name: () => "Show Bills Message",
        tooltip: () => "In the morning you'll see how much your sprinklers have cost.",
        getValue: () => this.Config.BalancedModeCostMessage,
        setValue: value => this.Config.BalancedModeCostMessage = value
      );


      configMenu.AddSectionTitle(mod: this.ModManifest, () => "Options:");

      configMenu.AddBoolOption(
        mod: this.ModManifest,
        name: () => "Show Overlay Grid",
        tooltip: () => "When checked the grid shows in F3 mode",
        getValue: () => this.Config.GridColour == Color.PowderBlue,
        setValue: value => this.Config.GridColour = value ? Color.PowderBlue : Color.Transparent
      );

      configMenu.AddBoolOption(
        mod: this.ModManifest,
        name: () => "Show Placement Coverage",
        tooltip: () => "When checked the Overlay shows Sprinkler/Scarecrow reach when placing.",
        getValue: () => this.Config.OverlayEnabledOnPlace,
        setValue: value => this.Config.OverlayEnabledOnPlace = value
      );

      configMenu.AddSectionTitle(mod: this.ModManifest, () => "Key Bindings:");

      configMenu.AddKeybind(
        mod: this.ModManifest,
        name: () => "Show Config Key",
        tooltip: () => "The key to press to change the boundary of all sprinklers",
        getValue: () => this.Config.ConfigKey,
        setValue: value => this.Config.ConfigKey = value
      );

      configMenu.AddKeybind(
        mod: this.ModManifest,
        name: () => "Show Overlay Key",
        tooltip: () => "The key to press to show the boundary of the highlighted sprinkler/scarecrow",
        getValue: () => this.Config.HighlightKey,
        setValue: value => this.Config.HighlightKey = value
      );
    }

    /*********
     ** Private methods
     *********/
    /****
     ** Event handlers
     ****/
    /// <summary>Raised after drawing the world.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnRenderedWorld(object sender, RenderedWorldEventArgs e)
    {
      if (Context.IsWorldReady && Game1.activeClickableMenu == null && Game1.CurrentEvent == null)
        this.RenderHighlight();
    }

    /// <summary>Raised after the game begins a new day (including when the player loads a save).</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnDayStarted(object sender, DayStartedEventArgs e)
    {
      this.RunSprinklers();
    }

    /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
    {
      if (Game1.activeClickableMenu != null || Game1.CurrentEvent != null)
        return;

      // show menu
      if (e.Button == this.Config.ConfigKey)
        Game1.activeClickableMenu = new SprinklerShapeEditMenu(this.Config, this.SaveChanges);

      // toggle overlay
      else if (e.Button == this.Config.HighlightKey)
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
    }

    /// <summary>Run all sprinklers.</summary>
    /// todo: this needs a lot of cleaning
    private void RunSprinklers()
    {
      var costPerTile = GetCostPerTile();
      var canAfford = (int)Math.Floor(Game1.player.Money / costPerTile);
      var started = true;
      var expectedCost = 0;
      
      this.Monitor.VerboseLog($"Calculating Affordability: {Game1.player.Money}G / {costPerTile} = {canAfford}");

      // Calculate Whether or not we can afford this watering
      if (this.Config.CannotAfford == (int)SprinklerModConfig.CannotAffordOptions.DoNotWater)
      {
        var (_, willWater) = WaterTiles(false, canAfford, true);
        if (willWater > canAfford) started = false;
        
        expectedCost = CalculateCost(willWater);
        
        this.Monitor.VerboseLog($"Started: {started}, Will Water: {willWater}, Expected Cost: {expectedCost}G");
      }

      // Do the watering
      var (finished, wateredTiles) = WaterTiles(started, canAfford);
      
      // Calculate the cost
      var cost = CalculateCost(wateredTiles);
      var startedLogMessage = !started ? " Could not water" : "";
      var finishedLogMessage = !finished ? " Could not finish watering" : "";
      this.Monitor.VerboseLog($"Start of day bills: {wateredTiles} watered, costing {cost}G{startedLogMessage}{finishedLogMessage}");

      // Pay yo' bills
      if (Game1.player.Money - cost >= 0)
      {
        Game1.player.Money -= cost;
      }
      else
      {
        Game1.player.Money = 0;
      }

      // Let the player know what's happened
      if (this.Config.BalancedModeCostMessage && cost > 0)
      {
        var finishedHudMessage = !finished ? ", not all crops were watered" : "";
        Game1.addHUDMessage(new HUDMessage($"Your sprinklers have run ({cost}G){finishedHudMessage}.", Color.Green, 5000f));
        return;
      }

      if (!started && this.Config.BalancedModeCostMessage)
      {
        Game1.addHUDMessage(new HUDMessage($"You could not to run your sprinklers today ({expectedCost}G).", Color.Green, 5000f));
      }
    }

    /// <summary>
    /// Water tiles, or dry run to calculate costs.
    /// </summary>
    /// <param name="started">If true we can start</param>
    /// <param name="canAfford">The amount of tiles we can afford to water</param>
    /// <param name="dryRun">If true, don't do the watering, just return whether or not we would finish, and the amount of tiles</param>
    /// <returns>True if finished/can finish, the number of tiles that will/would be watered.</returns>
    private (bool, int) WaterTiles(bool started, int canAfford, bool dryRun = false)
    {
      var wateredTiles = 0;
      var finished = true;
      
      if (!dryRun && !started) return (false, 0);
      
      foreach (var location in this.GetLocations())
      {
        foreach (var (targetTile, value) in location.objects.Pairs)
        {
          var targetId = value.ParentSheetIndex;
          if (this.Config.SprinklerShapes.TryGetValue(targetId, out int[,] grid))
          {
            GridHelper.ForCoveredTiles(targetTile, grid, tilePos =>
            {
              // we can't afford any more water
              // don't sprinkle
              if (!dryRun && wateredTiles >= canAfford && this.Config.CannotAfford == (int)SprinklerModConfig.CannotAffordOptions.CutOff)
              {
                finished = false;
                return;
              }

              if (location.terrainFeatures.TryGetValue(tilePos, out TerrainFeature terrainFeature) &&
                  terrainFeature is HoeDirt dirt)
              {
                if (!dryRun) dirt.state.Value = 1;
                if (!this.Config.BalancedModeCostsMoneyOnAnyTile)
                {
                  wateredTiles++;
                }
              }

              if (this.Config.BalancedModeCostsMoneyOnAnyTile)
              {
                wateredTiles++;
              }
            });
          }
        }
      }

      return (finished, wateredTiles);
    }

    /// <summary>
    /// Calculates the cost of watering x tiles
    /// </summary>
    /// <param name="wateredTiles">The number of tiles to water</param>
    /// <returns>The cost in G</returns>
    private int CalculateCost(int wateredTiles)
    {
      var costPerTile = GetCostPerTile();
      var cost = (int)Math.Round(wateredTiles * costPerTile);
      return cost;
    }

    private float GetCostPerTile()
    {
      try
      {
        return SprinklerModConfig.BalancedModeOptionsMultipliers[(int)this.Config.BalancedMode];
      }
      catch (Exception e)
      {
        this.Monitor.Log($"Error getting cost per tile {e.Message}", LogLevel.Error);
        return 0f;
      }
    }

    /// <summary>Get all game location.</summary>
    private IEnumerable<GameLocation> GetLocations()
    {
      return Game1.locations
        .Concat(
          from location in Game1.locations.OfType<BuildableGameLocation>()
          from building in location.buildings
          where building.indoors.Value != null
          select building.indoors.Value
        );
    }

    /// <summary>Highlight coverage for sprinklers and scarecrows based on the current context.</summary>
    [SuppressMessage("ReSharper", "PossibleLossOfFraction",
      Justification = "The decimals are deliberately truncated during conversion to tile coordinates.")]
    private void RenderHighlight()
    {
      var cursorPos = this.Helper.Input.GetCursorPosition();
      Vector2 mouseTile = new Vector2((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize,
        (Game1.viewport.Y + Game1.getOldMouseY()) / Game1.tileSize);

      // var tileToHighlightFrom = Game1.GetPlacementGrabTile(); // almost, off by direction

      SObject heldItem = Game1.player.ActiveObject;

      // highlight coverage for held item
      if (this.Config.OverlayEnabledOnPlace && heldItem != null)
      {
        // accounts for controller mode
        var tileToHighlightFrom = GetPlacementPosition();
        if (tileToHighlightFrom == null) return;

        if (this.Config.SprinklerShapes.ContainsKey(heldItem.ParentSheetIndex))
        {
          this.RenderSprinklerHighlight(heldItem.ParentSheetIndex, (Vector2)tileToHighlightFrom);
          return;
        }

        if (heldItem.bigCraftable.Value && heldItem.Name.Contains("arecrow"))
        {
          this.RenderScarecrowHighlight((Vector2)tileToHighlightFrom);
          return;
        }
      }

      // highlight coverage for item under cursor
      if (this.ShowInfoOverlay)
      {
        if (Game1.currentLocation.objects.TryGetValue(mouseTile, out SObject target))
        {
          if (this.Config.SprinklerShapes.ContainsKey(target.ParentSheetIndex))
            this.RenderSprinklerHighlight(target.ParentSheetIndex, cursorPos.Tile);
          else if (target.bigCraftable.Value && target.Name.Contains("arecrow"))
            this.RenderScarecrowHighlight(cursorPos.Tile);
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
      GridHelper.ForCoveredTiles(centerTile, grid,
        tilePos =>
        {
          Game1.spriteBatch.Draw(this.BuildingPlacementTiles,
            Game1.GlobalToLocal(Game1.viewport, tilePos * Game1.tileSize),
            Game1.getSourceRectForStandardTileSheet(this.BuildingPlacementTiles, 0), Color.White, 0f, Vector2.Zero, 1f,
            SpriteEffects.None, 0.999f);
        });
    }

    /// <summary>Render a grid showing the tile layout.</summary>
    private void RenderGrid()
    {
      int startingX = -Game1.viewport.X % Game1.tileSize;
      float startingY = -(float)Game1.viewport.Y % Game1.tileSize;
      for (int x = startingX; x < Game1.graphics.GraphicsDevice.Viewport.Width; x += Game1.tileSize)
        Game1.spriteBatch.Draw(Game1.staminaRect,
          new Rectangle(x, (int)startingY, 1, Game1.graphics.GraphicsDevice.Viewport.Height + Game1.tileSize),
          this.Config.GridColour);
      for (float y = startingY; y < Game1.graphics.GraphicsDevice.Viewport.Height; y += Game1.tileSize)
        Game1.spriteBatch.Draw(Game1.staminaRect,
          new Rectangle(startingX, (int)y, Game1.graphics.GraphicsDevice.Viewport.Width + Game1.tileSize, 1),
          this.Config.GridColour);
    }

    private static Vector2? GetPlacementPosition()
    {
      var activeObject = Game1.player.ActiveObject;
      if (activeObject == null) return null;

      var x = (int)Game1.GetPlacementGrabTile().X * 64;
      var y = (int)Game1.GetPlacementGrabTile().Y * 64;
      Game1.isCheckingNonMousePlacement = !Game1.IsPerformingMousePlacement();
      if (Game1.isCheckingNonMousePlacement)
      {
        var placementPosition =
          Utility.GetNearbyValidPlacementPosition(Game1.player, Game1.currentLocation, Game1.player.ActiveObject, x, y);
        x = (int)placementPosition.X;
        y = (int)placementPosition.Y;
      }

      return new Vector2
      {
        // ReSharper disable once PossibleLossOfFraction
        X = x / 64,
        // ReSharper disable once PossibleLossOfFraction
        Y = y / 64
      };
    }
  }
}