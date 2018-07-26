FROM microsoft/dotnet:2.0-runtime
EXPOSE 80

WORKDIR /service
COPY assets ./

ENTRYPOINT [ "dotnet", "KidsPrize.Http.dll", "--urls", "http://*:80" ]
