using MessagePack;

namespace Equibles.Websockets.Messages; 

[MessagePackObject]
public class Quote {
    [Key("t")]
    public string? Ticker { get; set; }
    
    [Key("p")]
    public double Price { get; set; }
    
    [Key("v")]
    public double Volume { get; set; }
    
    [Key("ts")]
    public long Timestamp { get; set; }
}