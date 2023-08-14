# Better Sprinklers Updated
![Build and Release](https://github.com/jamescodesthings/smapi-better-sprinklers/actions/workflows/build.yml/badge.svg)

[Releases](https://github.com/jamescodesthings/smapi-better-sprinklers/releases/)

[On Nexus mods](https://www.nexusmods.com/stardewvalley/mods/17767)

A fork of [Better Sprinklers by Maurício Gomes (Speeder)](http://www.nexusmods.com/stardewvalley/mods/41).

Sprinklers, but better.

Originally by Maurício Gomes (Speeder), maintained by [JamesCodesThings](https://codesthings.com).



# Contents
- [Features](#features)
- [Install](#install)
- [Configuration](#configuration)
- [Use](#use)
- [Compatibility](#compatibility)
- [Roadmap](#roadmap)
- [Versions](#versions)
- [Motivation](#motivation)
- [Found a bug?](#found-a-bug)

# Features
## Edit Sprinkler Coverage
Pressing `k` (configurable) allows you to change the coverage of each type of sprinkler.

## View Sprinkler/Scarecrow Coverage
Pressing `F3` (configurable) allows you to show the coverage of the highlighted sprinkler.

## Balanced Mode
In balanced mode, the sprinklers you have cost money every day.

> Q: What!? Did you implement bills in my cosy game?
>
> A: Yeah, I did a little. Don't worry though, you can turn them off, change the amount, etc.
> It felt like the most immersive way to balance the mod. The previous balance increased the cost of sprinklers, but that's one-off.
>
> Q: What if I run out of money?
>
> A: Yeah, your sprinklers stop sprinkling (on the last tile you can afford.
> Same again, configurable.

# Install
1. [Install the latest version of SMAPI](https://github.com/Pathoschild/SMAPI/releases).
2. Unzip [the mod files](https://www.nexusmods.com/stardewvalley/mods/17767) into your `Mods` folder.
3. Run the game using [SMAPI](https://github.com/Pathoschild/SMAPI/releases).

# Configuration
## Balanced Mode
__Off:__ Sprinklers do not have a daily cost.

__Easy:__ Sprinklers cost 0.1g per tile watered, per day.

__Normal:__ Sprinklers cost 0.25g per tile watered, per day.

__Hard:__ Sprinklers cost 0.5g per tile watered, per day.

__Very Hard:__ Sprinklers cost 1g per tile watered, per day.

## Can't Afford
If you can't afford to run the sprinklers:

__Off:__ the sprinklers run, your wallet is emptied.

__Cut Off:__ the sprinklers cut off after watering the tiles you can water.

__Don't Water:__ the sprinklers don't water your crops.

_note:_ at the moment this does not affect the default spinkler tiles(see the roadmap).


## Show Bills Message
__On:__ Every morning you'll see a message for how much your sprinklers cost.

__Off:__ Message is not shown.

## Show Can't Afford Warning
__On:__ If you can't afford to water all your crops one day, you'll get a warning.

__Off:__ Message is not shown.

This one's just for if you want to be warned but don't want the bills message every day.

## Water costs on any tile
__On:__ The water costs money even if the tile is not waterable.

__Off:__ Only waterable tiles cost money.

## Show Overlay Grid
__On:__ When the Coverage is shown, a grid is also shown.

__Off:__ No grid is shown.

## Show Placement Coverage
__On:__ The coverage of a sprinkler/scarecrow is shown when placing it.

__Off:__ No coverage is shown.

This is useful if you only want to use Data Layers.

## Show Config Key
__Use:__ Changes the hotkey to change sprinkler configuration.

## Show Overlay Key
__Use:__ Changes the hotkey to show coverage overlay.

# Use
## Editing sprinkler coverage
1. Press `K` to show a sprinkler coverage editor
  - This is editable in `config.json`, or using [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098)
1. Click the squares to change the area a sprinkler waters.
  - The default squares cannot be changed.

## Highlighting coverage
### When placing a sprinkler
- Coverage will be highlighted by default
- This can be switched off in the mod's config.

### Ad-hoc
1. Press F3
2. Point at a sprinkler or scarecrow.

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

# Roadmap
- [x] (balancing) Option: Do not sprinkle tiles if we run out of money.
- [x] (balancing) Option: Don't water if we can't afford vs Cut out.
- [x] (ui) Message: When watering is skipped/cut out, warn the user
  - [x] Option to enable/disable (using main option)
- [x] Update Mod Screenshots
- [x] (immersion) Same bills cutout with default tiles?
- [ ] (ui) Live Preview of costs on config menu
- [ ] (ui) Click & Drag to change multiple tiles quickly
- [ ] (gameplay) Ability to change maximum range of each sprinkler type.
- [ ] (balancing) Pressure Nozzle should cut cost/do something!
- [ ] (gamplay) Ability to activate one or all sprinklers manually (same cost).
  - [ ] (optional) Or just make sure mod is compatible with [Activated Sprinklers](https://github.com/JudeRV/Stardew-ActivatingSprinklers/blob/master/ActivatingSprinklers/ActivatingSprinklers.cs).
- [ ] (accessibility) i18n basics. 

# Versions
## 2.8.0
- Add options for what happens when you cannot afford to run the sprinklers
- Update readme

## 2.7.1
- Update Documentation.
- Don't show sprinkler cost if `cost = 0`

## 2.7.0
- Gut the old Balanced Mode
- ADD a per-day cost for water, water costs money.
- ADD do not sprinkle tiles if we run out of money for water.
- ADD Options to change difficulty and turn on/off these features

## 2.6.15
- Cleanup the config menu

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
