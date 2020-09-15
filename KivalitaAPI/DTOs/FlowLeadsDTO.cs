namespace KivalitaAPI.DTOs
{
    public class FlowTaskDTO
    {
        public string Type { get; set; }
        public string TemplateName { get; set; }
    }

    public class FlowLeadsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public FlowTaskDTO? Task { get; set; }
    }
}
