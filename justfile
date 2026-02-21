container := "did_you_know"

# Run all test runners in CI mode.

export CI := "1"

# Enables commands to be selected interactively.
default:
    @just --choose

# Run all install and run commands.
go: install run spellcheck

# Installs all dependencies.
install: js-install perl-install python-install

# Spellchecks the repository.
[working-directory("javascript")]
spellcheck:
    pnpm cspell ../**/*

# Updates all dependencies.
update: csharp-update js-update rust-update

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
run: csharp cpp javascript perl python rust

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

# Installs Perl dependencies.
[group("install")]
perl-install:
    sudo cpan -T App::cpanminus
    cpanm --local-lib=~/perl5 Test::Class File::Slurp

# Runs Perl tests.
[working-directory("perl")]
perl:
    PERL5LIB="$HOME/perl5/lib/perl5${PERL5LIB:+:${PERL5LIB}}" perl -I. main.t

# Runs Rust linting and tests.
[working-directory("rust")]
rust:
    cargo fmt -- --check
    cargo clippy -- -D warnings
    cargo test

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

# Updates Rust dependencies.
[group("update")]
[working-directory("rust")]
rust-update:
    cargo update

# Sets up a Mac to run these tests.
[macos]
setup:
    brew install dotnet-sdk node uv

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
