# lc-hax

[![version-check.yml](https://github.com/winstxnhdw/lc-hax/actions/workflows/version-check.yml/badge.svg)](https://github.com/winstxnhdw/lc-hax/actions/workflows/version-check.yml)
[![main.yml](https://github.com/winstxnhdw/lc-hax/actions/workflows/main.yml/badge.svg)](https://github.com/winstxnhdw/lc-hax/actions/workflows/main.yml)
[![formatter.yml](https://github.com/winstxnhdw/lc-hax/actions/workflows/formatter.yml/badge.svg)](https://github.com/winstxnhdw/lc-hax/actions/workflows/formatter.yml)
[![CodeQL](https://github.com/winstxnhdw/lc-hax/actions/workflows/github-code-scanning/codeql/badge.svg)](https://github.com/winstxnhdw/lc-hax/actions/workflows/github-code-scanning/codeql)
[![dependabot.yml](https://github.com/winstxnhdw/lc-hax/actions/workflows/dependabot.yml/badge.svg)](https://github.com/winstxnhdw/lc-hax/actions/workflows/dependabot.yml)

A powerful, feature-rich and highly performant portable Windows CLI-only internal cheat for [Lethal Company](https://en.wikipedia.org/wiki/Lethal_Company). This mod is compatible with [MoreCompany](https://github.com/notnotnotswipez/MoreCompany).

> [!IMPORTANT]\
> To aid in its speed of development, `lc-hax` has no concept of versioning aside from the Git commit history. We make no guarantees for any kind of backward compatiblity. Any feature can be removed or heavily modified to maximise user experience and overwrite poor early decisions.

<div align="center">
    <img alt="User in Phantom mode"
         src="https://raw.githubusercontent.com/wiki/winstxnhdw/lc-hax/resources/title.gif"
    />
</div>

## Requirements

- [Microsoft .NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download)
- [Git](https://git-scm.com/download/win)

## Installation

Clone the repository.

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
- Infinite charge
- Infinite grab distance
- Infinite item usage
- Infinite item deposit
- Grab through walls
- Pocket any item
- Instant interact
- No reload
- No item usage cooldown
- No lobby refresh delay
- No build constraints
- Always sane
- Always lightweight
- Always show HUD
- Always keep items on teleport
- Stable Zap gun
- Clear vision
- Crosshair
- ESP
- [Commands](#commands)
- [Binds](#binds)

### Commands

> [!NOTE]\
> **No one** can see the commands you send.

| Action                        | Command                                        |
| ----------------------------- | ---------------------------------------------- |
| Teleport outside entrance     | `/exit`                                        |
| Teleport inside entrance      | `/enter`                                       |
| Teleport to a player          | `/tp <player>`                                 |
| Teleport to a location        | `/tp <x> <y> <z>`                              |
| Teleport back to ship         | `/home <player?>`                              |
| Teleport enemies to player    | `/mob <player>`                                |
| Inverse teleport a player     | `/random <player>`                             |
| Chibaku Tensei                | `/ct <player>`                                 |
| Play noise on player          | `/noise <player> <duration=30>`                |
| Lure enemies to player        | `/hate <player>`                               |
| Spawn masked enemy on player  | `/mask <player>`                               |
| Heal player                   | `/heal <player?>`                              |
| Kill player                   | `/kill <player?>`                              |
| Kill all players              | `/kill --all`                                  |
| Kill all enemies              | `/kill --enemy`                                |
| Kill player with animation    | `/fatality <player> <enemy>`                   |
| Poison a player               | `/poison <player> <damage> <delay> <duration>` |
| Spoof a server message        | `/say <player> <message>`                      |
| Transmit a signal             | `/signal <message>`                            |
| Modify shovel damage          | `/shovel <force=1>`                            |
| Give or take money            | `/money <amount>`                              |
| Add or remove experience      | `/xp <amount>`                                 |
| Buy an item                   | `/buy <item> <quantity=1>`                     |
| Deposit valuable item(s)      | `/sell <quota?>`                               |
| Grab scrap(s)                 | `/grab <item?>`                                |
| Block incoming credits        | `/block credit`                               |
| Block any radar targets       | `/block radar`                                 |
| Become untargetable           | `/block enemy`                                 |
| Place an unlockable           | `/build <unlockable>`                          |
| Change moons                  | `/visit <moon>`                                |
| Stun enemies                  | `/stun <duration>`                             |
| Pull the ship's horn          | `/horn <duration>`                             |
| Toggle stun on click          | `/stunclick`                                   |
| Toggle enemy hit on click     | `/hitclick`                                    |
| End the game                  | `/end <player=-1>`                             |
| Start the game                | `/start`                                       |
| List all players              | `/players`                                     |
| Toggle the Beta badge         | `/beta`                                        |
| Get coordinates               | `/xyz`                                         |
| Toggle God mode               | `/god`                                         |
| Toggle rapid item usage       | `/rapid`                                       |
| Unlock all doors              | `/unlock`                                      |
| Lock all gates                | `/lock`                                        |
| Open ship door                | `/open`                                        |
| Close ship door               | `/close`                                       |
| Trigger the garage doors      | `/garage`                                      |
| Explode all jetpacks          | `/explode`                                     |
| Explode all landmines         | `/explode mine`                                |
| Upset all turrets             | `/berserk`                                     |
| Turn invisible to players     | `/invis`                                       |

### Binds

| Action                     | Input                                |
| -------------------------- | ------------------------------------ |
| Trigger various objects    | <kbd> M3 </kbd>                      |
| Follow player              | <kbd> M3 </kbd> + <kbd> F </kbd>     |
| Interact at a distance     | <kbd> M3 </kbd> + <kbd> E </kbd>     |
| Funny respawn mobs         | <kbd> M3 </kbd> + <kbd> R </kbd>     |
| Toggle Phantom             | <kbd> = </kbd>                       |
| Teleport to Phantom        | <kbd> + </kbd>                       |
| Change spectator index     | <kbd> ⇽ </kbd>                      |
| Change spectator index     | <kbd> ⇾ </kbd>                      |
| Increase mouse sensitivity | <kbd> ] </kbd>                       |
| Decrease mouse sensitivity | <kbd> [ </kbd>                       |
| Unpossess enemy            | <kbd> Z </kbd>                       |
| Toggle realistic movement  | <kbd> X </kbd>                       |
| Toggle possession NoClip   | <kbd> N </kbd>                       |
| Toggle Anti-Kick           | <kbd> \ </kbd>                       |

### TriggerMod

`TriggerMod` is a quality-of-life mod that allows you to trigger various objects in the game. It is activated by pointing at objects with `M3`. Currently, it can do the following.

- Explode landmines
- Explode unequipped jetpacks
- Trigger turrets
- Toggle gates
- Unlock doors
- Trigger Jeb's attack
- Lure enemies to player
- Follow a player's movements
- Interact with objects from far
- Possess enemies (only in `Phantom`)

### Fatality

The `fatality` command allows you to kill a player with a custom animation. The list of supported enemies are:

- Forest Giant
- Jester
- Masked
- Baboon Hawk
- Circuit Bee
- Thumper
- Eyeless Dog
- Bracken
- Nutcracker

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

## Whitelisting SharpMonoInjectorCore

Despite being compiled from source locally, SharpMonoInjector is occasionally falsely identified as a virus by Windows Defender. If you believe you know what you are doing, you can run the following command in PowerShell with administrator privileges to whitelist the injector.

> [!IMPORTANT]\
> Do ensure that you are currently in the `lc-hax` directory before executing the command.

```ps1
Add-MpPreference -ExclusionPath $pwd, "$env:TEMP\.net\SharpMonoInjector"
```

## Credits

Thanks to [Quesoteric](https://github.com/Quesoteric) for the title GIF.
