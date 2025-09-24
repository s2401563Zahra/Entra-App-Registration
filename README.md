# Todo API with Azure SQL Database and Microsoft Entra ID

A .NET Web API project that provides secure Todo functionality with Azure SQL Database integration and Microsoft Entra ID authentication.

## Features

- ✅ **Secure Authentication**: Uses Microsoft Entra ID (Azure AD) for JWT-based authentication
- ✅ **Azure SQL Database**: Integrated with Azure SQL Database using Entity Framework Core
- ✅ **User Isolation**: Each user can only access their own todos
- ✅ **RESTful API**: Complete CRUD operations for Todo items
- ✅ **Database Migrations**: Entity Framework Core migrations for schema management
- ✅ **Logging**: Structured logging for monitoring and debugging

## Prerequisites

- .NET 8.0 or later
- Azure subscription
- Azure SQL Database instance
- Microsoft Entra ID app registration

## Setup Instructions

### 1. Azure SQL Database Setup

1. Create an Azure SQL Database instance
2. Configure Azure AD authentication for the SQL server
3. Ensure your app registration has access to the database

### 2. Microsoft Entra ID App Registration

Follow the setup guide: [Azure Entra App Registration Setup](https://github.com/thxmuffe/bc_net24s_ryhma/blob/main/src/todo/azure-setup.md)

1. Register a new application in Azure AD
2. Configure API permissions
3. Note down the:
   - Tenant ID
   - Client ID (Application ID)
   - Domain

### 3. Configuration

Update the `appsettings.Development.json` file with your Azure settings:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:YOUR-SERVER.database.windows.net,1433;Initial Catalog=TodoDb;Authentication=Active Directory Default;Encrypt=True;"
  },
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "yourdomain.onmicrosoft.com",
    "TenantId": "your-tenant-id-guid",
    "ClientId": "your-app-registration-client-id",
    "Audience": "api://your-app-registration-client-id"
  }
}
```

### 4. Database Setup

Run Entity Framework migrations to create the database schema:

```bash
# Add the .NET tools to your PATH (if not already done)
export PATH="$PATH:/Users/yourusername/.dotnet/tools"

# Update database with the latest migrations
dotnet ef database update
```

### 5. Run the Application

```bash
# Start the application
dotnet run

# The API will be available at:
# - HTTP: http://localhost:5000
# - HTTPS: https://localhost:5001
```

## API Endpoints

All endpoints require a valid JWT token from Microsoft Entra ID.

| Method | Endpoint               | Description                              |
| ------ | ---------------------- | ---------------------------------------- |
| GET    | `/api/todos`           | Get all todos for the authenticated user |
| GET    | `/api/todos/{id}`      | Get a specific todo by ID                |
| POST   | `/api/todos`           | Create a new todo                        |
| PUT    | `/api/todos/{id}`      | Update an existing todo                  |
| DELETE | `/api/todos/{id}`      | Delete a todo                            |
| GET    | `/api/todos/completed` | Get all completed todos                  |
| GET    | `/api/todos/pending`   | Get all pending (incomplete) todos       |

### Example Request Body (POST/PUT)

```json
{
  "title": "Complete the project",
  "description": "Finish the Todo API with Azure integration",
  "isCompleted": false
}
```

### Example Response

```json
{
  "id": 1,
  "title": "Complete the project",
  "description": "Finish the Todo API with Azure integration",
  "isCompleted": false,
  "createdAt": "2025-09-24T10:30:00Z",
  "completedAt": null,
  "userId": "user-object-id-from-azure-ad"
}
```

## Authentication

The API uses JWT Bearer authentication with Microsoft Entra ID. Include the access token in the Authorization header:

```
Authorization: Bearer YOUR_ACCESS_TOKEN
```

## Security Features

- **User Isolation**: Users can only access their own todos
- **JWT Validation**: All tokens are validated against Microsoft Entra ID
- **SQL Injection Prevention**: Entity Framework Core provides protection
- **HTTPS**: Enforced in production
- **CORS**: Configured for specific origins

## Logging

The application logs important events including:

- Todo creation, updates, and deletions
- Authentication events
- Database operations
- Error conditions

## Troubleshooting

### Common Issues

1. **Authentication Failed**

   - Verify your Azure AD app registration settings
   - Check that the JWT token is valid and not expired
   - Ensure the audience claim matches your configuration

2. **Database Connection Issues**

   - Verify your connection string
   - Ensure your Azure AD identity has access to the SQL database
   - Check firewall settings on Azure SQL

3. **Migration Issues**
   - Ensure Entity Framework tools are installed: `dotnet tool install --global dotnet-ef`
   - Verify database permissions for schema changes

## Development

### Running in Development

The project includes development-specific configurations in `appsettings.Development.json`. Make sure to update the placeholder values with your actual Azure settings.

### Adding New Features

1. Update the `TodoItem` model in `Models/TodoItem.cs`
2. Create a new migration: `dotnet ef migrations add YourMigrationName`
3. Update the controller in `Controllers/TodosController.cs`
4. Apply the migration: `dotnet ef database update`

## Production Deployment

For production deployment:

1. Update `appsettings.json` with production values
2. Use Azure App Service managed identity for database authentication
3. Configure proper logging and monitoring
4. Set up CI/CD pipeline for automated deployments

## License

This project is for educational purposes as part of the BC .NET 24S course.
