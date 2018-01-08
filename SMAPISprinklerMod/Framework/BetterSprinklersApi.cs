using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;

namespace BetterSprinklers.Framework
{
    /// <summary>The API which provides access to Better Sprinklers for other mods.</summary>
    public class BetterSprinklersApi : IBetterSprinklersApi
    {
        /*********
        ** Properties
        *********/
        /// <summary>The mod helper.</summary>
        private readonly IModHelper Helper;

        /// <summary>The maximum sprinkler coverage supported by this mod (in tiles wide or high).</summary>
        private readonly int MaxGridSize;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="helper">The mod helper.</param>
        /// <param name="maxGridSize">The maximum sprinkler coverage supported by this mod (in tiles wide or high).</param>
        internal BetterSprinklersApi(IModHelper helper, int maxGridSize)
        {
            this.Helper = helper;
            this.MaxGridSize = maxGridSize;
        }

        /// <summary>Get the maximum sprinkler coverage supported by this mod (in tiles wide or high).</summary>
        public int GetMaxGridSize()
        {
            return this.MaxGridSize;
        }

        /// <summary>Get the relative tile coverage by supported sprinkler ID.</summary>
        public IDictionary<int, Vector2[]> GetSprinklerCoverage()
        {
            // get configured sprinkler shapes
            SprinklerModConfig config = this.Helper.ReadConfig<SprinklerModConfig>();
            IDictionary<int, int[,]> shapes = config.SprinklerShapes;

            // build tile grids
            IDictionary<int, Vector2[]> coverage = new Dictionary<int, Vector2[]>();
            foreach (KeyValuePair<int, int[,]> shape in shapes)
                coverage[shape.Key] = GridHelper.GetCoveredTiles(Vector2.Zero, shape.Value).ToArray();
            return coverage;
        }
    }
}
