#!/bin/bash

# generate publish: dotnet publish -c Release -o ./publish/

echo "copying site to aegis-hyper.com/terminal/"

sudo cp -r ./publish/* /var/www/golem-sharp/
# sudo dotnet /var/www/golem-sharp/GolemSharp.dll --urls http://localhost:5000

echo "publish completed"
