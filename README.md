# My Dating App API
## About
This app is created by Eric Zheng Gong with .NetCore 2.0 webapi and Angular 6.
## Config
#### Cloudinary
[Cloudinary](https://cloudinary.com/) is used to store user photo and diffrerent assets. Please configure your Cloudinary in appsetting.json.
[More Detail](https://cloudinary.com/documentation/dotnet_image_upload) for .Net Cloudinary.
```
  "CloudinarySettings": {
    "CloudName":"",
    "ApiKey":"",
    "ApiSecret":""
  }
```
#### Database
* Sqlite is used for Development, Please Change ConnectionString in appsetting.Development.json.
* Mysql is used for Production, Please Change ConnectionString in appsetting.json.
## Run
* Create Database: dotnet ef database update
* Run API: dotnet run