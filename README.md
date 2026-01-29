# The Echo of Aethelgard: Fate's Spoiler

## Story
Under the tyrannical rule of King Valerius III, the Kingdom of Aethelgard fell into darkness. The law only favored those of noble blood, while commoners who dared to speak out were immediately thrown into Ironfang Oubliette—an underground prison said to be impossible to escape. You are one of them, a prisoner incarcerated without trial simply for refusing to surrender your family's farmland.

However, Ironfang harbors a secret. Behind the damp cell walls lies a relic from the failed Red Revolution 50 years ago: "The Glimpse Grimoire." This is no ordinary spellbook; it connects to the already-written flow of time. Anyone who touches it is forced to see "spoilers" of their own fate exactly 60 seconds before it happens.

In this first edition, your journey begins from the lowest point. With the help of this cursed yet blessed book, you must navigate the deadly prison corridors, pass through forbidden alchemy laboratories, to the warden's secret library. Every step you take has already been "spoiled" by the book as failure or death. Your task is to manipulate the variables around you—symbols, chemical liquids, even object weights—to transform these deadly spoilers into a path to freedom. The ultimate goal? To obtain the Palace Map that will determine whether the fires of revolution will burn again or be extinguished forever in the king's hands.

## Tech Stack
| Technology | Version |
|------------|---------|
| Godot Engine | 4.6 (Mono) |
| Language | C# (.NET) |
| Physics Engine | Jolt Physics |
| Rendering | DirectX 12 (Forward+) |

## Implemented Features
| Feature | Description |
|---------|-------------|
| Dual Camera System | First-Person & Isometric 3D top-down view (toggle with C) |
| Player Movement | WASD movement with camera-relative direction |
| Item Interaction | Raycast pickup (FPP) & proximity detection (Isometric) |
| Inventory System | 6 slots (2x3 grid + 1x6 hotbar) with auto-stacking |
| Usable Items | Books with readable content, UI with BBCode support |
| Door Puzzle | 6-digit password puzzle with control panel interaction |
| Physics System | Drop items with realistic throw physics |
| UI Elements | Crosshair (FPP only), inventory panel, hotbar, item prompts |

## Game Controls
| Key/Input | Action | Mode |
|-----------|--------|------|
| W, A, S, D | Move character | Both |
| Mouse Move | Camera control | Both |
| E | Pickup/Interact with items | Both |
| Q | Drop 1 item from active slot | Both |
| Ctrl + Q | Drop entire stack | Both |
| F | Use item (e.g., read book) | Both |
| I / Tab | Toggle inventory panel | Both |
| 1-6 | Select hotbar slot | Both |
| Space | Jump | First-Person |
| C | Toggle camera mode (FPP ↔ Isometric) | Both |
| ESC | Release/Capture mouse cursor | Both |

## Development Team
**Politeknik Negeri Bandung**
- Farras Ahmad Rasyid
- Satria Permata Sejati
- Nieto Salim Maula
- Umar Faruq Robbany
- Muhammad Ichsan Rahmat Ramadhan

---

© 2026 Politeknik Negeri Bandung. All rights reserved.
