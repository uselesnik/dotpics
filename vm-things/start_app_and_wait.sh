#!/bin/bash

mkdir -p /app-logs

APP_LOGS="/app-logs/app.log"
PROVISION_LOG="/app-logs/ready_provision.log"
APP_READY_LOG="/app-logs/ready.txt"
APP_PUBLISH_DIR="/dotpics-publish"
APP_PUBLISH_NAME="DotPic.dll"

log() {
  local timestamp
  timestamp="$(date -Iseconds)"
  echo "[$timestamp] $*" | tee -a "$APP_LOGS"
}

provision_log() {
  local timestamp
  timestamp="$(date -Iseconds)"
  echo "[$timestamp] $*" | tee -a "$PROVISION_LOG"
}

ready_log() {
  local timestamp
  timestamp="$(date -Iseconds)"
  echo "[$timestamp] $*" | tee -a "$APP_READY_LOG"
}  


# Truncate logs
echo "" > "$APP_LOGS"
echo "" > "$PROVISION_LOG"

# Remove ready flag before building 
if [ -f "$APP_READY_LOG" ]; then
    rm "$APP_READY_LOG"
fi

cd /dotpics || { log "Failed to move to project directory (Does it exist?)"; exit; }

log "Trying to publish app again (in case of changes)" 
PUBLISHED=0
if dotnet publish /dotpics/DotPic.csproj -c Release -o "$APP_PUBLISH_DIR"; then
  PUBLISHED=1
fi

if command -v systemctl >/dev/null 2>&1; then
  if systemctl list-unit-files --type=service | grep -q '^myapp.service'; then
    log "myapp.service already installed; attempting to start it"
    systemctl daemon-reload || true
    systemctl restart myapp.service || log "systemctl restart myapp.service returned non-zero"
  fi
  sleep 1
fi

if command -v systemctl >/dev/null 2>&1 && systemctl is-active --quiet myapp.service; then
  log "myapp.service is active; not starting app with nohup"
else
  log "Service not active or systemd missing; falling back to nohup start"
  if $PUBLISHED; then
    cd "$APP_PUBLISH_DIR" || { log "How is the app published, but the directory doesn't exist?"; exit; }
    nohup dotnet "$APP_PUBLISH_DIR/$APP_PUBLISH_NAME" >> "$APP_LOGS" 2>&1 &
  else
    cd "$APP_PROJECT_DIR" || { log "The Project directory doesn't exist?"; exit; }
    nohup dotnet run --project /dotpics >> "$APP_LOGS" 2>&1 &
  fi
  log "App started in background via nohup"
fi

log "Checking if mongodb service is running"
if ! systemctl is-active --quiet mongod.service; then
    echo "Starting MongoDB..."
    sudo systemctl start mongod.service
fi
