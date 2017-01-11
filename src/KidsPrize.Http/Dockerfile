FROM microsoft/dotnet:1.0.1-core
WORKDIR /app
COPY . .
EXPOSE 5000/tcp
ENV ASPNETCORE_URLS http://*:5000
ENTRYPOINT dotnet KidsPrize.Http.dll