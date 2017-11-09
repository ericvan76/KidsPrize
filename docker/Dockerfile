FROM microsoft/dotnet:1.1-runtime

EXPOSE 80

ENV kidsprize:ConnectionStrings:DefaultConnection Server=db;Port=5432;Database=postgres;User Id=kidsprize;Password=password;
ENV kidsprize:JwtBearerOptions:Authority https://apperic.auth0.com/
ENV kidsprize:JwtBearerOptions:Audience <audience>
ENV kidsprize:HealthCheckPingUrl <health-check-ping-url>

WORKDIR /api
COPY assets ./

ENTRYPOINT [ "dotnet", "KidsPrize.Http.dll", "--urls", "http://*:80" ]
