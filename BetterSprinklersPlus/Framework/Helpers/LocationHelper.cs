using System.Collections.Generic;
using System.Linq;
using StardewValley;
using StardewValley.Locations;

namespace BetterSprinklersPlus.Framework.Helpers
{
  public static class LocationHelper
  {
    /// <summary>Get all game location.</summary>
    public static IEnumerable<GameLocation> GetAllBuildableLocations()
    {
      return Game1.locations
        .Concat(
          from location in Game1.locations.OfType<BuildableGameLocation>()
          from building in location.buildings
          where building.indoors.Value != null
          select building.indoors.Value
        );
    }
  }
}