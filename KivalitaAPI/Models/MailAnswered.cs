using KivalitaAPI.Enum;
using KivalitaAPI.Interfaces;
using Sieve.Attributes;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace KivalitaAPI.Models
{
    public class MailAnswered : IEntity
    {
        public int Id { get; set; }

        [ForeignKey("Leads")]
        public int LeadId { get; set; }
        public Leads Lead { get; set; }

        [ForeignKey("FlowTask")]
        public int TaskId { get; set; }
        public FlowTask FlowTask { get; set; }

        public int UserId { get; set; }
        public string MessageId { get; set; }
        public string Subject { get; set; }
        public string BodyPreview { get; set; }
        public string BodyContent { get; set; }
        public string Sender { get; set; }
        public string Recipient { get; set; }
        public MailAnsweredStatusEnum? Status { get; set; }

        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string GetTableName()
        {
            return "MailAnswered";
        }
    }
}
