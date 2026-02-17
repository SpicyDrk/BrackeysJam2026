# KEEP WALKING (Working Title)

## Game Design Document (GDD)

### Theme: Strange Places

### Tone: Liminal Dread / Sterile Corporate Isolation

### Visual Inspiration: Sterile white office spaces, tiled corridors, endless turns

------------------------------------------------------------------------

# 1. High Concept

You walk through a sterile, white corporate hallway that seems to loop
endlessly.

Each section looks nearly identical.

Sometimes something is wrong.

If there is an anomaly --- turn around. If everything is normal --- keep
walking forward.

Make the correct choice enough times in a row to escape.

The experience is seamless. There are no cuts, no fades, no level loads.
The hallway physically loops back into itself using spatial stitching.

------------------------------------------------------------------------

# 2. Core Pillars

1.  Seamless World -- No scene reloads or visible transitions.
2.  Subtle Anomalies -- Changes are small, psychological, and
    unsettling.
3.  Observation Over Reflex -- The player must slow down and truly look.
4.  Sterile Liminal Space -- White tile, fluorescent hum, minimal
    decoration.
5.  Increasing Dread -- The environment slowly becomes more oppressive.

------------------------------------------------------------------------

# 3. Core Gameplay Loop

1.  Walk forward through corridor.
2.  Observe carefully.
3.  Decide at the end:
    -   Continue forward (No anomaly)
    -   Turn around (Anomaly detected)
4.  If correct → streak increases.
5.  If incorrect → player is subtly reset deeper in the maze or streak
    resets.
6.  Reach required streak (e.g., 8 correct) to escape.

------------------------------------------------------------------------

# 4. Seamless World Design

## Corridor Structure

The hallway is built from modular corridor segments:

-   Straight segment
-   Left turn
-   Right turn
-   T intersection (rare)
-   Dead-end turn-around trigger zone

Corridor pieces are duplicated and repositioned behind the player as
they progress.

When player crosses a trigger volume: - The segment behind them is
recycled ahead. - Anomaly state for next section is determined. - No
loading screens. - No teleporting visible to player.

The illusion: an endless sterile office maze.

------------------------------------------------------------------------

# 5. Technical Architecture (Unity)

## Core Systems

### 1. CorridorManager

Responsible for: - Holding active corridor segments (3--5 at a time) -
Recycling segments behind player - Injecting anomaly variations -
Tracking current loop state

### 2. CorridorSegment

Prefab containing: - Static geometry - Light fixtures - Props - Anomaly
anchor points

Each segment has multiple anomaly slots that can be toggled.

### 3. AnomalyManager

-   Stores list of anomaly definitions
-   Determines whether next loop contains anomaly
-   Controls difficulty ramp
-   Prevents repeating same anomaly too often

### 4. DecisionZone

At end of loop: - Detects player direction choice - Compares to current
anomaly state - Updates streak count

### 5. GameStateManager

Tracks: - Current streak - Required streak to win - Increasing anomaly
subtlety - Ending trigger

------------------------------------------------------------------------

# 6. Environment & Visual Direction

Sterile corporate aesthetic:

-   White painted drywall
-   Glossy white tile floors
-   Drop ceiling panels
-   Fluorescent light strips
-   Minimal signage
-   Occasional identical office doors
-   No personal objects

Visual Effects:

-   Slight bloom
-   Soft chromatic aberration
-   Subtle film grain
-   Minimal HUD (maybe timestamp)

The world should feel clean but wrong.

------------------------------------------------------------------------

# 7. Anomaly Design Philosophy

Anomalies should:

-   Be subtle at first.
-   Avoid obvious horror tropes.
-   Create uncertainty.
-   Make the player doubt memory.

Later anomalies may combine multiple small changes.

------------------------------------------------------------------------

# 8. Anomaly Ideas

## Level 1 -- Obvious (Tutorial Tier)

-   A door missing.
-   Extra door added.
-   Exit sign pointing wrong direction.
-   Light flickering aggressively.
-   Ceiling tile missing.

## Level 2 -- Moderate

-   One light slightly dimmer.
-   Carpet tile rotated 90 degrees.
-   Wall tile misaligned slightly.
-   Fire extinguisher moved to opposite wall.
-   Door handle on wrong side.

## Level 3 -- Subtle

-   Hallway slightly longer.
-   Ceiling slightly lower.
-   Wall color slightly warmer tone.
-   Shadow direction slightly incorrect.
-   Fluorescent hum missing.

## Level 4 -- Psychological

-   Very faint silhouette at far end.
-   Frame on wall slightly tilted.
-   Door number changed by 1 digit.
-   Floor reflection slightly delayed.
-   Subtle breathing sound when standing still.

## Level 5 -- Reality Distortion

-   Corridor bends slightly unnaturally.
-   Perspective feels stretched.
-   Lights too perfectly aligned.
-   Player shadow missing.
-   Hallway perfectly silent.

------------------------------------------------------------------------

# 9. Difficulty Progression

Early: - 50% anomaly chance - Obvious changes

Mid: - 60% anomaly chance - Subtle changes

Late: - 70% anomaly chance - Combined anomalies - Psychological
distortion

------------------------------------------------------------------------

# 10. Win Condition

Correctly identify anomalies 8 times consecutively.

Final hallway becomes unnaturally long and silent.

A final door appears at the end.

When opened:

White light. Silence. Cut to black.

Optional twist: The same hallway begins again.

------------------------------------------------------------------------

# 11. 1-Week Production Plan

## Day 1 -- Core Corridor System

-   Build modular hallway pieces
-   Implement seamless segment recycling
-   Basic player controller

## Day 2 -- Anomaly Toggle System

-   Create anomaly anchor system
-   Implement anomaly selection logic
-   Inject anomaly per loop

## Day 3 -- Decision & Streak Logic

-   Turn-around detection
-   Forward detection
-   Streak system
-   Reset logic

## Day 4 -- 15--20 Anomalies

-   Implement visual swaps
-   Add light variations
-   Add small prop differences

## Day 5 -- Audio & Atmosphere

-   Fluorescent hum
-   Ambient loop
-   Silence triggers
-   Subtle distortion effects

## Day 6 -- Difficulty Ramp

-   Tune anomaly probability
-   Prevent repetition
-   Add subtle late-game anomalies

## Day 7 -- Polish

-   Remove obvious seams
-   Lighting pass
-   Add ending
-   Build export

------------------------------------------------------------------------

# 12. Why This Fits "Strange Places"

-   The architecture loops impossibly.
-   The sterile environment feels inhuman.
-   The player questions perception.
-   Space behaves consistently... until it doesn't.

The strangeness comes from doubt, not monsters.

------------------------------------------------------------------------

End of Document.
