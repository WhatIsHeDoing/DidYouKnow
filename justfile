container := "did_you_know"

# Run all test runners in CI mode.

export CI := "1"

# Enables commands to be selected interactively.
default:
    @just --choose

# Run all install and run commands.
go: install run spellcheck lint

# Installs all dependencies.
install: js-install perl-install python-install

# Spellchecks the repository.
[working-directory("javascript")]
spellcheck:
    pnpm cspell ../**/*

# Updates all dependencies.
update: csharp-update js-update rust-update

# Migrate any code artifacts to newer versions
[working-directory("javascript")]
migrate:
    pnpm biome migrate --fix

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

# Lints all languages.
lint: csharp-lint cpp-lint javascript-lint perl-lint python-lint rust-lint

# Lints C#.
[group("lint")]
[working-directory("csharp")]
csharp-lint:
    dotnet format --verify-no-changes

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

# Lints C++.
[group("lint")]
[working-directory("cpp")]
cpp-lint:
    clang-format --dry-run --Werror main.cpp

# Compiles and runs C++ tests.
[working-directory("cpp")]
cpp:
    g++ -o build/main.exe main.cpp
    ./build/main.exe

# Lints JavaScript.
[group("lint")]
[working-directory("javascript")]
javascript-lint:
    pnpm lint

# Runs JavaScript tests.
[working-directory("javascript")]
javascript:
    pnpm test

# Installs Perl dependencies.
[group("install")]
perl-install:
    cpanm --local-lib=~/perl5 Test::Class File::Slurp Perl::Critic

# Lints Perl.
[group("lint")]
[working-directory("perl")]
perl-lint:
    PERL5LIB="$HOME/perl5/lib/perl5${PERL5LIB:+:${PERL5LIB}}" $HOME/perl5/bin/perlcritic .

# Runs Perl tests.
[working-directory("perl")]
perl:
    PERL5LIB="$HOME/perl5/lib/perl5${PERL5LIB:+:${PERL5LIB}}" perl -I. main.t

# Lints Rust.
[group("lint")]
[working-directory("rust")]
rust-lint:
    cargo fmt -- --check
    cargo clippy -- -D warnings

# Runs Rust tests.
[working-directory("rust")]
rust:
    cargo test

# Lints Python.
[group("lint")]
[working-directory("python")]
python-lint:
    uv run ruff format --check
    uv run ruff check

# Runs Python tests.
[working-directory("python")]
python:
    uv run python -W error main.py

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
    brew install clang-format cpanminus dotnet-sdk node uv

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
