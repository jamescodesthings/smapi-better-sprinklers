using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;

namespace BetterSprinklers
{
    public class SprinklerModConfig
    {
        /*********
        ** Accessors
        *********/
        public SButton ConfigKey { get; set; } = SButton.K;
        public SButton HighlightKey { get; set; } = SButton.F3;
        public Color GridColour { get; set; } = Color.PowderBlue;
        public bool OverlayEnabledOnPlace { get; set; } = true;
        public bool BalancedModeEnabledCrafting { get; set; } = true;
        public bool BalancedModeEnabledBuying { get; set; } = true;
        public Dictionary<int, int[,]> SprinklerShapes { get; set; } = new()
        {
            [599] = new[,]
            {
                { 0, 0, 1, 1, 1, 0, 0 },
                { 0, 0, 1, 1, 1, 0, 0 },
                { 0, 0, 1, 1, 1, 0, 0 },
                { 0, 0, 1, 1, 1, 0, 0 },
                { 0, 0, 1, 1, 1, 0, 0 },
                { 0, 0, 1, 1, 1, 0, 0 },
                { 0, 0, 1, 1, 1, 0, 0 }
            },
            [621] = new[,]
            {
                { 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0 },
            },
            [645] = new[,]
            {
                { 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0 }
            }
        };
        public Dictionary<int, int> SprinklerPrices { get; set; } = new()
        {
            [599] = 1,
            [621] = 1,
            [645] = 1
        };
    }
}
