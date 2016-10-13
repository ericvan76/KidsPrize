#!/bin/bash
set -ev

# dotnet build & publish
dotnet restore
dotnet test ./test/KidsPrize.Tests
rm -rf $(pwd)/publish
dotnet publish -c Release ./src/KidsPrize.Http -o $(pwd)/publish

# Determine $TAG by GIT BRANCH AND TAG
echo TRAVIS_BRANCH=$TRAVIS_BRANCH
echo TRAVIS_TAG=$TRAVIS_TAG
if [ "$TRAVIS_BRANCH" = "master" ]; then export TAG=latest; fi
if [ "$TRAVIS_BRANCH" = "develop" ]; then export TAG=develop; fi
if [ "$TRAVIS_TAG" != "" ]; then export TAG=$TRAVIS_TAG; fi

# Validate $TAG
echo TAG=$TAG
if [ "$TAG" = "" ]; then exit 1; fi
export IMAGE_TAG="ericvan76/kidsprize:$TAG"
echo IMAGE_TAG=$IMAGE_TAG

# build docker
docker build publish -t "$IMAGE_TAG"

# docker push
docker login -u="$DOCKER_USERNAME" -p="$DOCKER_PASSWORD"
docker push "$IMAGE_TAG"

