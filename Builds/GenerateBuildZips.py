import argparse
import os
from tqdm import tqdm  # for showing progress bars
import zipfile
from easygui import multchoicebox, msgbox

'''
Description: A simple script to zip my builds for me.

In directory where this folder is found, run command:
pip install -r requirement.txt

Used python 3.6.4
'''

current_folder = os.getcwd()
asset_folder_name = "Assets"
version_file_name = "version.txt"

def get_version():
    # assuming you are in builds folder
    '''
    Assumed folder structure:
    ..\Diner Crash\Builds\                  <--- Assumes currently In
    ..\Diner Crash\Assets\version.txt       <--- Where the version file is located
    '''
    parent_folder = os.path.dirname(current_folder)

    asset_full_path = parent_folder + "\\" + asset_folder_name

    if os.path.exists(asset_full_path):
        version_full_path = asset_full_path + "\\" + version_file_name

        if os.path.exists(version_full_path):
            with open(version_full_path, "r") as version_file:
                version_text = version_file.readline().strip() # assumes version text is only on first line

                if version_text is not None and version_file != "":
                    if " " in version_text:
                        print(f"The version text contains spaces. Remove them or replace with underscores, please!")
                    else:
                        return version_text
                else:
                    print(f"No version found in '{version_file}' file. Can not grab version file.")

        else:
            print(f"'{version_file_name}' not found in '{asset_folder_name}' folder. Can not grab version file.")
    else:
        print(f"'{asset_folder_name}' not found in parent directory. Can not grab version file.")

    print(os.listdir(parent_folder))

    return None

def main():
    version = get_version()

    if version is None:
        return

    # gets all folder names in current directory; only pull if its a directory/folder
    folders = [folder for folder in os.listdir(current_folder) if os.path.isdir(folder)]#if not folder.endswith(".py")]

    # options = []
    #
    # # assumes there won't be 26 folders (since there are only 26 letters in alphabet)
    # for index in range(len(folders)):
    #     option = f"{chr(65 + index)}) {folders[index]}"
    #     options.append(option)
    #
    # options.append("0) All") # zero to do all

    # arguments doesn't make sense
    # parser = argparse.ArgumentParser(description="A simple script to zip my builds for me.",
    #                                  formatter_class=argparse.RawTextHelpFormatter, )
    # parser.add_argument(
    #     "-b",
    #     "--builds",
    #     default="all",
    #     type=str,
    #     choices=["all"].append(folders),
    #     help="Determines which builds to zip up.",
    # )
    # args = parser.parse_args()
    # print(f"Arguments passed: {args}")

    title = "Generate Build Zips"
    msg = f"Please select the directories that you wish to zip.\n\nUsing version: {version}"

    # Would like to default preselect to be all options rather than none, but doesn't seem possible at the moment.
    choice = multchoicebox(msg=msg, title=title, choices=folders,preselect= None, )
    print(f"choices selected: {choice}")
    print(folders)

    if choice is None:
        msg = "No folder selected. Ending script."
        #msgbox(msg=msg, title=title)
        print(msg)
        return



if __name__ == "__main__":
    main()
