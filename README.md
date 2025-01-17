# Project Service

## Table of Contents

1. [Description](#description)
2. [Purpose](#purpose)
3. [Getting Started](#getting-started)
   - [Prerequisites](#prerequisites)
   - [Clone the Repository](#clone-the-repository)
   - [Install Dependencies](#install-dependencies)
   - [Set Environment Variables](#set-environment-variables)
   - [Run the Server](#run-the-server)
   - [Access the Application](#access-the-application)
   - [Debugging (Development only)](#debugging-development-only)
4. [Endpoints](#endpoints)
   - [GET /api/project](#get-apiproject)
   - [GET /api/project/{id}](#get-apiprojectid)
   - [GET /api/project/{id}/content](#get-apiprojectidcontent)
   - [POST /api/project](#post-apiproject)
   - [PUT /api/project/{id}](#put-apiprojectid)
   - [DELETE /api/project/{id}](#delete-apiprojectid)
5. [Configuration](#configuration)
   - [Example appsettings.json](#example-appsettingsjson)


## Description

The Project Service is a web API for managing projects within the Searchable DB project. It enables users to perform CRUD operations on project data and associated content. The service supports user authentication and authorization through JWT tokens, ensuring that only authorized users can access or modify their data.


## Purpose

The primary purpose of this service is to provide an API for managing project data. It allows users to create, read, update, and delete projects, as well as manage the associated content stored in Azure Blob Storage. The service ensures that project data is securely handled and is only accessible to authorized users.


## Getting started

### Running locally
Follow these steps to set up the Project Service for the Searchable DB Project.

### Prerequisites
- **.NET 8 SDK** installed on your machine
- **MongoDB** instance running (for accessing the database)

### 1. Clone the Repository
Start by cloning the project repository to your local machine:
```bash
git clone https://github.com/Nelissen-searchable-db/API-Project
cd project-service
```

#### 2. Install Dependencies
Navigate to the project directory and restore the required NuGet packages:
```bash
dotnet restore
```

#### 3. Set environment variables

Before using this service, set the `ConnectionString` environment variable in `appsettings.json` with a valid MongoDB/MongoDB compatible URL.

##### [For Development Only]

If you are on a development build, create `appsettings.Development.json` next to the existing app settings file and add your connection string as follows:

```json
{
    "ConnectionString": "YOUR_CONNECTIONSTRING"
}
```

where `YOUR_CONNECTIONSTRING` refers to the location of a testing/development database.

> **Important!** You may need to generate dummy data to get the desired results from your development instance. To do this, use the our provided dummy generator, which you can find [here](https://github.com/Nelissen-searchable-db/DummyDataGenerator).

#### 4. Run the server

##### Production
Navigate to the project directory and run the following command
```bash
dotnet run
```

##### Development
Like production, run the following command in the project directory
```bash
dotnet run --environment Development
```

#### 5. Access the Application
The API server should be available at port `5174`.

#### 6. Debugging (Development only)

If you are running the application in development mode, you can access the swagger UI at `http://localhost:5174/swagger/index.html`.

This UI will help you debug the API endpoints.


## Endpoints

### **1. `GET /api/project`**

Retrieves all projects for the authenticated user.

#### **Request**
- **Method**: `GET`
- **Headers**:
    - `Authorization`: `Bearer <JWT>`
- **Body**: None

#### **Response**
- **Status Codes**:
    - `200 OK`: Projects successfully retrieved.
    - `401 Unauthorized`: Authentication failed or JWT is missing/invalid.
    - `404 Not Found`: No projects found for the user.
- **Body**:
    ```json
    [
      {
        "id": "project-id",
        "name": "project-name",
        "description": "project-description",
        "createdAt": "project-creation-date"
      }
    ]
    ```

### **2. `GET /api/project/{id}`**

Retrieves a specific project by its ID.

#### **Request**
- **Method**: `GET`
- **Headers**:
    - `Authorization`: `Bearer <JWT>`
- **Body**: None

#### **Response**
- **Status Codes**:
    - `200 OK`: Project successfully retrieved.
    - `401 Unauthorized`: Authentication failed or JWT is missing/invalid.
    - `404 Not Found`: Project with the given ID not found.
- **Body**:
    ```json
    {
      "id": "project-id",
      "name": "project-name",
      "description": "project-description",
      "createdAt": "project-creation-date"
    }
    ```

### **3. `GET /api/project/{id}/content`**

Retrieves the content associated with a specific project.

#### **Request**
- **Method**: `GET`
- **Headers**:
    - `Authorization`: `Bearer <JWT>`
- **Body**: None

#### **Response**
- **Status Codes**:
    - `200 OK`: Project content successfully retrieved.
    - `401 Unauthorized`: Authentication failed or JWT is missing/invalid.
    - `404 Not Found`: Project or content not found.
    - `400 Bad Request`: Error retrieving blob content or project data.
- **Body**:
    ```json
    {
      "id": "project-id",
      "name": "project-name",
      "content": {
        "key1": "value1",
        "key2": "value2"
      }
    }
    ```

### **4. `POST /api/project`**

Creates a new project for the authenticated user.

#### **Request**
- **Method**: `POST`
- **Headers**:
    - `Authorization`: `Bearer <JWT>`
    - `Content-Type`: `application/json`
- **Body**:
    ```json
    {
      "name": "new-project-name",
      "description": "new-project-description"
    }
    ```

#### **Response**
- **Status Codes**:
    - `201 Created`: Project successfully created.
    - `401 Unauthorized`: Authentication failed or JWT is missing/invalid.
    - `400 Bad Request`: Invalid request body or validation error.
- **Body**:
    ```json
    {
      "id": "new-project-id",
      "name": "new-project-name",
      "description": "new-project-description",
      "createdAt": "project-creation-date"
    }
    ```

### **5. `PUT /api/project/{id}`**

Updates an existing project.

#### **Request**
- **Method**: `PUT`
- **Headers**:
    - `Authorization`: `Bearer <JWT>`
    - `Content-Type`: `application/json`
- **Body**:
    ```json
    {
      "name": "updated-project-name",
      "description": "updated-project-description"
    }
    ```

#### **Response**
- **Status Codes**:
    - `200 OK`: Project successfully updated.
    - `401 Unauthorized`: Authentication failed or JWT is missing/invalid.
    - `404 Not Found`: Project not found.
    - `400 Bad Request`: Invalid request body or validation error.
- **Body**:
    ```json
    {
      "id": "updated-project-id",
      "name": "updated-project-name",
      "description": "updated-project-description",
      "createdAt": "project-creation-date"
    }
    ```

### **6. `DELETE /api/project/{id}`**

Deletes a project.

#### **Request**
- **Method**: `DELETE`
- **Headers**:
    - `Authorization`: `Bearer <JWT>`
- **Body**: None

#### **Response**
- **Status Codes**:
    - `204 No Content`: Project successfully deleted.
    - `401 Unauthorized`: Authentication failed or JWT is missing/invalid.
    - `404 Not Found`: Project not found.
    - `400 Bad Request`: Error occurred while deleting the project.
- **Body**: None



## Configuration

Example appsettings.json

```json
{
  "AppSettings": {
    "AllowedOrigins": [""]
  },
  "ConnectionStrings": {
    "AzureBlobStorage": "",
    "PostgresSQL_DB": ""
  },
  "JwtSettings": {
    "SecretKey": ""
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

