/*
    Copyright 2016 Maurício Gomes (Speeder)

    Configurable Improved Sprinklers mod is free software: 
    you can redistribute it and/or modify it under the terms of the 
    GNU General Public License as published by the Free Software Foundation,
    either version 3 of the License, or (at your option) any later version.

    Configurable Improved Sprinklers mod is distributed in the hope
    that it will be useful, but WITHOUT ANY WARRANTY; 
    without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Configurable Improved Sprinklers mod. 
    If not, see <http://www.gnu.org/licenses/>.
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using System.Collections.Generic;

namespace SMAPISprinklerMod
{
    public class SprinklerModConfig : Config
    {
        public Keys configKey;
        public Keys highlightKey;
        public Color gridColour;
        public Dictionary<int, int[,]> sprinklerShapes;
        public Dictionary<int, int> sprinklerPrices;

        public override T GenerateDefaultConfig<T>()
        {
            gridColour = Color.PowderBlue;
            configKey = Keys.K;
            highlightKey = Keys.F3;
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
