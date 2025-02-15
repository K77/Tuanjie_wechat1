#Code architecture notes

This document isn't intended as a tutorial, but as global indications on how the
code is organised.

Services
--------

By default all the services are disabled. When you enable all services, switch to a mobile 
platform as some services (e.g. Ads) only works on mobile platforms.

There is a special case for Unity In App Purchase : once you enable in app purchase
in the service windows, it will import a package into your project. If you have errors of
unfound StandardPurchasingModule class, the import may have failed, check in
'Plugins/UnityPurchasing' folder and you should see a UnityIAP package.
Double click it to import it, and it should fix those problems.

Now go into 'Plugins/UnityPurchasing/Resources' and look for UIFakeStoreCanvas
prefab, and change the Canvas sort order to 100. That will make it appear on
top of all canvas in the game, useful to test IAP in editor.

AssetBundle Testing
-------------------

To easily test in editor, set the AssetBundle manager into simulation mode
To do this, go to the menu Assets > AssetBundles > Simulation Mode

Building
--------

To build the game, you will first need to build the asset bundles. They can be built
throughout the Assets>Assets Bundles>Build Assets Bundles menu. It will create
a folder next to your Assets folders. Copy this folder (will be of the form
AssetsBundles/<platform>/...) into the StreamingAssets folder in your project
inside Unity (so you have StreamingAssets/AssetBundles/...).
You can then build to your chosen platform.

Game States
-----------

The whole game is using a state machine to handle the different states of the
game : Loadout, Game and Gameover.
The State Machine, called GameManager (Scripts/GameManager) is manually updating
the state on the top of its states list. When pushing a new state on top, you
can decide to pop the previous one or leave it and place the new one on top.
That allow things like pushing the GameOver state on top of the Game State,
keeping all game state intact to go back to it as it was.

Store
-----

The store could have been made with a state, but for the purpose of demoing
another technique, it uses additive scene loading.
The store is a separate scene with no camera and only UI. This allows us to load
that scene additively over any state (Loadout or Gameover) and unload when exit.

AssetBundles
------------

Theme and Characters are AssetBundles. Those would allow to potentially add new
themes or characters in the future without the need to rebuild the whole app.
When the game starts, a database of theme & characters is built throughout
the bundles manifests. Other parts of the game (loadout, store...) query the
database with the name of what they want to grab.
*NOTE*: The AssetBundles manager right now only handle AssetBundles shipped with
the game (i.e. placed in the StreamingAssets folder). This is for simplicity, but
another product could also check a local cache for extensibility
without the need to update the application.
To do this:
- At launch, application contact a server and ask the available bundles on there
(with potentially a version number per package)
- It then checks that list against the list of packages in a cache folder
- All missing packages are downloaded to that cache package.
- When loading the database, the game checks for both the streaming folder & the
cache folder (and if a bundle with same name exists in both, take the max version)
Assets Bundle are defined in the editor, check in the Bundles folder. Note that
only the themeData asset is tagged as part of the bundle, but as it resolve
dependency, everything needed will be pulled in the package (this allow to cull
all unused assets).

Save File
---------

The save file is stored locally. This is to keep the project
simple, but a full product would require some kind of validation linked to the
phone or an account. With the local save file, someone could potentially exploit items.

Shaders
-------

The game use custom shaders for most of its objects. This is due to the world
curving downward, to both give an impression of longer distance, and to hide the
horizon and the worst of the "popping" (sudden apparition) of the track piece being spawned.
There is also a shader to rotate the fishbones. It is way more efficient to do
it in the shader than with a script on each fishbones touching the transform, or
through an animator.

Link.xml
--------

The link.xml file you can find in the Assets folder is used to exclude some
classes from the code stripping that IL2CPP do. IL2CPP use static code analysis
to strip all unused assemblies, but in some cases (dynamic reflections etc.)
it can miss some, stripping them & thus resulting in compile or runtime error.
Adding them to link.xml forces Unity to keep those assemblies even if it
can't detect their use.
See [https://docs.unity3d.com/Manual/iphone-playerSizeOptimization.html] for
more complete info.
