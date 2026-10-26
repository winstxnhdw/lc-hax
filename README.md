# lc-hax

[![.NET](https://img.shields.io/badge/.NET%2010.0-512BD4?logo=dotnet&logoColor=fff)](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
![NuGet Version](https://img.shields.io/nuget/v/LethalCompany.GameLibs.Steam?style=flat&label=lc-hax&color=red)
[![main.yml](https://github.com/winstxnhdw/lc-hax/actions/workflows/main.yml/badge.svg)](https://github.com/winstxnhdw/lc-hax/actions/workflows/main.yml)
[![formatter.yml](https://github.com/winstxnhdw/lc-hax/actions/workflows/formatter.yml/badge.svg)](https://github.com/winstxnhdw/lc-hax/actions/workflows/formatter.yml)
[![CodeQL](https://github.com/winstxnhdw/lc-hax/actions/workflows/codeql.yml/badge.svg)](https://github.com/winstxnhdw/lc-hax/actions/workflows/codeql.yml)

A powerful, feature-rich and highly performant portable Windows CLI-only internal cheat for [Lethal Company](https://en.wikipedia.org/wiki/Lethal_Company). This mod is compatible with [MoreCompany](https://github.com/notnotnotswipez/MoreCompany).

> [!IMPORTANT]\
> To aid in its speed of development, `lc-hax` has no concept of versioning aside from the Git commit history. We make no guarantees for any kind of backward compatiblity. Any feature can be removed or heavily modified to maximise user experience and overwrite poor early decisions.

<div align="center">
    <img alt="User in Phantom mode"
         src="https://raw.githubusercontent.com/wiki/winstxnhdw/lc-hax/resources/title.gif"
    />
</div>

## Installation

> [!IMPORTANT]\
> Installation require **Windows 10 or later**.

Using PowerShell, install the required dependenc(ies) with `winget`.

```bash
winget install Git.Git Microsoft.DotNet.SDK.10
```

Re-open the terminal and clone the repository using `git`.

```bash
git clone --depth 10 https://github.com/winstxnhdw/lc-hax.git
```

## Usage

Run or double-click `launch.bat` before or after starting Lethal Company.

```bash
.\launch.bat
```

## Features

`lc-hax` has an extensive list of features. Features are split into two distinct categories: **mods** and **commands**. Mods are features that are always active, while commands are features that are only active for a limited period of time when invoked.

<div align="center">
    <img alt="User in Phantom mode"
         src="https://raw.githubusercontent.com/wiki/winstxnhdw/lc-hax/resources/inverse.gif"
    />
</div>

<br>

The complete feature set includes the following.

- Anti-kick
- Anti-flash
- Auto-reconnect
- Always sane
- Always lightweight
- Always show HUD
- Always keep items on teleport
- Build anywhere
- Connection spoofing
- Clear vision
- Crosshair
- ESP
- Enemy possession with abilities
- Automatic scrap collection
- Grab through walls
- Instant interact
- Infinite stamina
- Infinite scan range
- Infinite ammo
- Infinite charge
- Infinite grab distance
- Infinite item usage
- Infinite item deposit
- No/fake reload
- No item usage cooldown
- No lobby refresh delay
- No build constraints
- No character limit
- No pan limits
- Notify teammate's death
- Pocket any item
- Stable Zap gun
- Join any lobby with a Steam ID
- [Commands](#commands)
- [Binds](#binds)

### Commands

> [!NOTE]\
> **No one** can see the commands you send.

| Action                              | Command                                          |
| ----------------------------------- | ------------------------------------------------ |
| Teleport outside entrance           | `/exit`                                          |
| Teleport inside entrance            | `/enter`                                         |
| Teleport to a player                | `/tp <player>`                                   |
| Teleport player to player           | `/tp <player> <player>`                          |
| Teleport to a location              | `/tp <x> <y> <z>`                                |
| Teleport player to a location       | `/tp <player> <x> <y> <z>`                       |
| Teleports the player to hell        | `/void <player>`                                 |
| Teleport back to ship               | `/home <player?>`                                |
| Teleport enemies to player          | `/mob <player>`                                  |
| Inverse teleport a player           | `/random <player>`                               |
| Play virtual noise on a player      | `/noise <player> <duration=30>`                  |
| Bomb player                         | `/bomb <player>`                                 |
| Bombard player                      | `/bombard <player>`                              |
| Lure enemies to player              | `/hate <player>`                                 |
| Spawn masked enemy on a player      | `/mask <player?> <amount=1>`                     |
| Heal or revive the player           | `/heal <player?>`                                |
| Kill player                         | `/kill <player?>`                                |
| Kill all players                    | `/kill --all`                                    |
| Kill all enemies                    | `/kill --enemy`                                  |
| Kill player with animation          | `/fatality <player> <enemy>`                     |
| Burst a cadaver bloom from a player | `/bloom <target>`                                |
| Poison a player                     | `/poison <player> <damage> <duration> <delay=1>` |
| Poison all players                  | `/poison --all <damage> <duration> <delay=1>`    |
| Spoof a server message              | `/say <player> <message>`                        |
| Send a translated message           | `/translate <language> <message>`                |
| Transmit a signal                   | `/signal <message>`                              |
| Modify shovel damage                | `/shovel <force=1>`                              |
| Add or remove experience            | `/xp <amount>`                                   |
| Buy an item                         | `/buy <item> <quantity=1>`                       |
| Deposit valuable item(s)            | `/sell <quota?>`                                 |
| Grab scrap(s)                       | `/grab <item?>`                                  |
| Destroy held item                   | `/destroy`                                       |
| Destroy all items                   | `/destroy --all`                                 |
| Block any radar targets             | `/block radar`                                   |
| Become untargetable                 | `/block enemy`                                   |
| Place an unlockable                 | `/build <unlockable>`                            |
| Wear a suit                         | `/suit <suit>`                                   |
| Change moons                        | `/visit <moon>`                                  |
| Spin all placeable objects          | `/spin <duration>`                               |
| Stun enemies                        | `/stun <duration>`                               |
| Pull the ship's horn                | `/horn <duration>`                               |
| Toggle stun on click                | `/stunclick`                                     |
| End the game                        | `/end <player=-1>`                               |
| Start the game                      | `/start`                                         |
| List all players                    | `/players`                                       |
| Toggle the Beta badge               | `/beta`                                          |
| Get coordinates                     | `/xyz`                                           |
| Toggle God mode                     | `/god`                                           |
| Toggle NoClip                       | `/noclip`                                        |
| Toggle unlimited jump               | `/jump`                                          |
| Toggle rapid item usage             | `/rapid`                                         |
| Set all objects upright             | `/upright`                                       |
| Eavesdrop on all players            | `/hear`                                          |
| Fake player death                   | `/fakedeath`                                     |
| Unlock all doors                    | `/unlock`                                        |
| Lock all gates                      | `/lock`                                          |
| Open ship door                      | `/open`                                          |
| Close ship door                     | `/close`                                         |
| Trigger the garage doors            | `/garage`                                        |
| Explode all jetpacks                | `/explode`                                       |
| Explode all landmines               | `/explode mine`                                  |
| Upset all turrets                   | `/berserk`                                       |
| Turn invisible to players           | `/invis`                                         |
| Clear the chat for everyone         | `/clear`                                         |
| Toggle ship lights                  | `/light`                                         |
| Copy lobby ID to clipboard          | `/lobby`                                         |

## Privileged Commands

These commands are only available when the user is host.

| Action                        | Command                                          |
| ----------------------------- | ------------------------------------------------ |
| Set the timescale             | `/timescale <scale>`                             |
| Set the quota                 | `/quota <amount> <fulfilled=0>`                  |
| Spawn enem(ies) on player     | `/spawn <enemy> <player> <amount=1>`             |
| Give or take credit(s)        | `/credit <amount>`                               |
| Land ship quickly             | `/land`                                          |
| Eject all players             | `/eject`                                         |

## Special Commands

| Action                          | Command                                        |
| ------------------------------- | ---------------------------------------------- |
| Set the prefix for all commands | `!prefix <prefix>`                             |

### Binds

> [!TIP]\
> You can connect to a specific lobby with <kbd> Shift </kbd> + <kbd> F5 </kbd> if your clipboard contains a valid Steam ID!

| Action                        | Input                                |
| ----------------------------- | ------------------------------------ |
| Trigger various objects       | <kbd> M3 </kbd>                      |
| Interact at a distance        | <kbd> M3 </kbd> + <kbd> E </kbd>     |
| Toggle ESP                    | <kbd> Pause </kbd>                   |
| Toggle Phantom                | <kbd> = </kbd>                       |
| Toggle AutoPlay                | <kbd> F6 </kbd>                      |
| Teleport to Phantom           | <kbd> Shift </kbd> + <kbd> = </kbd>  |
| Change spectator index        | <kbd> ⇽ </kbd>                      |
| Change spectator index        | <kbd> ⇾ </kbd>                      |
| Increase mouse sensitivity    | <kbd> ] </kbd>                       |
| Decrease mouse sensitivity    | <kbd> [ </kbd>                       |
| Decrease light intensity      | <kbd> F4 </kbd>                      |
| Increase light intensity      | <kbd> F5 </kbd>                      |
| Toggle Anti-Kick              | <kbd> \ </kbd>                       |
| Disconnect from server        | <kbd> Shift </kbd> + <kbd> F4 </kbd> |
| Connect to last joined server | <kbd> Shift </kbd> + <kbd> F5 </kbd> |

### AutoPlayMod

Press <kbd> F6 </kbd> to toggle AutoPlay. It collects reachable scrap until the inventory is full, returns to the ship, unloads, and repeats. When it runs out of eligible targets, it returns home and checks for more. With five in-game minutes remaining before automatic departure, including early departure votes, it heads home and stays aboard. It follows walking routes, opens unlocked doors, and uses facility entrances. Routes requiring ladders, jumps, or elevators are unsupported, as are locked doors and enemy avoidance. Ship access can also require those routes on some moons. It skips unreachable scrap and reports a blocked route if it cannot get home. Route coverage and multiplayer behaviour need in-game testing.

### Possession

Possession has its own set of keybinds. Enable Phantom with <kbd> = </kbd>, point at an enemy and press <kbd> M3 </kbd> to possess it. Enemies that can use entrances pass through automatically when close and facing the door.

| Action                        | Input                                |
| ----------------------------- | ------------------------------------ |
| Use primary ability           | <kbd> M1 </kbd>                      |
| Use secondary ability         | <kbd> M2 </kbd>                      |
| Toggle possession NoClip      | <kbd> N </kbd>                       |
| Unpossess enemy               | <kbd> Z </kbd>                       |
| Kill and unpossess enemy      | <kbd> Del </kbd>                     |

### TriggerMod

`TriggerMod` is a quality-of-life mod that allows you to trigger various objects in the game. It is activated by pointing at objects with `M3`. Currently, it can do the following.

- Explode landmines
- Explode unequipped jetpacks
- Trigger turrets
- Toggle gates
- Unlock doors
- Trigger Jeb's attack
- Lure enemies to player
- Interact with objects from far
- Possess enemies (only in `Phantom`)

### Anti-Kick

If another player joins after you, Anti-Kick immediately leaves and makes one reconnect attempt. This only runs while the initial lobby exists, before game start, with the start lever unpulled and the ship not routing to another moon. Player identities use the server's slot assignments. Before a roster update arrives, only your own identity and the host's identity can be restored reliably.

### Fatality

The `fatality` command allows you to kill a player with a custom animation. The list of supported enemies are:

- Forest Giant
- Jester
- Masked
- Circuit Bees
- Eyeless Dog
- Bracken
- Nutcracker
- Maneater

### Build

The arguments for the `build` command can be executed with either the unlockable index or name. The list of unlockables can be found in the following.

> [!NOTE]\
> Unintuitively, not all unlockables are buildable. Thus, the `build` command also doubles as a pseudo unlock function for unlockables.

- Orange Suit
- Green Suit
- Hazard Suit
- Pajama Suit
- Cozy Lights
- Teleporter
- Television
- Cupboard
- File Cabinet
- Toilet
- Shower
- Light Switch
- Record Player
- Table
- Romantic Table
- Bunkbeds
- Terminal
- Signal Transmitter
- Loud Horn
- Inverse Teleporter
- Jack O' Lantern
- Welcome Mat
- Goldfish
- Plushie Pajama Man
- Purple Suit

## Settings

Settings in `lc-hax` are configurable at compile-time. Edit [Settings.xml](Settings.xml) to choose which mods and patches are included and adjust values such as crosshair size, brightness, and flight speed. Any modifications to the settings file will only take effect after relaunching `lc-hax`.

## Uninstalling

Unlike other cheats, `lc-hax` is portable and will never litter your computer with garbage. Deleting the `lc-hax` folder would be the equivalent to uninstalling.

## Alternatives

If `lc-hax` is not your fancy, here are excellent alternatives that may suit your needs!

### Project Apparatus

[Project Apparatus](https://github.com/KaylinOwO/Project-Apparatus) is a popular GUI-first internal cheat based on Infinite Company.

### Lethal Menu

[Lethal Menu](https://github.com/IcyRelic/LethalMenu) is a powerful GUI-first internal cheat based on `Project Apparatus` and `lc-hax`. It supports chams, super speed, fast climbing and more.

## Credits

A big thanks to every contributor of this project and to [Quesoteric](https://github.com/Quesoteric) for the title GIF.
