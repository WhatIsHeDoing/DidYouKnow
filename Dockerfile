FROM ubuntu:24.04

# https://stackoverflow.com/a/35976127
ENV CI=1 DEBIAN_FRONTEND=noninteractive

WORKDIR /build

RUN apt-get update && \
    apt-get install -y apt-utils dialog && \
    echo 'debconf debconf/frontend select Noninteractive' | debconf-set-selections && \
    snap install --edge --classic just

RUN apt-get install -y nodejs npm \
    # https://pnpm.io/installation#using-npm
    && npm install -g --no-progress npm pnpm

# Copy now so that the previous installation steps are cached.
COPY . .

RUN just javascript
RUN just python
RUN just cpp

ENTRYPOINT ["/bin/bash"]
