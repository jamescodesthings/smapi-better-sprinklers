using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Network;
using StardewValley.TerrainFeatures;
using Object = StardewValley.Object;

namespace BetterSprinklers.Framework.Helpers
{
  /**
   * Helps us get from GameLocations to Sprinkler objects and tiles
   */
  public static class SprinklersHelper
  {
    public static List<int> SprinklerObjectIds = new List<int>
    {
      599,
      621,
      645
    };
    
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
    
    public static void ForCoveredTiles(this Object sprinkler, SprinklerModConfig config, Vector2 tile, Action<Vector2> perform)
    {
      config.SprinklerShapes.TryGetValue(sprinkler.ParentSheetIndex, out int[,] grid);
      foreach (Vector2 coveredTile in GridHelper.GetCoveredTiles(tile , grid))
      {
        perform(coveredTile);
      }
    }

    public static bool IsDirt(this TerrainFeature terrainFeature)
    {
      return terrainFeature is HoeDirt;
    }
  }
}