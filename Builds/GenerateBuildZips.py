import argparse
import os
from tqdm import tqdm  # for showing progress bars
from zipfile import ZipFile
from easygui import multchoicebox, ynbox, msgbox

'''
Description: A simple script to zip my builds for me.

In directory where this folder is found, run command:
pip install -r requirement.txt

Used python 3.6.4
'''

current_folder = os.getcwd()
asset_folder_name = "Assets"
version_file_name = "version.txt"
project_name = "Diner_Crash"
version = None
title = "Generate Build Zips"

def get_version() -> str:
    # assuming you are in builds folder
    '''
    Assumed folder structure:
    ..\{parent project folder}\Builds\                  <--- Assumes currently In
    ..\{parent project folder}\Assets\version.txt       <--- Where the version file is located
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

def get_build_folder_selections() -> list:
    global version

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

    msg = f"Please select the build folders that you wish to zip.\n\nUsing version: {version}"

    # Would like to default preselect to be all options rather than none, but doesn't seem possible at the moment.
    choices = multchoicebox(msg=msg, title=title, choices=folders,preselect= None, )
    # print(f"choices selected: {choices}")
    # print(folders)

    if choices is None:
        msg = "No folder selected. Ending script."
        #msgbox(msg=msg, title=title)
        print(msg)
        return None

    return choices

def zip_build_folder(choices: list) -> dict:
    build_statuses = dict()

    for build_folder_name in choices:
        os.chdir(current_folder) # change back to original working directory
        #print(f"Current directory (Builds): {os.getcwd()}")

        build_path = current_folder + "\\" + build_folder_name

        # # grabs all the zip files to make sure you it doesn't already exist
        # zips = [zip_file for zip_file in os.listdir(build_path) if zip_file.endswith(".zip")]
        # #print(f"zips: {zips}")

        generated_zip_name = generate_zip_filename(build_folder_name=build_folder_name)
        generated_zip_path = build_path + "\\" + generated_zip_name
        print(f"Generated zip name: {generated_zip_name}")

        overwrite_status = ""
        #if generated_zip_name in zips:
        if os.path.exists(generated_zip_path):
            msg = f"Warning: The file '{generated_zip_name}' already exists.\n\nDo you want to overwrite it?"
            can_overwrite_zip = ynbox(msg=msg, title=title)

            if not can_overwrite_zip:
                print(f"Skipping building duplicate zip file: '{generated_zip_name}'")
                overwrite_status = "(Overwritten Skipped)"
                build_statuses[build_folder_name] = f"Failed {overwrite_status}"
                continue

            overwrite_status = "(Overwritten)"

        # grabs all non_zips' full path (including: folders, files without the .zip extension
        # if using 'Builds' folder as current working directory (cwd)
        #non_zip_paths = [build_path + "\\" + non_zip for non_zip in os.listdir(build_path) if not non_zip.endswith(".zip")]
        #non_zip_paths = [f"./{build_folder_name}\\" + non_zip for non_zip in os.listdir(build_path) if not non_zip.endswith(".zip")]


        # if changed working directory
        os.chdir(build_path) #changing working directory; doing this to prevent having the Build's folder name in the zip
        current_dir = os.getcwd()
        #print(f"Changed working directory: {current_dir}")
        non_zip_paths = [non_zip for non_zip in os.listdir(current_dir) if not non_zip.endswith(".zip")]
        #print(non_zip_paths)

        # Just a check to see that the paths are valid
        # for file in non_zip_paths:
        #     print(f"Does file path exist? {file} ::: {os.path.exists(file)}")


        # with ZipFile(generated_zip_path, 'w') as zipped_build:
        # #with ZipFile("test123.zip", 'w') as zipped_build:
        #     for non_zip_path in non_zip_paths:
        #         zipped_build.write(non_zip_path)

        with ZipFile(generated_zip_path, 'w') as zipped_build:
            #with ZipFile("test123.zip", 'w') as zipped_build:
            for non_zip_path in non_zip_paths:
                zipped_build.write(non_zip_path)


        # check that zip file is created successfully
        if os.path.exists(generated_zip_path):
            build_statuses[build_folder_name] = f"Created {overwrite_status}"

    os.chdir(current_folder) # change back to original working directory; just in case
    return build_statuses

def generate_zip_filename(build_folder_name : str) -> str:
    return project_name + "_" + build_folder_name + "_Build_" + version + ".zip"

def display_success(build_statuses : dict) -> None:
    msg = "No successful builds."

    if build_statuses is not None and len(build_statuses) > 0:
        msg = "Successful builds:\n\n"

        for build_folder in build_statuses.keys():
            status = build_statuses[build_folder]
            msg += f"{build_folder} : {status}\n\n"

    msg += "\nScript complete."

    msgbox(msg=msg, title=title)

def main():
    global version
    version = get_version()

    if version is None:
        return

    choices = get_build_folder_selections()

    if choices is None:
        return

    build_statuses = zip_build_folder(choices=choices)

    print(build_statuses)



if __name__ == "__main__":
    main()
