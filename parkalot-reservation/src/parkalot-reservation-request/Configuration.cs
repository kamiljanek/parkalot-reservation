namespace parkalot_reservation_request;

public class Configuration
{
    public const string ConfigurationSectionName = "Configuration";
    
    public string BaseUri { get; init; }
    public string Bearer { get; init; }
    public string MeId { get; init; }
    public string DeskParkingId { get; init; }
    public string ParkingParkingId { get; init; }
    public string DeskBodyV { get; init; }
}