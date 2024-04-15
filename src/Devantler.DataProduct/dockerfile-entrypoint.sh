#!/bin/bash
echo "📦 Copying config files to build directory 📦"
for file in /app/config.*; do
  if [ -f "$file" ]; then
    cp "$file" /build/src/Devantler.DataProduct/"$(basename "$file")"
  fi
done

echo "📦 Publishing Data Product 📦"
dotnet publish /build/src/Devantler.DataProduct -c Release --no-restore -o /app

echo "🧹 Cleaning up 🧹"
apt-get autoremove -y
apt-get autoclean -y
apt-get clean -y
rm -rf /usr/share/dotnet
rm -rf /var/lib/apt/lists/*
rm -rf /build
rm -rf /app/publish
rm -rf /tmp/*

echo "⚙️ Installing and configuring ASP.NET Core Runtime ⚙️"
wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
chmod +x ./dotnet-install.sh
./dotnet-install.sh --channel 8.0 --runtime aspnetcore
rm -rf dotnet-install.sh

echo "🚀 Starting Data Product 🚀"
adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
su appuser

cd /app || exit
"$HOME"/.dotnet/dotnet Devantler.DataProduct.dll
