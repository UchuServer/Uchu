"""
TheNexusAvenger, enteryournamehere

Creates the binaries for distribution.
"""

PROJECTS = [
    "Uchu.Master",
    "Uchu.Instance",
    "Uchu.StandardScripts",
]
DIRECTORY_ADDITIONS = {
    "Uchu.Instance": "/lib",
    "Uchu.StandardScripts": "/lib",
}
PLATFORMS = [
    ["Windows-x64", "win-x64"],
    ["macOS-x64", "osx-x64"],
    ["Linux-x64", "linux-x64"],
]

import os
import shutil
import subprocess
import sys



# Display a warning for Windows runs.
if os.name == "nt":
    sys.stderr.write("Windows was detected. Linux and macOS binaries will be missing the permissions to run.\n")

# Create the directory.
if os.path.exists("bin"):
    shutil.rmtree("bin")
os.mkdir("bin")

# Compile the releases.
for platform in PLATFORMS:
    # Create the base directory.
    print("Building Uchu for " + platform[0])
    platformDirectory = "bin/Uchu-" + platform[0]
    if os.path.exists(platformDirectory):
        shutil.rmtree(platformDirectory)
    os.mkdir(platformDirectory)

    for project in PROJECTS:
        # Compile the project for the platform.
        print("\tExporting " + project + " for " + platform[0])

        outputDirectory = platformDirectory + (project in DIRECTORY_ADDITIONS.keys() and DIRECTORY_ADDITIONS[project] or "")
        buildParameters = ["dotnet", "publish",
            "--runtime", platform[1],
            "--configuration", "Release",
            "--output", outputDirectory,
            project + "/" + project + ".csproj"
        ]
        subprocess.call(buildParameters, stdout=open(os.devnull, "w"))

        # Clear the unwanted files of the compile.
        for file in os.listdir(outputDirectory):
            if file.endswith(".pdb") or file.endswith(".bak"):
                os.remove(outputDirectory + "/" + file)

    # Add documentation & license to the output directory.
    for file in ["README.md", "Configuration.md", "LICENSE"]:
        shutil.copy(file, platformDirectory)

    # Create the archive.
    print("\tCreating archive for " + platform[0])
    shutil.make_archive("bin/Uchu-" + platform[0], "zip", platformDirectory)
    shutil.rmtree(platformDirectory, True)
