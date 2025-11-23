#!/bin/bash

mkdir -p /app-logs

APP_LOGS="/app-logs/app.log"
PROVISION_LOG="/app-logs/ready_provision.log"
APP_READY_LOG="app-logs/ready.txt"

log() {
  local timestamp
  timestamp="$(date -Iseconds)"
  echo "[$timestamp] $*" | tee -a $APP_LOGS
}

provision_log() {
  local timestamp
  timestamp="$(date -Iseconds)"
  echo "[$timestamp] $*" | tee -a $PROVISION_LOG
}

ready_log() {
  local timestamp
  timestamp="$(date -Iseconds)"
  echo "[$timestamp] $*" | tee -a 
}  


# Truncate logs
echo "" > $APP_LOGS
echo "" > $PROVISION_LOG

# Remove ready flag before building 
if [ -f "$APP_READY_LOG" ]; then
    rm "$APP_READY_LOG"
fi


log "Trying to publish app again (in case of changes)" 
dotnet publish --project /dotpics -c Release -o /dotpics/publish

if command -v systemctl >/dev/null 2>&1; then
  if systemctl list-unit-files --type=service | grep -q '^myapp.service'; then
    log "myapp.service already installed; attempting to start it"
    systemctl daemon-reload || true
    systemctl restart myapp.service || log "systemctl restart myapp.service returned non-zero"
#   else
#     attempts=$attempts-1
#     if [ "$attempts" -lt 0 ]; then
#         break
#     fi
    # if [ -x /usr/local/bin/start_myapp_service.sh ]; then
    #   log "Found /usr/local/bin/start_myapp_service.sh; running to create/start service"
    #   /usr/local/bin/start_myapp_service.sh || log "start_myapp_service.sh returned non-zero"
    # elif [ -x ./start_myapp_service.sh ]; then
    #   log "Found ./start_myapp_service.sh in repo; running to create/start service"
    #   ./start_myapp_service.sh || log "./start_myapp_service.sh returned non-zero"
    # else
    #   log "No service helper found; will attempt to start service directly if unit exists"
    # fi
  fi

  # Give systemd a moment to settle
  sleep 1
fi

if command -v systemctl >/dev/null 2>&1 && systemctl is-active --quiet myapp.service; then
  log "myapp.service is active; not starting app with nohup"
else
  log "Service not active or systemd missing; falling back to nohup start"
  nohup dotnet run --project /dotpics >> "$APP_LOGS" 2>&1 &
  log "App started in background via nohup"
fi

log "Checking if mongodb service is running"
if ! systemctl is-active --quiet mongod.service; then
    echo "Starting MongoDB..."
    sudo systemctl start mongod.service
fi
