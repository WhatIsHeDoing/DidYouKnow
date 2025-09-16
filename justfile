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

# Installs JavaScript dependencies.
js-install:
    cd javascript && pnpm i

# Runs all tests.
run: csharp cpp javascript python

# Runs C# tests.
csharp:
    cd csharp && dotnet test

# Compiles and runs C++ tests.
cpp:
    cd cpp && g++ -o build/main.exe main.cpp && ./build/main.exe

# Runs JavaScript tests.
javascript:
    cd javascript && pnpm lint && pnpm test

# Runs Perl tests.
perl:
    cd perl && perl main.t

# Runs Python tests.
python:
    cd python && ruff check && python3 main.py

# Installs Python dependencies.
python-install:
    pip install -r python/requirements.txt

# Builds and runs a Docker container for portable testing.
docker: docker_build docker_run

# Builds a Docker container.
docker_build:
    docker build --progress=plain -f Dockerfile -t {{container}} .

# Runs the test Docker container.
docker_run:
    docker run -it {{container}}
