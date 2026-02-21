# Hallway Segment Setup Guide

## Overview
Simple teleport-based system: One static hallway, props change each loop, player teleports back to start while maintaining their rotation for a seamless endless feeling.

---

## Required Setup

### 1. Hallway Prefab/GameObject Structure

```
HallwaySegment (in scene, not a prefab)
├── Geometry/ (your hallway mesh)
├── Markers/
│   ├── SegmentStart (starting position)
│   └── SegmentEnd (ending position, optional)
└── SpawnPoints/
    ├── DoorSpawn
    ├── LargeSign_1
    ├── LargeSign_2
    ├── SmallSign_1
    ├── SmallSign_2
    ├── SmallSign_3
    ├── SmallSign_4
    ├── PlantSpawn_1
    └── PlantSpawn_2

Note: Trigger colliders (DecisionTrigger, BackwardDetectionTrigger) are OPTIONAL
Decision detection now uses direct Z-plane threshold checking for precision!
```

### 2. Spawn Points
Create empty GameObjects with **PropSpawnPoint** component:
- **DoorSpawn**: PropType = Door
- **LargeSign_1 & 2**: PropType = LargeSign
- **SmallSign_1, 2, 3, 4**: PropType = SmallSign (stack vertically)
- **PlantSpawn_1 & 2**: PropType = Plant

Position these where you want props in your hallway.

### 4. Configure HallwaySegment Component
On root GameObject, add **HallwaySegment** component:
- **Segment Start**: Drag `Markers/SegmentStart`
- **Segment End**: Drag `Markers/SegmentEnd` (optional)
- **Door Spawn**: Drag spawn point
- **Large Sign Spawns** (size 2): Drag both
- **Small Sign Spawns** (size 4): Drag all 4
- **Plant Spawns** (size 2): Drag both

---

## Scene Setup

### 1. Player Spawn Point
- Create empty GameObject: **PlayerSpawnPoint**
- Position at start of hallway (where player appears after crossing forward plane)
- Rotation facing down the hallway
- When player crosses forward decision plane → teleports here

### 2. Backward Spawn Point
- Create empty GameObject: **BackwardSpawnPoint**
- Position between the two turns (where player appears after crossing backward plane)
- When player crosses backward plane → teleports here + rotates 180°
- This creates the looping effect - both directions lead forward through the hallway

### 3. HallwayManager
- Create empty GameObject: **HallwayManager**
- Add **HallwayManager** component:
  - **Hallway Segment**: Drag your HallwaySegment from scene
  - **Player**: Auto-finds Player tag
  - **Player Spawn Point**: Drag PlayerSpawnPoint (for forward decisions)
  - **Backward Spawn Point**: Drag BackwardSpawnPoint (for backward decisions)

**Decision Detection:**
- Create empty GameObject: **ForwardDecisionPlane**
  - Position at the **X coordinate** between the two turns (where forward decision triggers)
  - Example: If turns are at X=-12.5, place plane there
  - When player crosses this going forward (positive→negative X) → teleports to PlayerSpawnPoint
  - Drag into HallwayManager's **Forward Decision Plane** field
  
- Create empty GameObject: **BackwardDecisionPlane**
  - Position at the **X coordinate** near the start (where backward decision triggers)
  - Example: If spawn is at X=9.5, place plane slightly ahead at X=10
  - When player crosses this going backward (negative→positive X) → teleports to BackwardSpawnPoint + 180° rotation
  - Drag into HallwayManager's **Backward Decision Plane** field

**How it works:**
- Hallway layout: Straight segment → Left turn → Right turn
- Decision planes positioned in the Z direction between the turns
- Detection checks when player crosses specific **world X positions**
- Only the plane's X coordinate matters (Y and Z are ignored for detection)
- Teleportation is simple: exact spawn point position, preserve Y height only
- No lateral offset calculation needed - just teleport to the spawn point!

### 3. Reference Configuration
In HallwayManager, configure reference hallway:

**Door**: Array of 2+ door prefab variants, choose reference index  
**Large Signs**: Array of prefabs, set 2 variant indices  
**Small Signs**: Base prefab + 4 text strings  
**Plants**: 4 prefabs (index 0 = none, 1-3 = variants), choose 2 reference indices

---

## How It Works

1. Player walks through main hallway
2. **Two decision planes monitor player movement:**
   
   **Forward Decision** (at far end):
   - Player crosses **ForwardDecisionPlane** going forward → Teleport to PlayerSpawnPoint, keep rotation
   - Seamless loop back to start
   
   **Backward Decision** (at symmetry point):
   - Player sees room changed, turns around
   - Walks backward and crosses **BackwardDecisionPlane** → Teleport to BackwardSpawnPoint + rotate 180°
   - Now facing forward again, positioned "between the turned hallway"
   - They think they went backward, but world secretly rotated them forward

3. System scores choice (correct/incorrect)
4. **Teleports player** with precise lateral offset preservation from the appropriate plane
5. **Changes props** for next loop (same or different randomly)
6. Repeat seamlessly!

### The Non-Euclidean Trick
When player sees a difference and turns around:
- They walk backward through the **BackwardDecisionPlane** (at symmetry point)
- System teleports them to BackwardSpawnPoint (between the turned hallway)
- Rotates them 180° so they're now facing forward
- They continue walking "forward" but the world has changed
- Creates illusion of non-Euclidean space - walking backward but still approaching forward!

### Why Two-Plane Detection?
- **Independent detection** - forward and backward decisions trigger at different positions
- **Visually position** both planes - just move Transforms in the editor
- More precise - lateral offset calculated from the plane they actually crossed
- Eliminates visual "blip" from consistent trigger points
- Supports both forward loop and backward "secret teleport + rotate" mechanic
- Perfectly seamless endless hallway feel with non-Euclidean geometry illusion

---

## Scripts Summary

| Script | Purpose |
|--------|---------|
| **HallwaySegment** | Manages single static hallway |
| **PropSpawnPoint** | Marks prop locations |
| **HallwayConfiguration** | Prop variant data |
| **HallwayManager** | Orchestrates teleport, prop changes, and Z-plane decision detection |
| **DecisionTrigger** | **(Optional/Legacy)** Trigger-based forward detection |
| **BackwardDetectionTrigger** | **(Optional/Legacy)** Trigger-based backward detection |

---

## Requirements

- Player tag = **"Player"**
- Player must have Collider (CharacterController works)

---

## Testing

1. Press Play
2. Walk through hallway
3. Enter decision zone
4. Console shows your choice and if correct
5. You teleport back to start (facing opposite direction)
6. Props change (or don't)
7. Repeat

Enable **Debug Mode** in components for console logs.
