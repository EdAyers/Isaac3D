# What this app does
A Google Cardboard app using Unity that visualises concepts in physics. Currently in alpha.
Part of the [Isaac](isaacphysics.org) project.

The app intends to explore ways in which VR could be used for teaching students concepts in physics from GCSE to pre-university level. At the moment the app focuses on visualising hydrogenic orbitals.

# How to build

1. [Get the latest version of Unity](http://unity3d.com/get-unity/download), installing the free edition is sufficient
2. [Get the android SDK](http://developer.android.com/sdk/index.html#Other), the 'SDK tools only' should be sufficient.
3. Git clone this repo to your favourite directory.
4. In the Unity start screen, tap 'open other' and navigate to the place you cloned to, hit 'select folder'. Unity should now open its editor for you to hack with.
5. To build, go to File > Build Settings and select your target platform. At the moment we have only tested on android so use that one.
6. Hit build, select a build location and when prompted for the location of the android SDK navigate to the place you downloaded the files in step 2 to. This should produce an APK file. Use 'Build and Run' to run on a physical device.

email problems, queries and feedback to cardboard (at) isaacphysics.org

# Project structure

- `AssetMakers` contains some data/algorithms that are not part of the build but are used to make some of the assets used by the app.

- `Assets` contains the scenes, prefabs, scripts, textures etc used by the app. I'm in the process of reorganising this on the `Reorganise` branch. The WIP idea is that each demo in the app has its own folder in the `Demonstrations` subfolder and that commonly used assets will be in a different folder as well as a template blank demo. The `Cardboard` folder contains a slightly modified version of what Google gives you in the cardboard SDK.

- `ProjectSettings` contains Unity wizardry.
