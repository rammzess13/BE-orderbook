<template>
  <div class="order-books">
    <div class="chart-wrap">
      <Bar
        v-if="chartData"
        :data="chartData" :options="chartOptions"
        class="order-book-chart"
      />
    </div>

    <div class="form">
      <div class="form-row">
        <label for="currentBidPrice" class="form-label">Bid Price</label>
        <input type="number" id="currentBidPrice" class="form-item" v-model="currentBidPrice" disabled placeholder="Bid Price" />
      </div>
      <div class="form-row">
        <label for="newBidAmount" class="form-label">Bid Amount</label>
        <input type="number" id="newBidAmount" class="form-item" v-model.number="newBidAmount" placeholder="Bid Amount" />
      </div>
      <div class="form-row total-row">
        <p class="form-label">Total</p>
        <p class="form-item">{{ (currentBidPrice * newBidAmount).toLocaleString('en-GB', { style: 'currency', currency: 'EUR' }) }}</p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from "vue"
import * as signalR from "@microsoft/signalr"
import { Bar } from 'vue-chartjs'
import type { OrderBook } from '../types';
import { throttle } from '../utils/throttle';
import {
  Chart as ChartJS,
  Title,
  Tooltip,
  Legend,
  BarElement,
  CategoryScale,
  LinearScale
} from 'chart.js'

ChartJS.register(Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale)

interface ChartData {
  labels: string[];
  datasets: {
    label: string;
    backgroundColor: string;
    borderColor: string;
    borderWidth: number;
    data: number[];
  }[];
}

const chartData = ref<ChartData | null>(null);
const newBidAmount = ref(0);

const currentBidPrice = ref(0);

function transformOrderBookData(bids: [number, number][], asks: [number, number][]) {
  return {
    labels: [...asks.map(a => a[0].toString()), ...bids.map(b => b[0].toString())],
    datasets: [
      {
        label: "Bids",
        backgroundColor: "rgba(0, 255, 0, 0.5)",
        borderColor: "rgba(0, 255, 0, 1)",
        borderWidth: 1,
        data: [
          ...bids.map(bid => bid[1]),
          ...Array(asks.length).fill(null)
        ]
      },
      {
        label: "Asks",
        backgroundColor: "rgba(255, 0, 0, 0.5)",
        borderColor: "rgba(255, 0, 0, 1)",
        borderWidth: 1,
        data: [
          ...Array(bids.length).fill(null),
          ...asks.map(ask => ask[1])
        ]
      }
    ]
  };
}

const chartOptions = {
  responsive: false,
  maintainAspectRatio: true,
  aspectRatio: 4 / 1,
  scales: {
    x: {
      stacked: false,

      grid: {
        display: false
      }
    },
    y: {
      stacked: false,
      position: 'center',
      beginAtZero: true
    }
  },
  plugins: {
    tooltip: {
      callbacks: {
        label: function(context: any) {
          return `${context.dataset.label}: Amount ${context.parsed.y} @ Price ${context.label}`;
        }
      }
    },
    legend: {
      position: 'top' as const
    }
  }
};

const connection = new signalR.HubConnectionBuilder()
  .withUrl("http://localhost:5112/orderBookHub")
  .withAutomaticReconnect()
  .build();

connection.on("ReceiveOrderBook", throttle((orderBookData: string) => {
  const book = JSON.parse(orderBookData);
  console.log("Received order book data:", book);
  const { bids, asks } = book as OrderBook;
  const sortedBids = bids.sort((a, b) => b[0] - a[0]);
  const sortedAsks = asks.sort((a, b) => a[0] - b[0]);
  chartData.value = transformOrderBookData(sortedBids,sortedAsks);
  currentBidPrice.value = sortedBids[0] ? sortedBids[0][0] : 0;
}, 1000));

connection
  .start()
  .then(() => console.log("Connected to SignalR hub"))
  .catch((err) => {
    console.error("SignalR connection error:", err);
  });

</script>

<style scoped>
.order-books {
  width: 100%;
  display: flex;
  gap: 2rem;
}

.chart-wrap {
  width: 100%;
  position: relative;
  overflow-x: auto;
}
.order-books canvas {
  min-width: 1600px;
}

.form {
  width: 300px;
  flex-shrink: 0;
}
.form-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}
.form-item {
  width: 150px;
  height: 2rem;
  padding: 0 0.5rem;
  font-size: 1rem;
}
.total-row {
  font-weight: bold;
  border-top: 1px solid #ccc;
  padding-top: 1rem;
}
</style>