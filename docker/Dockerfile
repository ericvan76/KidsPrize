FROM microsoft/dotnet:2.0-runtime
EXPOSE 80

VOLUME [ "/service/logs" ]

WORKDIR /service
COPY assets ./

ENTRYPOINT [ "dotnet", "KidsPrize.Http.dll", "--urls", "http://*:80" ]
