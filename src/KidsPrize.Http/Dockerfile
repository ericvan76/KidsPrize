FROM microsoft/dotnet:1.1.0-runtime
WORKDIR /app
COPY . .
EXPOSE 5000/tcp
ENV ASPNETCORE_URLS http://*:5000
ENTRYPOINT dotnet KidsPrize.Http.dll