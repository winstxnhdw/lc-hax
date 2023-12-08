# lc-hax

[![main.yml](https://github.com/winstxnhdw/lc-hax/actions/workflows/main.yml/badge.svg)](https://github.com/winstxnhdw/lc-hax/actions/workflows/main.yml)
[![dependabot.yml](https://github.com/winstxnhdw/lc-hax/actions/workflows/dependabot.yml/badge.svg)](https://github.com/winstxnhdw/lc-hax/actions/workflows/dependabot.yml)

Probably the most creative portable Windows CLI-only internal cheat for [Lethal Company](https://en.wikipedia.org/wiki/Lethal_Company). No features in this mod require host privileges.

## Requirements

- [Microsoft .NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download)

## Installation

```bash
git clone --recursive https://github.com/winstxnhdw/lc-hax.git
```

You can now build and install the dependencies by executing `requirements.bat`.

## Usage

Execute `launch.bat` when you have launched the game.

## Commands

| Action                     | Command                 |
| -------------------------- | ----------------------- |
| Modify shovel damage       | `/shovel <force=1>`     |
| Teleport to a player       | `/tp <player>`          |
| Teleport to a location     | `/tp <x> <y> <z>`       |
| Teleport player to player  | `/tp <player> <player>` |
| Teleport back to ship      | `/home <player?>`       |
| Give or take money         | `/money <amount>`       |
| Inverse transport a player | `/random <player>`      |
| Play noise on player       | `/noise <player>`       |
| Kill player                | `/kill <player>`        |
| Kill all players           | `/kill --all`           |
| Kill self                  | `/kill`                 |
| List all players           | `/players`              |
| End the game               | `/end`                  |
| Toggle God mode            | `/god`                  |
| Unlock all doors           | `/unlock`               |
| Stun enemies               | `/stun`                 |
| Heal self                  | `/heal`                 |
| Explode all mines          | `/explode`              |
| Explode all jetpacks       | `/explode jet`          |
