FROM microsoft/dotnet:latest
COPY bin/Release/netcoreapp1.0/publish/ /root/
EXPOSE 4999/tcp
WORKDIR /root
ENTRYPOINT dotnet IdentityServer.dll