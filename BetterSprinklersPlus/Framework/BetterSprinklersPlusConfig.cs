// ReSharper disable MemberCanBePrivate.Global

using System;
using System.Collections.Generic;
using System.Linq;
using BetterSprinklersPlus.Framework.Helpers;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;

namespace BetterSprinklersPlus.Framework
{
  public class BetterSprinklersPlusConfig
  {
    public enum BalancedModeOptions
    {
      Off,
      Easy,
      Normal,
      Hard,
      VeryHard,
    }

    public enum CannotAffordOptions
    {
      Off,
      DoNotWater,
    }

    public static readonly string[] MaxSprinklerCoverageAllowedValues = {
      "7",
      "11",
      "15",
      "21",
      "25"
    };

    /// <summary>The maximum grid size. This one is Scarecrow grids</summary>
    public const int ScarecrowGridSize = 19;

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

    private static readonly string[] CannotAffordOptionsText =
    {
      "Off",
      "Don't Water",
    };

    public static BetterSprinklersPlusConfig Active { get; set; }
    public static IModHelper Helper { get; set; }
    public static IManifest Mod { get; set; }
    public static IMonitor Monitor { get; set; }
    public SButton ShowSprinklerEditKey { get; set; } = SButton.K;
    public SButton ShowOverlayKey { get; set; } = SButton.F3;
    public bool OverlayEnabledOnPlace { get; set; } = true;
    public int BalancedMode { get; set; } = (int)BalancedModeOptions.Normal;
    public int CannotAfford { get; set; } = (int)CannotAffordOptions.DoNotWater;
    public bool BalancedModeCostMessage { get; set; } = true;
    public bool BalancedModeCannotAffordWarning { get; set; } = true;

    public Dictionary<int, int> MaxCoverage { get; set; } = new()
    {
      [599] = 7,
      [621] = 11,
      [645] = 15,
    };

    public int MaxGridSize => MaxCoverage.Values.Prepend(ScarecrowGridSize).Max();

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

    public static void Init(IModHelper helper, IManifest mod, IMonitor monitor)
    {
      Helper = helper;
      Mod = mod;
      Monitor = monitor;

      ReadConfig();
      SetupGenericModConfigMenu();
    }

    /// <summary>
    /// Reads/Re-reads config
    /// </summary>
    public static void ReadConfig()
    {
      Active = Helper.ReadConfig<BetterSprinklersPlusConfig>();
    }

    /// <summary>Save changes to the mod configuration.</summary>
    public static void SaveChanges()
    {
      Helper.WriteConfig(Active);
      Game1.addHUDMessage(new HUDMessage("Sprinkler Configurations Saved", Color.Green, 3500f));
    }

    public static void UpdateMaxCoverage(BetterSprinklersPlusConfig config, int sprinklerId, string value) {
      try
      {
        var asInt = int.Parse(value);
        config.MaxCoverage[sprinklerId] = asInt;
        var grid = config.SprinklerShapes[sprinklerId];
        var resized = grid.Resize(asInt);
        config.SprinklerShapes[sprinklerId] = resized;
      }
      catch (Exception e)
      {
        Monitor.Log($"Error changing sprinkler value for {sprinklerId}: {e.Message}", LogLevel.Error);
      }
    }

    public static void SetupGenericModConfigMenu()
    {
      var configMenu = Helper.ModRegistry
        .GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

      if (configMenu is null)
        return;

      configMenu.Register(
        mod: Mod,
        reset: () => { BetterSprinklersPlusConfig.Active = new BetterSprinklersPlusConfig(); },
        save: SaveChanges);

      configMenu.AddSectionTitle(mod: Mod, () => "Balance:");

      configMenu.AddTextOption(
        mod: Mod,
        name: () => "Mode",
        tooltip: () => "Changes the amount that water costs.",
        getValue: () =>
        {
          try
          {
            return BalancedModeOptionsText[Active.BalancedMode] ?? "Off";
          }
          catch (Exception exception)
          {
            Monitor.Log($"Error Getting Balanced Mode option {Active.BalancedMode}: {exception.Message}",
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
            Active.BalancedMode = index;
          }
          catch (Exception exception)
          {
            Monitor.Log($"Error Setting Balanced Mode option {value}: {exception.Message}", LogLevel.Error);
            Active.BalancedMode = (int)BalancedModeOptions.Off;
          }
        },
        allowedValues: BalancedModeOptionsText
      );

      configMenu.AddTextOption(
        mod: Mod,
        name: () => "Can't Afford",
        tooltip: () => "What to do when you can't afford the cost",
        getValue: () =>
        {
          try
          {
            return CannotAffordOptionsText[Active.CannotAfford] ?? "Off";
          }
          catch (Exception exception)
          {
            Monitor.Log($"Error Getting Can't Afford option {Active.CannotAfford}: {exception.Message}",
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
            Active.CannotAfford = index;
          }
          catch (Exception exception)
          {
            Monitor.Log($"Error Setting Can't Afford option {value}: {exception.Message}",
              LogLevel.Error);
            Active.CannotAfford = (int)CannotAffordOptions.Off;
          }
        },
        allowedValues: CannotAffordOptionsText
      );

      configMenu.AddBoolOption(
        mod: Mod,
        name: () => "Show Bills Message",
        tooltip: () => "In the morning you'll see how much your sprinklers have cost.",
        getValue: () => Active.BalancedModeCostMessage,
        setValue: value => Active.BalancedModeCostMessage = value
      );

      configMenu.AddBoolOption(
        mod: Mod,
        name: () => "Show Can't Afford Warning",
        tooltip: () => "In the morning you'll be warned if watering did not finish.",
        getValue: () => Active.BalancedModeCannotAffordWarning,
        setValue: value => Active.BalancedModeCannotAffordWarning = value
      );

      configMenu.AddTextOption(
        mod: Mod,
        name: () => "Maximum Sprinkler Coverage",
        getValue: () => $"{Active.MaxCoverage[SprinklerHelper.SprinklerObjectIds[0]]}",
        setValue: value => UpdateMaxCoverage(Active, SprinklerHelper.SprinklerObjectIds[0], value),
        allowedValues: MaxSprinklerCoverageAllowedValues
      );

      configMenu.AddTextOption(
        mod: Mod,
        name: () => "Maximum Quality Sprinkler Coverage",
        getValue: () => $"{Active.MaxCoverage[SprinklerHelper.SprinklerObjectIds[1]]}",
        setValue: value => UpdateMaxCoverage(Active, SprinklerHelper.SprinklerObjectIds[1], value),
        allowedValues: MaxSprinklerCoverageAllowedValues
      );

      configMenu.AddTextOption(
        mod: Mod,
        name: () => "Maximum Iridium Sprinkler Coverage",
        getValue: () => $"{Active.MaxCoverage[SprinklerHelper.SprinklerObjectIds[2]]}",
        setValue: value => UpdateMaxCoverage(Active, SprinklerHelper.SprinklerObjectIds[2], value),
        allowedValues: MaxSprinklerCoverageAllowedValues
      );

      configMenu.AddSectionTitle(mod: Mod, () => "Options:");

      configMenu.AddBoolOption(
        mod: Mod,
        name: () => "Show Placement Coverage",
        tooltip: () => "When checked the Overlay shows Sprinkler/Scarecrow reach when placing.",
        getValue: () => Active.OverlayEnabledOnPlace,
        setValue: value => Active.OverlayEnabledOnPlace = value
      );

      configMenu.AddSectionTitle(mod: Mod, () => "Key Bindings:");

      configMenu.AddKeybind(
        mod: Mod,
        name: () => "Show Config Key",
        tooltip: () => "The key to press to change the boundary of all sprinklers",
        getValue: () => Active.ShowSprinklerEditKey,
        setValue: value => Active.ShowSprinklerEditKey = value
      );

      configMenu.AddKeybind(
        mod: Mod,
        name: () => "Show Overlay Key",
        tooltip: () => "The key to press to show the boundary of the highlighted sprinkler/scarecrow",
        getValue: () => Active.ShowOverlayKey,
        setValue: value => Active.ShowOverlayKey = value
      );
    }
  }
}
