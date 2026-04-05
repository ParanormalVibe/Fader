# Fader

A lane cover overlay for VSRGs (Vertical Scrolling Rhythm Games) that lack a built-in fader. Sits on top of your game window as a transparent-to-black gradient, letting you block out part of the playfield to help with reading at different scroll speeds.

## Features

- **Adjustable gradient** — control the start and end points of the fade
- **Always on top** — stays above your game window (toggleable)
- **Resizable & draggable** — position and size the overlay to fit your playfield
- **Profiles** — save and switch between presets for different games or scroll speeds

## Usage

Right-click the overlay to open the context menu:

- **Settings** — adjust gradient, toggle always-on-top, resizing, and dragging
- **Manage Profiles** — create, rename, and switch between saved presets
- **Close** — exit the application

Settings changes are automatically saved to the active profile.

## Building

Requires [.NET 9](https://dotnet.microsoft.com/download/dotnet/9.0).

```
dotnet build
dotnet run
```
