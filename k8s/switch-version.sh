#!/bin/bash

# Script to switch the app service between blue and green versions
# Usage: ./switch-version.sh <version>
# Where <version> is 'blue' or 'green'

if [ "$#" -ne 1 ]; then
    echo "Usage: $0 <version>"
    echo "Version must be 'blue' or 'green'"
    exit 1
fi

VERSION=$1

if [ "$VERSION" != "blue" ] && [ "$VERSION" != "green" ]; then
    echo "Error: Version must be 'blue' or 'green'"
    exit 1
fi

echo "Switching app service to version: $VERSION"

# Patch the service selector
microk8s kubectl patch service app -p "{\"spec\":{\"selector\":{\"version\":\"$VERSION\"}}}"

if [ $? -eq 0 ]; then
    echo "Successfully switched to $VERSION version"
else
    echo "Failed to switch version"
    exit 1
fi