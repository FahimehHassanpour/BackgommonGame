# Backgammon Multiplayer Game

![Unity Logo](path-to-unity_logo.svg) ![Backgammon Logo](path-to-backgammon_logo.svg)

![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)

## Overview
This project implements a classic Backgammon multiplayer game built in Unity. It allows two players to compete in real-time, with game logic handling piece movements, dice rolls, and win conditions.

## Features
- **Multiplayer Mode:** Supports player-to-player matches, with game state syncing via server communication.
- **Game Logic:** Manages all Backgammon rules, including dice rolling and piece movement.
- **Server Interaction:** Handles communication between the game and server for match setup and player moves.
- **UI Management:** Basic UI components for user profiles and game rooms.

## Installation
1. Clone the repository:
    ```bash
    git clone https://github.com/FahimehHassanpour/BackgommonGame.git
    ```
2. Open the project in Unity (recommended version 2021.3 or higher).
3. Set up your multiplayer service (e.g., Photon or a custom server).
4. Run the game in the Unity editor or build it for your desired platform.

## Technologies Used
- **Unity** (for game development)
- **C#** (game logic and networking)
- **Photon** (for multiplayer functionality)

## Contributing
Contributions are welcome! Please fork this repository and submit a pull request.

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
