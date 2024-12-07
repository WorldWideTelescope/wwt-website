FROM node:20 AS builder

WORKDIR /src
RUN git clone https://github.com/WorldWideTelescope/wwt-web-client

WORKDIR /src/wwt-web-client

COPY profile.yml profile-localtest.yml

RUN npm install
RUN npx grunt dist-localtest

FROM node:22

RUN npm install -g http-server@14.1.1
WORKDIR /webclient
COPY --from=builder /src/wwt-web-client/dist .

ENTRYPOINT [ "npx", "http-server", "." ]