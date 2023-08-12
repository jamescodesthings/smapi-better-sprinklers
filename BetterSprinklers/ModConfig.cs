using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;

namespace BetterSprinklers
{
  public class SprinklerModConfig
  {
    public enum BalancedModeOptions
    {
      Off,
      VeryEasy,
      Easy,
      Normal,
      Hard,
      VeryHard,
    };

    public static string[] BalancedModeOptionsText =
    {
      "Off",
      "Very Easy",
      "Easy",
      "Normal",
      "Hard",
      "Very Hard",
    };

    public static float[] BalancedModeOptionsMultipliers =
    {
      0f,
      0.05f,
      0.1f,
      0.25f,
      0.5f,
      1f
    };

    public enum CannotAffordOptions
    {
      Off,
      CutOff,
      DoNotWater,
    };

    public static string[] CannotAffordOptionsText =
    {
      "Off",
      "Cut Off",
      "Don't Water",
    };

    /*********
     ** Accessors
     *********/
    public SButton ConfigKey { get; set; } = SButton.K;
    public SButton HighlightKey { get; set; } = SButton.F3;
    public Color GridColour { get; set; } = Color.PowderBlue;
    public bool OverlayEnabledOnPlace { get; set; } = true;
    public int BalancedMode { get; set; } = (int)BalancedModeOptions.Normal;
    public int CannotAfford { get; set; } = (int)CannotAffordOptions.CutOff;
    public bool BalancedModeCostMessage { get; set; } = true;
    public bool BalancedModeCostsMoneyOnAnyTile { get; set; } = true;

    /// <summary>
    /// The sprinkler default sprinkler shape config
    /// Be warned, this is rotated 90deg (top to bottom is left to right)
    /// Don't remove the 2s, they are required (at the moment).
    /// </summary>
    public Dictionary<int, int[,]> SprinklerShapes { get; set; } = new Dictionary<int, int[,]>
    {
      [599] = new[,]
      {
        { 0, 0, 1, 1, 1, 0, 0 },
        { 0, 0, 1, 1, 1, 0, 0 },
        { 0, 0, 1, 2, 1, 0, 0 },
        { 0, 0, 2, 2, 2, 0, 0 },
        { 0, 0, 1, 2, 1, 0, 0 },
        { 0, 0, 1, 1, 1, 0, 0 },
        { 0, 0, 1, 1, 1, 0, 0 }
      },
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
  }
}