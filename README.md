# lc-hax

[![version-check.yml](https://github.com/winstxnhdw/lc-hax/actions/workflows/version-check.yml/badge.svg)](https://github.com/winstxnhdw/lc-hax/actions/workflows/version-check.yml)
[![main.yml](https://github.com/winstxnhdw/lc-hax/actions/workflows/main.yml/badge.svg)](https://github.com/winstxnhdw/lc-hax/actions/workflows/main.yml)
[![CodeQL](https://github.com/winstxnhdw/lc-hax/actions/workflows/github-code-scanning/codeql/badge.svg)](https://github.com/winstxnhdw/lc-hax/actions/workflows/github-code-scanning/codeql)
[![dependabot.yml](https://github.com/winstxnhdw/lc-hax/actions/workflows/dependabot.yml/badge.svg)](https://github.com/winstxnhdw/lc-hax/actions/workflows/dependabot.yml)

A powerful, feature-rich and highly performant portable Windows CLI-only internal cheat for [Lethal Company](https://en.wikipedia.org/wiki/Lethal_Company). This mod is partially compatible with [MoreCompany](https://github.com/notnotnotswipez/MoreCompany).

> [!IMPORTANT]\
> To aid in its speed of development, `lc-hax` has no concept of versioning aside from the Git commit history. We make no guarantees for any kind of backward compatiblity. Any feature can be removed or heavily modified to maximise user experience and overwrite poor early decisions.

<div align="center">
    <img src="https://raw.githubusercontent.com/wiki/winstxnhdw/lc-hax/resources/inverse.gif" />
</div>

## Requirements

- [Microsoft .NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download)

## Installation

> [!IMPORTANT]\
> Do ensure that you have used the `--recursive` flag when cloning the repository.

**Recursively** clone the repository.

```bash
git clone --recursive https://github.com/winstxnhdw/lc-hax.git
```

You can now build and install the dependencies by executing `requirements.bat`.

```bash
.\requirements.bat
```

## Usage

Execute `launch.bat` when you have launched the game to inject the assembly into the game process. Avoid doing this more than once per instance.

```bash
.\launch.bat
```

## Features

> [!NOTE]\
> There is **no** feature in this mod that require host privileges.

`lc-hax` has an extensive list of features. Features are split into two distinct categories: **mods** and **commands**. Mods are features that are always active, while commands are features that are only active for a limited period of time when invoked.

The complete feature set includes the following.

- Infinite stamina
- Infinite scan range
- Infinite ammo
- Always sane
- Always lightweight
- Always show HUD
- Build anywhere
- Clear vision
- Show player's name in HUD
- [Commands](#commands)
- [Binds](#binds)

### Commands

| Action                        | Command                         |
| ----------------------------- | ------------------------------- |
| Inverse teleport a player     | `/random <player>`              |
| Play noise on player          | `/noise <player> <duration=30>` |
| Chibaku Tensei                | `/ct <player>`                  |
| Teleport to entrance          | `/entrance`                     |
| Teleport inside entrance      | `/entrance inside`              |
| Teleport to a player          | `/tp <player>`                  |
| Teleport to a location        | `/tp <x> <y> <z>`               |
| Teleport player to player     | `/tp <player> <player>`         |
| Teleport player to a location | `/tp <player> <x> <y> <z>`      |
| Teleport back to ship         | `/home <player?>`               |
| Teleport enemies to player    | `/mob <player>`                 |
| Lure enemies to player        | `/hate <player>`                |
| Kill player                   | `/kill <player?>`               |
| Kill all players              | `/kill --all`                   |
| Kill all enemies              | `/kill --enemy`                 |
| Kill all enemies on client    | `/kill --localenemy`            |
| Modify shovel damage          | `/shovel <force=1>`             |
| Give or take money            | `/money <amount>`               |
| Place an unlockable           | `/build <unlockable>`           |
| Stun enemies                  | `/stun <duration>`              |
| Toggle stun on click          | `/stunclick`                    |
| End the game                  | `/end <player=-1>`              |
| Start the game                | `/start`                        |
| List all players              | `/players`                      |
| Get coordinates               | `/xyz`                          |
| Toggle God mode               | `/god`                          |
| Unlock all doors              | `/unlock`                       |
| Lock all gates                | `/lock`                         |
| Heal self                     | `/heal`                         |
| Explode all mines             | `/explode`                      |
| Explode all jetpacks          | `/explode jet`                  |

### Binds

| Action                      | Input      |
| --------------------------  | ---------- |
| Trigger various objects     | `M3`       |
| Follow player               | `M3` + `F` |
| Interact at a distance      | `M3` + `E` |
| Funny respawn mobs          | `M3` + `R` |
| Toggle Phantom              | `=`        |
| Change spectator index      | `⇦`        |
| Change spectator index      | `⇨`        |
| Increase sensitivity        | `]`        |
| Decrease sensitivity        | `[`        |

### TriggerMod

`TriggerMod` is a quality-of-life mod that allows you to trigger various objects in the game. It is activated by pointing at objects with `M3`. Currently, it can do the following.

- Explode landmines
- Explode jetpacks
- Trigger turrets
- Toggle gates
- Unlock doors
- Trigger Jeb's attack
- Lure enemies to player
- Follow a player's movements
- Interact with objects from far

### Build

The arguments for the `build` command can be executed with either the unlockable index or name. The list of unlockables can be found in the following table.

> [!NOTE]\
> Unintuitively, not all unlockables are buildable. Thus, the `build` command also doubles as a pseudo unlock function for unlockables.

| ID | Name               |
|--- |--------------------|
| 0  | ORANGE_SUIT        |
| 1  | GREEN_SUIT         |
| 2  | HAZARD_SUIT        |
| 3  | PAJAMA_SUIT        |
| 4  | COZY_LIGHTS        |
| 5  | TELEPORTER         |
| 6  | TELEVISION         |
| 7  | CUPBOARD           |
| 8  | FILE_CABINET       |
| 9  | TOILET             |
| 10 | SHOWER             |
| 11 | LIGHT_SWITCH       |
| 12 | RECORD_PLAYER      |
| 13 | TABLE              |
| 14 | ROMANTIC_TABLE     |
| 15 | BUNKBEDS           |
| 16 | TERMINAL           |
| 17 | SIGNAL_TRANSMITTER |
| 18 | LOUD_HORN          |
| 19 | INVERSE_TELEPORTER |
| 20 | JACK_O_LANTERN     |
| 21 | WELCOME_MAT        |
| 22 | GOLDFISH           |
| 23 | PLUSHIE_PAJAMA_MAN |

### Disabling Mods

You may permanently disable mods by removing the corresponding `Mod` class under the `LoadHaxModules` function from the following [file](lc-hax/Scripts/Loader.cs). In the following example, we disable `StaminaMod` and you will return to the game's default stamina mechanics in your next injection.

```cs
static void LoadHaxModules() {
    DontDestroyOnLoad(Loader.HaxModules);

    Loader.AddHaxModules<SaneMod>();
    Loader.AddHaxModules<ChatMod>();
    Loader.AddHaxModules<StunMod>();
    Loader.AddHaxModules<ShovelMod>();
    Loader.AddHaxModules<WeightMod>();
    // Loader.AddHaxModules<StaminaMod>();
    Loader.AddHaxModules<PhantomMod>();
    Loader.AddHaxModules<TriggerMod>();
    Loader.AddHaxModules<ClearVisionMod>();
    Loader.AddHaxModules<NameInWeightMod>();
    Loader.AddHaxModules<BuildAnywhereMod>();
    Loader.AddHaxModules<FollowAnotherPlayerMod>();
}
```
