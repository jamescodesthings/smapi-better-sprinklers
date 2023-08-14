// ReSharper disable MemberCanBePrivate.Global

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;

namespace BetterSprinklersPlus.Framework
{
  public class BetterSprinklersPlusConfig
  {
    /// <summary>The maximum grid size. This one is Scarecrow grids</summary>
    public const int MaxGridSize = 19;
    
    public enum BalancedModeOptions
    {
      Off,
      Easy,
      Normal,
      Hard,
      VeryHard,
    };

    private static readonly string[] BalancedModeOptionsText =
    {
      "Off",
      "Easy",
      "Normal",
      "Hard",
      "Very Hard",
    };

    public static readonly float[] BalancedModeOptionsMultipliers =
    {
      0f,
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

    private static readonly string[] CannotAffordOptionsText =
    {
      "Off",
      "Cut Off",
      "Don't Water",
    };

    public SButton ShowSprinklerEditKey { get; set; } = SButton.K;
    public SButton ShowOverlayKey { get; set; } = SButton.F3;
    public Color GridColour { get; set; } = Color.PowderBlue;
    public bool OverlayEnabledOnPlace { get; set; } = true;
    public int BalancedMode { get; set; } = (int)BalancedModeOptions.Normal;
    public int CannotAfford { get; set; } = (int)CannotAffordOptions.CutOff;
    public bool BalancedModeCostMessage { get; set; } = true;
    public bool BalancedModeCannotAffordWarning { get; set; } = true;
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

    /// <summary>
    /// Reads/Re-reads config
    /// </summary>
    public static BetterSprinklersPlusConfig ReadConfig(IModHelper helper)
    {
      return helper.ReadConfig<BetterSprinklersPlusConfig>();
    }
    
    /// <summary>Save changes to the mod configuration.</summary>
    public static void SaveChanges(IModHelper helper, BetterSprinklersPlusConfig config)
    {
      helper.WriteConfig(config);
      Game1.addHUDMessage(new HUDMessage("Sprinkler Configurations Saved", Color.Green, 3500f));
    }
    
    public static void SetupGenericModConfigMenu(IModHelper helper, IManifest mod, IMonitor monitor,
      Action onUpdate)
    {
      var config = helper.ReadConfig<BetterSprinklersPlusConfig>();

      var configMenu = helper.ModRegistry
        .GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

      if (configMenu is null)
        return;

      configMenu.Register(
        mod: mod,
        reset: () => { config = new BetterSprinklersPlusConfig(); },
        save: () =>
        {
          helper.WriteConfig(config);
          onUpdate();
        });

      configMenu.AddSectionTitle(mod: mod, () => "Balance:");

      configMenu.AddTextOption(
        mod: mod,
        name: () => "Mode",
        tooltip: () => "Changes the amount that water costs.",
        getValue: () =>
        {
          try
          {
            return BalancedModeOptionsText[config.BalancedMode] ?? "Off";
          }
          catch (Exception exception)
          {
            monitor.Log($"Error Getting Balanced Mode option {config.BalancedMode}: {exception.Message}",
              LogLevel.Error);
            return "Off";
          }
        },
        setValue: value =>
        {
          try
          {
            var index = Array.IndexOf(BalancedModeOptionsText, value);
            if (index == -1) index = 0;
            config.BalancedMode = index;
          }
          catch (Exception exception)
          {
            monitor.Log($"Error Setting Balanced Mode option {value}: {exception.Message}", LogLevel.Error);
            config.BalancedMode = (int)BalancedModeOptions.Off;
          }
        },
        allowedValues: BalancedModeOptionsText
      );

      configMenu.AddTextOption(
        mod: mod,
        name: () => "Can't Afford",
        tooltip: () => "What to do when you can't afford the cost",
        getValue: () =>
        {
          try
          {
            return CannotAffordOptionsText[config.CannotAfford] ?? "Off";
          }
          catch (Exception exception)
          {
            monitor.Log($"Error Getting Can't Afford option {config.CannotAfford}: {exception.Message}",
              LogLevel.Error);
            return "Off";
          }
        },
        setValue: value =>
        {
          try
          {
            var index = Array.IndexOf(CannotAffordOptionsText, value);
            if (index == -1) index = 0;
            config.CannotAfford = index;
          }
          catch (Exception exception)
          {
            monitor.Log($"Error Setting Can't Afford option {value}: {exception.Message}",
              LogLevel.Error);
            config.CannotAfford = (int)CannotAffordOptions.Off;
          }
        },
        allowedValues: CannotAffordOptionsText
      );

      configMenu.AddBoolOption(
        mod: mod,
        name: () => "Water costs on any tile",
        tooltip: () => "Water costs money even if it waters a tile that cannot be watered tile.",
        getValue: () => config.BalancedModeCostsMoneyOnAnyTile,
        setValue: value => config.BalancedModeCostsMoneyOnAnyTile = value
      );

      configMenu.AddBoolOption(
        mod: mod,
        name: () => "Show Bills Message",
        tooltip: () => "In the morning you'll see how much your sprinklers have cost.",
        getValue: () => config.BalancedModeCostMessage,
        setValue: value => config.BalancedModeCostMessage = value
      );

      configMenu.AddBoolOption(
        mod: mod,
        name: () => "Show Can't Afford Warning",
        tooltip: () => "In the morning you'll be warned if watering did not finish.",
        getValue: () => config.BalancedModeCannotAffordWarning,
        setValue: value => config.BalancedModeCannotAffordWarning = value
      );


      configMenu.AddSectionTitle(mod: mod, () => "Options:");

      configMenu.AddBoolOption(
        mod: mod,
        name: () => "Show Overlay Grid",
        tooltip: () => "When checked the grid shows in F3 mode",
        getValue: () => config.GridColour == Color.PowderBlue,
        setValue: value => config.GridColour = value ? Color.PowderBlue : Color.Transparent
      );

      configMenu.AddBoolOption(
        mod: mod,
        name: () => "Show Placement Coverage",
        tooltip: () => "When checked the Overlay shows Sprinkler/Scarecrow reach when placing.",
        getValue: () => config.OverlayEnabledOnPlace,
        setValue: value => config.OverlayEnabledOnPlace = value
      );

      configMenu.AddSectionTitle(mod: mod, () => "Key Bindings:");

      configMenu.AddKeybind(
        mod: mod,
        name: () => "Show Config Key",
        tooltip: () => "The key to press to change the boundary of all sprinklers",
        getValue: () => config.ShowSprinklerEditKey,
        setValue: value => config.ShowSprinklerEditKey = value
      );

      configMenu.AddKeybind(
        mod: mod,
        name: () => "Show Overlay Key",
        tooltip: () => "The key to press to show the boundary of the highlighted sprinkler/scarecrow",
        getValue: () => config.ShowOverlayKey,
        setValue: value => config.ShowOverlayKey = value
      );
    }
  }
}