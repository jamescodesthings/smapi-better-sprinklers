#!/usr/bin/env bash

VERSION=$1

MANIFEST=BetterSprinklers/manifest.json

if [ "$VERSION" == "" ]; then
  VERSION=$(git describe --tags --abbrev=0)  
fi

echo "Setting version to $VERSION"

cp $MANIFEST temp.json
cat temp.json | jq ".Version |= \"$VERSION\"" > $MANIFEST
rm temp.json