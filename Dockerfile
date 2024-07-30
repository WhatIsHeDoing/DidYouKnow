FROM ubuntu:24.04

# https://stackoverflow.com/a/35976127
ENV CI=1 DEBIAN_FRONTEND=noninteractive

WORKDIR /build

RUN apt-get update && \
    apt-get install -y apt-utils dialog && \
    echo 'debconf debconf/frontend select Noninteractive' | debconf-set-selections

RUN apt-get install -y nodejs npm \
    # https://pnpm.io/installation#using-npm
    && npm install -g --no-progress npm pnpm

# Copy now so that the previous installation steps are cached.
COPY . .

RUN cd javascript \
    && pnpm i \
    && pnpm test \
    && cd ..

RUN cd python \
    && python3 main.py \
    && cd ..

RUN cd cpp \
    && g++ -o build/main.exe main.cpp \
    && ./build/main.exe \
    cd ..

ENTRYPOINT ["/bin/bash"]
