# Run all test runners in CI mode.
export CI := "1"

# Enables commands to be selected interactively.
default:
    @just --choose

# Run all install and run commands.
go: install run

# Installs all dependencies.
install: js-install

# Installs JavaScript dependencies.
js-install:
    cd javascript && pnpm i

# Runs all tests.
run: csharp cpp javascript-run python

# Runs C# tests.
csharp:
    cd csharp && dotnet test

# Compiles and runs C++ tests.
cpp:
    cd cpp && g++ -o build/main.exe main.cpp && ./build/main.exe

# Runs JavaScript tests.
javascript-run:
    cd javascript && pnpm test

# Runs Perl tests.
perl:
    cd perl && perl main.t

# Runs Python tests.
python:
    cd python && python3 main.py
