namespace KivalitaAPI.DTOs
{
    public class RdStationLeadDTO
    {
        public string event_type { get; set; }
        public string event_family { get; set; }
        public RdStationLeadPayload payload { get; set; }
    }
}