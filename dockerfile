FROM alpine:latest AS build
RUN apk add --no-cache --virtual .build-deps git build-base
RUN mkdir -p /src
WORKDIR /src
RUN git clone https://github.com/cc65/cc65.git
WORKDIR /src/cc65
RUN nice make -j2
RUN make install PREFIX=/usr

# TODO: Optimize final image
 