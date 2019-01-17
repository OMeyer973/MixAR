# MixAR unity project
--------------------------------------

## how to install

- clone the project in you windows computer

- add the ProductionAssets folder that you will find on the google drive folder (folder containing all comics animations and textures)

- download the vuforia database from https://developer.vuforia.com/targetmanager and add the package into your project 
	-> double click on it with the unity project open

- if you have trouble with the vuforia database, go to vuforia, login, download the unity package corresponding to the vuforia database

- open it with unity version 2018.2.20.f1

- install the vuforia stuff that are missing (unity should pop up some windows to make you install the good stuff)

## general guidelines

### Folders organisation
- EVERYTHING we do will happend in the "Assets" folder, so when a [folder] is mentioned, it is always implied that we are talking about Assets/[folder].
other folders are configuration stuff specific to visual studio, unity, and the game settings that we can change in the unity editor

- please write all of your folders and file names in UpperCamelCase

- Editor : only vuforia stuff - this folder is made for elements that will have an impact on the unity editor UI, so we will not touch it

- Materials : materials that will be applied to the different objects - textures or custom shaders are stored in different folders

- Models : 3D models only.  no unity prefabs

- ProductionAssets : put here all of the stuff related to the story - models, comics and animations stuff. They will not be pushed in git, so put all the heavy stuff here.

- Ressources : Vuforia configuration stuff

- Scenes : unity scenes

- Streaming assets : Will also be used to store the cards matrix data in json files. Those are assets that need to be accessed via file names.

- Textures : textures that will be used in the main game - all animations and cards textures shoud go in the streaming assets folder 

- Vuforia : vuforia stuff - do not touch


### Code guidelines
- please have variables names in english (french comments are okay)

- Class names are written in UpperCamelCase

- functions names are written in UpperCamelCase (C# code guideline apparently)

- variables names are written in lowerCamelCase

- public class members are allowed (but do not put everything in public if not necessary !)
note : it is possible to change a private variable via inspector with a certain [flag] in the code (written between brackets[])

- private class members variables are written with " _ " as a prefix.

### Comic boxes Animations production process
#### for each animation
- go to unity scene Assets/scene/AnimationCreation

- duplicate the existing animation gameobject (char0 anim0 for example)

- rename it to your new animation name [animName]

- create a folder [animName] in Assets/ProductionAssets/

- paste your 4 png textures corresponding to the animation layers

#### for each layer
- create a new material with Unlit/transparentCutout for each layer (please call it "[animName] layer#")
	-> you can also copy it from another existing animation folder

- put the corresponding texture in the base RGBA texture of the material

- apply the material to the layer by dragging it onto it on the scene view

- to animate your layer, select it and open the Animation window

- create a new animation clip for your layer (make sure it goes into the Assets/ProductionAssets/[animName] folder)

- add the property you want to animate (scale, rotation ...)

- animate your element (don't forget to press the record button or your modifications will not be applied !)
	-> you can use the preview and play button to view your animation while making it
	-> you can tweak the animation curve in the curve tab of the animation window

- when you are happy with your animation, drag it from the scene hierarchy into the project window (in Assets/ProductionAssets/[animName] folder)

### cool unity shortcuts
- mouseleft : rotate view
- alt + mouseleft : zoom in and out view

- Q : pan tool - click to move left-right-up-down
- W : move tool - makes appear gameobject arrows to move an object
- E : rotate tool - makes appear gameobject circles to rotate an object
- R : scale tool - makes appear gameobject cubes to scale an object
- T : Bounding box scale tool - makes appear gameobject bounding box to scale an object by dragging it's corners
- Y : multi transform tool - makes appear all gameobject transform tools at the same time - move, rotate, scale

