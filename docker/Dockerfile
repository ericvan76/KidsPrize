FROM microsoft/dotnet:1.1-runtime
EXPOSE 80

WORKDIR /api
COPY assets ./

ENTRYPOINT [ "dotnet", "KidsPrize.Http.dll", "--urls", "http://*:80" ]
