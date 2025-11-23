#!/bin/bash
set -euo pipefail

LOGDIR="/app-logs"
mkdir -p "$LOGDIR"
LOGFILE="$LOGDIR/prereqs.log"
DOMAIN="${DOMAIN:-devops-sk-21.lrk.si}"
EMAIL="${EMAIL:-eg98918@student.uni-lj.si}"
APP_PROJECT_DIR="/dotpics"
APP_GIT_LINK="https://github.com/uselesnik/dotpics.git"

log () { 
    local ts
    ts="$(date -Iseconds)"
    echo "[$ts] $*" | tee -a "$LOGFILE"
}

# Truncate when running for the first time
log "Installing packages (curl, nginx, git)" > "$LOGFILE" 

apt-get update -y

apt-get install -y software-properties-common curl nginx certbot python3-certbot-nginx git

log "Installed packages"

log "Clonning project repository" 

cd / 
git clone "$APP_GIT_LINK"

if [ -d "$APP_PROJECT_DIR" ]; then
    log "Repository correctly cloned to $APP_PROJECT_DIR !" 
else 
    log "Critical failure! Unable to clone app, check github link: $APP_GIT_LINK"
    exit 1
fi

log "Cloned repository" 

log "Adding dotnet backport"

add-apt-repository -y ppa:dotnet/backports

log "Added dotnet backport"

log "Adding mongodb repo"

curl -fsSL https://www.mongodb.org/static/pgp/server-8.0.asc | gpg -o /usr/share/keyrings/mongodb-server-8.0.gpg --dearmor
echo "deb [ arch=amd64,arm64 signed-by=/usr/share/keyrings/mongodb-server-8.0.gpg ] https://repo.mongodb.org/apt/ubuntu noble/mongodb-org/8.0 multiverse" | tee /etc/apt/sources.list.d/mongodb-org-8.0.list

log "MongoDB repo added" 

log "Installing dotnet and mongodb"

apt-get update -y
apt-get install -y dotnet-sdk-9.0 mongodb-org

log "Succesfully installed dotnet and mongodb" 

log "Now making service for myapp" 

cat > /etc/systemd/system/myapp.service <<'EOF'
[Unit]
Description=My .NET App service
After=network.target

[Service]
# Run the published DLL for best performance. Working directory points to the publish output.
WorkingDirectory=/dotpics
# ExecStart runs the published dll. Ensure `dotnet publish` has been run to produce this file.
ExecStart=/usr/bin/dotnet /dotpics/publish/change-this-before-release
StandardOutput=append:/app-logs/myapp.log
StandardError=inherit
Restart=always
RestartSec=5
SyslogIdentifier=myapp
User=root
Environment=ASPNETCORE_ENVIRONMENT=Production
# Use ASPNETCORE_URLS so the process listens on the loopback TLS port expected by nginx
Environment=ASPNETCORE_URLS=https://127.0.0.1:5000
Environment=ASPNETCORE_HTTPS_PORT=443
Environment=ASPNETCORE_FORWARDEDHEADERS_ENABLED=true

[Install]
WantedBy=multi-user.target
EOF

log "Reloading daemon" 
systemctl daemon-reload

log "Enabling myapp.service"
systemctl enable myapp.service || log "systemctl enable returned non-zero (maybe already enabled)"


log "preparing nginx configuration" 
log "Writing nginx site config for $DOMAIN"
NGINX_CONF="/etc/nginx/sites-available/myapp"
cat > "$NGINX_CONF" <<EOF
server {
    listen 80;
    server_name $DOMAIN;

    location / {
      proxy_pass https://127.0.0.1:5000;
      proxy_http_version 1.1;
      proxy_set_header Upgrade \$http_upgrade;
      proxy_set_header Connection keep-alive;
      proxy_set_header Host \$host;
      proxy_set_header X-Real-IP \$remote_addr;
      proxy_set_header X-Forwarded-For \$proxy_add_x_forwarded_for;
      proxy_set_header X-Forwarded-Proto \$scheme;

      proxy_set_header X-Forwarded-Port \$server_port;

      # Rewrite upstream Location headers so redirects don't expose the internal port (5000)
      # This handles cases where Kestrel generates a redirect to https://...:5000
      proxy_redirect https://127.0.0.1:5000/ https://\$host/;
      proxy_redirect http://127.0.0.1:5001/ http://\$host/;
    }
}
EOF

ln -sf "$NGINX_CONF" /etc/nginx/sites-enabled/myapp

log "Testing nginx and reloading nginx"

nginx -t && systemctl reload nginx

log "nginx tested and reloaded" 

log "Attempting to obtain Let's Encrypt certificate for $DOMAIN"
if certbot --nginx -d "$DOMAIN" --agree-tos --non-interactive -m "$EMAIL" --redirect; then
  log "certbot succeeded"
else
  log "certbot failed (check DNS/ports), continuing without TLS"
fi

log "Prerequisites setup finished. Logs: $LOGFILE"