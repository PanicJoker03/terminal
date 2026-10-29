#!/bin/bash

# generate publish: dotnet publish -c Release -o ./publish/

echo "copying site to aegis-hyper.com/terminal/"

sudo cp -r ./publish/* /var/www/terminal-api/
# sudo dotnet /var/www/terminal-api/terminal-app.dll --urls http://localhost:5000

echo "publish completed"
