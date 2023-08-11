# Better Sprinklers Updated
![Build and Release](https://github.com/jamescodesthings/smapi-better-sprinklers/actions/workflows/build.yml/badge.svg)

[Releases](https://github.com/jamescodesthings/smapi-better-sprinklers/releases/)
[On Nexus mods](https://www.nexusmods.com/stardewvalley/mods/17767)

A fork of [Better Sprinklers by Maurício Gomes (Speeder)](http://www.nexusmods.com/stardewvalley/mods/41).

Sprinklers, but better.

Originally by Maurício Gomes (Speeder), maintained by [JamesCodesThings](https://codesthings.com).

## Additional Features
- Updated to latest versions of SMAPI and dotnet 5.
- Added Comaptibility with [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098)
- Added config options to give you more control.
- Controller Support

# Contents
- [Install](#install)
- [Use](#use)
- [Compatibility](#compatibility)
- [Versions](#versions)
- [Motivation](#motivation)
- [Found a bug?](#found-a-bug)

# Install
1. [Install the latest version of SMAPI](https://github.com/Pathoschild/SMAPI/releases).
2. Unzip [the mod files](https://www.nexusmods.com/stardewvalley/mods/17767) into your `Mods` folder. 
3. Run the game using [SMAPI](https://github.com/Pathoschild/SMAPI/releases).

# Use
## Notes
- The sprinklers activate in the morning when the day starts.
- The Cost of building/purchasing sprinklers is increased to balance gameplay.
  - This is editable in `config.json`, or using [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098)

## Editing sprinkler coverage
1. Press `K` to show a sprinkler coverage editor
  - This is editable in `config.json`, or using [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098)
1. Click the squares to change the area a sprinkler waters.
  - The default squares cannot be changed.
> ![](docs/screenshot.png)

## Highlighting coverage
### When placing a sprinkler
- Coverage will be highlighted by default
- This can be switched off in the mod's config.

### Ad-hoc
1. Press F3
2. Point at a sprinkler or scarecrow.
> ![](docs/scarecrowarea.png)

### Alternative
Alternatively, you can use [Data Layers](https://www.nexusmods.com/stardewvalley/mods/1691) to show coverage of sprinklers, scarecrows, and much more.

#### Note: Data Layers Compatibility
I'm currently waiting for a PR merge and release of Data Layers.

Until then there's a copy of Data Layers with the PR code:
[On the Release Page for 2.6.13](https://github.com/jamescodesthings/smapi-better-sprinklers/releases/tag/2.6.13)
[On the Nexus Mods Page](https://www.nexusmods.com/stardewvalley/mods/17767)

# Compatibility
Better Sprinklers is compatible with Stardew Valley 1.3+ on Linux/Mac/Windows, both single-player and multiplayer.

If two players have the mod installed in multiplayer, both ranges will apply.

Some mod incompatibilities have been observed, particularly mods that change sprinkler behaviour.

It's out of scope of my aims to improve this. But, I'll happily accept and consider checking out bug reports.

## Versions
## 2.6.14
- Fix divide by zero error (caused by me)
- Improve config options for coverage based balance
- Update default config options to match what (should?) be the default multipliers

## 2.6.13
- Add a copy of Data Layers with integration
- Raised PR with Data Layers to add support

## 2.6.12
- Moved to CI
- Fix Controller placement tile overlay

## 2.6.3 - 2.6.11
- Bug fixes and CI testing

## 2.6.2
- Fix placement overlay when switching from mouse to controller

## 2.6.1
- Add Support for [Generic Mod Config Menu](https://github.com/spacechase0/StardewValleyMods/tree/develop/GenericModConfigMenu)
  - Add config option for each price multiplier
  - Add config option for showing/hiding the grid on highlight
  - Add Keybind Changes

## 2.6.0

- Update to `.Net 5`
- Update License to MIT for this fork.
- Update Pathoschild.Stardew.ModBuildConfig to `4.1.1`
- Change TargetPlatform to `Any CPU`
- `SMAPI 4` compatibility:
  - Update `SprinklerMod.cs` to use `Content Interception API`
  - Update `SprinklerMod.cs` to use `helper.GameContent.Load`
  - Update `SprinklerMod.cs` to use `helper.GameContent.InvalidateCache`
  
## 2.4
- Updated for Stardew Valley 1.3 (including multiplayer support) and SMAPI 3.0.
- Added support for controller bindings.
- Added mod-provided API to let other mods access the custom sprinkler coverage.
- Added support for sprinklers inside custom buildings.
- Improved compatibility with other mods that change object/recipe data.

## 2.3
- Corrected forum thread link, and default config minor error.

## 2.2
- Updated for Stardew Valley 1.1 and SMAPI 0.40.0 1.1.

## 2.1
- Added highlighting to the area of sprinklers and scarecrows.
- Added grid rendering.
- Added html readme.
- Fixed config bug (it was always `K` even if you edited the config.json).

## 2.0.1
- Fixed compatibility with SMAPI 0.39.2.

## 2.0
- Updated to SMAPI 0.39.2.
- Added a GUI to configure the sprinklers.
- Sprinklers now work on all farmable areas, including greenhouses and anything added by mods.

# Motivation
A fork of [Better Sprinklers by Maurício Gomes (Speeder)](http://www.nexusmods.com/stardewvalley/mods/41).

The goal of this fork is to maintain and rebuild the current version of Better Sprinklers.

The original developer has understandably lost touch with the mod. 

I was starting a modded playthrough and loved the mod, but noticed some bugs shared by the wider community.

I decided to pick up the torch while I'm playing Stardew and give a little back.
I'm Updating the mod, fixing the bugs I'm aware of, and adding some features I'd like to see.

**r/iama:** Senior Software Engineer in the midlands in the UK. A loving father and when the kids are asleep I get a chance to play some games.

You can find more about me at [CodesThings.com](https://codesthings.com).

# Found a Bug?
Please report it in [Github Issues](https://github.com/jamescodesthings/smapi-better-sprinklers/issues).