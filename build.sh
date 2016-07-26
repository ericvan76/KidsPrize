#!/bin/bash

#dotnet restore

# Publish
dotnet publish -c Release src/IdentityServer
dotnet publish -c Release src/KidsPrize.Http

# Docker build
docker-compose build --force-rm
