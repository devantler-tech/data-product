#!/bin/bash
echo "📦 Copying config file to build directory 📦"
if ([ -f /app/config.yaml ]); then
    cp /app/config.yaml /build/src/Devantler.DataMesh.DataProduct/config.yaml
elif ([ -f /app/config.yml ]); then
    cp /app/config.yml /build/src/Devantler.DataMesh.DataProduct/config.yml
elif ([ -f /app/config.json ]); then
    cp /app/config.json /build/src/Devantler.DataMesh.DataProduct/config.json
fi

echo "📦 Publishing Data Product 📦"

dotnet publish /build/src/Devantler.DataMesh.DataProduct -c Release --no-restore -o /app/publish
cp -r /app/publish/* /app

echo "🧹 Cleaning up 🧹"
apt-get autoremove -y
apt-get autoclean -y
apt-get clean -y
rm -rf /usr/share/dotnet
rm -rf /var/lib/apt/lists/*
rm -rf /build
rm -rf /app/publish
rm -rf /tmp/*

echo "⚙️ Install ASP.NET Core Runtime ⚙️"
wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
chmod +x ./dotnet-install.sh
./dotnet-install.sh --channel 7.0 --runtime aspnetcore
rm -rf dotnet-install.sh

echo "🚀 Starting Data Product 🚀"
adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
su appuser

cd /app
$HOME/.dotnet/dotnet Devantler.DataMesh.DataProduct.dll
