namespace KivalitaAPI.DTOs
{
    public class FlowReportDTO
    {
        public int sentEmails { get; set; }
        public int answeredEmails { get; set; }
        public int openedEmails { get; set; }
        public int positiveAnsweredEmails { get; set; }
        public int negativeAnsweredEmails { get; set; }
        public int notFoundAnsweredEmails { get; set; }
        public int amountOfLeads { get; set; }
    }
}
