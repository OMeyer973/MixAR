#MixAR unity project
--------------------------------------

##how to install

- clone the project in you windows computer

- add the streaming assets folder that you will find on the google drive folder

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

- variables names are written in lowerCamelCase

- public class members are allowed (but do not put everything in public if not necessary !)
note : it is possible to change a private variable via inspector with a certain [flag] in the code (written between brackets[])

- private class members variables are written with " _ " as a prefix.


