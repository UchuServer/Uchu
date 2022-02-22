"""
TheNexusAvenger

Helper script for setting up temporary MySQL servers.
TODO: This does not check for local installs, so it does not work on macOS.
"""

import configparser
import os
import requests
import shutil
import socket
import subprocess
import tarfile
import time
import zipfile
from sys import platform
from typing import Callable


def downloadArchive(url: str, fileName: str, archiveType: str) -> None:
    """Downloads and extracts an archive.

    :param url: URL to download from.
    :param fileName: Path of the directory to download to.
    :param archiveType: Type of the archive to download.
    """

    # Set up the paths.
    extractDirectory = os.path.realpath(os.path.join(__file__, "..", fileName))
    archiveFile = extractDirectory + "." + archiveType
    if not os.path.exists(os.path.dirname(extractDirectory)):
        os.makedirs(os.path.dirname(extractDirectory))

    # Return if the downloaded directory exists.
    if os.path.exists(extractDirectory):
        return

    # Download the archive.
    if not os.path.exists(archiveFile):
        with open(archiveFile, "wb") as file:
            response = requests.get(url)
            file.write(response.content)

    # Extract the files.
    if not os.path.exists(extractDirectory):
        archiveType = archiveType.lower()
        if archiveType == "zip":
            file = zipfile.ZipFile(archiveFile)
            file.extractall(extractDirectory)
        elif archiveType == "tar.gz":
            file = tarfile.open(archiveFile)
            file.extractall(extractDirectory)
        else:
            raise Exception("Unsupported archive type: " + archiveType)

    # Move the extracted directory up if there is only 1 contained directory.
    if len(os.listdir(extractDirectory)) == 1:
        subDirectoryPath = str(os.path.join(extractDirectory, os.listdir(extractDirectory)[0]))
        if os.path.isdir(subDirectoryPath):
            shutil.move(subDirectoryPath, extractDirectory + "-tmp")
            shutil.rmtree(extractDirectory)
            shutil.move(extractDirectory + "-tmp", extractDirectory)


def getMariaDbDownload() -> (str, str, str):
    """Gets the MariaDB archive to download.

    :return: The URL, directory name, and archive type to use.
    """

    if platform == "win32":
        return "https://dlm.mariadb.com/1920019/MariaDB/mariadb-10.6.5/winx64-packages/mariadb-10.6.5-winx64.zip", "bin/mariadb-winx64", "zip"
    elif platform == "darwin":
        raise Exception("MaraiDB is not directly downloadable for macOS. This script does not support local installs of MySQL if you have one.")
    elif "linux" in platform:
        return "https://dlm.mariadb.com/1920008/MariaDB/mariadb-10.6.5/bintar-linux-systemd-x86_64/mariadb-10.6.5-linux-systemd-x86_64.tar.gz", "bin/mariadb-linux-systemd", "tar.gz"

    raise Exception("Unknown platform: " + platform)


def downloadMariaDb() -> str:
    """Downloads MariaDB.

    :return: The path of the folder for MariaDB.
    """

    url, relativePath, archiveType = getMariaDbDownload()
    downloadArchive(url, relativePath, archiveType)
    return os.path.realpath(os.path.join(__file__, "..", relativePath))


def portActive(port: int) -> bool:
    """Returns if a localhost port is active.

    :param port: Port to check.
    :return: Returns if the port is open.
    """

    sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    result = sock.connect_ex(("127.0.0.1", port))
    sock.close()
    return result == 0


def getRandomPort() -> int:
    """Gets a random port that is not in use.

    :return: Port that is open.
    """

    sock = socket.socket()
    sock.bind(("", 0))
    socketPort = sock.getsockname()[1]
    sock.close()
    return socketPort


def getExecutablePath(file: str) -> str:
    """Adds .exe to a path if an executable exists.

    :param file: File name to add .exe to if it exists.
    :return: The new path of the file.
    """

    if os.path.exists(file + ".exe"):
        return file + ".exe"
    return file


def performMySqlTask(task: Callable[[int], None]) -> None:
    """Runs a callable with a temporary MySQL server.

    :param task: Task to run when the MySQL server is up.
    """

    mariaDbDirectory = downloadMariaDb()
    mariaDbBinDirectory = os.path.join(mariaDbDirectory, "bin")
    mariaDbDataDirectory = os.path.join(mariaDbDirectory, "data")

    # Set up the database.
    if not os.path.exists(mariaDbDataDirectory):
        process = subprocess.Popen([getExecutablePath(os.path.join(mariaDbBinDirectory, "mysql_install_db"))], cwd=mariaDbBinDirectory)
        process.wait()
        if process.returncode != 0:
            raise Exception("DB install process failed.")

    # Set the random port.
    randomPort = getRandomPort()
    iniFile = os.path.join(mariaDbDataDirectory, "my.ini")
    config = configparser.ConfigParser()
    config.read(iniFile)
    if "client-server" not in config:
        config["client-server"] = {}
    config["client-server"]["port"] = str(randomPort)
    with open(iniFile, "w") as file:
        config.write(file)

    # Start the server and wait for it to be active.
    serverProcess = subprocess.Popen([getExecutablePath(os.path.join(mariaDbBinDirectory, "mysqld"))], cwd=mariaDbBinDirectory)
    while serverProcess.poll() is None and not portActive(randomPort):
        time.sleep(0.1)

    # Run the task and stop the database.
    try:
        task(randomPort)
    except Exception as e:
        raise e
    finally:
        serverProcess.kill()
