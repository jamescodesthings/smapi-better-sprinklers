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
        public bool BalancedModeEnabledByCoverage { get; set; } = true;
        /// <summary>
        /// The sprinkler default sprinkler shape config
        /// Be warned, this is rotated 90deg (top to bottom is left to right)
        /// Don't remove the 2s, they are required (at the moment).
        /// </summary>
        public Dictionary<int, int[,]> SprinklerShapes { get; set; } = new Dictionary<int, int[,]>
        {
            // balance = (new + original) / original + 1
            // 10 new, 5 original (15) = 4
            [599] = new[,]
            {
                { 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 1, 1, 1, 0, 0 },
                { 0, 0, 1, 2, 1, 0, 0 },
                { 0, 0, 2, 2, 2, 0, 0 },
                { 0, 0, 1, 2, 1, 0, 0 },
                { 0, 0, 1, 1, 1, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0 }
            },
            
            // 12 new, 9 original (21) = 3?
            [621] = new[,]
            {
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 2, 2, 2, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 2, 2, 2, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 2, 2, 2, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            },
            // 30 new, 25 original (55) = 3?
            [645] = new[,]
            {
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            }
        };
        public Dictionary<int, int> SprinklerPrices { get; set; } = new()
        {
            [599] = 4,
            [621] = 3,
            [645] = 3
        };
    }
}
