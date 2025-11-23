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
