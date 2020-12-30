FROM alpine:latest AS build
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

# TODO: Optimize final image

#RUN apk add --update --no-cache python3 && ln -sf python3 /usr/bin/python
#RUN python3 -m ensurepip
#RUN pip3 install --no-cache --upgrade pip setuptools