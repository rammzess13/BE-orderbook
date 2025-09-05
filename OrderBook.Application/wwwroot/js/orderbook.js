const connection = new signalR.HubConnectionBuilder()
  .withUrl('/orderBookHub')
  .withAutomaticReconnect()
  .build()

connection.on('ReceiveOrderBook', (data) => {
  const orderBook = JSON.parse(data)
  // Update UI with order book data
  console.log(orderBook)
})

connection.start().catch((err) => console.error(err))
