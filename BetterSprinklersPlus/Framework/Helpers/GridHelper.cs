using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace BetterSprinklersPlus.Framework.Helpers
{
    /// <summary>Provides methods for working with sprinkler grids.</summary>
    internal static class GridHelper
    {
      /// <summary>Highlight coverage based on a given grid.</summary>
      /// <param name="helper">The IModHelper</param>
      /// <param name="centerTile">The tile at the center of the grid.</param>
      /// <param name="grid">The grid indicating which tiles to highlight.</param>
      public static void RenderHighlight(IModHelper helper, Vector2 centerTile, int[,] grid)
      {
        var buildingPlacementTiles =  helper.GameContent.Load<Texture2D>("LooseSprites/buildingPlacementTiles");
        
        ForCoveredTiles(centerTile, grid,
          tilePos =>
          {
            Game1.spriteBatch.Draw(
              buildingPlacementTiles,
              Game1.GlobalToLocal(Game1.viewport, tilePos * Game1.tileSize),
              Game1.getSourceRectForStandardTileSheet(buildingPlacementTiles, 0), Color.White, 0f,
              Vector2.Zero, 1f,
              SpriteEffects.None, 0.999f);
          });
      }

      
        /*********
        ** Public methods
        *********/
        /// <summary>Get a grid of covered tiles centered on a sprinkler.</summary>
        /// <param name="center">The sprinkler's tile position.</param>
        /// <param name="grid">The grid to convert.</param>
        public static IEnumerable<Vector2> GetCoveredTiles(Vector2 center, int[,] grid)
        {
            int arrayHalfSizeX = grid.GetLength(0) / 2;
            int arrayHalfSizeY = grid.GetLength(1) / 2;
            int minX = (int)center.X - arrayHalfSizeX;
            int minY = (int)center.Y - arrayHalfSizeY;
            int maxX = (int)center.X + arrayHalfSizeX;
            int maxY = (int)center.Y + arrayHalfSizeY;

            for (int gridX = 0, x = minX; x <= maxX; x++, gridX++)
            {
                for (int gridY = 0, y = minY; y <= maxY; y++, gridY++)
                {
                    if (grid[gridX, gridY] > 0)
                        yield return new Vector2(x, y);
                }
            }
        }

        /// <summary>Get a tile grid centered on the given tile position.</summary>
        /// <param name="centerTile">The center tile position.</param>
        /// <param name="grid">The grid to get.</param>
        /// <param name="perform">The action to perform for each tile, given the tile position.</param>
        public static void ForCoveredTiles(Vector2 centerTile, int[,] grid, Action<Vector2> perform)
        {
            foreach (Vector2 tile in GridHelper.GetCoveredTiles(centerTile, grid))
            {
                perform(tile);
            }
        }
    }
}
