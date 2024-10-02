Backgammon Multiplayer Game Scripts
Overview
This repository contains the core scripts for a multiplayer Backgammon game developed in Unity. The scripts manage player turns, dice rolls, piece movement, game states, and communication between players over a network. It's designed for multiplayer gameplay, making it easy to set up online or local matches.

Features
Multiplayer Mode: Supports player-to-player matches, with game state syncing via server communication.
Game Logic: Manages all Backgammon rules, including dice rolling, piece movement, and hit counters.
Server Interaction: Scripts handle the connection to the server for match setup, player moves, and real-time game updates.
UI Management: Basic UI components for user profiles, game rooms, and settings.
Technologies Used
C# (core game logic, multiplayer interaction)
Unity (for eventual game integration, though only scripts are included here)
How to Use
Clone the repository:
bash
Copy code
git clone https://github.com/yourusername/backgammon-multiplayer-scripts.git
Import the scripts into your Unity project.
Attach the scripts to the appropriate game objects and integrate them with your multiplayer solution (such as Photon or a custom server).
Customize the scripts as needed to fit your game flow.
Scripts Overview
BackGamonBoard.cs: Core logic for managing the Backgammon board and game flow.
DiceController.cs & Dice.cs: Handles dice rolls and visual feedback for both players.
GameRoom.cs: Manages game room setup and player interactions.
ServerConnector.cs & ServerGameReq.cs: Handles the communication between the game and server, syncing game states across players.
GameStateManager.cs: Manages the different stages of the game, including start, in-game, and game over states.
Login.cs & UserProfile.cs: Handles user authentication and profile management for multiplayer matches.
Future Improvements
 Implement advanced matchmaking features.
 Add AI opponents for single-player mode.
 Improve UI/UX for smoother multiplayer experience.
Contributing
Feel free to fork the repository and submit pull requests with additional features, bug fixes, or improvements.
