For GameJam: LowRezJam 2021
https://itch.io/jam/lowrezjam-2021

https://code-the-muffin.itch.io/diner-crash

Using Unity 2020.3.12f1


## NOTE: This project may contain purchased assets, such as audio assets. If you have not purchased them, you can not use them.

**Credits:**
Some sound effects from Zapsplat.com

## Concept ##
Customers come in. They tell you what they want. You place their order. You give them their food (order box) to them.

Simple right?

Well the cook only told you what's inside an order box, not how many!?!

Remember who ordered what, while managing a very hungry crowd of impatient customers! Hopefully it doesn't cause a Diner Crash!

Gameplay is ~5 minutes.

## Controls ##
### Movement ###
- WASD/ Arrow keys
- Left stick / D-pad

### Interact ###
Interact with trash can, order (pick up and put down from/to counter), pc to order, and give order box to customer (if order is held).
- 'E' on keyboard
- 'A' on Xbox controller

### Scrolling ###
Scroll up/down in the chat box if the up and/or down arrow buttons are available (more content to show).

- Left Mouse Button- Hover over and click up/down arrows in chat box.
- **Scroll Up** (any of the following)
  - 'R' on keyboard
  - Up on right stick (Xbox controller)
Left Trigger (Xbox controller)
- **Scroll Down** (any of the following)
  - 'F' on keyboard
  - Up on right stick (Xbox controller)
  - Right Trigger (Xbox controller)
  
### Order Selection ###
For when you want to select different options when the ordering menu is opened. Order selection should be based on a customer's order in the chat box.
- Left Mouse Button

### Quick Close Menu ###
For when you want to quickly close the menu, if opened.
Does not clear menu.

- 'Q' on keyboard
- 'B' on Xbox controller

### Quick process order ###
For when you want to quickly process an order, if menu is opened.
It will even process empty orders!
- Space bar on keyboard
- 'X' on Xbox controller

## How to Play ## 
- First, I wish I had time to create tutorials, because there is a learning curve to understanding the game.
- When customer is in front of register, interact to get their order.
- Go to PC/register to place order based on what the customer wanted
  - You have the option to give them a discount which is helpful to get a better rating, especially, if you messed up their order!
  - Use the chatbox's arrows keys or other inputs (see above) to see what they want.
  - If you forget what they want, go up to them again and interact with them.
  - When holding an order, you can interact with the trash can to destroy it! Helps safe space!
- Once order is done, interact with order on the counter and give the RIGHT customer their order. 
- Customer's each have a unique progress bars (patience meters):
  - Waiting in line (before ordering), during ordering and waiting for receiving order.
  - Patience meter resets once they get into the new phase
  - If the patience meter runs out (no longer displays; 0%), then they head out the door!
- There are three progress bars on the top of the screen.
  - Left (green): Budget. Every order you place costs you money. Every order you give to the customer will give you money in return.
    - Don't waste money!
  - Middle (purple): Popularity/Rating. This is how the customers rate your business! It's dependent on if you got their orders correct or not. 
    - If you give them more than what they asked for you get a small bonus. 
    - If you mess up their order, but give them a discount, you aren't penalized as bad!
  - Top right (blue): Time. This is gameplay time. Currently, set to about 5 minutes. When time runs out, game is over!
- When game is over you are rated based on budget progression, popularity and how much time is left over if you assisted X customers for that level.
- There is one silly easter egg. 
  - **Hint:** Make sure things are in order!


### More details here: ###
- https://code-the-muffin.itch.io/diner-crash

## Run GenerateBuildZips.py ##
- It's a relatively simple script where you can select what Build folders via GUI you want to compress (currently only .zip) for faster uploading to itch.io, for example.
- In the `./Builds/` folder you can run `GenerateBuildZips.py` in a terminal/command prompt OR you can run the RunMe.bat (batch file).
- It pulls the game's version from the `./Assets/version.txt` file and includes it in the generated zip's filename.
- You can change the `project_name` (the start of the generated .zip file) by going into the script and updating the `overwrite_project_name` global variable to `True` and changing the `project_name` to what ever you want.
    - If you do overwrite the `project_name`, then I suggest keeping the name short and with underscores instead of spaces.
    - If `overwrite_project_name` is set to `False`, `project_name` will be the parent folder's name (aka the project's folder name).

Examples of generated zip filename: 
- `Unity_Project_WebGL_Build_v1.1.0.zip` or `Unity_Project_WebGL_Build.zip` (if version file is not found)
- Format: `{project_name}_{build_folder_name}_Build_{version}.zip`
    - `project_name`, if not overwritten, is pulled from the parent of the `Builds` folder (where this script should be located).
    - `build_folder_name` is one of the selected build folders' name.
    - `version` is the version found in the `./Assets/version.txt` file. 
        - If the Asset folder or version file is not found, it will say so and no version will be shown in the zip's filename.

### Limitations ###
- It **DOES NOT** upload the zip files!!
- Only compresses to .zip
- Only grabs the project's version located in the path: `./Assets/version.txt`. MUST be located here, otherwise it won't work.
    - Does not look at Unity's version files (aka the actual project version)