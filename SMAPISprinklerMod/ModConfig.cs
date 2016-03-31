using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAPISprinklerMod
{
    public class SprinklerModConfig : Config
    {
        public Keys configKey;
        public Dictionary<int, int[,]> sprinklerShapes;
        public Dictionary<int, int> sprinklerPrices;

        public override T GenerateDefaultConfig<T>()
        {
            configKey = Keys.K;
            int[,] sprinklerGrid = new int[7, 7];
            sprinklerShapes = new Dictionary<int, int[,]>();
            sprinklerPrices = new Dictionary<int, int>();
            sprinklerShapes.Add(599, sprinklerGrid);
            sprinklerGrid = new int[11, 11];
            sprinklerShapes.Add(621, sprinklerGrid);
            sprinklerGrid = new int[15, 15];
            sprinklerShapes.Add(645, sprinklerGrid);
            sprinklerPrices.Add(599, 1);
            sprinklerPrices.Add(621, 1);
            sprinklerPrices.Add(645, 1);
            return this as T;
        }
    }
}
