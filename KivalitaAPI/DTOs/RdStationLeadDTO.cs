namespace KivalitaAPI.DTOs
{
    public class RdStationLeadDTO
    {
        public string event_name { get; set; }
        public string event_family { get; set; }
        public RdStationLeadPayload payload { get; set; }
    }
}