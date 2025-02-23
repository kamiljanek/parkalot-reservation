namespace parkalot_reservation_request;

public class Configuration
{
    public const string ConfigurationSectionName = "Configuration";
    
    public string Url { get; set; }
    public string Bearer { get; set; }
    public string MeId { get; set; }
    public string ParkingIdDesk { get; set; }
    public string ParkingIdParking { get; set; }
    public string DeskBodyV { get; set; }
}