#!/usr/bin/env bash
# Builds only the client and uploads it to wwwroot on MonsterASP over SFTP; the API keeps running.
# Usage: deploy/deploy-client.sh   (sftp asks for the password)
# SFTP_TARGET (user@host) comes from the environment or the root .env file.
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
mkdir -p publish/wwwroot
cp -R client/dist/client/browser/. publish/wwwroot/
mv publish/wwwroot/index.html publish/index.html

# Hashed bundles go first and index.html last, so a page loaded mid-upload never points
# at bundles that are not there yet. Old bundles stay until the next full deploy.
sftp "$SFTP_TARGET" <<'EOF'
cd wwwroot/wwwroot
put -r publish/wwwroot/* .
put publish/index.html
bye
EOF

echo "Client deployed. Open https://comments-spa.runasp.net"
