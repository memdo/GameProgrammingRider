# XRider

A 2D endless runner/racing game built with Unity, featuring physics-based vehicle control, dynamic theme selection, and global leaderboards.

## Table of Contents

- [Project Overview](#project-overview)
- [Technology Stack and Dependencies](#technology-stack-and-dependencies)
- [Installation and Deployment](#installation-and-deployment)
- [Usage Instructions](#usage-instructions)
- [API Keys and Environment Variables](#api-keys-and-environment-variables)
- [Known Issues and Troubleshooting](#known-issues-and-troubleshooting)
- [License and Credits](#license-and-credits)

## Project Overview

XRider is a 2D endless runner game where players control a vehicle through procedurally generated terrain. The game features physics-based vehicle control, multiple vehicle types, customizable themes, and a global leaderboard system.

### Core Gameplay Mechanics

- **Physics-Based Vehicle Control**: Realistic 2D physics simulation with vehicle flipping mechanics and ground detection
- **Procedural Level Generation**: Chunk-based system that generates terrain with progressive difficulty
- **Distance-Based Scoring**: Score increases based on distance traveled and coins collected
- **Coin Collection System**: Collect coins during gameplay to unlock new vehicles and themes
- **Vehicle Selection**: Multiple vehicle types (bikes, cars) with unlockable variants
- **Theme System**: Customizable parallax background themes with selection UI
- **Global Leaderboards**: Firebase-powered leaderboard system for competitive gameplay

### Key Features

1. **Vehicle System**: Multiple vehicle types with selection and unlock system
2. **Theme System**: Parallax background themes with visual selection interface
3. **Scoring System**: Distance-based scoring with coin collection mechanics
4. **Firebase Integration**: Real-time leaderboard with global high scores
5. **Shop System**: In-game shop for vehicle and theme purchases
6. **Audio System**: Sound effects and music management with volume controls
7. **Procedural Generation**: Chunk-based level generation with progressive difficulty scaling
8. **Settings System**: Configurable audio and game settings

### Prerequisites

- **Unity Version**: 6000.2.8f1 (Unity 6)
- **Platform Support**: Android
- **Required Knowledge**: Basic Unity editor familiarity

## Technology Stack and Dependencies

### Core Technologies

- **Unity Engine**: 6000.2.8f1
- **Programming Language**: C# (.NET)
- **Rendering Pipeline**: Universal Render Pipeline (URP) 17.2.0
- **Input System**: Unity Input System 1.14.2

### Unity Packages

The project uses the following Unity packages (see `Packages/manifest.json`):

- **2D Animation**: 12.0.2
- **2D Aseprite**: 2.0.2
- **2D Sprite Shape**: 12.0.1
- **2D Tilemap Extras**: 5.0.1
- **Cinemachine**: 3.1.5
- **Input System**: 1.14.2
- **TextMeshPro**: 2.0.0 (via UGUI)
- **Timeline**: 1.8.9
- **Visual Scripting**: 1.9.7
- **Universal Render Pipeline**: 17.2.0

### External Dependencies

- **Firebase SDK**: Firebase Realtime Database for leaderboard functionality
  - Location: `Assets/Firebase/`
  - Configuration: `Assets/google-services.json` (Android)

### Key Script Locations

- **Main Game Manager**: `Assets/Personel Folders/Ege/NEW/_Scripts/GameManager.cs`
- **Vehicle Controller**: `Assets/Personel Folders/Yaman/Scripts/RiderLikeController.cs`
- **Theme System**: `Assets/Personel Folders/Umut/Scripts/`
  - `ThemeLoader.cs`
  - `ThemeSelectionUI.cs`
  - `SelectedThemeManager.cs`
  - `ThemeAwareParallaxController.cs`
- **Firebase Integration**: `Assets/Personel Folders/Ege/NEW/_Scripts/FirebaseManager.cs`
- **Audio System**: `Assets/Personel Folders/Ege/NEW/_Scripts/AudioManager.cs`
- **Chunk Manager**: `Assets/Personel Folders/Ege/NEW/_Scripts/ChunkManager.cs`
- **Vehicle Selection**: `Assets/Personel Folders/Ege/NEW/BikeSelector.cs`

## Installation and Deployment

### Initial Setup

1. **Install Unity Hub**
   - Download and install Unity Hub from [unity.com](https://unity.com/download)
   - Ensure Unity version 6000.2.8f1 is installed

2. **Open the Project**
   - Open Unity Hub
   - Click "Add" and select the project folder
   - Click "Open" to launch the project in Unity Editor

3. **Package Installation**
   - Unity will automatically resolve and import all packages listed in `Packages/manifest.json`
   - Wait for package import to complete (may take several minutes on first open)
   - If packages fail to import, go to `Window > Package Manager` and manually install missing packages

4. **Firebase Configuration**
   - The project includes a pre-configured `google-services.json` file in `Assets/`
   - For Android builds, ensure the file is properly configured for your Firebase project
   - See [API Keys and Environment Variables](#api-keys-and-environment-variables) for custom setup

### Scene Setup

**Main Scenes:**
- **Main Menu**: `Assets/Personel Folders/Ege/NEW/gameui.unity`
- **Gameplay**: `Assets/Personel Folders/Ege/NEW/New Scene.unity`
- **Garage/Vehicle Selection**: `Assets/Personel Folders/Ege/NEW/garagescene.unity`
- **Theme Selection**: `Assets/Personel Folders/Umut/Scenes/ThemeSelectionScene.unity`
- **Leaderboard**: `Assets/Personel Folders/Ege/NEW/Leaderboard.unity`

**Build Settings Configuration:**
1. Open `File > Build Settings`
2. Add the following scenes in order:
   - Main Menu (gameui.unity)
   - Theme Selection Scene
   - Garage Scene
   - Gameplay Scene (New Scene.unity)
   - Leaderboard Scene

### Building

1. **Configure Build Settings**
   - Go to `File > Build Settings`
   - Select "Android" platform
   - Click "Switch Platform" if needed

2. **Player Settings**
   - Open `Edit > Project Settings > Player`
   - Configure package name, version, and other Android-specific settings
   - Ensure `google-services.json` is in `Assets/` folder

3. **Build**
   - Click "Build" or "Build and Run"
   - Select output directory
   - Wait for build to complete

## Usage Instructions

### How to Play

1. **Start the Game**
   - Launch the game from the main menu
   - Enter your player name when prompted
   - Click the "PLAY" button

2. **Gameplay Controls**
   - **Left Side of the Screen (Hold)**: Accelerate forward and flip/rotate vehicle while airborne
   - **Right Side of the Screen (Hold)**: Deaccelerate and reverse flip/rotate vehicle while airborne
   - **Boost Button**: Vehicle gains a speed boost for 2 seconds with a 5 second cooldown
   - **Pause Button**: See settings menu, restart the game or exit to the main menu

3. **Select a Vehicle** (Optional)
   - Navigate to the garage scene
   - Browse available vehicles using left/right navigation
   - Unlock vehicles by spending coins
   - Select a vehicle to start playing

4. **Select a Theme** (Optional)
   - Navigate to the theme selection scene
   - Browse available themes
   - Click "SELECT" to apply a theme
   - Themes affect the dynamic background in gameplay

5. **Gameplay Objectives**
   - Travel as far as possible
   - Collect coins scattered throughout the level
   - Avoid crashing head-first into obstacles
   - Achieve high scores to rank on the leaderboard

6. **Shop System**
   - Access the shop from the pause menu during gameplay
   - Purchase vehicles and themes using collected coins
   - View your total coin balance

7. **Leaderboard**
   - View global high scores
   - Your best score is automatically uploaded to Firebase
   - Leaderboard updates in real-time

### Theme System Usage

1. **Selecting a Theme**
   - Navigate to Theme Selection Scene
   - Use left/right navigation buttons to browse themes
   - Preview the theme before selecting
   - Click "SELECT" to apply the theme

2. **Theme Components**
   - Each theme includes:
     - Parallax background layers
     - Sky and cloud sprites
     - Representation sprite (preview image)

3. **Theme Setup in Gameplay**
   - Themes are automatically loaded in gameplay scenes
   - Ensure `ThemeAwareParallaxController` is attached to a `ParallaxContainer` GameObject
   - Configure parallax speed and layer spacing in the Inspector

### Vehicle System

1. **Unlocking Vehicles**
   - Collect coins during gameplay
   - Navigate to garage scene
   - Purchase locked vehicles using coins
   - First vehicle is unlocked by default

2. **Vehicle Selection**
   - Selected vehicle is saved in PlayerPrefs
   - Vehicle spawns at configured coordinates in gameplay scene
   - Vehicle selection persists between game sessions

## API Keys and Environment Variables

### Firebase Setup

The project includes Firebase Realtime Database integration for leaderboard functionality.

#### Current Configuration

- **Status**: Pre-configured with `google-services.json`
- **Location**: `Assets/google-services.json`
- **Platform**: Android (configured)

#### Using Existing Firebase Project

The project is already configured with a Firebase project. The `google-services.json` file is present in the `Assets/` directory and will be automatically used for Android builds.

**Note**: The game will function without Firebase, but leaderboard features will be unavailable. Firebase connection status is logged in the Unity Console.

#### Setting Up Your Own Firebase Project

If you want to use your own Firebase project:

1. **Create Firebase Project**
   - Go to [Firebase Console](https://console.firebase.google.com/)
   - Create a new project or select an existing one
   - Enable Realtime Database

2. **Configure Android App**
   - Add Android app to Firebase project
   - Download `google-services.json`
   - Replace `Assets/google-services.json` with your downloaded file

3. **Database Rules**
   - Configure Realtime Database rules to allow read/write:
   ```json
   {
     "rules": {
       "scores": {
         ".read": true,
         ".write": true
       }
     }
   }
   ```

#### Environment Variables

No additional environment variables are required. All configuration is handled through:
- `google-services.json` (Firebase)
- Unity PlayerPrefs (game settings)
- Unity Project Settings (build configuration)

## Known Issues and Troubleshooting

### Common Setup Issues

#### Packages Not Importing

**Problem**: Unity packages fail to import or show errors.

**Solution**:
1. Close Unity Editor
2. Delete `Library/` folder (Unity will regenerate it)
3. Reopen the project
4. Wait for package import to complete
5. If issues persist, manually install packages via `Window > Package Manager`

#### Firebase Connection Errors

**Problem**: "Could not resolve Firebase dependencies" error in console.

**Solution**:
1. Ensure `google-services.json` is in `Assets/` folder
2. Check Firebase SDK is properly imported in `Assets/Firebase/`
3. Verify internet connection (Firebase SDK downloads dependencies on first run)
4. Check Unity Console for specific dependency errors
5. Game will function without Firebase, but leaderboard won't work

#### Theme Not Loading in Gameplay

**Problem**: Selected theme doesn't appear in gameplay scene.

**Solution**:
1. Ensure you clicked "SELECT" button in theme selection scene
2. Check that `ThemeAwareParallaxController` exists in gameplay scene
3. Verify "Load From Selected Theme" is enabled in Inspector
4. Check Unity Console for theme loading errors
5. Ensure theme resources are in `Assets/Resources/Themes/` folder structure

#### Scene Navigation Issues

**Problem**: Scenes don't load or buttons don't work.

**Solution**:
1. Verify scenes are added to Build Settings (`File > Build Settings`)
2. Check scene names match exactly (case-sensitive)
3. Ensure `MainMenuUI` script is attached to UI buttons
4. Verify button onClick events are configured in Inspector

#### Vehicle Not Spawning

**Problem**: Vehicle doesn't appear in gameplay scene.

**Solution**:
1. Check `GameManager` has vehicle prefabs assigned in Inspector
2. Verify spawn coordinates are set correctly
3. Ensure vehicle prefabs are tagged "Player"
4. Check that `GameManager` GameObject exists in scene
5. Verify selected vehicle index is valid

#### Build Errors

**Problem**: Build fails with errors.

**Solution**:
1. Check Unity Console for specific error messages
2. Ensure all required scenes are in Build Settings
3. Check `google-services.json` is present
4. Clear build cache and rebuild

### Performance Issues

#### Low Frame Rate

**Solution**:
1. Reduce number of active chunks in `ChunkManager`
2. Lower parallax layer count
3. Optimize sprite import settings
4. Adjust URP quality settings

#### Memory Issues

**Solution**:
1. Check chunk cleanup is working (old chunks should be destroyed)
2. Verify audio clips are not loaded multiple times
3. Monitor texture memory usage in Profiler

### Debugging Tips

1. **Enable Debug Logs**
   - Check `RiderLikeController` debug flags in Inspector
   - Enable "Debug Input" and "Debug Physics" for detailed logs

2. **Firebase Debugging**
   - Check Unity Console for Firebase connection status
   - Verify database rules allow read/write operations
   - Test Firebase connection in Unity Editor before building

3. **Scene Hierarchy**
   - Ensure all required GameObjects exist in scenes
   - Check component references are not null
   - Verify tags and layers are set correctly

## License and Credits

### License

This project is licensed under the MIT License. See LICENSE file for details.

### Contributors

- **Umut Sidar Tahtasakal - *21070001213***
- **Remzi Yaman Esen - *21070006026***
- **Salih Aydın - *22070001011***
- **Murat Ege Baykent - *22070001018***
- **Ece Coşdur - *22070001050***
- **Mustafa Berkay Düzenli - *22070001068***
- **Cenk Serbest - *22070001079***
### Asset Credits

- **Audio**: DavidKBD - Pink Bloom Pack

### Third-Party Libraries

- **Firebase SDK**: [Firebase Unity SDK](https://firebase.google.com/docs/unity/setup)
- **Unity Technologies**: Unity Engine and associated packages
