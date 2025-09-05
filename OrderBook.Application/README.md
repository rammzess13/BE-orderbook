# OrderBook WebSocket Service

A real-time order book tracking service that connects to Bitstamp's WebSocket API and provides live updates through SignalR.

## Prerequisites

- .NET 9.0 SDK
- Visual Studio Code (recommended)
- Git

## Getting Started

1. Clone the repository:
```powershell
git clone <repository-url>
cd OrderBook
```

2. Install Entity Framework Core tools:
```powershell
dotnet tool install --global dotnet-ef
```

3. Set up the database:
```powershell
# Create initial migration
dotnet ef migrations add InitialCreate

# Create SQLite database and apply migrations
dotnet ef database update
```

4. Run the application:
```powershell
dotnet run
```

## Features

- Real-time order book updates from Bitstamp
- WebSocket connection with automatic reconnection
- SignalR integration for client updates
- Audit logging of all order book snapshots
- SQLite database for easy setup and portability

## API Endpoints

- WebSocket Hub: `https://localhost:5001/orderBookHub`
- REST API: `https://localhost:5001/api/orderbook`

## Database Management

The application uses SQLite for data storage. The database file (`OrderBook.db`) will be created automatically in the project root directory when you run the migrations.

To reset the database:
```powershell
# Remove existing database
del OrderBook.db

# Remove existing migrations
rm -r Migrations

# Create new migration
dotnet ef migrations add InitialCreate

# Create new database
dotnet ef database update
```

## Troubleshooting

If you encounter locked file errors:
```powershell
# Kill any running instances of the application
taskkill /F /IM OrderBook.exe

# Clean the solution
dotnet clean
```
