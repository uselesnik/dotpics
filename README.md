# DotPic - Blazor Image Storage App with MongoDB

A modern, cloud-based image storage application built with Blazor and MongoDB. Features include image uploads, gallery management, and user management.

## Features

### Core Functionality
- Image Upload - Drag-and-drop style upload with preview
- Image Gallery - Grid view with search and filtering
- Smart Search - Search by filename, description, or tags
- Image Management - Delete and manage uploaded images
- Responsive Design - Works on desktop and mobile devices



## Technology Stack

- Frontend: Blazor Server (.NET 8.0)
- Database: MongoDB (Document storage)
- UI: Bootstrap 5 + Custom CSS
- Icons: Bootstrap Icons

## Prerequisites

Before running this application, ensure you have the following installed:
- redis
- .NET 8.0 SDK
- MongoDB (v6.0+)

### Installation Commands

Ubuntu/Debian:
# Install MongoDB
```bash
wget -qO - https://www.mongodb.org/static/pgp/server-7.0.asc | sudo gpg --dearmor -o /usr/share/keyrings/mongodb-archive-keyring.gpg
echo "deb [ arch=amd64,arm64 signed-by=/usr/share/keyrings/mongodb-archive-keyring.gpg ] https://repo.mongodb.org/apt/ubuntu focal/mongodb-org/7.0 multiverse" | sudo tee /etc/apt/sources.list.d/mongodb-org-7.0.list
sudo apt-get update
sudo apt-get install -y mongodb-org

# Start MongoDB service
sudo systemctl start mongod
sudo systemctl enable mongod
```

# Build the project
```
dotnet build
```
# Run the application
```
dotnet run --urls="http://localhost:5000"
```
### 5. Access the Application
Open your browser and navigate to: http://localhost:5000

# Using docker 
Use the `Dockerfile` and `docker-compose.yml`, then start with `docker compose up`. The files have to be in the same directory as the project files (DotPic.csproj)

# Asciinema demonstration
The video is at 2x speed, and with some spelling mistakes, because I forgot to switch to sudo user before running some commands. Be aware this video does not demonstrate setting up certificates, nginx configuration or making a systemd service to run the app. 
[![asciicast](https://asciinema.org/a/Pe7yFyo1J8Uhw5cWYPjapmNAv.svg)](https://asciinema.org/a/Pe7yFyo1J8Uhw5cWYPjapmNAv)
## Asciinema Vagrant 
[![asciicast](https://asciinema.org/a/zQnVGylzy6KpOIozxcLVVlBKy.svg)](https://asciinema.org/a/zQnVGylzy6KpOIozxcLVVlBKy)
# Example of service file and nginx config
## Systemd-service
By enabling this service it will always run on startup, also can be restarted with `systemctl restart myapp.service` with sudo privileges. 
Remember to replace variables such as `$APP_PUBLISH_DIR` with actual values 
```
[Unit]
Description=My .NET App service
After=network.target

[Service]
# Run the published DLL for best performance. Working directory points to the publish output.
WorkingDirectory=$APP_PUBLISH_DIR
# ExecStart runs the published dll. Ensure dotnet publish has been run to produce this file.
ExecStart=/usr/bin/dotnet $APP_PUBLISH_DIR/$APP_PUBLISH_NAME
StandardOutput=append:$LOGDIR/myapp.log
StandardError=inherit
Restart=always
RestartSec=5
# This will allow you to restart the service with name set here
SyslogIdentifier=myapp
User=root
Environment=ASPNETCORE_ENVIRONMENT=Production
# Use ASPNETCORE_URLS so the process listens on the loopback TLS port expected by nginx
Environment=ASPNETCORE_URLS=https://127.0.0.1:5000
Environment=ASPNETCORE_HTTPS_PORT=443
Environment=ASPNETCORE_FORWARDEDHEADERS_ENABLED=true

[Install]
WantedBy=multi-user.target
```
## nginx configuration
before certbot letsencrypt certificate configuration you can write something like this in the nginx configuration files (`/etc/nginx/sites-available/myapp`). Running certbot will append information to the file afterwards. Variables like `$DOMAIN` must be replaced with actual values. 
```
server {
    listen 80;
    server_name $DOMAIN;

    location / {
      # This should mirror the URLS your app will be runnign on
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
      proxy_redirect http://127.0.0.1:5194/ http://\$host/;
    }
}
```

## Project Structure
```
DotPic/
├── Components/
│   ├── Layout/
│   │   ├── MainLayout.razor
│   │   └── NavMenu.razor
│   ├── Pages/
│   │   ├── Home.razor
│   │   ├── Upload.razor
│   │   ├── Gallery.razor
│   │   └── UserManager.razor
│   └── App.razor
├── Models/
│   ├── User.cs
│   ├── StoredImage.cs
│   └── MongoDbSettings.cs
├── Services/
│   ├── IUserService.cs
│   ├── UserService.cs
│   ├── IImageService.cs
│   └── ImageService.cs
├── Utilities/
│   └── ImageHelper.cs
├── Program.cs
├── appsettings.json
└── DotPic.csproj
```
## Key Features Explained

### Image Storage & Management
- Store images in MongoDB as binary data
- Support for JPG, PNG, GIF, BMP, and WebP formats
- File size limit: 5MB per image
- Automatic thumbnail generation and preview
- Metadata storage (filename, description, tags, upload date)

### Application Pages

#### Home Page
- Welcome page with application overview
- Basic functionality testing
- Navigation to all features

#### Upload Page
- File selection with drag-and-drop support
- Image preview before upload
- Metadata input (description and tags)
- File validation and size limits

#### Gallery Page
- Grid view of all uploaded images
- Search functionality across filenames, descriptions, and tags
- Image details modal with full information
- Delete functionality with confirmation


## Configuration

### MongoDB Settings
{
  "MongoDbSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "DotPicDb"
  }
}

### Environment Variables
export ConnectionStrings__MongoDb="mongodb://localhost:27017"
export ASPNETCORE_ENVIRONMENT="Development"


## Acknowledgments

- Blazor - .NET web framework using WebAssembly
- MongoDB - NoSQL database
- Bootstrap - CSS framework
- Bootstrap Icons - Icon library

---

DotPic - A modern image storage solution built with Blazor and MongoDB
