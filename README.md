# OrderBook WebSocket Service

A real-time order book tracking service that connects to Bitstamp's WebSocket API and provides live updates through SignalR.

## Prerequisites

- .NET 9.0 SDK
- SQL Server LocalDB or SQL Server
- Visual Studio Code (recommended) or any other IDE
- Git

## Installation Steps

1. Install SQL Server LocalDB:
   - Download [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
   - Choose "Download Express" option
   - During installation, select "LocalDB" feature
   - Or run this command in PowerShell as administrator:
```powershell
winget install Microsoft.SQLServer.2022.LocalDB
```

2. Verify LocalDB installation:
```cmd
sqllocaldb info
```
This should show "MSSQLLocalDB" instance

3. Clone the repository:
```bash
git clone <repository-url>
cd OrderBook
```

4. Create LocalDB instance if not exists:
```cmd
sqllocaldb create MSSQLLocalDB
sqllocaldb start MSSQLLocalDB
```

5. Update database:
```bash
dotnet tool install --global dotnet-ef
dotnet ef database update
```

6. Run the application:
```bash
dotnet run
```

7. Build and run the application:
```bash
dotnet build
dotnet run
```

## Features

- Real-time order book updates from Bitstamp
- WebSocket connection with automatic reconnection
- SignalR integration for client updates
- Audit logging of all order book snapshots
- SQL Server database storage

## API Endpoints

- WebSocket Hub: `https://localhost:5001/orderBookHub`
- REST API: `https://localhost:5001/api/orderbook`

## Client Connection Example

```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/orderBookHub")
    .withAutomaticReconnect()
    .build();

connection.on("ReceiveOrderBook", (data) => {
    console.log(JSON.parse(data));
});

connection.start()
    .catch(err => console.error(err));
```

## Project Structure

- `/Controllers` - API controllers
- `/Services` - Business logic and WebSocket service
- `/Models` - Data models
- `/Data` - Database context and configurations
- `/Hubs` - SignalR hub for real-time communications

## Configuration

The application can be configured through `appsettings.json`:

- `ConnectionStrings:DefaultConnection` - Database connection string
- `ExternalApi:WebSocketUrl` - Bitstamp WebSocket URL
- `Logging:LogLevel` - Logging configuration

## Development

To run the project in development mode:

```bash
dotnet watch run
```

This will enable hot reload and automatic browser refresh.

## Troubleshooting

If you encounter the "The file is locked by OrderBook" error:
```bash
taskkill /F /IM OrderBook.exe
```

If you get SQL connection errors:

1. Verify LocalDB is running:
```cmd
sqllocaldb info MSSQLLocalDB
```

2. Check connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=OrderBookDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

3. Try starting LocalDB manually:
```cmd
sqllocaldb start MSSQLLocalDB
```
