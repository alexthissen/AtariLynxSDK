# See here for image contents: https://github.com/microsoft/vscode-dev-containers/tree/v0.154.0/containers/alpine/.devcontainer/base.Dockerfile

# [Choice] Alpine version: 3.12, 3.11, 3.10
ARG VARIANT="3.12"
FROM mcr.microsoft.com/vscode/devcontainers/base:0-alpine-${VARIANT}

RUN apk add --no-cache --virtual .build-deps git build-base
RUN mkdir -p /src
WORKDIR /src
RUN git clone https://github.com/cc65/cc65.git
RUN git clone https://github.com/42Bastian/sprpck.git
WORKDIR /src/cc65
RUN nice make -j2
RUN make install PREFIX=/usr
WORKDIR /src/sprpck/src
RUN make
RUN install sprpck /usr/bin
WORKDIR /src
