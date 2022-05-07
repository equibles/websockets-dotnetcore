using System.Globalization;
using Equibles.Websockets.Client;

// Creates the websocket client
var client = new EquiblesWebSocketsClient("my-api-key", Endpoint.Cryptos, new List<string>(){"BTC-USD"});

// Register a callback action to be executed when a quote is received
client.OnQuote(q => {
    Console.WriteLine("Ticker: " + q.Ticker + 
                      " | Volume: " + q.Volume.ToString(CultureInfo.InvariantCulture) +  
                      " | Price: " + q.Price.ToString("C", CultureInfo.CreateSpecificCulture("en")));
});

// Connect and authenticate
await client.Connect();

// After 5 seconds I also want to listen to ETH quotes
await Task.Delay(5000);
client.AddTickers(new []{"ETH-USD"});

// Prevents the program from exiting
await client.Wait();

// Example output from this program:
// Ticker: BTC-USD | Volume: 0.00404 | Price: $35,807.42
// Ticker: BTC-USD | Volume: 0.0011 | Price: $35,807.42
// Ticker: ETH-USD | Volume: 0.0905 | Price: $2,675.66

