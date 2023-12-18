# lc-hax

[![version-check.yml](https://github.com/winstxnhdw/lc-hax/actions/workflows/version-check.yml/badge.svg)](https://github.com/winstxnhdw/lc-hax/actions/workflows/version-check.yml)
[![main.yml](https://github.com/winstxnhdw/lc-hax/actions/workflows/main.yml/badge.svg)](https://github.com/winstxnhdw/lc-hax/actions/workflows/main.yml)
[![CodeQL](https://github.com/winstxnhdw/lc-hax/actions/workflows/github-code-scanning/codeql/badge.svg)](https://github.com/winstxnhdw/lc-hax/actions/workflows/github-code-scanning/codeql)
[![dependabot.yml](https://github.com/winstxnhdw/lc-hax/actions/workflows/dependabot.yml/badge.svg)](https://github.com/winstxnhdw/lc-hax/actions/workflows/dependabot.yml)

A powerful, feature-rich and highly performant portable Windows CLI-only internal cheat for [Lethal Company](https://en.wikipedia.org/wiki/Lethal_Company). This mod is partially compatible with [MoreCompany](https://github.com/notnotnotswipez/MoreCompany).

## Requirements

- [Microsoft .NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download)

## Installation

**Recursively** clone the repository.

```bash
git clone --recursive https://github.com/winstxnhdw/lc-hax.git
```

You can now build and install the dependencies by executing `requirements.bat`.

## Usage

Execute `launch.bat` when you have launched the game.

## Features

**No** features in this mod require host privileges.

- Infinite stamina
- Infinite scan range
- Always sane
- Always lightweight
- Always show HUD
- Build anywhere
- Clear vision
- [Commands](#commands)
- [Binds](#binds)

## Commands

| Action                        | Command                    |
| ----------------------------- | -------------------------- |
| Inverse transport a player    | `/random <player>`         |
| Play noise on player          | `/noise <player>`          |
| Chibaku Tensei                | `/ct <player>`             |
| Teleport to entrance          | `/entrance`                |
| Teleport inside entrance      | `/entrance inside`         |
| Teleport to a player          | `/tp <player>`             |
| Teleport to a location        | `/tp <x> <y> <z>`          |
| Teleport player to player     | `/tp <player> <player>`    |
| Teleport player to a location | `/tp <player> <x> <y> <z>` |
| Teleport back to ship         | `/home <player?>`          |
| Lure enemies to player        | `/hate <player>`           |
| Kill player                   | `/kill <player?>`          |
| Kill all players              | `/kill --all`              |
| Modify shovel damage          | `/shovel <force=1>`        |
| Give or take money            | `/money <amount>`          |
| Place an unlockable           | `/build <unlockable>`      |
| Stun enemies                  | `/stun <duration>`         |
| Toggle stun on click          | `/stunclick`               |
| List all players              | `/players`                 |
| Get coordinates               | `/xyz`                     |
| End the game                  | `/end <player?>`           |
| Start the game                | `/start`                   |
| Force everyone to be ready    | `/ready`                   |
| Toggle God mode               | `/god`                     |
| Unlock all doors              | `/unlock`                  |
| Lock all gates                | `/lock`                    |
| Heal self                     | `/heal`                    |
| Explode all mines             | `/explode`                 |
| Explode all jetpacks          | `/explode jet`             |

## Binds

| Action                      | Input  |
| --------------------------  | ------ |
| Trigger various objects     | `M3`   |
| Toggle Phantom              | `=`    |
| Change spectator index      | `⇦`   |
| Change spectator index      | `⇨`   |
| Increase sensitivity        | `]`    |
| Decrease sensitivity        | `[`    |
