# Hallway Segment Prefab Setup Guide

## Overview
This guide covers the components and scripts needed for the hallway segment prefab.

---

## Required Prefab Structure

```
HallwaySegment (Root)
├── Geometry/
│   ├── EntryCurve (your geometry)
│   ├── StraightSection (your geometry)
│   └── ExitCurve (your geometry)
├── Markers/
│   ├── SegmentStart (empty transform)
│   └── SegmentEnd (empty transform)
├── SpawnPoints/
│   ├── DoorSpawn
│   ├── LargeSign_1
│   ├── LargeSign_2
│   ├── SmallSign_1
│   ├── SmallSign_2
│   ├── SmallSign_3
│   └── PlantSpawn
└── DecisionZone (BoxCollider + DecisionTrigger)
```

---

## Component Setup

### 1. Root GameObject - HallwaySegment
- Add **HallwaySegment** component
- This script manages spawn points and prop configuration

### 2. Markers
Create two empty GameObjects to mark segment boundaries:

#### SegmentStart
- Empty GameObject positioned where the straight section begins (after entry curve)
- Used by HallwayManager to know where segment starts

#### SegmentEnd  
- Empty GameObject positioned where segment ends (after exit curve)
- Used by HallwayManager to spawn next segment at correct position

### 3. Spawn Points
Create empty GameObjects with **PropSpawnPoint** component:

- **DoorSpawn** - PropType: Door
- **LargeSign_1** - PropType: LargeSign  
- **LargeSign_2** - PropType: LargeSign
- **SmallSign_1** - PropType: SmallSign
- **SmallSign_2** - PropType: SmallSign  
- **SmallSign_3** - PropType: SmallSign
- **PlantSpawn** - PropType: Plant

Position these where you want props to appear in your straight section.

### 4. Decision Zone
- Create GameObject with **BoxCollider** (Is Trigger = checked)
- Add **DecisionTrigger** component
- Position near end of straight section, before exit curve
- Size to cover full hallway width/height

### 5. Configure HallwaySegment Component

In the HallwaySegment component inspector:

#### Segment Configuration
- **Segment Start**: Drag `Markers/SegmentStart`
- **Segment End**: Drag `Markers/SegmentEnd`
- **Segment Length**: Auto-calculated from marker positions

#### Spawn Points Arrays
- **Door Spawn**: Drag `SpawnPoints/DoorSpawn`
- **Large Sign Spawns**: Size = 2
  - Element 0: `SpawnPoints/LargeSign_1`
  - Element 1: `SpawnPoints/LargeSign_2`
- **Small Sign Spawns**: Size = 3
  - Element 0: `SpawnPoints/SmallSign_1`
  - Element 1: `SpawnPoints/SmallSign_2`
  - Element 2: `SpawnPoints/SmallSign_3`
- **Plant Spawn**: Drag `SpawnPoints/PlantSpawn`

### 6. Save as Prefab
- Save GameObject as prefab in `Assets/Prefabs/`
- Name: `HallwaySegment`

---

## Scene Setup

### HallwayManager Configuration

1. Create empty GameObject: `HallwayManager`
2. Add **HallwayManager** component
3. **Hallway Prefab**: Drag your `HallwaySegment` prefab
4. **Player**: Auto-finds Player tag, or drag manually

### Reference Configuration

Configure the reference hallway in HallwayManager inspector:

#### Door Configuration
- **Door Prefabs**: Array of door variant prefabs (size 2+)
- **Door Variant**: Index of which variant to use as reference

#### Large Sign Configuration  
- **Large Sign Prefabs**: Array of sign variant prefabs
- **Large Sign Variants**: Size = 2, indices for each spawn point

#### Small Sign Configuration
- **Small Sign Prefab**: Base sign prefab with TextMeshPro
- **Small Sign Texts**: Size = 3, text for each sign

#### Plant Configuration
- **Plant Prefabs**: Size = 4 (index 0 = empty/null, 1-3 = plant variants)
- **Plant Variant**: 0-3, which state for reference

---

## How It Works

1. **HallwayManager** spawns 3 segments initially (Previous, Current, Future)
2. Current segment uses your reference configuration
3. Future segment is randomly same or different from reference
4. Player walks through and reaches **DecisionZone**
5. DecisionTrigger detects if player went forward or turned back
6. Correct choice: Same hallway → forward, Different → turn back
7. On decision, segments shift and new future spawns
8. Props are spawned/cleared using **HallwayConfiguration** data

---

## Required Components Summary

| Script | Purpose |
|--------|---------|
| **HallwaySegment** | Manages segment markers and spawn points |
| **PropSpawnPoint** | Marks where props can spawn |
| **HallwayConfiguration** | Data class defining prop variants |
| **HallwayManager** | Spawns/manages 3 segments, handles decisions |
| **DecisionTrigger** | Detects player forward/backward movement |

---

## Player Requirements

- Player GameObject must have tag: **"Player"**
- Player must have a Collider (CharacterController works)

---

## Testing

Once setup:
1. Press Play
2. You should see 2 hallways (Current + Future)
3. Walk through hallway
4. Enter decision zone
5. Console shows if hallways match and your choice
6. New hallway spawns after decision

Enable **Debug Mode** in HallwayManager and DecisionTrigger for console logs.
