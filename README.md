# SpellForge Showdown

A first-person multiplayer magic combat game built with Unity 6. Players wield elemental spells, capture control points, and battle waves of enemies in an action-packed arena experience.

![Unity Version](https://img.shields.io/badge/Unity-6000.2.6f2-blue)
![License](https://img.shields.io/badge/License-MIT-green)

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Getting Started](#getting-started)
- [Gameplay](#gameplay)
- [Element System](#element-system)
- [Controls](#controls)
- [Project Structure](#project-structure)
- [Scripts Overview](#scripts-overview)
- [Networking](#networking)
- [Contributing](#contributing)

---

## Overview

**SpellForge Showdown** is a round-based, first-person shooter where players harness the power of elemental magic to defeat enemies and capture strategic points. The game features a unique element crafting system, multiple spell types with distinct behaviors, and wave-based PvE combat with multiplayer support.

## Features

- **Elemental Combat System** - 8 unique elements with different weapon behaviors
- **Element Crafting** - Combine elements to create powerful new spells (Fire + Water = Steam)
- **Round-Based Gameplay** - Progressive difficulty with scaling enemy waves
- **Capture Point Mechanics** - Strategic control point objectives
- **First-Person Movement** - Smooth FPS controls with sprint, jump, and external forces
- **Multiplayer Support** - Lobby system with room-based matchmaking
- **Visual Effects** - Particle-based spell effects and impact visuals

---

## Getting Started

### Prerequisites

- **Unity 6** (Version 6000.2.6f2 or later)
- Git (for version control)

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/7bubba10/SpellForge-Showdown.git
   ```

2. Open the project in Unity Hub and select Unity 6000.2.6f2

3. Open the main scene:
   - Navigate to `Assets/Scenes/`
   - Open `MainScene.unity` or `TestScene.unity`

4. Press Play to test the game in the editor

### Build

1. Go to **File > Build Settings**
2. Select your target platform
3. Click **Build and Run**

---

## Gameplay

### Game Flow

1. **Round Start** - A capture point activates on the map
2. **Combat Phase** - Enemies spawn and chase players
3. **Capture Objective** - Stand on the capture point to accumulate progress
4. **Round Complete** - Fill the capture bar and eliminate remaining enemies
5. **Next Round** - Difficulty increases with more enemies and higher capture goals

### Objectives

- **Primary**: Capture control points by standing in the capture zone
- **Secondary**: Survive enemy waves and defeat all enemies
- **Score**: Earn points through capturing and combat

---

## Element System

SpellForge Showdown features 8 unique elements, each with distinct combat properties:

### Base Elements

| Element | Damage | Speed | Behavior | Spell Name |
|---------|--------|-------|----------|------------|
| **Fire** | 15 | 5 | Automatic, high fire rate | Flame Blast |
| **Earth** | 10 | 7 | Standard projectile | Stone Barrage |
| **Air** | 5 | 20 | Automatic, very fast | Gale Shot |
| **Water** | 20 | 12 | Sniper, high damage | Tidal Pierce |

### Advanced Elements

| Element | Damage | Speed | Behavior | How to Obtain |
|---------|--------|-------|----------|---------------|
| **Steam** | 25 | 12 | Charged shot | Craft: Fire + Water |
| **Ice** | 40 | 30 | High damage, slow fire rate | Pickup |
| **Lightning** | 30 | AOE | Area of effect | Teammate ability |
| **Shadow** | 25 | 20 | Bouncing void projectile | Pickup |

### Element Inventory

- Players can hold **2 elements** at a time (Slot A and Slot B)
- Switch between equipped elements with **Q**
- Craft new elements with **F** (when conditions are met)
- Pick up new elements by holding **E** near element pickups

---

## Controls

### Movement
| Key | Action |
|-----|--------|
| W/A/S/D | Move |
| Left Shift | Sprint |
| Space | Jump |
| Mouse | Look around |

### Combat
| Key | Action |
|-----|--------|
| Left Mouse | Fire/Charge spell |
| Right Mouse | (Reserved) |
| R | Reload |
| Q | Switch element |
| F | Craft element |

### Interaction
| Key | Action |
|-----|--------|
| E (Hold) | Pick up element |
| Escape | Unlock cursor |

---

## Project Structure

```
SpellForge-Showdown/
├── Assets/
│   ├── Scripts/           # C# game scripts
│   │   ├── Networking/    # Multiplayer/lobby scripts
│   │   └── *.cs           # Core gameplay scripts
│   ├── Scenes/            # Unity scenes
│   │   ├── MainScene.unity
│   │   ├── TestScene.unity
│   │   └── LobbyMenu.unity
│   ├── Resources/         # Runtime-loaded assets
│   ├── Prefabs/           # Reusable game objects
│   └── [Asset Packs]/     # Third-party assets
├── Packages/              # Unity package dependencies
├── ProjectSettings/       # Unity project configuration
└── README.md
```

---

## Scripts Overview

### Core Systems

| Script | Description |
|--------|-------------|
| `GameManager.cs` | Singleton managing rounds, enemy spawning, and game state |
| `Health.cs` | Player health system with death handling |
| `PlayerScore.cs` | Score tracking and capture progress |
| `ScoreManager.cs` | Global score management |

### Player

| Script | Description |
|--------|-------------|
| `FirstPersonController.cs` | FPS movement with sprint, jump, and external forces |
| `PlayerElementManager.cs` | Element inventory, switching, and crafting |
| `PlayerHUD.cs` | UI display for health, mana, and spell info |
| `PlayerLook.cs` | Camera/mouse look handling |

### Combat

| Script | Description |
|--------|-------------|
| `WeaponRaycast.cs` | Projectile spawning and firing logic |
| `ElementWeaponProperties.cs` | Weapon configuration (pellets, spread, fire rate) |
| `MagicProjectile.cs` | Standard projectile behavior with VFX |
| `VoidProjectile.cs` | Bouncing shadow projectile |
| `Hitscan.cs` | Instant hit detection |

### Elements & Spells

| Script | Description |
|--------|-------------|
| `ElementPickup.cs` | Collectible element pickups |
| `AirSpellCaster.cs` | Air element specific behavior |
| `FireShotgun.cs` | Fire element shotgun variant |
| `AirBurstAOE.cs` | Air burst area effect |
| `MagicAOE.cs` | Generic area of effect damage |
| `AOESpawner.cs` | AOE effect spawning |

### Enemies

| Script | Description |
|--------|-------------|
| `EnemyAI.cs` | NavMesh-based enemy AI with detection and combat |
| `EnemyHealth.cs` | Enemy health and death handling |
| `EnemySpawner.cs` | Enemy wave spawning |

### Capture Points

| Script | Description |
|--------|-------------|
| `CapturePointController.cs` | Manages multiple capture points per round |
| `SimpleCaptureZone.cs` | Trigger-based capture zone logic |
| `CapturePointLogic.cs` | Additional capture mechanics |

### Networking

| Script | Description |
|--------|-------------|
| `LobbyManager.cs` | Create and join lobbies via REST API |
| `LobbyBrowser.cs` | UI for browsing available lobbies |
| `LobbyItem.cs` | Individual lobby list item |
| `MatchStarter.cs` | Match initialization |
| `SocketRunner.cs` | WebSocket connection handling |
| `NetworkUI.cs` | Network status UI |

### UI

| Script | Description |
|--------|-------------|
| `ScoreUI.cs` | Score display |
| `ScoreBarHUD.cs` | Capture progress bar |
| `RoundAnnouncement.cs` | Round start/end announcements |
| `NotificationUI.cs` | In-game notifications |
| `LoadingBarStraight.cs` | Loading progress UI |

### Utilities

| Script | Description |
|--------|-------------|
| `UpdraftZone.cs` | Applies upward force to players |
| `CubeSpawner.cs` | Debug/test object spawner |
| `ActiveCameraForOwner.cs` | Network camera ownership |

---

## Networking

SpellForge Showdown includes a lobby-based multiplayer system:

### Server Configuration

The game connects to a backend server for lobby management:

```csharp
public string serverUrl = "http://localhost:3003/api/lobbies";
```

### API Endpoints

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/api/lobbies/create` | POST | Create a new lobby |
| `/api/lobbies/join` | POST | Join existing lobby by room code |

### Setting Up Multiplayer

1. Start the backend server (separate repository)
2. Update `serverUrl` in `LobbyManager.cs`
3. Use the Lobby Menu scene to create/join games

---

## Asset Credits

This project uses the following third-party assets:

- **8Bit Music** - Background music
- **Blue Olive Studio** - Crystal assets
- **Footsteps Mini Sound Pack** - Footstep audio
- **Gabriel Aguiar Productions** - VFX
- **Hovl Studio** - Spell effects
- **Simple Nature Pack** - Environment assets
- **Polytope Studio** - 3D models
- **TextMesh Pro** - UI text rendering
- **Starter Assets** - Character controller base

---

## Contributing

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/new-feature`
3. Commit changes: `git commit -m 'Add new feature'`
4. Push to branch: `git push origin feature/new-feature`
5. Open a Pull Request

### Coding Standards

- Follow C# naming conventions
- Use `[Header("Section")]` attributes for Inspector organization
- Comment complex logic
- Keep scripts focused on single responsibilities

---

## License

This project is for educational and portfolio purposes. Third-party assets are subject to their respective licenses.

---

## Roadmap

- [ ] Additional elements (Nature, Metal)
- [ ] PvP game modes
- [ ] Dedicated server support
- [ ] Character customization
- [ ] Leaderboard system
- [ ] Mobile platform support

---

## Contact

For questions or feedback, please open an issue on GitHub.

**Repository**: [https://github.com/7bubba10/SpellForge-Showdown](https://github.com/7bubba10/SpellForge-Showdown)
