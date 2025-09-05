# Orderbook Visualization

A real-time orderbook visualization tool built with Vue 3, TypeScript, and SignalR. This application displays bid and ask orders in a dynamic chart format.

## Features

- Real-time orderbook updates via SignalR connection
- Visual representation of bids and asks using Chart.js
- Split view with asks on the left and bids on the right
- Price-based ordering with highest prices in the middle
- Bidding form interface
- Horizontal scrolling for large datasets

## Tech Stack

- Vue 3
- TypeScript
- Vite
- Chart.js
- SignalR
- CSS3

## Getting Started

1. Clone the repository
```bash
git clone <repository-url>
cd orderbook
```

2. Install dependencies
```bash
npm install
```

3. Start the development server
```bash
npm run dev
```

4. Make sure the SignalR backend server is running at `http://localhost:5112`

## Project Structure

- `src/components/OrderBook.vue` - Main orderbook component with chart visualization
- `src/types/` - TypeScript type definitions
- `src/utils/` - Utility functions including throttle implementation

## Configuration

The SignalR connection is configured to connect to `http://localhost:5112/orderBookHub`. Update this URL in `OrderBook.vue` if your backend is running on a different address.
