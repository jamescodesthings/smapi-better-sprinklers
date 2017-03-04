using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SMAPISprinklerMod
{
    public class SprinklerModConfig
    {
        /*********
        ** Accessors
        *********/
        public Keys ConfigKey { get; set; } = Keys.K;
        public Keys HighlightKey { get; set; } = Keys.F3;
        public Color GridColour { get; set; } = Color.PowderBlue;
        public Dictionary<int, int[,]> SprinklerShapes { get; set; } = new Dictionary<int, int[,]>
        {
            [599] = new int[7, 7],
            [621] = new int[11, 11],
            [645] = new int[15, 15]
        };
        public Dictionary<int, int> SprinklerPrices { get; set; } = new Dictionary<int, int>
        {
            [599] = 1,
            [621] = 1,
            [645] = 1
        };
    }
}
