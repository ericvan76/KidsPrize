FROM microsoft/dotnet:latest

RUN apt-get update && apt-get install -y supervisor
COPY supervisord.conf /root/supervisord.conf

COPY IdentityServer/bin/Release/netcoreapp1.0/publish/ /root/idsvr
COPY KidsPrize.Http/bin/Release/netcoreapp1.0/publish/ /root/api

EXPOSE 4999 5000

ENTRYPOINT ["/usr/bin/supervisord", "-c", "/root/supervisord.conf"]
