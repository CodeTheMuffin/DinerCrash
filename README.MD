For GameJam: LowRezJam 2021
https://itch.io/jam/lowrezjam-2021

https://code-the-muffin.itch.io/diner-crash

Using Unity 2020.3.12f1


## NOTE: This project may contain purchased assets, such as audio assets. If you have not purchased them, you can not use them.

Credits:
Some sound effects from Zapsplat.com


## Run GenerateBuildZips ##
- It's a relatively simple script where you can select what Build folders you want to compress (currently only .zip) for faster uploading to itch.io, for example.
- It **DOES NOT** upload the zip files!!
- In the `./Builds/` folder you can run GenerateBuildZips.py in a terminal/command prompt or you can run the RunMe.bat (batch file).
- The script was written in Python 3.6.4
- It pulls the game's version from the `./Assets/version.txt` file and includes it in the name. (currently does not look at Unity's version files)
- If you want to 