using Equibles.Websockets.Messages;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

namespace Equibles.Websockets.Client; 

public class EquiblesWebSocketsClient {
    private const string Domain = "https://websockets.equibles.com";
    private readonly string _apiKey;
    private readonly IEnumerable<string> _tickers;
    private readonly HubConnection _connection;
    private readonly List<Action<Quote>> _quoteListeners = new();
    private bool _listenersRegistered = false;

    public EquiblesWebSocketsClient(string apiKey, Endpoint endpoint, IEnumerable<string> tickers) {
        _apiKey = apiKey;
        _tickers = tickers;
        
        _connection = new HubConnectionBuilder()
            .WithUrl(new Uri($"{Domain}/{endpoint.ToString().ToLower()}"))
            .WithAutomaticReconnect()
            .AddMessagePackProtocol()
            .Build();
    }

    private void RegisterListeners() {
        if (_listenersRegistered) return;
        _listenersRegistered = true;
        
        _connection.Closed += (error) => {
            Console.WriteLine($"Connection closed. Reason: {error?.Message}");
            return Task.CompletedTask;
        };

        _connection.Reconnected += async (message) => {
            Console.WriteLine($"Reconnected. " + message);
            await _connection.SendAsync("Authentication", _apiKey);
        };
        
        _connection.On<Quote>("Quote", (quote) => {
            foreach (var listener in _quoteListeners) {
                listener(quote);
            }
        });

        _connection.On<bool, string>("AuthenticationResult", (success, errorMessage) => {
            if (success) {
                SendAddTickers(_tickers.ToList());
            } else {
                Console.WriteLine("Error while authenticating. Message: " + errorMessage);
            }
        });

        _connection.On<bool, string>("StartListeningResult", (success, errorMessage) => {
            if (success) {
                Console.WriteLine("Connection successful. Waiting for quotes...");
            } else {
                Console.WriteLine("Error while adding tickers. Message: " + errorMessage);
            }
        });

        _connection.On<bool, string>("StopListeningResult", (success, errorMessage) => {
            if (success) {
                Console.WriteLine("Stopped listening to tickers with success.");
            } else {
                Console.WriteLine("Error while removing tickers. Message: " + errorMessage);
            }
        });
    }

    private void SendAddTickers(IEnumerable<string> tickers) {
        _connection.SendAsync("StartListening", tickers.ToList());
    }
    
    private void SendStopTickers(IEnumerable<string> tickers) {
        _connection.SendAsync("StopListening", tickers.ToList());
    }

    public async Task Connect() {
        if (_connection.State != HubConnectionState.Disconnected) {
            Console.WriteLine("Can not disconnect the client if it is not in a disconnected state.");
            return;
        }
        // Register the message listeners
        RegisterListeners();
        
        // Open the connection
        await _connection.StartAsync();
        
        // Authenticate
        await _connection.SendAsync("Authentication", _apiKey);
    }

    public void OnQuote(Action<Quote> action) {
        if (action == null) throw new Exception("The action can not be null");
        _quoteListeners.Add(action);
    }
    
    public void RemoveQuoteListener(Action<Quote> action) {
        _quoteListeners.Remove(action);
    }
    
    public void ClearQuoteListeners() {
        _quoteListeners.Clear();
    }

    public void AddTickers(IEnumerable<string> tickers) {
        SendAddTickers(tickers);
    }
    
    public void RemoveTickers(IEnumerable<string> tickers) {
        SendStopTickers(tickers);
    }

    public async Task Disconnect() {
        if (_connection.State != HubConnectionState.Disconnected) {
            await _connection.StopAsync();
        }
    }

    public async Task Wait(CancellationToken cancellationToken = new()) {
        while (!cancellationToken.IsCancellationRequested) {
            await Task.Delay(60000, cancellationToken);
        }
    }
}