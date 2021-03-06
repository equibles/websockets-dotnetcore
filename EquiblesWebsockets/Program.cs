using System.Globalization;
using Equibles.Websockets.Client;

// Creates the websocket client
var client = new EquiblesWebSocketsClient("my_apy_key", Endpoint.Stocks, new List<string>(){"TSLA"});

// Register a callback action to be executed when a quote is received
client.OnQuote(q => {
    Console.WriteLine("Ticker: " + q.Ticker + 
                      " | Volume: " + q.Volume.ToString(CultureInfo.InvariantCulture) +  
                      " | Price: " + q.Price.ToString("C", CultureInfo.CreateSpecificCulture("en")));
});

// Connect and authenticate
await client.Connect();

// After 5 seconds I also want to listen to Apple quotes
await Task.Delay(5000);
client.AddTickers(new []{"AAPL"});

// Prevents the program from exiting
await client.Wait();

// Example output from this program:
// Ticker: AAPL | Volume: 56 | Price: $168.42
// Ticker: AAPL | Volume: 100 | Price: $168.43
// Ticker: TSLA | Volume: 33 | Price: $1,075.66

