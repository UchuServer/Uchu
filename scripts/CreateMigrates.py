"""
TheNexusAvenger

Helper script for creating database migrates.
"""

import re
import os
import subprocess
from typing import Callable
from Helper.MySqlHelper import performMySqlTask


def createMigrate(contextName: str, migrationName: str) -> None:
    """Creates a database migrate for a database context.

    :param contextName: Name of the database context file.
    :param migrationName: Name of the database migrate to create.
    """

    # Start the dotnet build.
    uchuCoreDirectory = os.path.realpath(os.path.join(__file__, "..", "..", "Uchu.Core"))
    process = subprocess.Popen(["dotnet", "ef", "migrations", "add", migrationName, "--context", contextName], cwd=uchuCoreDirectory)

    # Wait for the build to complete and throw an exception if it failed (exit code is not 0).
    process.wait()
    if process.returncode != 0:
        raise Exception("Migrate add process failed with exist code " + str(process.returncode))


def prepareMySqlMigrateCallable(migrationName: str) -> Callable[[int], None]:
    """Creates the callable for a database MySQL migration since it requires an actual database.

    :param migrationName: Name of the database migrate to create.
    :return: Callable to use with prepareMySqlMigrateCallable.
    """

    def createMySqlMigration(port: int) -> None:
        """Creates the MySql database migrate.

        :param port: Port that the MySql server is running on.
        """

        # Read the original context file.
        contextFileLocation = os.path.realpath(os.path.join(__file__, "..", "..", "Uchu.Core", "Database", "Providers", "MySqlContext.cs"))
        with open(contextFileLocation) as file:
            originalContextFileContents = file.read()

        # Get the connection string.
        pattern = re.compile(r"\$\"(Server.+)\";", re.DOTALL)
        connectionString = pattern.findall(originalContextFileContents)[0]

        # Modify the context file with the connection to temporarily use.
        # There may be a better way to do this in-code. This works without code changes though.
        modifiedContextFileContents = originalContextFileContents.replace(connectionString, "Server=127.0.0.1;Port=" + str(port) + ";Database=Uchu;Uid=root;")
        with open(contextFileLocation, "w") as file:
            file.write(modifiedContextFileContents)

        # Create the database migrate and revert the file.
        try:
            createMigrate("MySqlContext", "MySql" + migrationName)
        except Exception as e:
            raise e
        finally:
            with open(contextFileLocation, "w") as file:
                file.write(originalContextFileContents)

    return createMySqlMigration


def createMigrates(migrationName: str) -> None:
    """Creates database migrates for the supported database contexts.

    :param migrationName: Name of the database migrate to create.
    """

    # Create the normal database migrates.
    createMigrate("SqliteContext", "Sqlite" + migrationName)
    createMigrate("PostgresContext", "Postgres" + migrationName)

    # Create the MySQL database migrate with a temporary MySQL database.
    performMySqlTask(prepareMySqlMigrateCallable(migrationName))


if __name__ == '__main__':
    import sys

    # Get the database migrate name.
    if len(sys.argv) < 2:
        print("Usage: CreateMigrates.py migrateName")
        exit(-1)
    migrateName = sys.argv[1]

    # Create the database migrates.
    createMigrates(migrateName)
