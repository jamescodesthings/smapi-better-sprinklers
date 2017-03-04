using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;

namespace SMAPISprinklerMod
{
    public class SprinklerModConfig : Config
    {
        /*********
        ** Accessors
        *********/
        public Keys ConfigKey { get; set; }
        public Keys HighlightKey { get; set; }
        public Color GridColour { get; set; }
        public Dictionary<int, int[,]> SprinklerShapes { get; set; }
        public Dictionary<int, int> SprinklerPrices { get; set; }


        /*********
        ** Public methods
        *********/
        public override T GenerateDefaultConfig<T>()
        {
            this.GridColour = Color.PowderBlue;
            this.ConfigKey = Keys.K;
            this.HighlightKey = Keys.F3;
            this.SprinklerShapes = new Dictionary<int, int[,]>();
            this.SprinklerPrices = new Dictionary<int, int>();
            this.SprinklerShapes.Add(599, new int[7, 7]);
            this.SprinklerShapes.Add(621, new int[11, 11]);
            this.SprinklerShapes.Add(645, new int[15, 15]);
            this.SprinklerPrices.Add(599, 1);
            this.SprinklerPrices.Add(621, 1);
            this.SprinklerPrices.Add(645, 1);
            return this as T;
        }
    }
}
