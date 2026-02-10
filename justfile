container := "did_you_know"

# Run all test runners in CI mode.

export CI := "1"

# Enables commands to be selected interactively.
default:
    @just --choose

# Run all install and run commands.
go: install run

# Installs all dependencies.
install: js-install python-install

# Updates all dependencies.
update: csharp-update js-update

# Installs JavaScript dependencies.
[group("install")]
[working-directory("javascript")]
js-install:
    pnpm i

# Updates JavaScript dependencies.
[group("update")]
[working-directory("javascript")]
js-update:
    pnpm update --interactive --latest

# Runs all tests.
run: csharp cpp javascript python

# Runs C# tests.
[working-directory("csharp")]
csharp:
    dotnet test

# Updates C# dependencies.
[group("update")]
[working-directory("csharp")]
csharp-update:
    dotnet tool update -g dotnet-outdated-tool
    dotnet tool update -g upgrade-assistant
    dotnet outdated --upgrade
    upgrade-assistant upgrade csharp.csproj

# Compiles and runs C++ tests.
[working-directory("cpp")]
cpp:
    g++ -o build/main.exe main.cpp
    ./build/main.exe

# Runs JavaScript tests.
[working-directory("javascript")]
javascript:
    pnpm lint
    pnpm test

# Runs Perl tests.
[working-directory("perl")]
perl:
    perl main.t

# Runs Python tests.
[working-directory("python")]
python:
    uv run ruff check
    uv run main.py

# Installs Python dependencies.
[group("install")]
[working-directory("python")]
python-install:
    uv sync

# Builds and runs a Docker container for portable testing.
[group("docker")]
docker: docker_build docker_run

# Builds a Docker container.
[group("docker")]
docker_build:
    docker build --progress=plain -f Dockerfile -t {{ container }} .

# Runs the test Docker container.
[group("docker")]
docker_run:
    docker run -it {{ container }}
