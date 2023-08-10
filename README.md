# Better Sprinklers Updated
A fork of [Better Sprinklers by Maurício Gomes (Speeder)](http://www.nexusmods.com/stardewvalley/mods/41).

The goal of this fork is to maintain and rebuild the current version of Better Sprinklers.

The original mod is a great piece of work, however; the original developer (understandably) is no longer maintaining the mod. I'm currently starting a modded playthrough, so I may as well pick up from where he left off. I'll also no doubt eventually be done too, but endeavour to maintain this copy for as long as possible. Anyone is welcome to fork and continue on in kind.

## Original Description

**Better Sprinklers** by Maurício Gomes (Speeder) is a [Stardew Valley](http://stardewvalley.net/)
mod which lets you edit sprinkler coverage (while adjusting their crafting cost), and lets you view
a sprinkler's or scarecrow's coverage by pointing at it in `F3` mode.

## Contents
* [Install](#install)
* [Use](#use)
* [Compatibility](#compatibility)
* [Versions](#versions)
* [See also](#see-also)

## Install
1. [Install the latest version of SMAPI](https://github.com/Pathoschild/SMAPI/releases).
3. Unzip [the mod files](https://www.nexusmods.com/stardewvalley/mods/17767) into your `Mods` folder.
4. Run the game using SMAPI.

## Use
### Editing sprinkler coverage
* Press `K` (editable in `config.json`) to show a sprinkler coverage editor and click the squares
  to set the coverage. You can use any shape you want, though you can't remove the default squares.
  Press "OK" when you're done, and the mod will automatically adjust the crafting and purchase cost
  for balance.

  > ![](docs/screenshot.png)
  > ![](docs/circle.png)
  > ![](docs/butterfly.png)

* When placing a sprinkler or scarecrow, its coverage will be highlighted automatically. You can also
  press `F3` and point at a sprinkler or scarecrow to highlight its coverage.

  > ![](docs/scarecrowarea.png)

* The sprinklers activate in the morning when the day starts.

## Compatibility
Better Sprinklers is compatible with Stardew Valley 1.3+ on Linux/Mac/Windows, both single-player and multiplayer.

If two players have the mod installed in multiplayer, both ranges will apply.

## Versions
### 2.6.1
- Add Support for [Generic Mod Config Menu](https://github.com/spacechase0/StardewValleyMods/tree/develop/GenericModConfigMenu)
  - Add config option for each price multiplier
  - Add config option for showing/hiding the grid on highlight
  - Add Keybind Changes

### 2.6.0

- Update to `.Net 5`
- Update License to MIT for this fork.
- Update Pathoschild.Stardew.ModBuildConfig to `4.1.1`
- Change TargetPlatform to `Any CPU`
- `SMAPI 4` compatibility:
  - Update `SprinklerMod.cs` to use `Content Interception API`
  - Update `SprinklerMod.cs` to use `helper.GameContent.Load`
  - Update `SprinklerMod.cs` to use `helper.GameContent.InvalidateCache`
  
### 2.4
- Updated for Stardew Valley 1.3 (including multiplayer support) and SMAPI 3.0.
- Added support for controller bindings.
- Added mod-provided API to let other mods access the custom sprinkler coverage.
- Added support for sprinklers inside custom buildings.
- Improved compatibility with other mods that change object/recipe data.

### 2.3
- Corrected forum thread link, and default config minor error.

### 2.2
- Updated for Stardew Valley 1.1 and SMAPI 0.40.0 1.1.

### 2.1
- Added highlighting to the area of sprinklers and scarecrows.
- Added grid rendering.
- Added html readme.
- Fixed config bug (it was always `K` even if you edited the config.json).

### 2.0.1
- Fixed compatibility with SMAPI 0.39.2.

### 2.0
- Updated to SMAPI 0.39.2.
- Added a GUI to configure the sprinklers.
- Sprinklers now work on all farmable areas, including greenhouses and anything added by mods.

