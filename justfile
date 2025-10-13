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
update: csharp-update js-update python-update

# Installs JavaScript dependencies.
[working-directory: "javascript"]
[group("install")]
js-install:
    pnpm i

# Updates JavaScript dependencies.
[group("update")]
[working-directory: "javascript"]
js-update:
    pnpm update --interactive --latest

# Runs all tests.
run: csharp cpp javascript python

# Runs C# tests.
[working-directory: "csharp"]
csharp:
    dotnet test

# Updates C# dependencies.
[group("update")]
[working-directory: "csharp"]
csharp-update:
    dotnet tool install -g upgrade-assistant
    dotnet tool update -g dotnet-outdated-tool
    dotnet outdated --upgrade
    upgrade-assistant upgrade csharp.csproj

# Compiles and runs C++ tests.
[working-directory: "cpp"]
cpp:
    g++ -o build/main.exe main.cpp
    ./build/main.exe

# Runs JavaScript tests.
[working-directory: "javascript"]
javascript:
    pnpm lint
    pnpm test

# Runs Perl tests.
[working-directory: "perl"]
perl:
    perl main.t

# Runs Python tests.
[working-directory: "python"]
python:
    ruff check
    python3 main.py

# Installs Python dependencies.
[group("install")]
python-install:
    pip install -r python/requirements.txt

# Updates Python dependencies.
[working-directory: "python"]
[group("update")]
python-update:
    pur -r requirements.txt

# Builds and runs a Docker container for portable testing.
[group("docker")]
docker: docker_build docker_run

# Builds a Docker container.
[group("docker")]
docker_build:
    docker build --progress=plain -f Dockerfile -t {{container}} .

# Runs the test Docker container.
[group("docker")]
docker_run:
    docker run -it {{container}}
