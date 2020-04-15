# SpawnSystemPlugin

|---------------------------------------- Thank you for using Spawn System! --------------------------------------------|

|-----------------------------------------------------------------------------------------------------------------------|
                                                    DESCRIPTION
                                                    
Spawn System lets you create, customize and place your own spawn point/ area in the game 
through the Spawn System Editor window. A=You can add up to 10 different objects and their rate to spawn within
one spawn point.
Editor location: Window > Spawn Editor Window.
|-----------------------------------------------------------------------------------------------------------------------|

                                                   HOW IT WORKS
When opened, the editor window will automatically create a Temporary Spawn object in the hierarchy
which will serve as a target. You can change the target in the window by dragging and dropping another spawn object 
or customize the temporary one.
|--------------------------------------------------------------------------------------------------------------------|
| Do not delete the temporary target while using it. This will cause errors and  will require restarting the window. |
|--------------------------------------------------------------------------------------------------------------------|
To avoid errors that may occur during the play mode, PLEASE EXIT THE EDITOR WINDOW every time you finish editing 
your spawns. 
 
|-----------------------------------------------------------------------------------------------------------------------|
                                                      HOW TO USE

                    Left Column                            |                       Right Column
------------------------------------------------------------------------------------------------------------------------            
- Change the target to edit;                               |  This column is destined for the objects array and their 
- Change the color identifier in the Scene window for      |  rate of spawn, in percentages. The higher the rate, the 
  the current target spawn area;                           |  more likely for that object to spawn****. 
- Set the area size in which the objects will be spawned;  |  The list can be reordered.  
- Set the number of objects to spawn* ;                    |  If the list rates doesn't sum up 100, the FIRST ELEMENT 
- SPAWN SETTINGS: set the spawn fixed time or random       |  of the list will have a higher spawn rate (will take  
(between 2 constants) time between the spawn of 2 objects; |  the remaining percentages). 
- RESPAWN SETTINGS:                                        |  
        1. Should the spawn restart if objects are         |
           destroyed**?                                    |
        2. How many objects should be destroyed before     |
           re - filling the empty places***?               | 
        3. Set the respawn fixed time or random (between   |
           2 constants) time after which the spawning      |
           will start again;                               |
- Set the position where you want the object to be         |
  placed (Only applies after the object is placed);        |
- Set the name of the object (Only applies after the       |
  object is placed);                                       | 
------------------------------------------------------------------------------------------------------------------------

*    NOTE 1: The number of objects to spawn is relative to the spawn area.
**   NOTE 2: The objects that were spawned need to be destroyed (Destroy(Game Object)) for the respawn to work. 
             It won't work if they are disabled.
***  NOTE 3: The spawn will re-spawn ONLY the number of objects which were destroyed to keep the number of objects 
             the same and not overload.
**** NOTE 4: The rate of spawn is still random. The 1% rate DOESN'T necessarily mean that 1 in 100 object will be that 
              specific one. 1% value means a very slim chance of getting the desired object spawned.
