# Architectural Portfolio: Dynamic AI Navigation & Pathfinding Subsystem

Welcome! This repository is a small technical showcase of an early build decoupled, performance-focused AI navigation subsystem pulled from an ongoing 3D First-Person Shooter project I'm building in Unity (C#).

Instead of leaning entirely on built-in engine solutions, this framework experiments with a custom spatial data layout, load-balanced algorithmic pathfinding, and state-driven entity physics to handle real-time enemy behaviors.

---

## 🛠️ Core Engineering Highlights

* **Language & Engine:** C# / Unity Engine
* **Architectural Paradigms:** Object-Oriented Programming (OOP), Component-Based Architecture, Finite State Machines (FSM)
* **Optimization Patterns:** Asynchronous Coroutines, Callback/Action Matrices, O(1) HashSet Lookups, Local Vector Caching

---

## 🕹️ System Architecture & Code Breakdown

The subsystem is built around three independent, highly decoupled scripts that communicate through structured data layers:

### 1. Spatial Coordinate Management (`GridManager.cs`)

* **Role:** Serves as the world-to-matrix translator.
* **Key Implementation:** Computes grid bounds using multidimensional spatial calculations, mapping global 3D vector coordinates into discrete 2D matrix array indices. This gives the pathfinder a simple, reliable way to read and navigate walkable cell environments.

### 2. Load-Balanced Algorithmic Processing (`Pathfinder.cs`)

* **Role:** Handles the mathematical side of the pathfinding algorithm.
* **Key Implementation:** Uses an asynchronous execution queue powered by Unity Coroutines and C# Action callbacks. By spreading calculations across successive engine frames, the system can generate automated paths towards target.

### 3. Contextual AI Actuation (`EnemyMovementExcerpt.cs`)

* **Role:** Controls individual enemy behaviors and executes physical movement.
* **Key Implementation:** Evaluates structural target variables to move between more complex AI state logic (Melee, Charging, Mid-Range, and Tracking). It updates physical velocity using frame-rate independent physics actuators (`Rigidbody.MovePosition`) while smoothing orientation angles through vector interpolation.

---

## 📺 Gameplay Mechanics Demonstration

### 1. System Coordination Overview

Below is a demonstration of the `GridManager`, `Pathfinder`, and `EnemyMovement` components working together in real time to test navigation relative to player:

![Pathfinding Demonstration Workflow](https://github.com/sidequest-302/unity-fps-pathfinding-subsystem/blob/main/Enemy%20Movement_2.gif?raw=true)

A demonstration of how the `EnemyMovement` components evaluates structural target variables to move between more complex AI state logic (Melee, Charging, Mid-Range, and Tracking):

![Pathfinding Demonstration Workflow](https://github.com/sidequest-302/unity-fps-pathfinding-subsystem/blob/main/Enemy%20movement.gif?raw=true)

### 2. Weapon & Projectile Physics Sandbox

A demonstration of the physical interaction layer, including custom velocity vectors, lifecycle decay timers, and target impact detection tags:

![Projectile Interaction Sandbox](https://github.com/sidequest-302/unity-fps-pathfinding-subsystem/blob/main/Plasma%20Physics.gif?raw=true)

### 3. Bullet Impact Physics & Surface Detection

A look at the high-velocity raycasting pipeline, evaluating surface material tags and dynamically spawning particle impact effects relative to collision normals:

![Bullet Impact Physics Sandbox](https://github.com/sidequest-302/unity-fps-pathfinding-subsystem/blob/main/Bullet%20Physics.gif?raw=true)

---

## 🚀 Quality Standards for AI Training Evaluation

* **Strict Decoupling:** Scripts are designed around single-responsibility principles, keeping dependencies light and making the subsystem easier to extend and reuse.
* **Proactive Defensiveness:** The codebase includes active validation loops and null-reference checks to help maintain runtime stability.
* **Style Guide Alignment:** Written with clean naming conventions and consistent PascalCase property patterns for readability and maintainability.


