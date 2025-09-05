export interface OrderBook {
  timestamp: number;
  microtimestamp: number;
  bids: [number, number][];
  asks: [number, number][];
}