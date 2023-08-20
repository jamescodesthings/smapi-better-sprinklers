using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using Object = StardewValley.Object;

namespace BetterSprinklersPlus.Framework.Helpers
{
  /**
   * Helps us get from GameLocations to Sprinkler objects and tiles
   */
  public static class SprinklerHelper
  {
    public static readonly List<int> SprinklerObjectIds = new()
    {
      599,
      621,
      645,
    };

    public static readonly int PressureNozzleId = 915;
    public static readonly Dictionary<int, string> SprinklerTypes = new()
    {
      [599] = "Sprinkler",
      [621] = "Quality Sprinkler",
      [645] = "Iridium Sprinkler",
    };

    public static bool IsSprinkler(this Object obj)
    {
      return SprinklerObjectIds.Contains(obj.ParentSheetIndex);
    }

    public static IEnumerable<KeyValuePair<Vector2, Object>> AllSprinklers(this IEnumerable<GameLocation> locations)
    {
      var allSprinklers = new List<KeyValuePair<Vector2, Object>>();
      foreach (var location in locations)
      {
        allSprinklers.AddRange(location.AllSprinklers());
      }

      return allSprinklers;
    }
    public static IEnumerable<KeyValuePair<Vector2, Object>> AllSprinklers(this GameLocation location)
    {
      return location.objects.Pairs
        .Where(obj => SprinklerObjectIds.Contains(obj.Value.ParentSheetIndex));
    }

    public static IEnumerable<Vector2> AllPressureNozzles(this GameLocation location)
    {
      return location.objects.Pairs
        .Where(obj => obj.Value.ParentSheetIndex == PressureNozzleId).Select((pn) => pn.Key);
    }

    public static void ForCoveredTiles(this Object sprinkler, BetterSprinklersPlusConfig config, Vector2 tile, Action<Vector2> perform)
    {
      config.SprinklerShapes.TryGetValue(sprinkler.ParentSheetIndex, out var grid);
      foreach (var coveredTile in GridHelper.GetCoveredTiles(tile , grid))
      {
        perform(coveredTile);
      }
    }

    public static int CountCoveredTiles(this int type)
    {
      BetterSprinklersPlusConfig.Active.SprinklerShapes.TryGetValue(type, out var grid);
      if (grid == null) return 0;

      return grid.CountCoveredTiles();
    }
    
    public static int CountCoveredTiles(this int[,] grid)
    {
      Logger.Verbose($"CountCoveredTiles(int[,] grid)");
      if (grid == null)
      {
        Logger.Warn($"CountCoveredTiles: Grid was null, returning 0");
        return 0;
      }

      var count = grid.Cast<int>().Count(cell => cell > 0);

      Logger.Verbose($"Count of covered tiles: {count}");
      return count;
    }

    public class SprinklerTile
    {
      public int X { get; set; }
      public int Y { get; set; }
      public bool IsCovered { get; set; }

      public SprinklerTile(int x, int y, bool isCovered)
      {
        X = x;
        Y = y;
        IsCovered = isCovered;
      }

      public Vector2 ToVector2()
      {
        return new Vector2(X, Y);
      }
    }

    public static void ForAllTiles(this Object sprinkler, Vector2 tile, Action<SprinklerTile> perform)
    {
      Logger.Verbose($"ForAllTiles(sprinkler, {tile.X}x{tile.Y}, perform)");
      BetterSprinklersPlusConfig.Active.SprinklerShapes.TryGetValue(sprinkler.ParentSheetIndex, out var grid);
      foreach (var coveredTile in GridHelper.GetAllTiles(tile , grid))
      {
        Logger.Verbose($"Performing for CoveredTile: {coveredTile.X}x{coveredTile.Y}");
        perform(coveredTile);
      }
    }

    public static bool IsDirt(this TerrainFeature terrainFeature)
    {
      return terrainFeature is HoeDirt;
    }

    public static bool HasPressureNozzle(this Object sprinkler)
    {
#pragma warning disable AvoidImplicitNetFieldCast
      if (sprinkler == null || sprinkler.heldObject == null) return false;
#pragma warning restore AvoidImplicitNetFieldCast
      
#pragma warning disable AvoidImplicitNetFieldCast
      return Utility.IsNormalObjectAtParentSheetIndex(sprinkler.heldObject, 915);
#pragma warning restore AvoidImplicitNetFieldCast
    }

    public static float CalculateCostForSprinkler(this int type, bool hasPressureNozzle = false)
    {
      var count = type.CountCoveredTiles();
      var costPerTile = type.GetCostPerTile(hasPressureNozzle);

      return count * costPerTile;
    }
    
    
    public static float CalculateCostForSprinkler(this int[,] grid, int type)
    {
      Logger.Verbose($"CalculateCostForSprinkler(int[,] grid, {SprinklerHelper.SprinklerTypes[type]})");
      var count = grid.CountCoveredTiles();
      Logger.Verbose($"Count of covered tiles: {count}");
      var costPerTile = type.GetCostPerTile();
      Logger.Verbose($"Cost Per Tile: {costPerTile}G");

      var costForSprinkler = count * costPerTile;
      Logger.Verbose($"Cost for sprinkler: {costForSprinkler}G");
      return costForSprinkler;
    }

    public static float GetCostPerTile(this int type, bool hasPressureNozzle = false)
    {
      Logger.Verbose($"GetCostPerTile({type}, {hasPressureNozzle})");
      var baseCost = GetCostPerTile();
      float multiplier;

      try
      {
        multiplier = BetterSprinklersPlusConfig.Active.CostMultiplier[type];
      }
      catch (Exception)
      {
        multiplier = 1;
      }

      var costAfterMultiplier = baseCost * multiplier;
      Logger.Verbose($"cost after type multiplier: {multiplier}");
      
      if (hasPressureNozzle)
      {
        var pressureNozzleMultiplier = BetterSprinklersPlusConfig.Active.PressureNozzleMultiplier;
        Logger.Verbose($"cost after has pressure nozzle (true): {costAfterMultiplier * pressureNozzleMultiplier}");
        return costAfterMultiplier * pressureNozzleMultiplier;
      }

      Logger.Verbose($"cost after has pressure nozzle (false): {costAfterMultiplier}");
      return costAfterMultiplier;
    }
    
    /// <summary>
    /// Gets the cost per tile in .Gs
    /// </summary>
    /// <returns>The cost of watering one tile (as a fraction of a G)</returns>
    public static float GetCostPerTile()
    {
      try
      {
        var costPerTile = BetterSprinklersPlusConfig.BalancedModeOptionsMultipliers[BetterSprinklersPlusConfig.Active.BalancedMode];
        Logger.Verbose($"GetCostPerTile(): {costPerTile}G");
        return costPerTile;
      }
      catch (Exception e)
      {
        Logger.Error($"GetCostPerTile(): {e.Message}, returning 0G");
        return 0f;
      }
    }
  }
}
