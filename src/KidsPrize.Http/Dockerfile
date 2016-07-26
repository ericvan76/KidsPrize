FROM microsoft/dotnet:latest
COPY bin/Release/netcoreapp1.0/publish/ /root/
EXPOSE 5000/tcp
WORKDIR /root
ENTRYPOINT dotnet KidsPrize.Http.dll