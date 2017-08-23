#!/bin/bash
set -ev

# dotnet build & publish
dotnet restore
dotnet test ./KidsPrize.Tests/KidsPrize.Tests.csproj
rm -rf $(pwd)/publish
dotnet publish -c Release ./KidsPrize.Http/KidsPrize.Http.csproj -o $(pwd)/publish

# do not build docker for pull request
if [[ $TRAVIS_PULL_REQUEST = true ]]; then exit 0; fi

export REPO_NAME="ericvan76/kidsprize"

# build docker
docker build publish -t $REPO_NAME

# tag
if [[ ! -z $TRAVIS_TAG ]]
then
    docker tag $REPO_NAME $REPO_NAME:$TRAVIS_TAG
fi

# docker push
docker login -u="$DOCKER_USERNAME" -p="$DOCKER_PASSWORD"
docker push $REPO_NAME
