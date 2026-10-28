#!/usr/bin/env bash
# Builds the client and the API into ./publish and uploads them to MonsterASP over SFTP.
# Usage: deploy/deploy.sh   (sftp asks for the password)
# SFTP_TARGET (user@host) comes from the environment or the root .env file.
# appsettings.Production.json with the connection string lives only on the server and is never uploaded.
set -euo pipefail

ROOT="$(cd "$(dirname "$0")/.." && pwd)"

if [[ -z "${SFTP_TARGET:-}" && -f "$ROOT/.env" ]]; then
  set -a
  source "$ROOT/.env"
  set +a
fi
: "${SFTP_TARGET:?SFTP_TARGET is not set (add it to .env, see .env.example)}"

cd "$ROOT/client"
npx ng build

cd "$ROOT"
rm -rf publish
dotnet publish server/src/Comments.Api -c Release -r win-x86 --self-contained false -o publish
mkdir -p publish/wwwroot
cp -R client/dist/client/browser/. publish/wwwroot/

# app_offline.htm makes IIS stop the app and release its DLLs; removing it starts the new version.
# A leading "-" tells sftp to ignore the error (e.g. nothing to delete on the first deploy).
sftp "$SFTP_TARGET" <<'EOF'
cd wwwroot
put deploy/app_offline.htm
!sleep 3
-rm wwwroot/*
put -r publish/* .
rm app_offline.htm
bye
EOF

echo "Deployed. Open https://comments-spa.runasp.net"
